using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.App;
using System;

namespace BuddyApp.IOT
{
    public class AIOTStateMachineBehaviours : AStateMachineBehaviour
    {
        protected STTError mError;

        protected List<int> mHashList = new List<int>();
        public List<int> HashList { get { return mHashList; } }

        protected enum HashTrigger : int { NEXT, NETWORK_ERROR, MATCH_ERROR, TIMEOUT_ERROR, LISTENED, iot_connect, iot_action, iot_add, iot_account, CHOICE, CLOSE, ACTION, BACK};

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
