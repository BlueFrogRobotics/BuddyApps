using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

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
            Interaction.VocalManager.StopListenBehaviour();
            Interaction.Mood.Set(MoodType.THINKING);
            mSoundsManager.PlaySound(SoundsManager.Sound.COMPUTING);
            while (mSoundsManager.IsPlaying)
                yield return null;
            Interaction.Mood.Set(MoodType.NEUTRAL);
            if (mQuizzBehaviour.Players.Count==1)
            {
                GiveScore1Player();
                //Interaction.TextToSpeech.Say(Dictionary.GetRandomString("givescore").Replace("[name]", "" + mQuizzBehaviour.Players[0].Name).Replace("[score]", ""+mQuizzBehaviour.Players[0].Score));
            }
            else if (mWinners.Count == 1 && mWinners[0].Score < QuizzBehaviour.MAX_ROUNDS)
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("givewinnername").Replace("[name]", "" + mWinners[0].Name));
            else if (mWinners.Count == 1 && mWinners[0].Score == QuizzBehaviour.MAX_ROUNDS)
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("perfectwinnername").Replace("[name]", "" + mWinners[0].Name).Replace("[number]", "" + QuizzBehaviour.MAX_ROUNDS));
            else
            {
                string lNames = "";
                for (int i = 0; i < mWinners.Count; i++)
                {
                    lNames += " , ";
                    if (i == mWinners.Count - 1)
                    {
                        lNames += Dictionary.GetString("and");
                        lNames += " ";
                    }
                    lNames += mWinners[i].Name;
                }
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("severalwinners").Replace("[names]", lNames));
            }
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            //QuitApp();
            Trigger("AskRestart");
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

        private void GiveScore1Player()
        {
            switch(mQuizzBehaviour.Players[0].Score)
            {
                case 0:
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("nogoodanswer").Replace("[name]", "" + mQuizzBehaviour.Players[0].Name).Replace("[n]", "" + QuizzBehaviour.MAX_ROUNDS));
                    break;
                case 1:
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("onegoodanswer").Replace("[name]", "" + mQuizzBehaviour.Players[0].Name).Replace("[n]", "" + QuizzBehaviour.MAX_ROUNDS));
                    break;
                case 2:
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("twogoodanswers").Replace("[name]", "" + mQuizzBehaviour.Players[0].Name).Replace("[n]", "" + QuizzBehaviour.MAX_ROUNDS));
                    break;
                case 3:
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("threegoodanswers").Replace("[name]", "" + mQuizzBehaviour.Players[0].Name).Replace("[n]", "" + QuizzBehaviour.MAX_ROUNDS));
                    break;
                case 4:
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("fourgoodanswers").Replace("[name]", "" + mQuizzBehaviour.Players[0].Name).Replace("[n]", "" + QuizzBehaviour.MAX_ROUNDS));
                    break;
                case 5:
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("perfectwinnername").Replace("[name]", "" + mQuizzBehaviour.Players[0].Name).Replace("[number]", "" + QuizzBehaviour.MAX_ROUNDS));
                    break;
            }
        }
    }
}