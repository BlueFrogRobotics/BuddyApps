﻿using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.App;
using BuddyFeature.Navigation;
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
            GetComponent<RoombaNavigation>().enabled = true;
            //GetGameObject(0).SetActive(true);
            //mFace.SetExpression(MoodType.LOVE);
            mYesHinge.SetPosition(20);
            mMood.Set(MoodType.LISTENING);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

    }
}