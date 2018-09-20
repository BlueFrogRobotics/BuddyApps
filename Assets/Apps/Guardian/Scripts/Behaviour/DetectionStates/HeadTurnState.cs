using UnityEngine;
using BlueQuark;
using System.Collections;
using System;

namespace BuddyApp.Guardian
{
	public sealed class HeadTurnState : AStateMachineBehaviour
	{
		private DetectionManager mDetectionManager;
		//private IEnumerator mAction;
		private bool mHasFinishedScan;
        private float mAngleDestination;

        public override void Start()
		{
			mDetectionManager = GetComponent<DetectionManager>();
			mDetectionManager.PreviousScanLeft = false;
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Debug.Log("EnterHead Turn!!!!");
			mHasFinishedScan = false;
            Buddy.Behaviour.SetMood(Mood.LISTENING);
			
			StartCoroutine(ScanNextPose());
		}

		private IEnumerator ScanNextPose()
		{
            Debug.Log("ScanNext Pose");
			mDetectionManager.IsDetectingMovement = GuardianData.Instance.MovementDetection;
			//mDetectionManager.IsDetectingKidnapping = GuardianData.Instance.KidnappingDetection;

			// Random time before turn

			// Check for numpad
			if (GetBool("Password"))
				yield return new WaitForSeconds(1F);


			yield return new WaitForSeconds(UnityEngine.Random.Range(1F, 4F));


			mDetectionManager.IsDetectingMovement = false;
			//mDetectionManager.IsDetectingKidnapping = false;

			// If current angle is not middle, go to middle
			if (Math.Abs(Buddy.Actuators.Head.No.Angle) > 10) {
				Debug.Log("ScanNext Pose middle");
                Buddy.Actuators.Head.No.SetPosition(0, NoHeadHinge.MAX_ANG_VELOCITY);
                mAngleDestination = 0;

            }
			// else if middle and previous was left, go right
			else if (mDetectionManager.PreviousScanLeft) {
				Debug.Log("ScanNext Pose right");
                Buddy.Actuators.Head.No.SetPosition(NoHeadHinge.MAX_RIGHT_ANGLE, NoHeadHinge.MAX_ANG_VELOCITY);
				mDetectionManager.PreviousScanLeft = false;
                mAngleDestination = NoHeadHinge.MAX_RIGHT_ANGLE;
                // else go left
            } else {
				Debug.Log("ScanNext Pose left");
                Buddy.Actuators.Head.No.SetPosition(NoHeadHinge.MAX_LEFT_ANGLE, NoHeadHinge.MAX_ANG_VELOCITY);
				mDetectionManager.PreviousScanLeft = true;
                mAngleDestination = NoHeadHinge.MAX_LEFT_ANGLE;

            }

			if (Math.Abs(Buddy.Actuators.Head.No.Angle - mAngleDestination) < 7) {
				Debug.Log("Head pose not reached: " + Math.Abs(Buddy.Actuators.Head.No.Angle - mAngleDestination));
				yield return null;
			}

			Debug.Log("Head Pose reached");

			yield return new WaitForSeconds(1F);
			Debug.Log("ScanNext Pose done");
            mHasFinishedScan = true;
        }

		public void StopTurnCoroutines()
		{
				StopCoroutine(ScanNextPose());
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (mHasFinishedScan) {
				Debug.Log("ScanNext Update");
				mHasFinishedScan = false;
				StartCoroutine(ScanNextPose());
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			// Go back to middle
			Buddy.Actuators.Head.No.SetPosition(0, NoHeadHinge.MAX_ANG_VELOCITY);
			Buddy.Behaviour.SetMood(Mood.NEUTRAL);
		}
	}
}