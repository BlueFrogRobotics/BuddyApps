using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.UI;

namespace BuddyApp.Guardian
{
    public class HeadControlState : AStateGuardian
    {
        private Motors mMotors;
        private RGBCam mRGBCam;

        private float mNoSpeed = 30.0F;

        private float mYesSpeed = 30.0F;
        private float mYesAngle = 0.0F;
        private float mNoAngle = 0.0F;

        private bool mYesUp = false;
        private bool mYesDown = false;
        private bool mNoLeft = false;
        private bool mNoRight = false;

        private bool mGoBack = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mGoBack = false;
            mMotors = BYOS.Instance.Motors;
            mRGBCam = BYOS.Instance.RGBCam;
            //StateManager.HeadControlWindow.gameObject.SetActive(true);
            StateManager.HeadControlWindow.HeadControlAnimator.SetTrigger("Open_WHeadController");
            StateManager.HeadControlWindow.ButtonBack.onClick.AddListener(GoBack);
            StateManager.HeadControlWindow.ButtonLeft.onClick.AddListener(MoveNoLeft);
            StateManager.HeadControlWindow.ButtonRight.onClick.AddListener(MoveNoRight);
            StateManager.HeadControlWindow.ButtonUp.onClick.AddListener(MoveYesUp);
            StateManager.HeadControlWindow.ButtonDown.onClick.AddListener(MoveYesDown);

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ControlNoAxis();
            ControlYesAxis();

            StateManager.HeadControlWindow.RawCamImage.texture = mRGBCam.FrameTexture2D;

            if (mGoBack && StateManager.HeadControlWindow.HeadControlAnimator.GetCurrentAnimatorStateInfo(0).IsName("WindowHeadController_Gardien_Off"))
            {
                StateManager.BackgroundAnimator.SetTrigger("Open_BG");
                animator.SetInteger("DebugMode", -1);
                mGoBack = false;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StateManager.HeadControlWindow.ButtonBack.onClick.RemoveAllListeners();
            StateManager.HeadControlWindow.ButtonLeft.onClick.RemoveAllListeners();
            StateManager.HeadControlWindow.ButtonRight.onClick.RemoveAllListeners();
            StateManager.HeadControlWindow.ButtonUp.onClick.RemoveAllListeners();
            StateManager.HeadControlWindow.ButtonDown.onClick.RemoveAllListeners();
            
            //StateManager.HeadControlWindow.gameObject.SetActive(false);
        }

        /// <summary>
        /// Function to control the head hinge
        /// </summary>
        private void ControlNoAxis()
        {
            bool lChanged = false;
            if (mNoLeft)
            {
                mNoAngle = mMotors.NoHinge.CurrentAnglePosition + 10;
                lChanged = true;
                mNoLeft = false;
            }

            if (mNoRight)
            {
                mNoAngle = mMotors.NoHinge.CurrentAnglePosition - 10;
                lChanged = true;
                mNoRight = false;
            }

            if (lChanged)
            {
                mMotors.NoHinge.SetPosition(mNoAngle, mNoSpeed);
            }
        }

        /// <summary>
        /// Function to control the neck hinge 
        /// </summary>
        private void ControlYesAxis()
        {
            bool lChanged = false;
            if (mYesDown)
            {
                mYesAngle = mMotors.YesHinge.CurrentAnglePosition + 10;
                lChanged = true;
                mYesDown = false;
            }

            if (mYesUp)
            {
                mYesAngle = mMotors.YesHinge.CurrentAnglePosition - 10;
                lChanged = true;
                mYesUp = false;
            }

            if (lChanged)
                mMotors.YesHinge.SetPosition(mYesAngle, mYesSpeed);
        }

        private void MoveNoLeft()
        {
            Debug.Log("left");
            mNoLeft = true;
        }

        private void MoveNoRight()
        {
            Debug.Log("right");
            mNoRight = true;
        }

        private void MoveYesUp()
        {
            Debug.Log("up");
            mYesUp = true;
        }

        private void MoveYesDown()
        {
            Debug.Log("down");
            mYesDown = true;
        }

        private void GoBack()
        {
            //mDebugSoundAnimator.SetTrigger("Close_WDebugs");
            mGoBack = true;
            
            StateManager.HeadControlWindow.HeadControlAnimator.SetTrigger("Close_WHeadController");
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