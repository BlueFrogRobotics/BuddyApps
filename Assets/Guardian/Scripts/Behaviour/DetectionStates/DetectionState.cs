using UnityEngine;
using Buddy;

namespace BuddyApp.Guardian
{
    public class DetectionState : AStateMachineBehaviour
    {
        private DetectionManager mDetectionManager;

        public override void Start()
        {
            mDetectionManager = GetComponent<DetectionManager>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mDetectionManager.MovementDetector.SetSensibilityThreshold(1 - ((float)GuardianData.Instance.MovementDetectionThreshold / 10));
            //mDetectionManager.SoundDetector.SetSensibilityThreshold(1 - ((float)GuardianData.Instance.SoundDetectionThreshold / 10));

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