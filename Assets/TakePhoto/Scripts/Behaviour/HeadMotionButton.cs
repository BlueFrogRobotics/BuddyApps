using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BuddyOS;

namespace BuddyApp.TakePhoto
{
	public class HeadMotionButton : MonoBehaviour
	{

		private Motors mMotors;
		private Face mFace;
		//private Speaker mSpeaker;
		//private float mNoSpeed = 100.0F;
		private float mYesSpeed = 100.0F;
		
		private string mButtonPushed;

		void Start()
		{
			mMotors = BYOS.Instance.Motors;
			mFace = BYOS.Instance.Face;
			//mSpeaker = BYOS.Instance.Speaker;
			mButtonPushed = "";
		}


		void Update()
		{
			if (!string.IsNullOrEmpty(mButtonPushed)) {
				if (mButtonPushed == "left") {
					MoveNoLeft();
				} else if (mButtonPushed == "right") {
					MoveNoRight();
				} else if (mButtonPushed == "up") {
					MoveYesUp();
				} else if (mButtonPushed == "down") {
					MoveYesDown();
				}
			}
			//ControlNoAxis();
			//ControlYesAxis();
		}


		public void MoveHead(string iButton)
		{
			mButtonPushed = iButton;
		}

		public void StopMoveHead()
		{
			mButtonPushed = "";
			mFace.LookAt(600, 0);
		}

		private void MoveNoLeft()
		{
			float lNoAngle = 0.0F;
			lNoAngle = mMotors.NoHinge.CurrentAnglePosition + 20;
			mMotors.NoHinge.SetPosition(lNoAngle);
			mFace.LookAt(1800, 600);
			//mSpeaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);

		}

		private void MoveNoRight()
		{
			float lNoAngle = 0.0F;
			lNoAngle = mMotors.NoHinge.CurrentAnglePosition - 20;
			mMotors.NoHinge.SetPosition(lNoAngle);
			mFace.LookAt(-600, 600);
			//mSpeaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
		}

		private void MoveYesUp()
		{
			float lYesAngle = 0.0F;
			lYesAngle = mMotors.YesHinge.CurrentAnglePosition - 15;
			mMotors.YesHinge.SetPosition(lYesAngle, mYesSpeed);
			mFace.LookAt(600, 600);
			//mSpeaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
		}

		private void MoveYesDown()
		{
			float lYesAngle = 0.0F;
			lYesAngle = mMotors.YesHinge.CurrentAnglePosition + 15;
			mMotors.YesHinge.SetPosition(lYesAngle, mYesSpeed);
			mFace.LookAt(600, -600);
			//mSpeaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
		}

	}
}
