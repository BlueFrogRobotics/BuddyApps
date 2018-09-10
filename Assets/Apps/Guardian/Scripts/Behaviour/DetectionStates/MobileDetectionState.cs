using UnityEngine;

namespace BuddyApp.Guardian
{
	public class MobileDetectionState : AStateMachineBehaviour
	{
        private GuardianData mData;
		public override void Start()
		{
			//CommonIntegers.Add("Angle", 0);
            //mData.Angle = 0;
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			//if (CommonIntegers["Angle"] >= 180) {
			//	CommonIntegers["Angle"] = 0;
			//}

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

			//if (CommonIntegers["Angle"] == 0) {
			//	Trigger("Walk");
			//} else
			//	Trigger("Turn");
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}