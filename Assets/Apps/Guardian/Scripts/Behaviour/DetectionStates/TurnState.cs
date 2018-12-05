using BlueQuark;

using UnityEngine;

using System.Collections;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State where the robot turn and scan until it did a complete turn
    /// </summary>
    public sealed class TurnState : AStateMachineBehaviour
    {
        private GuardianData mData;
        private DetectionManager mDetectionManager;
        private IEnumerator mAction;

        public override void Start()
        {
            mDetectionManager = GetComponent<DetectionManager>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.LISTENING);

            mAction = WatchAtAngleAsync();
            StartCoroutine(mAction);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        }

        private IEnumerator WatchAtAngleAsync()
        {
            mDetectionManager.IsDetectingMovement = GuardianData.Instance.MovementDetection;
            mDetectionManager.IsDetectingKidnapping = GuardianData.Instance.KidnappingDetection;

            yield return new WaitForSeconds(2F);

            mDetectionManager.IsDetectingMovement = false;
            mDetectionManager.IsDetectingKidnapping = false;
            
            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(30.0f, 70.0F);

            if (Buddy.Actuators.Wheels.IsBusy)
                yield return null;

            yield return new WaitForSeconds(1F);

            mData.Angle += 30;
            Trigger("MobileDetection");
        }

        public void StopTurnCoroutines()
        {
            if (mAction != null)
                StopCoroutine(mAction);
        }

        
    }
}