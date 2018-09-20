using UnityEngine;
using BlueQuark;
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
            //mDetectionManager.NoiseStimulus.Threshold = (1 - ((float)GuardianData.Instance.SoundDetectionThreshold / 100.0f)) * 0.3f;
            mDetectionManager.IsDetectingMovement = GuardianData.Instance.MovementDetection;
            mDetectionManager.IsDetectingSound = GuardianData.Instance.SoundDetection;
            mDetectionManager.IsDetectingFire = GuardianData.Instance.FireDetection;
            mDetectionManager.IsDetectingKidnapping = GuardianData.Instance.KidnappingDetection;
            //BYOS.Instance.WebService.EMailSender.enabled = true;
            Debug.Log("detecting sound set a " + mDetectionManager.IsDetectingSound);
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Screen.Locked = true;

            if (!mDetectionManager.HasLinkedDetector)
            {
                Debug.Log("on link detectors");
                mDetectionManager.LinkDetectorsEvents();
            }
            if (!mMediamanager.enabled)
                mMediamanager.enabled = true;

            IEnumerator lAction = DelayAlert();
            StartCoroutine(lAction);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

        private IEnumerator DelayAlert()
        {
            yield return new WaitForSeconds(4F);
            if (GuardianData.Instance.MobileDetection)
                Trigger("MobileDetection");
            else
                Trigger("FixedDetection");
        }
	}
}