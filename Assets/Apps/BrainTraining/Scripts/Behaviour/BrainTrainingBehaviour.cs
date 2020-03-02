using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.BrainTraining
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class BrainTrainingBehaviour : MonoBehaviour
    {
        [SerializeField]
        internal Text TextUI;

        [SerializeField]
        internal GameObject TextBackground;

        /// <summary>
        /// Contains all the possible questions of the quizz retrieved from the xml
        /// </summary>
        public CategoriesData Categories { get; private set; }

        /// <summary>
        /// Contains all the possible questions of the quizz retrieved from the xml
        /// </summary>
        public QuestionsData Questions { get; private set; }

        /// <summary>
        /// The question which will be asked
        /// </summary>
        public QuestionData ActualQuestion { get; set; }


        /// <summary>
        /// List of the id of the questions that have already been asked
        /// </summary>
        public HashSet<int> ListQuestionsIdAsked { get; set; }

        /// <summary>
        /// Return the actual language abreviation in lower case
        /// </summary>
        public string Lang { get { return Buddy.Platform.Language.OutputLanguage.ISO6391Code.ToString().ToLower(); } }

        /// <summary>
        /// Event that triggers when the language of buddy changes
        /// </summary>
        public Action OnLanguageChange;

        /// <summary>
        /// Maximum number of rounds
        /// </summary>
        public const int MAX_ROUNDS = 10;

        /// <summary>
        /// Maximum number of categories
        /// </summary>
        public const int MAX_CATEGORIES = 5;

        /*
            * Data of the application. Save on disc when app is quitted
            */
        private BrainTrainingData mAppData;

        private Language mCurrentLanguage;

        private bool mSuccess;

        private List<string> mQuestionsFiles;

        private int mCurrentGameIndex;

        void Start()
        {
            /*
            * You can setup your App activity here.
            */
            BrainTrainingActivity.Init(null);

            // Enable echocancellation
            Buddy.Sensors.Microphones.EnableEchoCancellation = true;

            //Debug.Log("QuizzApp language " + Buddy.Platform.Language.OutputLanguage + " lang: " + Buddy.Platform.Language.OutputLanguage.ISO6391Code.ToString());

            /*
            * Init your app data
            */
            mAppData = BrainTrainingData.Instance;

            if (mAppData.NbQuestions == 0 || mAppData.NbQuestions > MAX_ROUNDS)
                mAppData.NbQuestions = MAX_ROUNDS;
            if (mAppData.NbCategories == 0 || mAppData.NbCategories > MAX_CATEGORIES)
                mAppData.NbCategories = MAX_CATEGORIES;

            ListQuestionsIdAsked = new HashSet<int>();
            mQuestionsFiles = new List<string>();
            mCurrentGameIndex = 0;
            mSuccess = false;

            //mCurrentLanguage = Buddy.Platform.Language.OutputLanguage;         
        }

        /// <summary>
        /// Load available categories from the categories configuration file
        /// </summary>
        public void LoadCategories()
        {
            string filename = "categories";
            //if (!string.IsNullOrEmpty(mAppData.QuizzFileName))
            //    quizzName = mAppData.QuizzFileName;
            //string questionFileName = Buddy.Resources.AppRawDataPath + filename + "_" + Lang + ".xml";
            string categoriesFileName = Buddy.Resources.AppRawDataPath + filename + "_" + "fr" + ".xml";
            try
            {
                Categories = Utils.UnserializeXML<CategoriesData>(categoriesFileName);
            }
            catch (NullReferenceException e)
            {
                Debug.LogError("BrainTraining: error parsing file " + categoriesFileName + " : " + e.Message);
            }
        }

        /// <summary>
        /// Update the categories configuration file
        /// </summary>
        public void SaveCategories()
        {
            string filename = "categories";
            //if (!string.IsNullOrEmpty(mAppData.QuizzFileName))
            //    quizzName = mAppData.QuizzFileName;
            //string questionFileName = Buddy.Resources.AppRawDataPath + filename + "_" + Lang + ".xml";
            string categoriesFileName = Buddy.Resources.AppRawDataPath + filename + "_" + "fr" + ".xml";
            try
            {
                Utils.SerializeXML<CategoriesData>(Categories, categoriesFileName);
            }
            catch (NullReferenceException e)
            {
                Debug.LogError("BrainTraining: error writing file " + categoriesFileName + " : " + e.Message);
            }
        }

        /// <summary>
        /// Create a random list of question files according to the categories selected
        /// </summary>
        private void CreateQuestionList()
        {
            mQuestionsFiles.Clear();
            List<int> categoryIndexList = new List<int>();
            int nbCategories = BrainTrainingData.Instance.NbCategories;

            // Check if there are enough categories activated
            int nbActiveCategories = 0;
            for (int i = 0; i < Categories.Categories.Count; i++)
            {
                if (Categories.Categories[i].IsActive)
                    nbActiveCategories++;
            }
            if (nbActiveCategories < nbCategories)
                nbCategories = nbActiveCategories;

            while (mQuestionsFiles.Count < nbCategories)
            {
                int i = 0;
                do
                {
                    i = UnityEngine.Random.Range(0, Categories.Categories.Count);
                }
                while (categoryIndexList.Contains(i));
                categoryIndexList.Add(i);

                CategoryData data = Categories.Categories[i];
                if (data.IsActive && data.Files.Count > 0)
                {
                    int index = UnityEngine.Random.Range(0, data.Files.Count);
                    mQuestionsFiles.Add(data.Files[index]);
                }
            }
        }

        /// <summary>
        /// Start a new quizz
        /// </summary>
        public void StartNewQuizz()
        {
            // Reset
            ListQuestionsIdAsked.Clear();

            // Load new quizz
            LoadQuizz();
        }

        /// <summary>
        /// Load a new set of questions
        /// </summary>
        private void LoadQuizz()
        {
            if (IsGameOver())
            {
                return;
            }
            string filename = mQuestionsFiles[mCurrentGameIndex];
            mCurrentGameIndex++;
            string quizzName = filename;
            //if (!string.IsNullOrEmpty(mAppData.QuizzFileName))
            //    quizzName = mAppData.QuizzFileName;
            //string questionFileName = Buddy.Resources.AppRawDataPath + quizzName + "_" + Lang + ".xml";
            string questionFileName = Buddy.Resources.AppRawDataPath + quizzName + "_" + "fr" + ".xml";
            try
            {
                Questions = Utils.UnserializeXML<QuestionsData>(questionFileName);
            }
            catch (NullReferenceException e)
            {
                Debug.LogError("BrainTraining: error parsing file " + questionFileName + " : " + e.Message);
            }
        }

        public void CheckAnswer(int lAnswerId)
        {
            mSuccess = (ActualQuestion.GoodAnswer == lAnswerId);
        }

        public bool IsSuccess()
        {
            return mSuccess;
        }

        /// <summary>
        /// Build a new list of question files according to the selected categories
        /// </summary>
        public void ResetQuestionList()
        {
            mCurrentGameIndex = 0;
            CreateQuestionList();
        }

        public bool IsFirstGame()
        {
            return (mCurrentGameIndex == 1);
        }

        public bool IsGameOver()
        {
            return (mCurrentGameIndex >= mQuestionsFiles.Count);
        }

        /// <summary>
        /// Removes the special characters in a string
        /// </summary>
        /// <param name="iStr">the string which has special characters</param>
        /// <returns>the string without special characters</returns>
        public string RemoveSpecialCharacters(string iStr)
        {
            char[] lSeparators = new char[] { ';', ',', '~', '?', '!', ':', '/', '*', '(', ')', '|', '[', ']', '<', '>', '%', '^' };

            string[] lTemp = iStr.Split(lSeparators, StringSplitOptions.None);
            string lReturn = String.Join(" ", lTemp);
            return lReturn;
        }        

        public void ShowText(string text)
        {
            TextUI.gameObject.SetActive(true);
            TextBackground.SetActive(true);
            TextUI.text = text;
        }

        public void HideText()
        {
            TextUI.gameObject.SetActive(false);
            TextBackground.SetActive(false);
        }
    }
}