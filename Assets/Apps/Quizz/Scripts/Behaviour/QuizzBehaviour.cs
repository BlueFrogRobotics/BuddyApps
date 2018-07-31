using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

    /// <summary>
    /// Classes that is used mainly to expose some global variables used in the states
    /// </summary>
    public class QuizzBehaviour : MonoBehaviour
    {

        private int mNumPlayer = 0;

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
        public string Lang { get { return BYOS.Instance.Language.CurrentLang.ToString().ToLower(); } }

        /// <summary>
        /// Event that triggers when the language of buddy changes
        /// </summary>
        public Action OnLanguageChange;

        /// <summary>
        /// Maximum number of rounds
        /// </summary>
        public const int MAX_ROUNDS = 5;

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
            //BYOS.Instance.Interaction.BMLManager.LaunchByName("Sad02");
            Debug.Log("format: " + BYOS.Instance.Language.CurrentFormat + " lang: " + BYOS.Instance.Language.CurrentLang.ToString());
            Debug.Log(RemoveSpecialCharacters("les chips c'est émouvant (lol) à manger"));
            /*
			* Init your app data
			*/
            mAppData = QuizzData.Instance;
            Debug.Log("player truc: " + BYOS.Instance.Resources.GetPathToRaw("player_names_" + Lang + ".xml"));
            PlayerNamesData = Utils.UnserializeXML<PlayerNamesData>(BYOS.Instance.Resources.GetPathToRaw("player_names_" + Lang + ".xml"));
            foreach (string player in PlayerNamesData.Names)
            {
                Debug.Log(player);
            }

            Debug.Log("avant deserialisation: ");
            Questions = Utils.UnserializeXML<QuestionsData>(BYOS.Instance.Resources.GetPathToRaw("quizz_" + Lang + ".xml"));
            Debug.Log("nombre de questions: " + Questions.Questions.Count);
            ListQuestionsIdAsked = new HashSet<int>();
            mCurrentLanguage = BYOS.Instance.Language.CurrentLang;


        }

        private void Update()
        {
            if (BYOS.Instance.Language.CurrentLang != mCurrentLanguage && OnLanguageChange != null)
            {
                OnLanguageChange();
                mCurrentLanguage = BYOS.Instance.Language.CurrentLang;
            }
        }

        /// <summary>
        /// Initilize the player and round id
        /// </summary>
        public void Init()
        {
            ActualPlayerId = 0;
            ActualRound = 0;
            ListQuestionsIdAsked = new HashSet<int>();
        }

        /// <summary>
        /// Retrive a random nickname for each player from the list
        /// </summary>
        public void InitPlayers()
        {
            Players = new List<Player>();

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
                    lText = BYOS.Instance.Dictionary.GetString("cinema");
                    break;
                case Theme.GENERAL_KNOWLEDGE:
                    lText = BYOS.Instance.Dictionary.GetString("generalknowledge");
                    break;
                case Theme.GEOGRAPHY:
                    lText = BYOS.Instance.Dictionary.GetString("geography");
                    break;
                case Theme.HISTORY:
                    lText = BYOS.Instance.Dictionary.GetString("history");
                    break;
                case Theme.MUSIC:
                    lText = BYOS.Instance.Dictionary.GetString("music");
                    break;
                case Theme.NATURE:
                    lText = BYOS.Instance.Dictionary.GetString("nature");
                    break;
                case Theme.SPORT:
                    lText = BYOS.Instance.Dictionary.GetString("sport");
                    break;
                case Theme.TV_SHOW:
                    lText = BYOS.Instance.Dictionary.GetString("tvshow");
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