using UnityEngine;

namespace BuddyApp.Guardian
{
    public sealed class FixedDetectionState : AStateMachineBehaviour
    {
        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
			//if (GuardianData.Instance.ScanDetection) {
			//	Trigger("Turn");
			Debug.Log("enter!!!!!!!!!!!");
			//}
		}

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
			Debug.Log("Fixed State");
			if (GuardianData.Instance.ScanDetection) {
				Trigger("Turn");
				Debug.Log("Turn triggered!!!!!!!!!!");
			}
		}

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}