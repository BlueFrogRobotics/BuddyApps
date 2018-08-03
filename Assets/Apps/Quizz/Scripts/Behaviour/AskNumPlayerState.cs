using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.Quizz
{
    /// <summary>
    /// State in which the number of player will be asked
    /// </summary>
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
            Debug.Log("ask num player");
            Interaction.VocalManager.UseVocon = true;
            Interaction.VocalManager.ClearGrammars();
            Interaction.VocalManager.AddGrammar("commands", Buddy.LoadContext.APP);
            Interaction.VocalManager.AddGrammar("number_player", Buddy.LoadContext.APP);
            Interaction.VocalManager.OnVoconBest = VoconBest;
            Interaction.VocalManager.OnVoconEvent = EventVocon;
            mQuizzBehaviour.OnLanguageChange = OnLanguageChange;
            mQuizzBehaviour.LastStateId = 0;
            StartCoroutine(AskNumberPlayer());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mQuizzBehaviour.OnLanguageChange = null;
            Debug.Log("avant arret coroutine");
            StopCoroutine(AskNumberPlayer());
            Debug.Log("apres arret coroutine");
        }

        private IEnumerator AskNumberPlayer()
        {
            Debug.Log("coroutine ask num player");
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Interaction.TextToSpeech.SayKey("howmanyplayer");
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Interaction.VocalManager.StartInstantReco();
        }

        private void EventVocon(VoconEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(VoconResult iBestResult)
        {
            Debug.Log("le best result: " + iBestResult.Utterance + " confidence: " + iBestResult.Confidence + " best rule: " + iBestResult.StartRule);
            if (iBestResult.Utterance == null || iBestResult.Utterance == "" || iBestResult.Confidence == 0)
            {
                //Interaction.VocalManager.StopRecognition();
                Interaction.VocalManager.StartInstantReco();
            }
            else if (iBestResult.StartRule == "commands_" + mQuizzBehaviour.Lang + "#quit" && iBestResult.Confidence > 6000)
            {
                Trigger("Exit");
            }
            else
            {
                Debug.Log("best result: " + iBestResult.Utterance);
                string[] lElements = iBestResult.Utterance.Trim().Split(' ');
                int lNumPlayer = 0;
                Debug.Log("le element" + lElements[lElements.Length - 1]);
                int.TryParse(lElements[lElements.Length - 1].Trim(), out lNumPlayer);
                if (lNumPlayer <= 0)
                {
                    Interaction.TextToSpeech.SayKey("notenoughplayer");
                    StartCoroutine(AskNumberPlayer());
                }
                else if (lNumPlayer > QuizzBehaviour.MAX_PLAYER)
                {
                    Interaction.TextToSpeech.SayKey("toomuchplayer");
                    StartCoroutine(AskNumberPlayer());
                }
                else
                {
                    mQuizzBehaviour.NumPlayer = lNumPlayer;
                    Debug.Log("nb player: " + lNumPlayer);
                    Trigger("Engagement");
                }
            }
        }

        
    }
}