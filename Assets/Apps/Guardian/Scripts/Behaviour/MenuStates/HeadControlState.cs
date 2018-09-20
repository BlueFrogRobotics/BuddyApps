using UnityEngine;
using System.Collections;
using BlueQuark;

namespace BuddyApp.Guardian
{
	/// <summary>
	/// State which show the window to set the head orientation and get the camera stream
	/// </summary>
	public sealed class HeadControlState : AStateMachineBehaviour
	{
        private RGBCamera mRGBCam;

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
			mHeadControllerWindow = GetGameObject(HEAD_CONTROLLER).GetComponent<HeadControllerWindow>();
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mHeadControllerWindow.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1F;
			mGoBack = false;

			mHeadControllerWindow.HeadControlAnimator.SetTrigger("Open_WHeadController");
			mHeadControllerWindow.ButtonBack.onClick.AddListener(GoBack);
			mHeadControllerWindow.ButtonLeft.onClick.AddListener(MoveNoLeft);
			mHeadControllerWindow.ButtonRight.onClick.AddListener(MoveNoRight);
			mHeadControllerWindow.ButtonUp.onClick.AddListener(MoveYesUp);
			mHeadControllerWindow.ButtonDown.onClick.AddListener(MoveYesDown);
            if (!mRGBCam.IsOpen)
                mRGBCam.Open();

            Buddy.Vocal.SayKey("headorientationmessage");

        }

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			ControlNoAxis();
			ControlYesAxis();

            mHeadControllerWindow.RawCamImage.texture = mRGBCam.TexFrame;
			mHeadControllerWindow.RawBuddyFaceImage.texture = Buddy.Behaviour.Face.Texture;

			if (mGoBack && mHeadControllerWindow.HeadControlAnimator.GetCurrentAnimatorStateInfo(0).IsName("WindowHeadController_Gardien_Off")) {
                iAnimator.SetInteger("DebugMode", -1);
				mGoBack = false;
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            if (mRGBCam.IsOpen)
                mRGBCam.Close();
            GuardianData.Instance.FirstRunParam = false;
            mHeadControllerWindow.ButtonBack.onClick.RemoveAllListeners();
			mHeadControllerWindow.ButtonLeft.onClick.RemoveAllListeners();
			mHeadControllerWindow.ButtonRight.onClick.RemoveAllListeners();
			mHeadControllerWindow.ButtonUp.onClick.RemoveAllListeners();
			mHeadControllerWindow.ButtonDown.onClick.RemoveAllListeners();
            mHeadControllerWindow.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0F;
        }

		/// <summary>
		/// Function to control the head hinge
		/// </summary>
		private void ControlNoAxis()
		{
			bool lChanged = false;
			if (mNoLeft) {
				mNoAngle = Buddy.Actuators.Head.No.Angle + 15;
				lChanged = true;
				mNoLeft = false;
			}

			if (mNoRight) {
				mNoAngle = Buddy.Actuators.Head.No.Angle - 15;
				lChanged = true;
				mNoRight = false;
			}

			if (lChanged) {
                Buddy.Actuators.Head.No.SetPosition(mNoAngle, mNoSpeed);
			}
		}

		/// <summary>
		/// Function to control the neck hinge 
		/// </summary>
		private void ControlYesAxis()
		{
			bool lChanged = false;
			if (mYesDown) {
				mYesAngle = Buddy.Actuators.Head.Yes.Angle + 10;
				lChanged = true;
				mYesDown = false;
			}

			if (mYesUp) {
				mYesAngle = Buddy.Actuators.Head.Yes.Angle - 10;
				lChanged = true;
				mYesUp = false;
			}

            if (lChanged)
            {
                Buddy.Actuators.Head.Yes.SetPosition(mYesAngle, mYesSpeed);
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
