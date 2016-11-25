using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.RLGL
{
    public class ResultState : StateMachineBehaviour
    {

        private Face mFace;
        private TextToSpeech mTTS;
        private SpeechToText mSTT;
        private float mTimer;
        private bool mIsSentenceDone;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mIsSentenceDone = false;
            mTimer = 0.0f;
            mFace = BYOS.Instance.Face;
            mTTS = BYOS.Instance.TextToSpeech;
            mSTT = BYOS.Instance.SpeechToText;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if (!mTTS.IsSpeaking() && mTimer < 6.0f && !mIsSentenceDone)
            {
                mTTS.Say("Bravo tu as gagner, tu as été trop rapide pour moi!");
                mIsSentenceDone = true;
            }
            if (mTimer > 6.0f)
                animator.SetBool("IsReplayTrue", true);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("IsReplayTrue", false);
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
