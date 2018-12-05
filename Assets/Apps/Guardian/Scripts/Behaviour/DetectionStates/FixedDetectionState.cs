using UnityEngine;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State in which monitoring occurs in fix mode.
    /// It can move the head if scan detection is enabled.
    /// </summary>
    public sealed class FixedDetectionState : AStateMachineBehaviour
    {
        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

		}

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
			if (GuardianData.Instance.ScanDetection) {
				Trigger("Turn");
			}
		}

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}