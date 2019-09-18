using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

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
            Debug.Log("Quizz: ask num player state");
            mQuizzBehaviour.LastStateId = 0;

            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            string[] grammars = { "commands", "number_player" };
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters
            {
                Grammars = grammars
            };
            Buddy.Vocal.OnEndListening.Add(OnEndListening);
            
            StartCoroutine(AskNumberPlayer());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StopCoroutine(AskNumberPlayer());
            Buddy.Vocal.OnEndListening.Clear();
        }

        private IEnumerator AskNumberPlayer()
        {
            yield return new WaitUntil(() => !Buddy.Vocal.IsBusy);
            Buddy.Vocal.SayKey("howmanyplayer");
            yield return new WaitUntil(() => !Buddy.Vocal.IsBusy);
            Buddy.Vocal.Listen();
        }

        //private void EventVocon(VoconEvent iEvent)
        //{
        //    Debug.Log(iEvent);
        //}

        private void OnEndListening(SpeechInput iSpeechInput)
        {
            if (string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                Buddy.Vocal.Listen();
            }
            else if (iSpeechInput.Rule == "commands_" + mQuizzBehaviour.Lang + "#quit")
            {
                Trigger("Exit");
            }
            else
            {
                string[] lElements = iSpeechInput.Utterance.Trim().Split(' ');
                int lNumPlayer = 0;
                if (int.TryParse(lElements[lElements.Length - 1].Trim(), out lNumPlayer))
                {
                    if (lNumPlayer <= 0)
                    {
                        Buddy.Vocal.SayKey("notenoughplayer");
                        StartCoroutine(AskNumberPlayer());
                    }
                    else if (lNumPlayer > QuizzBehaviour.MAX_PLAYER)
                    {
                        Buddy.Vocal.SayKey("toomuchplayer");
                        StartCoroutine(AskNumberPlayer());
                    }
                    else
                    {
                        mQuizzBehaviour.NumPlayer = lNumPlayer;
                        Debug.Log("Quizz: nb player: " + lNumPlayer);
                        Trigger("Engagement");
                    }
                }
                else
                {
                    StartCoroutine(AskNumberPlayer());
                }
            }
        }

        
    }
}