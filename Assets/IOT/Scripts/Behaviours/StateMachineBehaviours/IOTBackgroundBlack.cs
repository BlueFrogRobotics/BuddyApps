using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTBackgroundBlack : AIOTStateMachineBehaviours
    {
        [SerializeField]
        private bool fadeIn = false;
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            Animator lBG = GetGameObject(9).GetComponent<Animator>();
            if (fadeIn)
            {
                if (lBG.GetCurrentAnimatorStateInfo(0).IsName("Background_Black_Off") && !lBG.GetBool("Open_BG"))
                    lBG.SetTrigger("Open_BG");
            }
            else
            {
                if (lBG.GetCurrentAnimatorStateInfo(0).IsName("Background_Black_Idle") && !lBG.GetBool("Close_BG"))
                    lBG.SetTrigger("Close_BG");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
