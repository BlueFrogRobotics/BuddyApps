using UnityEngine;
using System;
using System.Collections;
using System.Globalization;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class LoadingPatrolState : AStateGuardian
    {

        private GameObject mLoadingObject;
        private Face mFace;
        private bool mStartTimer = false;
        private float mTimer = 1.0f;
        private Animator mLoadingAnimator;
        private DetectionManager mDetectorManager;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mLoadingAnimator = mStatePatrolManager.LoadingAnimator;
            mDetectorManager = mStatePatrolManager.DetectorManager;
            mLoadingObject = mStatePatrolManager.Loading;
            //BYOS.Instance.RGBCam.Resolution = RGBCamResolution.W_176_H_144;

            animator.SetBool("ChangeState", false);
            mFace = BYOS.Instance.Face;
            mTimer = 1.0f;
            if (mDetectorManager.SoundDetector.IsInit)
                mDetectorManager.SoundDetector.Stop();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("face stable: "+ mFace.IsStable);
            if (mFace.IsStable && !mStartTimer)
            {
                mStartTimer = true;

            }
            else if (mStartTimer)
                mTimer -= Time.deltaTime;

            if (mTimer < 0.0f)
                mLoadingAnimator.SetBool("Close", true);

            if (!mLoadingObject || !mLoadingObject.activeInHierarchy)
                animator.SetBool("ChangeState", true);
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