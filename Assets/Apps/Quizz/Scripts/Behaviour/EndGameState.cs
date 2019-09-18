using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Quizz
{
    public class EndGameState : AStateMachineBehaviour
    {
        private QuizzBehaviour mQuizzBehaviour;
        private SoundsManager mSoundsManager;
        private List<Player> mWinners;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
            mSoundsManager = GetComponent<SoundsManager>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("end game state");
            mWinners = new List<Player>();
            StartCoroutine(EndGame());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private IEnumerator EndGame()
        {
            GetWinnerName();
            Buddy.Vocal.OnEndListening.Clear();

            Buddy.Vocal.StopAndClear();
            Buddy.Behaviour.Interpreter.StopAndClear();

            Buddy.Behaviour.SetMood(Mood.THINKING);
            mSoundsManager.PlaySound(SoundsManager.Sound.COMPUTING);
            yield return new WaitForSeconds(3.5F);
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);

            string text = "";
            if (mQuizzBehaviour.Players.Count==1)
            {
                text = GiveScore1Player();
            }
            else if (mWinners.Count == 1 && mWinners[0].Score < QuizzData.Instance.NbQuestions)
                text = Buddy.Resources.GetRandomString("givewinnername").Replace("[name]", "" + mWinners[0].Name);
            else if (mWinners.Count == 1 && mWinners[0].Score == QuizzData.Instance.NbQuestions)
                text = Buddy.Resources.GetRandomString("perfectwinnername").Replace("[name]", "" + mWinners[0].Name).Replace("[number]", "" + QuizzData.Instance.NbQuestions);
            else
            {
                string lNames = "";
                for (int i = 0; i < mWinners.Count; i++)
                {
                    lNames += " , ";
                    if (i == mWinners.Count - 1)
                    {
                        lNames += Buddy.Resources.GetString("and");
                        lNames += " ";
                    }
                    lNames += mWinners[i].Name;
                }
                text = Buddy.Resources.GetRandomString("severalwinners").Replace("[names]", lNames);
            }
            Buddy.Vocal.Say(text, (iOutput) => {
                Trigger("Exit");
            });
        }

        private string GetWinnerName()
        {
            string lPlayerName = "";
            int lMaxScore = 0;
            foreach (Player player in mQuizzBehaviour.Players)
            {
                if (player.Score > lMaxScore)
                {
                    mWinners.Clear();
                    mWinners.Add(player);
                    lMaxScore = player.Score;
                    lPlayerName = player.Name;
                }
                else if (player.Score == lMaxScore)
                    mWinners.Add(player);
            }
            return lPlayerName;
        }

        private string GiveScore1Player()
        {
            string text = "";
            switch(mQuizzBehaviour.Players[0].Score)
            {
                case 0:
                    text = Buddy.Resources.GetRandomString("nogoodanswer");
                    break;
                case 1:
                    text = Buddy.Resources.GetRandomString("onegoodanswer");
                    break;
                case 2:
                    text = Buddy.Resources.GetRandomString("twogoodanswers");
                    break;
                case 3:
                    text = Buddy.Resources.GetRandomString("threegoodanswers");
                    break;
                case 4:
                    text = Buddy.Resources.GetRandomString("fourgoodanswers");
                    break;
                case 5:
                    text = Buddy.Resources.GetRandomString("perfectwinnername");
                    break;
            }
            if (!string.IsNullOrEmpty(text) && text.Contains("[n]"))
                text = text.Replace("[n]", "" + QuizzData.Instance.NbQuestions);

            return text;
        }
    }
}