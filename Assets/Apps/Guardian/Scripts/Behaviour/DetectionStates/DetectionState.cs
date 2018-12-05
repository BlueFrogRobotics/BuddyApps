using BlueQuark;

using UnityEngine;

using System.Collections;

namespace BuddyApp.Guardian
{
	/// <summary>
	/// State that init that activate the detections chosen by hte user and pass to the next mode state
	/// </summary>
	public sealed class DetectionState : AStateMachineBehaviour
	{
		private DetectionManager mDetectionManager;
        private MediaManager mMediamanager;

		public override void Start()
		{
			mDetectionManager = GetComponent<DetectionManager>();
            mDetectionManager.IsAlarmWorking = false;
            mMediamanager = GetComponent<MediaManager>();
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mDetectionManager.IsDetectingMovement = GuardianData.Instance.MovementDetection;
            mDetectionManager.IsDetectingSound = GuardianData.Instance.SoundDetection;
            mDetectionManager.IsDetectingFire = GuardianData.Instance.FireDetection;
            mDetectionManager.IsDetectingKidnapping = GuardianData.Instance.KidnappingDetection;

            
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Screen.Locked = true;

            if (!mDetectionManager.HasLinkedDetector)
            {
                mDetectionManager.LinkDetectorsEvents();
            }
            if (!mMediamanager.enabled)
                mMediamanager.enabled = true;

            IEnumerator lAction = DelayAlertAsync();
            StartCoroutine(lAction);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

        private IEnumerator DelayAlertAsync()
        {
            yield return new WaitForSeconds(4F);
            if (GuardianData.Instance.MobileDetection)
                Trigger("MobileDetection");
            else
                Trigger("FixedDetection");
        }
	}
}