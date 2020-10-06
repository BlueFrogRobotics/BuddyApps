using BlueQuark;

using UnityEngine;

using System.Collections;
using System;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State in which the Buddy turn its head and detect movement only when it stop moving
    /// </summary>
	public sealed class HeadTurnState : AStateMachineBehaviour
	{
		private DetectionManager mDetectionManager;
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
			
			StartCoroutine(ScanNextPoseAsync());
		}

		private IEnumerator ScanNextPoseAsync()
		{
			mDetectionManager.IsDetectingMovement = GuardianData.Instance.MovementDetection;
			mDetectionManager.IsDetectingKidnapping = GuardianData.Instance.KidnappingDetection;

			// Random time before turn

			// Check for numpad
			if (GetBool("Password"))
				yield return new WaitForSeconds(1F);


			yield return new WaitForSeconds(UnityEngine.Random.Range(1F, 4F));


			mDetectionManager.IsDetectingMovement = false;
			mDetectionManager.IsDetectingKidnapping = false;

			// If current angle is not middle, go to middle
			if (Math.Abs(Buddy.Actuators.Head.No.Angle) > 10) {
                Buddy.Actuators.Head.No.SetPosition(0, NoHeadHinge.MAX_ANG_VELOCITY);
                mAngleDestination = 0;

            }
			// else if middle and previous was left, go right
			else if (mDetectionManager.PreviousScanLeft) {
                Buddy.Actuators.Head.No.SetPosition(Buddy.Actuators.Head.No.AngleMax, NoHeadHinge.MAX_ANG_VELOCITY);
				mDetectionManager.PreviousScanLeft = false;
                mAngleDestination = Buddy.Actuators.Head.No.AngleMax;
                // else go left
            } else {
                Buddy.Actuators.Head.No.SetPosition(Buddy.Actuators.Head.No.AngleMin, NoHeadHinge.MAX_ANG_VELOCITY);
				mDetectionManager.PreviousScanLeft = true;
                mAngleDestination = Buddy.Actuators.Head.No.AngleMin;

            }

			if (Math.Abs(Buddy.Actuators.Head.No.Angle - mAngleDestination) < 7) {
				yield return null;
			}

			yield return new WaitForSeconds(1F);
            mHasFinishedScan = true;
        }

		public void StopTurnCoroutines()
		{
		    StopCoroutine(ScanNextPoseAsync());
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (mHasFinishedScan) {
				mHasFinishedScan = false;
				StartCoroutine(ScanNextPoseAsync());
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