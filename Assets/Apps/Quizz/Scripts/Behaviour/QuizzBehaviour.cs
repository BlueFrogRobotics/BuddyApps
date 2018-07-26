using Buddy;
using Buddy.UI;

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

        private int mNumPlayer = 0;

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

        public PlayerNamesData PlayerNamesData { get; private set; }

        public List<Player> Players { get; private set; }

        public QuestionsData Questions { get; private set; }

        public QuestionData ActualQuestion { get; set; }

        public int ActualPlayerId { get; set; }

        public int ActualRound { get; set; }

        public bool Beginning { get; set; }

        public HashSet<int> ListQuestionsIdAsked { get; set; }

        public int LastStateId { get; set; }

        public string Lang { get { return BYOS.Instance.Language.CurrentLang.ToString().ToLower(); } }

        public Action OnLanguageChange;

        public const int MAX_ROUNDS = 5;

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
            Debug.Log("format: " + BYOS.Instance.Language.CurrentFormat + " lang: " + BYOS.Instance.Language.CurrentLang.ToString());
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

        public void Init()
        {
            ActualPlayerId = 0;
            ActualRound = 0;
            ListQuestionsIdAsked = new HashSet<int>();
        }

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

        public string ThemeToString(Theme iTheme)
        {
            string lText = "";
            switch(iTheme)
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
    }
}