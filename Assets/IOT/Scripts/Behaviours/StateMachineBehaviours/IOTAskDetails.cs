﻿using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTAskDetails : AIOTStateMachineBehaviours
    {
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            GetGameObject(6).GetComponent<IOTDetails>().Object = GetGameObject(5).transform.GetChild(0).GetChild(0).GetChild(0).GetChild(iAnimator.GetInteger(HashList[(int)HashTrigger.Choice]) - 1).GetComponent<IOTObjectContainer>().Object;
            iAnimator.SetTrigger(HashList[(int)HashTrigger.NEXT]);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
