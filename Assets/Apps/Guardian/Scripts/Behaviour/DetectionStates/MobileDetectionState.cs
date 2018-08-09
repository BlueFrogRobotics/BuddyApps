using UnityEngine;

namespace BuddyApp.Guardian
{
	public class MobileDetectionState : AStateMachineBehaviour
	{
		public override void Start()
		{
			CommonIntegers.Add("Angle", 0);
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (CommonIntegers["Angle"] >= 180) {
				CommonIntegers["Angle"] = 0;
			}

			if (CommonIntegers["Angle"] == 0) {
				Trigger("Walk");
			} else
				Trigger("Turn");
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}