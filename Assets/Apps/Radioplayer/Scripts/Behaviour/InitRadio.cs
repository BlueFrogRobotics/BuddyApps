using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Radioplayer
{
    public class InitRadio : AStateMachineBehaviour
    {

        private RadioStream mStream;

        public override void Start()
        {
            mStream = GetComponent<RadioStream>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStream.GetToken();
            Trigger("Play");
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }
    }
}