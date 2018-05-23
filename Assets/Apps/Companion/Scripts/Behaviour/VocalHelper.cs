using Buddy.UI;
using Buddy;
using UnityEngine;

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace BuddyApp.Companion
{
	enum RequestType
	{
		DEFINITION,
		NEWS,
		WEATHER
	}

	/// <summary>
	/// Use this function in order to get the Type of question found
	/// </summary>
	/// <remarks>
	/// The value is a string and can be a lot of different things like Babyphone, VolumeUp
	/// </remarks>
	/// <param name="iType"></param>
	public delegate void QuestionAnalysed(string iType);

	/// <summary>
	/// This provides a standard vocal feature for basic question answering through pre-answered questions
	/// wolfram alpha queries and/or cleverbot answering
	/// </summary>
	public class VocalHelper : MonoBehaviour
	{
		/// <summary>
		///  Authorizes automatic notification display after STT recognition or error. True as default
		/// </summary>
		public bool WithNotification { get { return mWithNotification; } set { mWithNotification = value; } }

		/// <summary>
		///  Authorizes to know if an answer is pending
		/// </summary>
		public bool BuildingAnswer { get { return mBuildingAnswer; } }

		private bool mBuildingAnswer;

		/// <summary>
		///  Answer given
		/// </summary>
		//public bool AnswerGiven { get { return mAnswerGiven; } }

		//private bool mAnswerGiven;

		private bool mChatBotRequested;
		private bool mWithNotification;
		private short mErrorCount;
		private Language mCurrentLanguage;
		private float mTimerErrorCount;
		private string mQuestionsFile;
		private string mSynonymesFile;
		private bool mActive = false;

		private Dictionary<RequestType, string> mWebsiteHash;

		private List<string> mDefinitionSpeech; //A VOIR

		private List<string> mDidntUnderstandSpeech;
		private List<string> mDontMoveSpeech;
		private List<string> mDoSomethingSpeech;
		private List<string> mFollowMeSpeech;
		private List<string> mGameSpeech;
		private List<string> mGetSpeech;
		private List<string> mHeadDownSpeech;
		private List<string> mHeadUpSpeech;
		private List<string> mHeadLeftSpeech;
		private List<string> mHeadRightSpeech;
		private List<string> mHourSpeech;
		private List<string> mICouldntSpeech;
		private List<string> mJokeSpeech;
		private List<string> mLeftSpeech;
		private List<string> mLookAtMeSpeech;
		private List<string> mLookForSpeech;
		private List<string> mMoveBackwardSpeech;
		private List<string> mMoveForwardSpeech;
		private List<string> mMoveLeftSpeech;
		private List<string> mMoveRightSpeech;
		private List<string> mNewsSpeech;
		private List<string> mPlaySpeech;
		private List<string> mQuestionSpeech;
		private List<string> mQuitSpeech;
		private List<string> mRepeatAfterMeSpeech;
		private List<string> mRepeatPlzSpeech;
		private List<string> mSeveralResSpeech;
		private List<string> mSorrySpeech;
		private List<string> mTemperatureSpeech;
		private List<string> mTempSpeech;
		private List<string> mThanksSpeech;
		private List<string> mTurnSpeech;
		private List<string> mURWelcomeSpeech;
		private List<string> mVolumeSpeech;
		private List<string> mVolumeDownSpeech;
		private List<string> mVolumeUpSpeech;
		private List<string> mWanderSpeech;
		private List<string> mWantToKnowSpeech;
		private List<string> mWhoIs;

		private TextToSpeech mTTS;
		private VocalManager mVocalManager;
		private Face mFace;
		private LED mLED;

		private ChatterBotFactory mChatBotFactory;
		private ChatterBot mCleverbot;
		private ChatterBotSession mCleverbotSession;

		public string Answer { get; private set; }

		/// <summary>
		/// Use this callback to know the Question type found (as string) 
		/// </summary>
		public QuestionAnalysed OnQuestionTypeFound { get; set; }

		void Start()
		{
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			mVocalManager = BYOS.Instance.Interaction.VocalManager;
			mFace = BYOS.Instance.Interaction.Face;
			mLED = BYOS.Instance.Primitive.LED;
			mCurrentLanguage = BYOS.Instance.Language.CurrentLang;

			mChatBotRequested = false;
			mBuildingAnswer = false;
			mErrorCount = 0;

			//Init all the questions and synonyms from files depending on language
			InitLanguageFiles();

			//Init websites for queries on weather, defintion and news
			mWebsiteHash = new Dictionary<RequestType, string>();
			//mWebsiteHash.Add(RequestType.WEATHER, "http://www.infoclimat.fr/public-api/gfs/xml?_auth=ABpQRwB%2BUnAFKFViBnBXflM7BzJdK1RzA39VNl8xAn8CaANjVD9VNFc8VyoALwMzAi9TOwg3CDgDaQdiWigEeABgUDQAYFI1BWxVPwY3V3xTfwdlXWdUcwN%2FVTpfMAJ%2FAmMDY1QxVSlXP1c9AC4DNQIxUzoIKAgvA2EHY1ozBG8Aa1AyAGZSNAVpVTQGKVd8U2UHZ11kVGoDYFVmXzsCaAJoA2ZUMVU2V2lXPQAuAzQCM1MwCDQIMANoB2RaNQR4AHxQTQAQUi0FKlV1BmNXJVN9BzJdPFQ4&_c=f30d5acf5e18de4f029990712316a936&_ll=");
			//if (mCurrentLanguage == Language.EN)
			mWebsiteHash.Add(RequestType.DEFINITION, "https://" + mCurrentLanguage.ToString().ToLower() + ".wikipedia.org/w/api.php?format=xml&action=query&prop=extracts|categories|links&exintro=&explaintext=&titles=");
			//else
			//	mWebsiteHash.Add(RequestType.DEFINITION, "https://fr.wikipedia.org/w/api.php?format=xml&action=query&prop=extracts|categories|links&exintro=&explaintext=&titles=");
			//mWebsiteHash.Add(RequestType.NEWS, "https://api.cognitive.microsoft.com/bing/v5.0/news/?Market=fr-FR&?count=1");

			// Init list of speech with same meaning
			InitSpeech();
		}

		void Update()
		{
			if (mActive) {

				mTimerErrorCount += Time.deltaTime;

				//if (mTimerErrorCount >= RESET_ERROR_COUNT) {
				//	mTimerErrorCount = 0F;
				//	mErrorCount = 0;
				//}

				//If language has changed, questions and synonyms have to be reloaded to current language
				if (mCurrentLanguage != BYOS.Instance.Language.CurrentLang) {
					mCurrentLanguage = BYOS.Instance.Language.CurrentLang;
					Utils.LogI(LogContext.INTERACTION, "Switching language");

					InitLanguageFiles();
					mWebsiteHash[RequestType.DEFINITION] = "https://" + mCurrentLanguage.ToString().ToLower() + ".wikipedia.org/w/api.php?format=xml&action=query&prop=extracts|categories|links&exintro=&explaintext=&titles=";

					InitSpeech();
				}
			}
		}

		private void InitLanguageFiles()
		{

			StreamReader lStreamReader;
			try {
				lStreamReader = new StreamReader(BYOS.Instance.Resources.GetPathToRaw("questions-" + mCurrentLanguage.ToString().ToLower() + ".xml"));
			} catch {
				lStreamReader = new StreamReader(BYOS.Instance.Resources.GetPathToRaw("questions-en.xml"));
			}

			mQuestionsFile = lStreamReader.ReadToEnd();
			lStreamReader.Close();



			try {
				lStreamReader = new StreamReader(BYOS.Instance.Resources.GetPathToRaw("synonymes-" + mCurrentLanguage.ToString().ToLower() + ".xml"));
			} catch {
				lStreamReader = new StreamReader(BYOS.Instance.Resources.GetPathToRaw("synonymes-en.xml"));
			}

			mSynonymesFile = lStreamReader.ReadToEnd();
			lStreamReader.Close();

		}

		private void InitSpeech()
		{
			//Reads through the synonym file to check for specific keywords
			mDefinitionSpeech = new List<string>();

			mDidntUnderstandSpeech = new List<string>();
			mDontMoveSpeech = new List<string>();
			mDoSomethingSpeech = new List<string>();
			mFollowMeSpeech = new List<string>();
			mGameSpeech = new List<string>();
			mGetSpeech = new List<string>();
			mHeadDownSpeech = new List<string>();
			mHeadLeftSpeech = new List<string>();
			mHeadRightSpeech = new List<string>();
			mHeadUpSpeech = new List<string>();
			mHourSpeech = new List<string>();
			mICouldntSpeech = new List<string>();
			mJokeSpeech = new List<string>();
			mLeftSpeech = new List<string>();
			mLookAtMeSpeech = new List<string>();
			mLookForSpeech = new List<string>();
			mMoveBackwardSpeech = new List<string>();
			mMoveForwardSpeech = new List<string>();
			mMoveLeftSpeech = new List<string>();
			mMoveRightSpeech = new List<string>();
			mNewsSpeech = new List<string>();
			mPlaySpeech = new List<string>();
			mQuestionSpeech = new List<string>();
			mQuitSpeech = new List<string>();
			mRepeatAfterMeSpeech = new List<string>();
			mRepeatPlzSpeech = new List<string>();
			mSeveralResSpeech = new List<string>();
			mSorrySpeech = new List<string>();
			mTemperatureSpeech = new List<string>();
			mTempSpeech = new List<string>();
			mThanksSpeech = new List<string>();
			mTurnSpeech = new List<string>();
			mURWelcomeSpeech = new List<string>();
			mVolumeSpeech = new List<string>();
			mVolumeDownSpeech = new List<string>();
			mVolumeUpSpeech = new List<string>();
			mWanderSpeech = new List<string>();
			mWantToKnowSpeech = new List<string>();
			mWhoIs = new List<string>();

			FillListSyn("Definition", mDefinitionSpeech);

			FillListSyn("DidntUnderstand", mDidntUnderstandSpeech);
			FillListSyn("DontMove", mDontMoveSpeech);
			FillListSyn("DoSomething", mDoSomethingSpeech);
			FillListSyn("FollowMe", mFollowMeSpeech);
			FillListSyn("Game", mGameSpeech);
			FillListSyn("Get", mGetSpeech);
			FillListSyn("HeadDown", mHeadDownSpeech);
			FillListSyn("HeadLeft", mHeadLeftSpeech);
			FillListSyn("HeadRight", mHeadRightSpeech);
			FillListSyn("HeadUp", mHeadUpSpeech);
			FillListSyn("Hour", mHourSpeech);
			FillListSyn("Left", mLeftSpeech);
			FillListSyn("LookAtMe", mLookAtMeSpeech);
			FillListSyn("ICouldnt", mICouldntSpeech);
			FillListSyn("Joke", mJokeSpeech);
			FillListSyn("LookFor", mLookForSpeech);
			FillListSyn("MoveBackward", mMoveBackwardSpeech);
			FillListSyn("MoveForward", mMoveForwardSpeech);
			FillListSyn("MoveLeft", mMoveLeftSpeech);
			FillListSyn("MoveRight", mMoveRightSpeech);
			FillListSyn("News", mNewsSpeech);
			FillListSyn("Play", mPlaySpeech);
			FillListSyn("Question", mQuestionSpeech);
			FillListSyn("Quit", mQuitSpeech);
			FillListSyn("Repeat", mRepeatPlzSpeech);
			FillListSyn("RepeatAfterMe", mRepeatAfterMeSpeech);
			FillListSyn("SeveralRes", mSeveralResSpeech);
			FillListSyn("Sorry", mSorrySpeech);
			FillListSyn("Temperature", mTemperatureSpeech);
			FillListSyn("Temp", mTempSpeech);
			FillListSyn("Thanks", mThanksSpeech);
			FillListSyn("Turn", mTurnSpeech);
			FillListSyn("URWelcome", mURWelcomeSpeech);
			FillListSyn("Volume", mVolumeSpeech);
			FillListSyn("VolumeDown", mVolumeDownSpeech);
			FillListSyn("VolumeUp", mVolumeUpSpeech);
			FillListSyn("Wander", mWanderSpeech);
			FillListSyn("WantToKnow", mWantToKnowSpeech);
			FillListSyn("WhoIs", mWhoIs);
		}

		//private void InitChatBot()
		//{
		//	//Pretty self-explanatory
		//	if (mCleverbotSession == null && mChatBotRequested) {
		//		//Debug.Log("retry chatbot init");
		//		if (mChatBotFactory == null)
		//			mChatBotFactory = new ChatterBotFactory();
		//		mCleverbot = mChatBotFactory.Create(ChatterBotType.CLEVERBOT);
		//		mCleverbotSession = mCleverbot.CreateSession();
		//	}
		//}

		private void FillListSyn(string iXmlCode, List<string> iSynList)
		{
			using (XmlReader lReader = XmlReader.Create(new StringReader(mSynonymesFile))) {

				if (lReader.ReadToFollowing(iXmlCode)) {
					string lContent = lReader.ReadElementContentAsString();
					string[] lSynonymes = lContent.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

					for (int i = 0; i < lSynonymes.Length; ++i)
						iSynList.Add(lSynonymes[i]);
				}
			}
		}

		/// <summary>
		/// Start a new dialogue
		/// </summary>
		//public void StartDialogueWithPhrase()
		//{
		//	//Useless ...
		//	//TTSProcessAndSay("Que puis-je faire pour vous?");
		//	//mFace.SetExpression(MoodType.LISTENING);
		//	//mLED.SetBodyLight(LEDColor.BLUE_LISTENING);
		//	mVocalManager.StartInstantReco();
		//}

		/// <summary>
		/// Activate the vocal recognition
		/// </summary>
		public void Activate()
		{
			//mAnswerGiven = false;
			mTimerErrorCount = 0F;
			mActive = true;
			//BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
			//BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
		}

		/// <summary>
		/// Disactivate the vocal recognition
		/// </summary>
		public void DisActivate()
		{
			//mAnswerGiven = false;
			mTimerErrorCount = 0F;
			mActive = false;
			//BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
		}

		//private void TTSProcessAndSay(string iSpeech, bool iStack = false)
		//{
		//	string lCorrectedSpeech = iSpeech.Replace("vous", "vou");
		//	string[] lSentences = lCorrectedSpeech.Split(new string[] { "[silence]" }, StringSplitOptions.None);

		//	foreach (string lSentence in lSentences) {
		//		mTTS.Say(lSentence, iStack);
		//		mTTS.Silence(500, true);
		//	}
		//	mPreviousAnswer = iSpeech;
		//}

		//private void OnSpeechRecognition(string iVoiceInput)
		//{
		//	mErrorCount = 0;
		//	string lLowVoiceInput = iVoiceInput.ToLower();
		//	Debug.Log("On Speech Recognition input : " + lLowVoiceInput);

		//	if (!SpecialRequest(lLowVoiceInput))
		//		BuildGeneralAnswer(lLowVoiceInput);
		//}

		//private void OnPartialRecognition(string iVoiceInput)
		//{
		//	mErrorCount = 0;
		//	string lLowVoiceInput = iVoiceInput.ToLower();
		//	Debug.Log("On Partial Recognition input : " + lLowVoiceInput);

		//	if (!SpecialRequest(lLowVoiceInput))
		//		BuildGeneralAnswer(lLowVoiceInput);
		//}

		//private void ErrorSTT(STTError iError)
		//{
		//	if (iError == STTError.ERROR_NO_MATCH || iError == STTError.ERROR_SPEECH_TIMEOUT)
		//		return;

		//	if (mWithNotification)
		//		DisplayNotification("Error on vocal recognition", new Color32(230, 30, 30, 255));
		//	++mErrorCount;

		//	// If too much error (or no answer), ask for answer. If still no answer, get back to IDLE
		//	if (mErrorCount == 4) {
		//		TTSProcessAndSay("I can't hear you anymore ! [silence] Are you still there ?");
		//	} else if (mErrorCount == 6) {
		//		TTSProcessAndSay("I am sorry, I can't hear you");
		//	} else if (mErrorCount == 1) {
		//		string lSentence = "";

		//		switch (iError) {
		//			case STTError.ERROR_AUDIO: lSentence = "There is a microphone issue !"; break;
		//			case STTError.ERROR_NETWORK: lSentence = "There is a connection issue !"; break;
		//			case STTError.ERROR_RECOGNIZER_BUSY: lSentence = "Vocal recognition is already busy !"; break;
		//			default: lSentence = RandomString(mDidntUnderstandSpeech); break;
		//		}

		//		TTSProcessAndSay(lSentence);
		//	}
		//}

		public bool SpecialRequest(string iSpeech)
		{
			//Search for specific keywords and send the type through the delegate QuestionAnalyzed
			string lType = "";

            /////////////////////////
            //Apps
            ////////////////////////////


            if (ContainsOneOf(iSpeech, "alarm"))
                lType = "Alarm";
            //else if (ContainsOneOf(iSpeech, mQuizzSpeech))
            //	lType = "Quizz";
            else if (ContainsOneOf(iSpeech, "calculationgame"))
                lType = "Calcul";
            else if (ContainsOneOf(iSpeech, "memorygame"))
                lType = "Memory";
            //else if (ContainsOneOf(iSpeech, "babyphone"))
            //	lType = "Babyphone";
            else if (ContainsOneOf(iSpeech, "freezedance"))
                lType = "FreezeDance";
            else if (ContainsOneOf(iSpeech, "guardian"))
                lType = "Guardian";
            else if (ContainsOneOf(iSpeech, "iot"))
                lType = "IOT";
            else if (ContainsOneOf(iSpeech, "jukebox"))
                lType = "Jukebox";
            //else if (ContainsOneOf(iSpeech, mRecipeSpeech))
            //	lType = "Recipe";
            else if (ContainsOneOf(iSpeech, "rlgl"))
                lType = "RLGL";
            else if (ContainsOneOf(iSpeech, "reminder"))
                lType = "Reminder";
            //else if (ContainsOneOf(iSpeech, mHideSeekSpeech))
            //	lType = "HideSeek";
            else if (ContainsOneOf(iSpeech, "pose"))
                lType = "Pose";
            else if (ContainsOneOf(iSpeech, "photo"))
                lType = "Photo";
            else if (ContainsOneOf(iSpeech, "weather"))
                lType = "Weather";

            //else if (ContainsOneOf(iSpeech, "story"))
            //	lType = "Story";

            /////////////////////////
            //BML
            ////////////////////////////
            else if (ContainsOneOf(iSpeech, "demoshort"))
                lType = "DemoShort";
            else if (ContainsOneOf(iSpeech, "demofull"))
                lType = "DemoFull";
            else if (ContainsOneOf(iSpeech, "dance"))
                lType = "Dance";
            else if (ContainsOneOf(iSpeech, "iloveyou") || ContainsOneOf(iSpeech, "kissme"))
                lType = "UserLove";
            else if (ContainsOneOf(iSpeech, "ihateyou"))
                lType = "UserHate";
            else if (ContainsOneOf(iSpeech, "welcome"))
                lType = "Welcome";
            else if (ContainsOneOf(iSpeech, mJokeSpeech) || ContainsOneOf(iSpeech, "knockknock"))
            {
                Debug.Log("Vocal helper joke");
                if (iSpeech.ToLower().Contains(BYOS.Instance.Dictionary.GetString("i")) || ContainsOneOf(iSpeech, "knockknock"))
                    lType = "ListenJoke";
                else
                    lType = "Joke";








            }
            else if (ContainsOneOf(iSpeech, "accept"))
                lType = "Accept";
            else if (ContainsOneOf(iSpeech, mQuitSpeech))
                lType = "Quit";
            else if (ContainsOneOf(iSpeech, mRepeatAfterMeSpeech))
            {
                int lKeywordsIndex = WordIndexOfOneOf(iSpeech, mRepeatAfterMeSpeech);
                string[] lWords = iSpeech.Split(' ');
                string lSentenceToRepeat = "";

                if (lKeywordsIndex != -1 && lKeywordsIndex != lWords.Length)
                {
                    for (int j = lKeywordsIndex + 1; j < lWords.Length; j++)
                        lSentenceToRepeat += lWords[j] + " ";
                }
                lType = "Answer";
                Answer = lSentenceToRepeat;
                //TTSProcessAndSay(lSentenceToRepeat, true);
                //mAnswerGiven = true;
            }
            else if (ContainsOneOf(iSpeech, mRepeatPlzSpeech))
            {
                //TTSProcessAndSay(mPreviousAnswer, true);
                //mAnswerGiven = true;
                lType = "Repeat";
            }
            else if (ContainsOneOf(iSpeech, "mood"))
            {
                Debug.Log("vocalHelper mood");
                lType = "Mood";

            }
            else if (ContainsOneOf(iSpeech, "definition"))
            {
                lType = "Definition";
                //We search for the location of the weather request
                int lKeywordIndex = WordIndexOfOneOf(iSpeech, mDefinitionSpeech);
                string[] lWords = iSpeech.Split(' ');
                string lDefinitionWord = "";

                if (lKeywordIndex != -1 && lKeywordIndex != lWords.Length)
                {
                    for (int j = lKeywordIndex + 1; j < lWords.Length; j++)
                        lDefinitionWord += lWords[j] + " ";
                }
                StartCoroutine(BuildDefinitionAnswer(lDefinitionWord));
            }
            else if (ContainsOneOf(iSpeech, mWhoIs))
            {
                Debug.Log("Contains who is");
                //We search for the location of the weather request
                int lKeywordIndex = WordIndexOfOneOf(iSpeech, mWhoIs);
                string[] lWords = iSpeech.Split(' ');
                if (lKeywordIndex + 1 < lWords.Length)
                {
                    if (char.IsUpper(lWords[lKeywordIndex + 1][0]))
                    {
                        Debug.Log("next is upper: " + lWords[lKeywordIndex + 1]);
                        lType = "Definition";
                        string lDefinitionWord = "";

                        if (lKeywordIndex != -1 && lKeywordIndex != lWords.Length)
                        {
                            for (int j = lKeywordIndex + 1; j < lWords.Length; j++)
                                lDefinitionWord += lWords[j] + " ";
                        }
                        StartCoroutine(BuildDefinitionAnswer(lDefinitionWord));
                    }
                    else
                    {

                        Debug.Log("next is not upper: " + lWords[lKeywordIndex + 1]);
                    }
                }

                // General answer if not common name
                if (lType != "Definition")
                {
                    lType = "Answer";
                    Answer = BuildGeneralAnswer(iSpeech.ToLower());
                }

            }
            else if (ContainsOneOf(iSpeech, "buddylab"))
                lType = "BuddyLab";
            else if (ContainsOneOf(iSpeech, "timer"))
                lType = "Timer";
            else if (ContainsOneOf(iSpeech, mFollowMeSpeech))
                lType = "FollowMe";
            else if (ContainsOneOf(iSpeech, mLookAtMeSpeech))
                lType = "LookAtMe";
            else if (ContainsOneOf(iSpeech, "heat"))
                lType = "Heat";
            else if (ContainsOneOf(iSpeech, "colourseen"))
                lType = "ColourSeen";
            else if (ContainsOneOf(iSpeech, "detectobject"))
                lType = "DetectObject";
            else if (ContainsOneOf(iSpeech, "mirror"))
                lType = "Mirror";
            else if (ContainsOneOf(iSpeech, "nap"))
                lType = "Nap";
            else if (ContainsOneOf(iSpeech, "copy"))
                lType = "Copy";
            else if (ContainsOneOf(iSpeech, "connection"))
                lType = "Connection";
            else if (ContainsOneOf(iSpeech, mWanderSpeech))
            {
                Answer = FindMood(iSpeech.ToLower());
                lType = "Wander";
            }
            else if (ContainsOneOf(iSpeech, "canmove"))
                lType = "CanMove";
            else if (ContainsOneOf(iSpeech, mDontMoveSpeech))
                lType = "DontMove";
            else if (ContainsOneOf(iSpeech, mHeadDownSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "HeadDown";
            }
            else if (ContainsOneOf(iSpeech, mHeadLeftSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "HeadLeft";
            }
            else if (ContainsOneOf(iSpeech, mHeadRightSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "HeadRight";
            }
            else if (ContainsOneOf(iSpeech, mHeadUpSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "HeadUp";
            }
            else if (ContainsOneOf(iSpeech, mTurnSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                if (iSpeech.ToLower().Contains(BYOS.Instance.Dictionary.GetString("head")))
                    lType = "Head";
                else
                    lType = "Move";

                if (ContainsOneOf(iSpeech, mLeftSpeech))
                    lType += "Left";
                else
                    lType += "Right";
            }
            else if (ContainsOneOf(iSpeech, mMoveBackwardSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "MoveBackward";
            }
            else if (ContainsOneOf(iSpeech, mMoveForwardSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "MoveForward";
            }
            else if (ContainsOneOf(iSpeech, mMoveLeftSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "MoveLeft";
            }
            else if (ContainsOneOf(iSpeech, mMoveRightSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "MoveRight";
            }
            else if (ContainsOneOf(iSpeech, "battery"))
            {
                lType = "Battery";
            }
            else if (ContainsOneOf(iSpeech, mVolumeSpeech))
            {
                Answer = "" + BYOS.Instance.Primitive.Speaker.GetVolume();
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "Volume";
            }
            else if (ContainsOneOf(iSpeech, mVolumeDownSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "VolumeDown";
            }
            else if (ContainsOneOf(iSpeech, mVolumeUpSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "VolumeUp";
                //} else if (ContainsOneOf(iSpeech, "switchlanguage")) {
                //	Answer = "";
                //	if (ContainsOneOf(iSpeech, "english"))
                //		Answer = "English";
                //	else if (ContainsOneOf(iSpeech, "french"))
                //		Answer = "French";
                //	else if (ContainsOneOf(iSpeech, "Italian"))
                //		Answer = "Italian";

                //	lType = "SwitchLanguage";
            }
            else if (ContainsOneOf(iSpeech, mVolumeSpeech))
            {
                Answer = "" + BYOS.Instance.Primitive.Speaker.GetVolume();
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "Volume";
            }
            else if (ContainsOneOf(iSpeech, mVolumeDownSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "VolumeDown";
            }
            else if (ContainsOneOf(iSpeech, mVolumeUpSpeech))
            {
                Answer = GetNextNumber(iSpeech);
                Debug.Log("Vocal helper answer: " + Answer);
                lType = "VolumeUp";
            }
            else if (ContainsOneOf(iSpeech, mThanksSpeech))
            {
                //TTSProcessAndSay(RandomString(mURWelcomeSpeech));
                lType = "Answer";
                Answer = RandomString(mURWelcomeSpeech);
            }
            else if (ContainsOneOf(iSpeech, "date"))
            {
                lType = "Date";
                //TTSProcessAndSay("Today, we are the  " + DateTime.Now.Day +
                //" " + DateTime.Now.Month +
                //		" " + DateTime.Now.Year, true);
            }
            else if (ContainsOneOf(iSpeech, mHourSpeech))
            {
                lType = "Hour";
                //TTSProcessAndSay("After the third beep, it will exactly be " +
                //	DateTime.Now.Hour + " hours " +
                //	DateTime.Now.Minute + " minutes and " +
                //	DateTime.Now.Second + " seconds ", true);
                //mTTS.Silence(1000, true);
                //TTSProcessAndSay("beep", true);
                //mTTS.Silence(1000, true);
                //TTSProcessAndSay("beep", true);
                //mTTS.Silence(1000, true);
                //TTSProcessAndSay("and beep", true);

            }
            else if (ContainsOneOf(iSpeech, mPlaySpeech))
                lType = "Play";
            else if (ContainsOneOf(iSpeech, "tellsomething"))
                lType = "TellSomething";
            else if (ContainsOneOf(iSpeech, mDoSomethingSpeech))
                lType = "DoSomething";
            else if (iSpeech.ToLower().Contains("propose"))
                //lType = Suggest();
                lType = "propose";
            //else if (iSpeech.Contains("cleverbot")) {
            //	lType = "Cleverbot";

            //	if (mChatBotRequested) {
            //		TTSProcessAndSay("Cleverbot disabled", true);
            //		mChatBotRequested = false;
            //	} else {
            //		TTSProcessAndSay("Cleverbot enabled", true);
            //		mChatBotRequested = true;
            //		InitChatBot();
            //	}
            else if (!iSpeech.Any(c => char.IsDigit(c)))
            {
                lType = "Answer";
                Answer = BuildGeneralAnswer(iSpeech.ToLower());
            }
            else
            {

                string lSpeech = iSpeech.Trim();
                //lSpeech = lSpeech.Replace("x", "*");
                //lSpeech = lSpeech.Replace("÷", "/");


                //if (lSpeech.Contains("√")) {
                //	lSpeech = lSpeech.Replace("√", "sqrt");
                //	lSpeech = Regex.Replace(lSpeech, @"\d", "($0)").Replace("sqrt ", "sqrt");
                //}

                Debug.Log("pre: " + lSpeech);
                string pattern = @"(\s?)(\d+\.?((?<=\.)\d+)?)";
                Regex rgx = new Regex(pattern);
                lSpeech = rgx.Replace(lSpeech, "($2)");

                //lSpeech = Regex.Replace(lSpeech, @"\d+\.\d+", "($0)");
                var parser = new ExpressionParser();

                Debug.Log("post: " + lSpeech);




                try
                {
                    Expression exp = parser.EvaluateExpression(lSpeech);
                    Debug.Log("Operation " + iSpeech);
                    lType = "Operation";
                }
                catch
                {
                    lType = "Answer";
                    Answer = BuildGeneralAnswer(iSpeech.ToLower());
                }
                //if () {
                //	Debug.Log("Operation? " + iSpeech);
                //	lType = "Operation";
                //} else {
                //	lType = "Answer";
                //	Answer = BuildGeneralAnswer(iSpeech.ToLower());
                //}
            }

			OnQuestionTypeFound(lType);
			return true;
		}

		private string FindMood(string iSpeech)
		{
			if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("sad")))
				return MoodType.SAD.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("happy")))
				return MoodType.HAPPY.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("lovely")))
				return MoodType.LOVE.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("sick")))
				return MoodType.SICK.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("tired")))
				return MoodType.TIRED.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("angry")))
				return MoodType.ANGRY.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("grumpy")))
				return MoodType.GRUMPY.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("scared")))
				return MoodType.SCARED.ToString();
			else
				return MoodType.NEUTRAL.ToString(); ;
		}

		//private string GetNextNumber(string iText, List<string> iSpeech)
		//{
		//	string lResult = "";
		//	int lKeywordsIndex = WordIndexOfOneOf(iText, iSpeech);
		//	string[] lWords = iText.Split(' ');
		//	float n = 0F;

		//	if (lKeywordsIndex == -1) {
		//		return lResult;
		//	} else if (lKeywordsIndex != lWords.Length) {
		//		for (int j = lKeywordsIndex + 1; j < lWords.Length; j++)
		//			if (float.TryParse(lWords[j], out n)) {
		//				if (!iSpeech.Contains(" meter") && !iSpeech.Contains(" mètre") && ((iSpeech.Contains("centimeter") || iSpeech.Contains("centimètre") || iSpeech.Contains(" cm"))))
		//					lResult = "" + n / 100;
		//				else
		//					lResult = lWords[j];
		//				break;
		//			} else if (float.TryParse(lWords[j].Remove(lWords[j].Length - 2), out n) && lWords[j][lWords[j].Length - 2] == 'c' && lWords[j][lWords[j].Length - 1] == 'm')
		//				lResult = "" + n / 100;
		//	}
		//	return lResult;
		//}

		private string GetNextNumber(string iText)
		{
			if (!string.IsNullOrEmpty(iText)) {
				string[] lWords = iText.Split(' ');
				for(int i=0; i<lWords.Length; ++i) {
					try {
						float.Parse(lWords[i]);
						return lWords[i];
					}

					catch {
						continue;
					}
				}
			}
			return "";
		}

		private string BuildGeneralAnswer(string iData)
		{
			//In case there is no special keyword, look through the pre-answered questions from the question file

			string lFormatedData = Regex.Replace(iData, @"[^\w\s]", " ");
			//Debug.Log("BuildGeneralAnswer - ponctu " + lFormatedData);
			string lAnswer = "";

			string[] lWords = lFormatedData.Split(' ');
			//Debug.Log("Looking for input " + lFormatedData);

			using (XmlReader lReader = XmlReader.Create(new StringReader(mQuestionsFile))) {
				while (lReader.ReadToFollowing("QA")) {
					lReader.ReadToFollowing("question");
					//Remove ponctuation
					string lContentQ = Regex.Replace(lReader.ReadElementContentAsString().ToLower(), @"[^\w\s]", " ");

					if (lContentQ.Contains(lFormatedData)) {
						Debug.Log("Found Content Question : " + lContentQ);
						bool lFoundInput = true;

						//We need to check for single input words for some reason, otherwise it doesn't work ...
						if (lWords.Length < 2) {
							//Debug.Log("Input corresponds to only one word");
							lFoundInput = false;
							string[] lQuestions = lContentQ.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
							for (int i = 0; i < lQuestions.Length && !lFoundInput; ++i) {
								if (lQuestions[i] == lFormatedData) {
									lFoundInput = true;
								}
							}
						}

						if (lFoundInput) {
							lReader.ReadToFollowing("answer");
							string lContentA = lReader.ReadElementContentAsString();
							string[] lAnswers = lContentA.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

							if (lAnswers.Length == 1)
								lAnswer = lAnswers[0];
							else {
								System.Random lRnd = new System.Random();
								lAnswer = lAnswers[lRnd.Next(0, lAnswers.Length)];
							}
							Debug.Log("Found  Content Answer : " + lAnswer);
						}
						break;
					}
				}
			}

			//if (string.IsNullOrEmpty(lAnswer)) {
			//	if (!mChatBotRequested || mCleverbotSession == null) {
			//		lAnswer = RandomString(mSorrySpeech) + " " +
			//			RandomString(mICouldntSpeech) + " " +
			//			RandomString(mGetSpeech) + " " +
			//			RandomString(mAnswersSpeech);
			//		OnQuestionTypeFound("");
			//		Debug.Log("CleverBot : Not activated with answer : " + lAnswer);
			//	} else {
			//		lAnswer = mCleverbotSession.Think(iData);
			//		Debug.Log("CleverBot answer : " + lAnswer);
			//	}
			//}

			return lAnswer;
		}

		private IEnumerator BuildWeatherAnswer(string iData)
		{

			mBuildingAnswer = true;
			//Weather answer happens in two part :
			//First we get the location of the city (if provided) through the googleapi in latitude longitude
			//If not provided, we get the info through the geolocation of the tablet (else, default is Paris temperature)
			//Then make the actual request of weather
			string lXmlData = "";
			string lKeyword = "48.853,2.35";

			if (!string.IsNullOrEmpty(iData)) {
				WWW lWww = new WWW("http://maps.googleapis.com/maps/api/geocode/xml?address=" + iData);

				float lElapsedTime = 0.0f;
				while (!lWww.isDone) {
					lElapsedTime += Time.deltaTime;

					if (lElapsedTime >= 5f)
						break;

					yield return new WaitForSeconds(0.5f);
				}

				if (lWww.isDone && string.IsNullOrEmpty(lWww.error)) {
					string lXml = lWww.text;
					using (XmlReader lReader = XmlReader.Create(new StringReader(lXml))) {
						if (lReader.ReadToFollowing("location")) {
							lReader.ReadToFollowing("lat");
							lReader.Read();
							lKeyword = lReader.ReadContentAsString();
							lReader.ReadToFollowing("lng");
							lReader.Read();
							lKeyword += "," + lReader.ReadContentAsString();
						}
						lReader.Close();
					}
				}
			} else if (Input.location.isEnabledByUser) {
				Input.location.Start();

				int lmaxWait = 7;
				while (Input.location.status == LocationServiceStatus.Initializing && lmaxWait > 0) {
					yield return new WaitForSeconds(1f);
					lmaxWait--;
					Debug.Log("Waiting for geoloc");
				}
				if (Input.location.status != LocationServiceStatus.Failed && lmaxWait > 0) {
					lKeyword = Input.location.lastData.latitude + "," + Input.location.lastData.longitude;
				}

				Input.location.Stop();
			}

			yield return StartCoroutine(MakeRequest(RequestType.WEATHER, lKeyword, null, value => lXmlData = value));

			//We take the answer from the weather website and extract the temperature as a string
			string lTemperature = "";
			WeatherInfo[] lInfos = new WeatherInfo[6];

			using (XmlReader lReader = XmlReader.Create(new StringReader(lXmlData))) {
				while (lReader.ReadToFollowing("echeance")) {
					lReader.MoveToAttribute("timestamp");
					string lReadTime = lReader.Value.Substring(0, lReader.Value.Length - 4);
					if (DateTime.Now.CompareTo(Convert.ToDateTime(lReadTime)) < 0) {
						lReader.ReadToFollowing("level");
						lTemperature = lReader.ReadElementContentAsString();
						break;
					}
				}

				//Build the weather infos for the different times according to the weather notification format
				int lCount = 0;
				while (lCount < 6 && lReader.ReadToFollowing("echeance")) {
					lReader.MoveToAttribute("timestamp");
					string lReadTime = lReader.Value;
					string[] lReadTimeSplit = lReadTime.Split(' ');
					int lHour = 0;
					Int32.TryParse(lReadTimeSplit[1].Split(':')[0], out lHour);

					lReader.ReadToFollowing("level");
					string lTemp = lReader.ReadElementContentAsString();
					int lTempInt = 0;
					if (Int32.TryParse(lTemp.Split('.')[0], out lTempInt))
						lTempInt -= 274;

					lReader.ReadToFollowing("pluie");
					int lRainLevel = 0;
					Int32.TryParse(lReader.ReadElementContentAsString(), out lRainLevel);

					lInfos[lCount] = new WeatherInfo {
						Hour = lHour,
						MaxTemperature = lTempInt,
						Type = lRainLevel > 3 ? WeatherType.RAIN : WeatherType.SUNNY
					};

					lCount++;
				}
				lReader.Close();
			}

			//This part is for the speech output of the temperature.
			string[] lsubstrings = lTemperature.Split('.');
			int loutValue = 0;
			string lFinalSentence = RandomString(mICouldntSpeech) +
				" " + RandomString(mGetSpeech) +
				" " + RandomString(mTemperatureSpeech);
			if (!string.IsNullOrEmpty(lTemperature) && Int32.TryParse(lsubstrings[0], out loutValue)) {
				loutValue = loutValue - 274;
				lFinalSentence = RandomString(mTempSpeech) + " " + loutValue.ToString() + " " + BYOS.Instance.Dictionary.GetRandomString("degrees");
				StartCoroutine(DisplayWeatherNot(loutValue, iData, lInfos));
			}
			Answer = lFinalSentence;
			mBuildingAnswer = false;
		}

		private IEnumerator BuildDefinitionAnswer(string iData)
		{
			mBuildingAnswer = true;
			string lXmlData = "";
			yield return StartCoroutine(MakeRequest(RequestType.DEFINITION, iData, null, value => lXmlData = value));
			//Get only the first sentence of wikipedia introduction as definition. And this is bad in a lot of cases
			//Starting from here, we have the result of the page with the intro of selected page, categories and outlinks
			string lAnswer = "";

			//Check if the result is a multiple answer page
			if (lXmlData.Contains("Catégorie:Homonymie") || lXmlData.Contains("Category:Disambiguation")) {
				Debug.Log("Definition contains multiple answers.");
				string lOutput = "";

				using (XmlReader lReader = XmlReader.Create(new StringReader(lXmlData))) {
					lReader.ReadToFollowing("links");

					while (lReader.ReadToFollowing("pl")) {
						lReader.MoveToAttribute("title");
						string lValue = lReader.Value;
						if (!lValue.Contains("Liste"))
							lOutput += lValue + ". ";
					}
					lReader.Close();
				}
				lAnswer = RandomString(mSeveralResSpeech) + " " + lOutput.ToString();
			} else {
				//We found the article of the desired research. We get the first sentence of the introduction text
				string lOutput = "";

				using (XmlReader lReader = XmlReader.Create(new StringReader(lXmlData))) {
					if (lReader.ReadToFollowing("extract")) {
						lOutput += lReader.ReadElementContentAsString();

						if (string.IsNullOrEmpty(lOutput.ToString())) {
							//lAnswer = RandomString(mICouldntSpeech) + " " +
							//    RandomString(mGetSpeech) + " " +
							//    RandomString(mAnswersSpeech);
							lReader.MoveToContent();
							//Debug.Log("After MoveToContent : " + lReader.ReadElementContentAsString());
							if (lReader.ReadToFollowing("pl")) {
								lReader.MoveToAttribute("title");
								string lValue = lReader.Value;
								//Debug.Log("Searching other definition : " + lValue);
								yield return BuildDefinitionAnswer(lValue);
								yield break;
							}
						} else {
							string lOutputString = lOutput.ToString();

							if (lOutputString.Length <= 100)
								lAnswer = lOutputString;
							else {
								int lIndex = lOutputString.IndexOf('.', 100);
								lAnswer = lOutputString.Substring(0, lIndex);
							}
						}
					} else
						lAnswer = RandomString(mICouldntSpeech) + " " +
							RandomString(mGetSpeech) + " " +
							BYOS.Instance.Dictionary.GetRandomString("answers");

					lReader.Close();
				}
			}

			mBuildingAnswer = false;

			Answer = lAnswer;
		}

		private IEnumerator MakeRequest(RequestType iType, string iKeyword, Dictionary<string, string> iHeader, Action<string> ioResult)
		{
			WWW lWww = new WWW(mWebsiteHash[iType] + Uri.EscapeUriString(iKeyword), null, iHeader);

			float lElapsedTime = 0.0f;
			while (!lWww.isDone) {
				lElapsedTime += Time.deltaTime;
				if (lElapsedTime >= 5f) break;
				yield return null;
			}
			if (!lWww.isDone || !string.IsNullOrEmpty(lWww.error)) {
				Debug.Log("Request error: " + lWww.error);
				ioResult(null);
				yield break;
			}
			ioResult(lWww.text);
		}

		private IEnumerator DisplayWeatherNot(int iTemp, string iLoc, WeatherInfo[] iInfo)
		{
			yield return new WaitForSeconds(1F);

			BYOS.Instance.Notifier.Display<WeatherNot>(5F).With(iTemp, "",
															DateTime.Now.ToString(),
															string.IsNullOrEmpty(iLoc) ? "" : iLoc,
															"",
															iInfo);
		}

		//private string Suggest()
		//{
		//	if (UnityEngine.Random.value > 0.4)
		//		return SuggestGame();
		//	else if (UnityEngine.Random.value > 0.2) {
		//		//TTSProcessAndSay("J'ai envie de te prendre en photo!", true);
		//		return "Photo";
		//		//link.animator.SetTrigger("Photo");
		//	} else {
		//		//TTSProcessAndSay("J'adore faire la star, prends moi en photo!", true);
		//		return "Pose";
		//		//link.animator.SetTrigger("Pose");
		//	}
		//}

		//private string SuggestGame()
		//{
		//	if (UnityEngine.Random.value > 0.8) {
		//		//TTSProcessAndSay("J'ai envie de poser des questions! Allez, faisons un quizz!", true);
		//		return "Quizz";
		//		//link.animator.SetTrigger("Quizz");
		//	} else if (UnityEngine.Random.value > 0.6) {
		//		//TTSProcessAndSay("J'ai envie de jouer! Faisons le test de mémoire!", true);
		//		return "Memory";
		//		//link.animator.SetTrigger("Memory");
		//	} else if (UnityEngine.Random.value > 0.4) {
		//		//TTSProcessAndSay("J'ai envie de tester tes facultés cognitives, faisons des calculs!", true);
		//		return "Calcul";
		//		//link.animator.SetTrigger("Calcul");
		//	} else if (UnityEngine.Random.value > 0.2) {
		//		//TTSProcessAndSay("J'adore le jeu des couleurs, allez, faisons une partie!", true);
		//		return "Colors";
		//		//link.animator.SetTrigger("Colors");
		//	} else {
		//		//TTSProcessAndSay("Allez, faisons un jeu ensemble !", true);
		//		return "Games";
		//		//link.animator.SetTrigger("Games");
		//	}
		//}

		private bool ContainsOneOf(string iSpeech, List<string> iListSpeech)
		{
			for (int i = 0; i < iListSpeech.Count; ++i) {
				string[] words = iListSpeech[i].Split(' ');
				if (words.Length < 2) {
					words = iSpeech.Split(' ');
					foreach (string word in words) {
						if (word.ToLower() == iListSpeech[i].ToLower().Trim()) {
							return true;
						}
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower().Trim()))
					return true;
			}
			return false;
		}


		private bool ContainsOneOf(string iSpeech, string iKeySpeech)
		{
			string[] iListSpeech = BYOS.Instance.Dictionary.GetPhoneticStrings(iKeySpeech);


			for (int i = 0; i < iListSpeech.Length; ++i) {

				if (string.IsNullOrEmpty(iListSpeech[i]))
					continue;

				string[] words = iSpeech.Split(' ');
				if (words.Length < 2 && !string.IsNullOrEmpty(words[0])) {
					if (words[0].ToLower() == iListSpeech[i].ToLower().Trim()) {
						return true;
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower().Trim()))
					return true;
			}
			return false;
		}

		private int WordIndexOfOneOf(string iSpeech, List<string> iListSpeech)
		{
			//Returns the index of one of the words of iListSpeech keywords in the speech iSpeech
			for (int i = 0; i < iListSpeech.Count; ++i) {
				string[] lWords = iListSpeech[i].Split(' ');
				string lKeyword = lWords[lWords.Length - 1];
				string[] lSpeechWords = iSpeech.Split(' ');
				for (int j = 0; j < lSpeechWords.Length; j++) {
					if (lSpeechWords[j] == lKeyword)
						return j;
				}
			}
			return -1;
		}

		private string RandomString(List<string> iListStr)
		{
			if (iListStr.Count == 0) {
				Debug.Log("the following list is empty!!! " + iListStr.ToString());
			}
			System.Random lRnd = new System.Random();
			return iListStr[lRnd.Next(0, iListStr.Count)];
		}

		private void DisplayNotification(string iStr, Color32 iColor)
		{
			BYOS.Instance.Notifier.Display<SimpleNot>().With(iStr,
							BYOS.Instance.Resources.GetSpriteFromAtlas(ResourceManager.MESSAGE_SPRITE_NAME),
							iColor);
		}
	}
}
