using UnityEngine;
using Buddy;
using System.Collections;
using System;

namespace BuddyApp.Guardian
{
	public class HeadTurnState : AStateMachineBehaviour
	{
		private DetectionManager mDetectionManager;
		//private IEnumerator mAction;
		private bool mHasFinishedScan;

		public override void Start()
		{
			mDetectionManager = GetComponent<DetectionManager>();
			mDetectionManager.PreviousScanLeft = false;
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Debug.Log("EnterHead Turn!!!!");
			mHasFinishedScan = false;
            Interaction.Mood.Set(MoodType.LISTENING);
			
			StartCoroutine(ScanNextPose());
		}

		private IEnumerator ScanNextPose()
		{
            Debug.Log("ScanNext Pose");
			mDetectionManager.IsDetectingMovement = GuardianData.Instance.MovementDetection;
			//mDetectionManager.IsDetectingKidnapping = GuardianData.Instance.KidnappingDetection;

			// Random time before turn

			yield return new WaitForSeconds(UnityEngine.Random.Range(1F, 4F));

			mDetectionManager.IsDetectingMovement = false;
			//mDetectionManager.IsDetectingKidnapping = false;

			// If current angle is not middle, go to middle
			if (Math.Abs(Primitive.Motors.NoHinge.CurrentAnglePosition) > 10) {
				Debug.Log("ScanNext Pose middle");
				Primitive.Motors.NoHinge.SetPosition(0, Primitive.Motors.NoHinge.MaximumSpeed);
			}
			// else if middle and previous was left, go right
			else if (mDetectionManager.PreviousScanLeft) {
				Debug.Log("ScanNext Pose right");
				Primitive.Motors.NoHinge.SetPosition(Primitive.Motors.NoHinge.MaximumAngle, Primitive.Motors.NoHinge.MaximumSpeed);
				mDetectionManager.PreviousScanLeft = false;

				// else go left
			} else {
				Debug.Log("ScanNext Pose left");
				Primitive.Motors.NoHinge.SetPosition(Primitive.Motors.NoHinge.MinimumAngle, Primitive.Motors.NoHinge.MaximumSpeed);
				mDetectionManager.PreviousScanLeft = true;
			}

			if (Math.Abs(Primitive.Motors.NoHinge.CurrentAnglePosition - Primitive.Motors.NoHinge.DestinationAnglePosition) < 7) {
				Debug.Log("Head pose not reached: " + Math.Abs(Primitive.Motors.NoHinge.CurrentAnglePosition - Primitive.Motors.NoHinge.DestinationAnglePosition));
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
			Primitive.Motors.NoHinge.SetPosition(0, Primitive.Motors.NoHinge.MaximumSpeed);
			Interaction.Mood.Set(MoodType.NEUTRAL);
		}
	}
}