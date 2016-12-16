using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class TurnState : AStateMachineBehaviour
    {
        private float mTimer;

        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0.0f;

            // mWheels.MoveDistance(100, 100, 10, 0.1f);
            MovementDetector.Direction lDir = (MovementDetector.Direction)iAnimator.GetInteger("MovingDetect");
            if (lDir != MovementDetector.Direction.NONE)
                mTTS.Say("Je t'ai vu bouger");
            if (lDir==MovementDetector.Direction.LEFT)
                mWheels.TurnAngle(45.0f, 200.0f, 0.02f);
            else if (lDir == MovementDetector.Direction.RIGHT)
                mWheels.TurnAngle(-45.0f, 200.0f, 0.02f);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;

            //mWheels.TurnAngle(30.0f, 200.0f, 0.1f);
            if (mTimer>1.5f && mWheels.Status == MobileBaseStatus.MOTIONLESS)
            {
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            iAnimator.ResetTrigger("ChangeState");
        }
    }
}