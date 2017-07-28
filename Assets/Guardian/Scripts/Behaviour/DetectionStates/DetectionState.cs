using UnityEngine;
using Buddy;

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
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mDetectionManager.MovementTracker.Threshold = (1 - ((float)GuardianData.Instance.MovementDetectionThreshold / 100.0f))*200.0f;
            mDetectionManager.NoiseStimulus.Threshold = (1 - ((float)GuardianData.Instance.SoundDetectionThreshold / 100.0f))*0.3f;

            mDetectionManager.IsDetectingMovement = GuardianData.Instance.MovementDetection;
            mDetectionManager.IsDetectingSound = GuardianData.Instance.SoundDetection;
            mDetectionManager.IsDetectingFire = GuardianData.Instance.FireDetection;
            mDetectionManager.IsDetectingKidnapping = GuardianData.Instance.KidnappingDetection;

            BYOS.Instance.Toaster.Hide();
            AAppActivity.LockScreen();

            if (GuardianData.Instance.Mode == GuardianMode.FIXED)
                Trigger("FixedDetection");

            if (GuardianData.Instance.Mode == GuardianMode.MOBILE)
                Trigger("MobileDetection");
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}