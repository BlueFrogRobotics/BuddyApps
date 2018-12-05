using UnityEngine;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State in which monitoring occurs in mobile mode
    /// </summary>
	public sealed class MobileDetectionState : AStateMachineBehaviour
	{
        private GuardianData mData;

		public override void Start()
		{
            mData = GuardianData.Instance;
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

            if (mData.Angle >= 180)
                mData.Angle = 0;
            if(mData.Angle == 0)
            {
                Trigger("Walk");
            }
            else
            {
                Trigger("Turn");
            }

		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}