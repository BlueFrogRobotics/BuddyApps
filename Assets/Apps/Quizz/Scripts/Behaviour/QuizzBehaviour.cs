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

        public const int MAX_ROUNDS = 2;

        /*
         * Data of the application. Save on disc when app is quitted
         */
        private QuizzData mAppData;

        void Start()
        {
            /*
			* You can setup your App activity here.
			*/
            QuizzActivity.Init(null);

            /*
			* Init your app data
			*/
            mAppData = QuizzData.Instance;

            PlayerNamesData = Utils.UnserializeXML<PlayerNamesData>(BYOS.Instance.Resources.GetPathToRaw("player_names.xml"));
            foreach (string player in PlayerNamesData.Names)
            {
                Debug.Log(player);
            }

            Questions = Utils.UnserializeXML<QuestionsData>(BYOS.Instance.Resources.GetPathToRaw("quizz_fr.xml"));
            //PlayerNamesData playerNamesData = new PlayerNamesData();
            //playerNamesData.Names = new List<string>();
            //playerNamesData.Names.Add("Aigle");
            //playerNamesData.Names.Add("Poupette");
            //Utils.SerializeXML(playerNamesData, BYOS.Instance.Resources.GetPathToRaw("player_names"));
            //QuestionsData questions = new QuestionsData();
            //questions.Questions = new List<QuestionData>();
            //QuestionData question = new QuestionData();
            //question.Answers = new List<string>();
            //question.Question = "Qui a mis le premier le pied sur la Lune ?";
            //question.Theme = Theme.HISTORY;
            //question.Answers.Add("Neil Armstrong");
            //question.Answers.Add("Youri Gagarine");
            //question.Answers.Add("Buzz Aldrin");
            //question.GoodAnswer = 0;
            //question.AnswerComplement = "L'astronaute américain Neil Armstrong a été le premier homme à marcher sur la Lune le 21 juillet 1969. En posant son pied gauche sur le sol lunaire, il a prononcé ces mots désormais célèbres : « C'est un petit pas pour l'homme, un bond de géant pour l'humanité. ».";
            //questions.Questions.Add(question);
            //QuestionData question2 = new QuestionData();
            //question2.Answers = new List<string>();
            //question2.Question = "Quelle est la capitale du Brésil ?";
            //question2.Theme = Theme.GEOGRAPHY;
            //question2.Answers.Add("Rio de Janeiro");
            //question2.Answers.Add("Brasilia");
            //question2.Answers.Add("Sao Paulo");
            //question2.GoodAnswer = 1;
            //question2.AnswerComplement = "C'est en 1960 que les autorités brésiliennes, qui ne parvenaient pas à se décider entre Rio et Sao Paulo, décident de faire de Brasilia la capitale du pays. La ville fut construite pour l'occasion, en moins de trois ans. Aujourd'hui, elle fait partie des plus grandes villes du Brésil.";
            //questions.Questions.Add(question2);
            //Utils.SerializeXML(questions, BYOS.Instance.Resources.GetPathToRaw("quizz_fr.xml"));

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
    }
}