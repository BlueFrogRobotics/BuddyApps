using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.App;
using BuddyFeature.Navigation;
using System;

namespace BuddyApp.HideAndSeek
{
    public class StopRoombaNavigation : AStateMachineBehaviour
    {
        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetComponent<RoombaNavigation>().enabled = false;
            //GetGameObject(0).SetActive(false);
            mWheels.StopWheels();
            //mFace.SetExpression(MoodType.NEUTRAL);
            mMood.Set(MoodType.NEUTRAL);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mWheels.Status == MovingState.MOTIONLESS)
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