﻿using UnityEngine;
using System.Collections;
using Buddy;
using Buddy.UI;

namespace BuddyApp.Guardian
{
	/// <summary>
	/// State which show the window to set the head orientation and get the camera stream
	/// </summary>
	public class HeadControlState : AStateMachineBehaviour
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

		private HeadControllerWindow mHeadControllerWindow;

		public override void Start()
		{
			mHeadControllerWindow = GetGameObject(StateObject.HEAD_CONTROLLER).GetComponent<HeadControllerWindow>();
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mGoBack = false;
			mMotors = Primitive.Motors;
			mRGBCam = Primitive.RGBCam;
			mHeadControllerWindow.HeadControlAnimator.SetTrigger("Open_WHeadController");
			mHeadControllerWindow.ButtonBack.onClick.AddListener(GoBack);
			mHeadControllerWindow.ButtonLeft.onClick.AddListener(MoveNoLeft);
			mHeadControllerWindow.ButtonRight.onClick.AddListener(MoveNoRight);
			mHeadControllerWindow.ButtonUp.onClick.AddListener(MoveYesUp);
			mHeadControllerWindow.ButtonDown.onClick.AddListener(MoveYesDown);
            if (!mRGBCam.IsOpen)
                mRGBCam.Open();

            Interaction.TextToSpeech.SayKey("headorientationmessage", true);

        }

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			ControlNoAxis();
			ControlYesAxis();

            mHeadControllerWindow.RawCamImage.texture = mRGBCam.FrameTexture2D;
			mHeadControllerWindow.RawBuddyFaceImage.texture = Interaction.Face.Texture;

			if (mGoBack && mHeadControllerWindow.HeadControlAnimator.GetCurrentAnimatorStateInfo(0).IsName("WindowHeadController_Gardien_Off")) {
				animator.SetInteger("DebugMode", -1);
				mGoBack = false;
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            if (mRGBCam.IsOpen)
                mRGBCam.Close();
			mHeadControllerWindow.ButtonBack.onClick.RemoveAllListeners();
			mHeadControllerWindow.ButtonLeft.onClick.RemoveAllListeners();
			mHeadControllerWindow.ButtonRight.onClick.RemoveAllListeners();
			mHeadControllerWindow.ButtonUp.onClick.RemoveAllListeners();
			mHeadControllerWindow.ButtonDown.onClick.RemoveAllListeners();

		}

		/// <summary>
		/// Function to control the head hinge
		/// </summary>
		private void ControlNoAxis()
		{
			bool lChanged = false;
			if (mNoLeft) {
				mNoAngle = mMotors.NoHinge.CurrentAnglePosition + 15;
				lChanged = true;
				mNoLeft = false;
			}

			if (mNoRight) {
				mNoAngle = mMotors.NoHinge.CurrentAnglePosition - 15;
				lChanged = true;
				mNoRight = false;
			}

			if (lChanged) {
				mMotors.NoHinge.SetPosition(mNoAngle, mNoSpeed);
			}
		}

		/// <summary>
		/// Function to control the neck hinge 
		/// </summary>
		private void ControlYesAxis()
		{
			bool lChanged = false;
			if (mYesDown) {
				mYesAngle = mMotors.YesHinge.CurrentAnglePosition + 10;
				lChanged = true;
				mYesDown = false;
			}

			if (mYesUp) {
				mYesAngle = mMotors.YesHinge.CurrentAnglePosition - 10;
				lChanged = true;
				mYesUp = false;
			}

            if (lChanged)
            {
                mMotors.YesHinge.SetPosition(mYesAngle, mYesSpeed);
                Debug.Log("bouge tete");
            }
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
			mGoBack = true;

			mHeadControllerWindow.HeadControlAnimator.SetTrigger("Close_WHeadController");
		}

	}
}