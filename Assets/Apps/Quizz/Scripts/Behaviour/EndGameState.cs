using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Quizz
{
    public class EndGameState : AStateMachineBehaviour
    {
        private QuizzBehaviour mQuizzBehaviour;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("end game state");
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
            
            Interaction.TextToSpeech.Say(Dictionary.GetRandomString("givewinnername").Replace("[name]", "" + GetWinnerName()));
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Trigger("AskRestart");
        }

        private string GetWinnerName()
        {
            string lPlayerName = "";
            int lMaxScore = 0;
            foreach(Player player in mQuizzBehaviour.Players)
            {
                if(player.Score>lMaxScore)
                {
                    lMaxScore = player.Score;
                    lPlayerName = player.Name;
                }
            }
            return lPlayerName;
        }
    }
}