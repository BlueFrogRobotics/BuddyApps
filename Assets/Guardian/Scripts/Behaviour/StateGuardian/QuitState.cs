using UnityEngine;
using BuddyOS;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class QuitState : AStateGuardian
    {

        private bool mHasExit = false;
        private TextToSpeech mTTS;
        private Dictionary mDictionary;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mHasExit = false;
            mDictionary = BYOS.Instance.Dictionary;
            mTTS = BYOS.Instance.TextToSpeech;
            mTTS.Say(mDictionary.GetRandomString("iQuitApp")); //"I quit the application");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            if (!mHasExit && !mTTS.IsSpeaking)
            {
                mHasExit = true;
                StateManager.QuitApplication();
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

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