using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BrainTraining
{
    public class GiveAnswerState : AStateMachineBehaviour
    {
        private BrainTrainingBehaviour mBrainTrainingBehaviour;

        public override void Start()
        {
            mBrainTrainingBehaviour = GetComponent<BrainTrainingBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Run behavior while speaking
            string BIname = "LookAt" + Random.Range(1, 4);
            Buddy.Behaviour.Interpreter.Run(BIname);

            string text = "";
            if (mBrainTrainingBehaviour.IsSuccess())
                text += Buddy.Resources.GetRandomString("win") + " ";

            if (!string.IsNullOrEmpty(mBrainTrainingBehaviour.ActualQuestion.FullAnswer.Trim()))
                text += mBrainTrainingBehaviour.ActualQuestion.FullAnswer;

            Buddy.Vocal.Say(text, (iOutput) => {
                Buddy.Behaviour.Interpreter.Stop();
                Trigger("CheckNumQuestion");
            });
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //}
    }
}