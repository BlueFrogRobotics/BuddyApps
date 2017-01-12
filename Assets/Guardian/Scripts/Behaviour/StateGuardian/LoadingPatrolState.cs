using UnityEngine;
using System;
using System.Collections;
using System.Globalization;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class LoadingPatrolState : AStateGuardian
    {

        private bool mStartTimer = false;
        private float mTimer = 1.0f;
        private Animator mLoadingAnimator;
        private DetectionManager mDetectorManager;
        private bool mHasFinishedLoading = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DisableWindowAppOverBuddy();
            mLoadingAnimator = StateManager.LoadingAnimator;
            mDetectorManager = StateManager.DetectorManager;
            mLoadingAnimator.SetTrigger("Open_WLoading");
            BYOS.Instance.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
            BYOS.Instance.VocalManager.EnableTrigger = false;
            mHasFinishedLoading = false;
            animator.SetBool("ChangeState", false);
            animator.SetBool("IsDetecting", false);
            mTimer = 5.0f;
            if (mDetectorManager.SoundDetector.IsInit)
                mDetectorManager.SoundDetector.Stop();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("face stable: "+ mFace.IsStable);
            if (/*mFace.IsStable && */!mStartTimer)
            {
                mStartTimer = true;

            }
            else if (mStartTimer)
                mTimer -= Time.deltaTime;

            if (mTimer < 0.0f)
            {
                mLoadingAnimator.SetTrigger("Close_WLoading");
                mHasFinishedLoading = true;
                
            }

            if(mHasFinishedLoading && mLoadingAnimator.GetCurrentAnimatorStateInfo(0).IsName("LoadingScreen_Off"))
            {
                mLoadingAnimator.ResetTrigger("Open_WLoading");
                mLoadingAnimator.ResetTrigger("Close_WLoading");
                animator.SetBool("ChangeState", true);
            }
            //mLoadingAnimator.SetBool("Close", true);

            //if (!mLoadingObject || !mLoadingObject.activeInHierarchy)
                //animator.SetBool("ChangeState", true);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            animator.SetBool("ChangeState", false);
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