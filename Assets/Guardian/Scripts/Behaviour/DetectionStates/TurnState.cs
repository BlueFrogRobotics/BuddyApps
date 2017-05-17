using UnityEngine;
using Buddy;
using Buddy.Features.Detection;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class TurnState : AStateMachineBehaviour
    {
        private DetectionManager mDetectionManager;
        private IEnumerator mAction;

        public override void Start()
        {
            mDetectionManager = GetComponent<DetectionManager>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMood.Set(MoodType.LISTENING);

            mAction = WatchAtAngle();
            StartCoroutine(mAction);
        }

        private IEnumerator WatchAtAngle()
        {
            mDetectionManager.IsDetectingMovement = GuardianData.Instance.MovementDetection;
            mDetectionManager.IsDetectingKidnapping = GuardianData.Instance.KidnappingDetection;

            yield return new WaitForSeconds(2F);

            mDetectionManager.IsDetectingMovement = false;
            mDetectionManager.IsDetectingKidnapping = false;

            mWheels.TurnAngle(30.0f, 70.0F, 0.02F);

            if (mWheels.Status != MovingState.MOTIONLESS)
                yield return null;

            yield return new WaitForSeconds(1F);

            CommonIntegers["Angle"] += 30;
            Trigger("MobileDetection");
        }

        public void StopTurnCoroutines()
        {
            if (mAction != null)
                StopCoroutine(mAction);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMood.Set(MoodType.NEUTRAL);
        }
    }
}