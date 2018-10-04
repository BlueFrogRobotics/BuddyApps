using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public sealed class DateChoiceState : AStateMachineBehaviour
    {
        private const int TRY_NUMBER = 2;
        private int mListen;
        private SpeechInputParameters mGrammar;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mListen = 0;

            // Setting of the grammar for STT
            ReminderData.Instance.DateChoice = "";
            mGrammar = new SpeechInputParameters();
            mGrammar.Grammars = new string[] { "reminder" };

            // Setting of the callback
            Buddy.Vocal.OnEndListening.Add((iSpeechInput) => { VoconGetResult(iSpeechInput); });
        }

        private void VoconGetResult(SpeechInput iSpeechInput)
        {
            Debug.Log("SPEECH.ToString: " + iSpeechInput.ToString());
            Debug.Log("SPEECH.Utterance: " + iSpeechInput.Utterance);
            ReminderData.Instance.DateChoice = iSpeechInput.Utterance;
            mListen++;
            // TODO - Consistency test
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mListen < 1 && string.IsNullOrEmpty(ReminderData.Instance.DateChoice)) {
                if (!Buddy.Vocal.IsBusy) {
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("when"), mGrammar.Grammars);
                }
            }
            if (mListen >= TRY_NUMBER || Input.GetKeyDown(KeyCode.A))
                Trigger("DateEntryState");
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("------ NEXT STEP ------");
            Buddy.Vocal.Stop();
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}
