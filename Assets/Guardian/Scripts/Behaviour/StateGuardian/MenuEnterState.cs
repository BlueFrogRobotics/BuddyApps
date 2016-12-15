using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class MenuEnterState : AStateGuardian
    {

        private TextToSpeech mTTS;
        private bool mHasTalked = false;
        private bool mHasTriedToTalked = false;
        private float mTimer = 4.4f;
        private Dictionary mDictionnary;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetWindowAppOverBuddyColor(0);
            mTTS = BYOS.Instance.TextToSpeech;
            mDictionnary = BYOS.Instance.Dictionary;
            animator.SetBool("ChangeState", false);
            mHasTalked = false;
            mHasTriedToTalked = false;
            mTimer = 1.5f;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("face stable: "+mFaceManager.IsStable);
            mTimer -= Time.deltaTime;
            //if (!mHasTalked && mFaceManager.IsStable && mTimer<0.0f)
            if (/*mFaceManager.IsStable && */!mHasTalked && !mTTS.IsSpeaking())
            {
                Debug.Log("fixe ou mobile?");
                mTTS.Say(mDictionnary.GetString("choiceMode"));//"Quel mode souhaite tu. Fixe ou mobile");
                mHasTriedToTalked = true;
            }
            else if (/*!mFaceManager.IsStable && */mHasTriedToTalked && !mHasTalked)
                mHasTalked = true;
            else if (mHasTalked && !mTTS.IsSpeaking())
                animator.SetBool("ChangeState", true);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("ChangeState", false);
            mHasTalked = false;
            animator.SetInteger("Mode", 0);
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