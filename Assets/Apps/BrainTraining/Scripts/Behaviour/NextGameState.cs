using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BrainTraining
{

    public class NextGameState : AStateMachineBehaviour
    {
        private BrainTrainingBehaviour mBrainTrainingBehaviour;

        public override void Start()
        {
            Buddy.GUI.Header.OnClickParameters.Add(() =>
            {
                Trigger("Settings");
            });
            mBrainTrainingBehaviour = GetComponent<BrainTrainingBehaviour>();
        }

       
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // If all the quizz have been played, the game is over
            if (mBrainTrainingBehaviour.IsGameOver())
            {
                Trigger("EndGame");
            }
            else
            {
                // Start a new quizz
                mBrainTrainingBehaviour.StartNewQuizz();

                // Run behavior while speaking
                string BIname = "LookAt" + Random.Range(1, 4);
                Buddy.Behaviour.Interpreter.Run(BIname);

                string key = mBrainTrainingBehaviour.IsFirstGame() ? "tostart" : "next";
                string msg = Buddy.Resources.GetString(key);
                msg += " [20] " + mBrainTrainingBehaviour.Questions.Explanation;

                Buddy.Vocal.Say(msg, (output) =>
                {
                    Buddy.Behaviour.Interpreter.Stop();
                    Trigger("Begin");
                });
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {A
        //
        //}

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
