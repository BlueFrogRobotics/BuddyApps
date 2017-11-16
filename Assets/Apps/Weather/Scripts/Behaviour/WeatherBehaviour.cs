using UnityEngine.UI;
using UnityEngine;

using Buddy.UI;
using System.Collections;
using System.Xml;
using System.IO;
using System;
using System.Collections.Generic;

using Buddy;

namespace BuddyApp.Weather
{

        /* A basic monobehaviour as "AI" behaviour for your app */
        public partial class WeatherBehaviour : MonoBehaviour
        {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private WeatherData mAppData;

        //NEW ADD//

        // Public Attributes modifiable from Unity Interface 
        public string sentence = "";
        public int numberReturn = 5;
        public bool checkSentence = false;
        public bool V2 = true;
        public bool V3 = true;
        public bool word2vec = false;

        // Phrase Engine Attr
        PhraseEngine phraseEngine = null;
        PhraseEngineV2 phraseEngineV2 = null;
        PhraseEngineV3 phraseEngineV3 = null;
        Vocabulary vocabulary = null;

        // Question Answer Engine 
        Dictionary<string, string> actype_answer = null;

        // Define folders  
        string absoluteStreamingAssetsPath = "C:/Users/mathi/Documents/DiscussionEngine/Assets/WordEmbedding/StreamingAssets/";
        string folderData = "Data/";
        string folderExperiments = "Experiments/";
        string folderStopwords = "StopwordsLists/";
        string folderReferences = "References/";
        string folderResults = "Results/";

        string folderModel = "Models/";

        // Define file location with respect to StreamingAssets folder.
        string experimentInputPath = "";
        string experimentOutputCommonPath = "";
        string stopwordsPath = "";
        string stopwordsV2Path = "";
        string word2vecModelPath = "";
        string refPhrasesPath = "";
        string refChunksPath = "";
        string questionAnswerPath = "";
        string questionAnswerActPath = "";
        string questionAnswerActPathRelative = "";
        string categoriesPath = "";

        // Vocal interface
        TextToSpeech tts = null;

        // Categories
        List<String> categories = null;
        string genericCategory = "general";


        //NEW ADD//

        internal int mIndice;
        internal WeatherInfo[] mWeatherInfos;
        internal WeatherRequestError mRequestError = 0;

        internal enum WeatherRequestError
        {
            UNKNOWN,

            NONE,

            TOO_FAR
        }

        internal string mVocalRequest;
        internal string mLocation;
        internal WeatherType mForecast;
        internal int mDate;
        internal int mHour;
        internal bool mWhen;
        
        void Start()
        {
            this.actype_answer = new Dictionary<string, string>();

            // Build path for each file based on folder architecture
            this.experimentInputPath = BYOS.Instance.Resources.GetPathToRaw(folderData + folderExperiments + "experience.csv");
            this.experimentOutputCommonPath = BYOS.Instance.Resources.GetPathToRaw(folderResults + "experience_results");
            this.stopwordsPath = BYOS.Instance.Resources.GetPathToRaw(folderData + folderStopwords + "stopwordsV1.txt");
            this.stopwordsV2Path = BYOS.Instance.Resources.GetPathToRaw(folderData + folderStopwords + "stopwordsV2.txt");
            this.word2vecModelPath = BYOS.Instance.Resources.GetPathToRaw("fr_word2vec.bin", LoadContext.OS);
            this.refPhrasesPath = BYOS.Instance.Resources.GetPathToRaw(folderData + folderReferences + "phrases.csv");
            this.refChunksPath = BYOS.Instance.Resources.GetPathToRaw(folderData + folderReferences + "chunksReferences_words.csv");
            this.questionAnswerPath = BYOS.Instance.Resources.GetPathToRaw(folderData + "questions_reponses.xml");
            this.questionAnswerActPath = BYOS.Instance.Resources.GetPathToRaw(absoluteStreamingAssetsPath + folderData + "/questions_reponses_act.xml");
            this.questionAnswerActPathRelative = BYOS.Instance.Resources.GetPathToRaw(folderData + "questions_reponses_act.xml");
            this.categoriesPath = BYOS.Instance.Resources.GetPathToRaw(folderData + folderReferences + "Categories/");
            this.categoriesPath = BYOS.Instance.Resources.GetPathToRaw(folderData + folderReferences + "Categories/");

            this.categories = new List<String>();

            this.categories.Add("chauffage");
            this.categories.Add("courses");
            this.categories.Add("deviceManagement");
            this.categories.Add("meteo");
            this.categories.Add("news");
            this.categories.Add("general");



            //////////////////////////// TOUT POUR INITIALISER LES TROIS VERSIONS DU MODEL ////////////////////////////////////////////////////////
            // Load Word2Vec
            vocabulary = new Word2VecBinaryReader().Read(word2vecModelPath);
            Debug.Log("LOADED : Word2Vec (example => manger " + vocabulary.Distance("manger", 1)[0] + ")");

            // Create Phrase Engine V1 
            PhraseEngineBuilder phraseEngineBuilder = new PhraseEngineBuilder();
            this.phraseEngine = phraseEngineBuilder.BuildPhraseEngine(vocabulary, refPhrasesPath, stopwordsPath);

            // Create Phrase Engine V2 
            PhraseEngineV2Builder phraseEngineV2Builder = new PhraseEngineV2Builder();
            this.phraseEngineV2 = phraseEngineV2Builder.BuildPhraseEngine(vocabulary, refChunksPath, stopwordsV2Path);

            // Create Phrase Engine V3
            PhraseEngineV3Builder phraseEngineV3Builder = new PhraseEngineV3Builder();
            // BUG : Du aux stopwords, le modèle est meilleur avec mais j'ai eu des erreurs bizarres. Autant ne pas s'en servir, ça aurait mérité plus d'ingénierie mais
            // pas le temps hélas.
            //this.phraseEngineV3 = phraseEngineV3Builder.BuildPhraseEngine (vocabulary, categoriesPath, genericCategory, this.categories, this.processSentence, stopwordsPath);
            this.phraseEngineV3 = phraseEngineV3Builder.BuildPhraseEngine(vocabulary, categoriesPath, genericCategory, this.categories, this.ProcessSentence);
            //////////////////////////// TOUT POUR INITIALISER LES TROIS VERSIONS DU MODEL ////////////////////////////////////////////////////////

            // Start Buddy TTS
            this.tts = BYOS.Instance.Interaction.TextToSpeech;

            // Start Buddy STT
            BYOS.Instance.Interaction.VocalManager.EnableTrigger = true;
            //mVocalManager.StartInstantReco ();
            //BYOS.Instance.Interaction.VocalManager.OnEndReco = OnSpeechRecognition;
            BYOS.Instance.Interaction.VocalManager.EnableDefaultErrorHandling = true;
            /*
			* You can setup your App activity here.
			*/
            WeatherActivity.Init(null);

            /*
			* Init your app data
			*/
            mAppData = WeatherData.Instance;
        }

        /// <summary>
        /// Processes the sentence.
        /// </summary>
        /// <returns>The sentence.</returns>
        /// <param name="sentence">Sentence.</param>
        public string ProcessSentence(string sentence, StopwordProcessor stopwordProcessor = null)
        {

            string newPhrase = (string)sentence.Clone();
            if (stopwordProcessor != null)
            {
                newPhrase = stopwordProcessor.Process(newPhrase);
            }

            newPhrase = newPhrase.ToLower();
            newPhrase = newPhrase.Replace(" ?", "");
            newPhrase = newPhrase.Replace("?", "");
            newPhrase = newPhrase.Replace(" .", "");
            newPhrase = newPhrase.Replace(".", "");
            newPhrase = newPhrase.Replace(" !", "");
            newPhrase = newPhrase.Replace("!", "");
            string[] splitedSentence = newPhrase.Split(' ');
            for (int i = 0; i < splitedSentence.Length; i++)
            {
                if (!vocabulary.ContainsWord(splitedSentence[i]))
                {
                    //Debug.Log ("On n'a pas trouvé le mot en effet :" + newPhrase);
                    string[] splitHyphen = splitedSentence[i].Split('-');
                    string[] splitSingleQuote = splitedSentence[i].Split('\'');
                    string[] splitSingleQuoteSpecial = splitedSentence[i].Split('’');
                    //Debug.Log ("splitHyphen : " + splitHyphen.Length + " & splitSingleQuote : " + splitSingleQuote.Length);
                    if (splitHyphen.Length > 1)
                    {
                        splitedSentence[i] = String.Join(" ", splitHyphen);
                    }
                    else if (splitSingleQuote.Length > 1)
                    {
                        splitedSentence[i] = String.Join(" ", splitSingleQuote);
                    }
                    else if (splitSingleQuoteSpecial.Length > 1)
                    {
                        splitedSentence[i] = String.Join(" ", splitSingleQuoteSpecial);
                    }
                }
            }
            newPhrase = String.Join(" ", splitedSentence);
            //Debug.Log ("Final Sentence " + newPhrase);
            //if (this.StopwordProcessor != null)
            //	newPhrase = StopwordProcessor.Process (newPhrase);
            return newPhrase;
        }

        /// <summary>
		/// Raises the speech recognition event.
		/// </summary>
		/// <param name="iVoiceInput">I voice input.</param>
		public string OnSpeechRecognition(string iVoiceInput)
        {
            Debug.Log("Laun ching V3");
            string correspondingCategory = "";
            string similarSentence = "";
            double similarity = 0;
            Debug.Log("Coucou" + iVoiceInput);
            string act = this.phraseEngineV3.SentenceToSemantic(iVoiceInput, 0.5, 0.1, out correspondingCategory, out similarSentence, out similarity);
            Debug.Log("Non non");
            //string act = "IsOn(Plottwist=RaleBol);Request(Type=Metro,Location=SanFrancisco)";
            string[] acts = act.Split(';');
            BYOS.Instance.Toaster.Display<TextToast>().With(act);
            Debug.Log("$$$$$$$$$$$$$$$$$$$$$ ACTS = " + act + " $$$$$$$$$$$$$$$$$$$$$$");
            foreach (string actToAnswer in acts)
            {
                if (this.actype_answer.ContainsKey(act))
                {
                    if (actToAnswer != "" && actToAnswer != " ")
                    {
                        //this.BuddySay(actToAnswer);
                        //this.BuddySayRealy(this.actype_answer[actToAnswer]);
                    }
                    Debug.Log("Act: " + actToAnswer);
                    Debug.Log("Actype transf : " + this.actype_answer[actToAnswer]);
                    return (actToAnswer);
                }
                else
                {
                    Debug.Log("Actype : " + actToAnswer);
                    //this.BuddySay(act);
                    //this.BuddySayRealy("Je ne sais pas quoi répondre.");
                    return (actToAnswer);
                }
                Debug.Log("Corresponding Category : " + correspondingCategory);
                Debug.Log("Similar Cluster Sentence : " + similarSentence);
                Debug.Log("Similarity with Sentence : " + similarity);
            }
            return ("NON CA NE MARCHE PAS");
        }
    }
}
