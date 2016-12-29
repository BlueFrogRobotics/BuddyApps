using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class WaitForAnim : AStateMachineBehaviour
    {
        private bool mDone;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mDone = false;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mDone && GetGameObject(4).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Window_FullImage_Off"))
            {
                mDone = true;
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}