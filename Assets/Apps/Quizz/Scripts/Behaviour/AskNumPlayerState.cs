using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.Quizz
{
    public class AskNumPlayerState : AStateMachineBehaviour
    {

        private QuizzBehaviour mQuizzBehaviour;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.VocalManager.UseVocon = true;
            Interaction.VocalManager.AddGrammar("number_player", Buddy.LoadContext.APP);
            Interaction.VocalManager.OnVoconBest = VoconBest;
            Interaction.VocalManager.OnVoconEvent = EventVocon;
            StartCoroutine(AskNumberPlayer());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private IEnumerator AskNumberPlayer()
        {
            Interaction.TextToSpeech.SayKey("howmanyplayer");
            while(!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Interaction.VocalManager.StartInstantReco();
        }

        private void EventVocon(VoconEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(VoconResult iBestResult)
        {
            Debug.Log("best result: " + iBestResult.Utterance);
            string[] lElements = iBestResult.Utterance.Trim().Split(' ');
            int lNumPlayer = 0;
            Debug.Log("le element" + lElements[lElements.Length - 1]);
            int.TryParse(lElements[lElements.Length - 1].Trim(), out lNumPlayer);
            mQuizzBehaviour.NumPlayer = lNumPlayer;
            Debug.Log("nb player: " + lNumPlayer);
            Trigger("Engagement");
        }
    }
}