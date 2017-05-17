using UnityEngine;
using Buddy.Features.Detection;
using Buddy.Features.Navigation;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class WalkState : AStateMachineBehaviour
    {
        private DetectionManager mDetectionManager;

        private IEnumerator mAction;

        public override void Start()
        {
            mDetectionManager = GetComponent<DetectionManager>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mDetectionManager.IsDetectingMovement = false;
            mDetectionManager.IsDetectingKidnapping = false;

            mNoHinge.SetPosition(0);
            mYesHinge.SetPosition(20, 80);

            mAction = RoombaPatrol();
            StartCoroutine(mAction);
        }

        private IEnumerator RoombaPatrol()
        {
            mDetectionManager.Roomba.enabled = true;

            yield return new WaitForSeconds(15F);

            mDetectionManager.Roomba.enabled = false;
            Trigger("Turn");
        }

        public void StopWalkCoroutines()
        {
            if (mAction != null)
                StopCoroutine(mAction);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWheels.StopWheels();
            mNoHinge.SetPosition(0);
        }
    }
}