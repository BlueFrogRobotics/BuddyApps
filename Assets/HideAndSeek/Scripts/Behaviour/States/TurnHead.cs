using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class TurnHead : AStateMachineBehaviour
    {
        private float mTargetAngle;

        public override void Init()
        {
            mTargetAngle = 60;
            
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mNoHinge.SetPosition(mTargetAngle, 200);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Mathf.Abs(mNoHinge.CurrentAnglePosition - mNoHinge.DestinationAnglePosition) < 10.5f)
            {
                mTargetAngle *= -1;
                mNoHinge.SetPosition(mTargetAngle, 200);
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            iAnimator.ResetTrigger("ChangeState");
        }
    }
}