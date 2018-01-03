using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy.UI;
using Buddy;

namespace BuddyApp.Somfy
{
    public class SomfyManualCommands : AStateMachineBehaviour
    {
        private SomfyBehaviour mSomfyBehaviour;
        private IOTCommandsLayout mCommandsLayout;
        private bool mInitialized;
        private bool mChangeState;

        public override void Start()
        {
            mSomfyBehaviour = GetComponent<SomfyBehaviour>();
            mCommandsLayout = new IOTCommandsLayout(mSomfyBehaviour);
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Toaster.Display<ParameterToast>().With(mCommandsLayout, () => { Trigger("Vocal"); });
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
     
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

    }
}