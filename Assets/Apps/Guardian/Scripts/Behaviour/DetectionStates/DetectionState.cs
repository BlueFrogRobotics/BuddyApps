using UnityEngine;
using Buddy;
using System.Collections;

namespace BuddyApp.Guardian
{
	/// <summary>
	/// State that init that activate the detections chosen by hte user and pass to the next mode state
	/// </summary>
	public class DetectionState : AStateMachineBehaviour
	{
		private DetectionManager mDetectionManager;

		public override void Start()
		{
			mDetectionManager = GetComponent<DetectionManager>();
            mDetectionManager.IsAlarmWorking = false;
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            //mDetectionManager.NoiseStimulus.Threshold = (1 - ((float)GuardianData.Instance.SoundDetectionThreshold / 100.0f)) * 0.3f;
            iAnimator.ResetTrigger("InitDetection");
            if (iAnimator.GetBool("Password") == false)
            {
                mDetectionManager.IsDetectingMovement = GuardianData.Instance.MovementDetection;
                mDetectionManager.IsDetectingSound = GuardianData.Instance.SoundDetection;
                mDetectionManager.IsDetectingFire = GuardianData.Instance.FireDetection;
                mDetectionManager.IsDetectingKidnapping = GuardianData.Instance.KidnappingDetection;
            }
            //BYOS.Instance.WebService.EMailSender.enabled = true;

            BYOS.Instance.Toaster.Hide();
            AAppActivity.LockScreen();

            if (!mDetectionManager.HasLinkedDetector)
                mDetectionManager.LinkDetectorsEvents();

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
