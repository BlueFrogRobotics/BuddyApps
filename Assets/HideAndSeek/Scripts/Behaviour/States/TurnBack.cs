using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class TurnBack : AStateMachineBehaviour
    {

        public override void Init()
        {
            
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
           // mWheels.MoveDistance(100, 100, 10, 0.1f);
            mWheels.TurnAngle(180.0f, 100.0f, 0.02f);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mWheels.TurnAngle(30.0f, 200.0f, 0.1f);
            if (mWheels.Status==MovingState.MOTIONLESS)
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