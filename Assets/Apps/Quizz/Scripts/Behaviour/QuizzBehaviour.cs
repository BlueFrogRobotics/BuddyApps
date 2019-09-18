using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Quizz
{
    public class Player
    {
        public int Score { get; set; }
        public string Name { get; set; }

        public Player(string iName, int iScore)
        {
            Score = iScore;
            Name = iName;
        }
    }

    /* A basic monobehaviour as "AI" behaviour for your app */
    public class QuizzBehaviour : MonoBehaviour
    {
        private int mNumPlayer = 1;

        /// <summary>
        /// Number of players in the quizz
        /// </summary>
        public int NumPlayer
        {
            get { return mNumPlayer; }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 4)
                    value = 4;
                else
                    mNumPlayer = value;
            }
        }

        /// <summary>
        /// Contains the list of players possible nickname retrieved from the xml
        /// </summary>
        public PlayerNamesData PlayerNamesData { get; private set; }

        /// <summary>
        /// The list of players in the game
        /// </summary>
        public List<Player> Players { get; private set; }

        /// <summary>
        /// Contains all the possible questions of the quizz retrieved from the xml
        /// </summary>
        public QuestionsData Questions { get; private set; }

        /// <summary>
        /// The question which will be asked
        /// </summary>
        public QuestionData ActualQuestion { get; set; }

        /// <summary>
        /// The number of the player who will answer now
        /// </summary>
        public int ActualPlayerId { get; set; }

        /// <summary>
        /// Number of the actual round
        /// </summary>
        public int ActualRound { get; set; }

        /// <summary>
        /// Tells if the game has started
        /// </summary>
        public bool Beginning { get; set; }

        /// <summary>
        /// List of the id of the questions that have already been asked
        /// </summary>
        public HashSet<int> ListQuestionsIdAsked { get; set; }

        /// <summary>
        /// Return the last state id when entering the ask exit state
        /// </summary>
        public int LastStateId { get; set; }

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
        public const int MAX_ROUNDS = 3;

        /// <summary>
        /// Maximum number of players
        /// </summary>
        public const int MAX_PLAYER = 4;

        /*
         * Data of the application. Save on disc when app is quitted
         */
        private QuizzData mAppData;

        private Language mCurrentLanguage;

        void Start()
        {
            /*
			* You can setup your App activity here.
			*/
            QuizzActivity.Init(null);
            
            //Debug.Log("QuizzApp language " + Buddy.Platform.Language.OutputLanguage + " lang: " + Buddy.Platform.Language.OutputLanguage.ISO6391Code.ToString());
            
            /*
			* Init your app data
			*/
            mAppData = QuizzData.Instance;


            if (mAppData.NbQuestions == 0 || mAppData.NbQuestions > MAX_ROUNDS)
                mAppData.NbQuestions = MAX_ROUNDS;

            string playerFileName = Buddy.Resources.AppRawDataPath + "player_names_" + Lang + ".xml";
            try
            {
                PlayerNamesData = Utils.UnserializeXML<PlayerNamesData>(playerFileName);
                Debug.Log("QuizzApp: success parsing file " + playerFileName);
            }
            catch (NullReferenceException e)
            {
                Debug.LogError("QuizzApp: error parsing file " + playerFileName + " : " + e.Message);
            }

            string quizzName = "quizz";
            if (!string.IsNullOrEmpty(mAppData.QuizzFileName))
                quizzName = mAppData.QuizzFileName;
            string questionFileName = Buddy.Resources.AppRawDataPath + quizzName + "_" + Lang + ".xml";
            try
            {
                Questions = Utils.UnserializeXML<QuestionsData>(questionFileName);
                Debug.Log("QuizzApp nb questions " + Questions.Questions.Count);
            }
            catch (NullReferenceException e)
            {
                Debug.LogError("QuizzApp: error parsing file " + questionFileName + " : " + e.Message);
            }


            //mCurrentLanguage = Buddy.Platform.Language.OutputLanguage;         

        }

        private void Update()
        {
            //if (Buddy.Platform.Language.OutputLanguage != mCurrentLanguage && OnLanguageChange != null)
            //{
            //    OnLanguageChange();
            //    mCurrentLanguage = Buddy.Platform.Language.OutputLanguage;
            //}
        }

        /// <summary>
        /// Initilize the player and round id
        /// </summary>
        public void Init()
        {
            ActualPlayerId = 0;
            ActualRound = 0;
            ListQuestionsIdAsked = new HashSet<int>();
            Beginning = true;
            ListQuestionsIdAsked = new HashSet<int>();
            Players = new List<Player>();
            InitPlayers();
        }

        /// <summary>
        /// Retrive a random nickname for each player from the list
        /// </summary>
        public void InitPlayers()
        {
            Players.Clear();

            System.Random random = new System.Random();
            HashSet<int> hashset = new HashSet<int>();
            while (hashset.Count < NumPlayer)
            {
                int lId = random.Next(PlayerNamesData.Names.Count);
                hashset.Add(lId);
            }

            foreach (int hash in hashset)
            {
                string lName = PlayerNamesData.Names[hash];
                Players.Add(new Player(lName, 0));
            }
        }

        /// <summary>
        /// Used to convert the enum theme into a string
        /// </summary>
        /// <param name="iTheme">the theme</param>
        /// <returns>the string in the actual language</returns>
        public string ThemeToString(Theme iTheme)
        {
            string lText = "";
            switch (iTheme)
            {
                case Theme.CINEMA:
                    lText = Buddy.Resources.GetString("cinema");
                    break;
                case Theme.GENERAL_KNOWLEDGE:
                    lText = Buddy.Resources.GetString("generalknowledge");
                    break;
                case Theme.GEOGRAPHY:
                    lText = Buddy.Resources.GetString("geography");
                    break;
                case Theme.HISTORY:
                    lText = Buddy.Resources.GetString("history");
                    break;
                case Theme.MUSIC:
                    lText = Buddy.Resources.GetString("music");
                    break;
                case Theme.NATURE:
                    lText = Buddy.Resources.GetString("nature");
                    break;
                case Theme.SPORT:
                    lText = Buddy.Resources.GetString("sport");
                    break;
                case Theme.TV_SHOW:
                    lText = Buddy.Resources.GetString("tvshow");
                    break;
                default:
                    break;
            }
            return lText;
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
    }
}