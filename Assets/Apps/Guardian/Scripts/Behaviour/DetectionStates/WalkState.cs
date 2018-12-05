using BlueQuark;

using UnityEngine;

using System.Collections;


namespace BuddyApp.Guardian
{
    /// <summary>
    /// Detects fire and sound while doing a roomba walk
    /// </summary>
    public sealed class WalkState : AStateMachineBehaviour
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

            Buddy.Actuators.Head.No.SetPosition(0);
            Buddy.Actuators.Head.Yes.SetPosition(20, 80);

            mAction = RoombaPatrolAsync();
            StartCoroutine(mAction);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Actuators.Wheels.Stop();
            Buddy.Actuators.Head.No.SetPosition(0);
        }

        private IEnumerator RoombaPatrolAsync()
        {
            //OLD VERSION
            //BYOS.Instance.Navigation.RandomWalk.StartWander(MoodType.NEUTRAL);
            //mDetectionManager.Roomba.enabled = true;

            //NEW VERSION
            //Buddy.Navigation.Run<RoamStrategy>().Until();
            yield return new WaitForSeconds(15F);
            //mDetectionManager.Roomba.enabled = false;

            //OLD VERSION
            //BYOS.Instance.Navigation.Stop();
            Trigger("Turn");
        }

        public void StopWalkCoroutines()
        {
            if (mAction != null)
                StopCoroutine(mAction);
        }

        
    }
}