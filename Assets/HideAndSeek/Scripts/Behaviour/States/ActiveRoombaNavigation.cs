using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class ActiveRoombaNavigation : AStateMachineBehaviour
    {
        public override void Init()
        {
            
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(0).SetActive(true);
            mFace.SetExpression(MoodType.LOVE);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

    }
}