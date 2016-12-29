using UnityEngine;
using System.Collections;
using System;
using BuddyOS;

namespace BuddyApp.IOT
{


    public class IOTNotificationErrorMatch : AIOTStateMachineBehaviours
    {

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            mNotManager.Display<SimpleNot>().With("I didn't understand : " + CommonStrings["STT"]);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
