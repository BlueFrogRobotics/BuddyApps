using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class TurnHead : AStateMachineBehaviour
    {
        private float mTargetAngle;
        private float mTimer;
        private bool mIsTurningHead = true;

        public override void Init()
        {
            mTargetAngle = 60;
            
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mNoHinge.SetPosition(mTargetAngle, 200);
            mTimer = 0.0f;
            mIsTurningHead = true;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;

            if(mTimer>4.0f)
            {
                mIsTurningHead = !mIsTurningHead;
                mTimer = 0.0f;
            }

            if (mIsTurningHead && Mathf.Abs(mNoHinge.CurrentAnglePosition - mNoHinge.DestinationAnglePosition) < 10.5f)
            {
                mTargetAngle *= -1;
                mNoHinge.SetPosition(mTargetAngle, 200);
            }
            else if(!mIsTurningHead)
                mNoHinge.SetPosition(0, 200);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            iAnimator.ResetTrigger("ChangeState");
        }
    }
}