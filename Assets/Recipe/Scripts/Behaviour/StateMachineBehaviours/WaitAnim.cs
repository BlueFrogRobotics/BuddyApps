using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class WaitAnim : AStateMachineBehaviour
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
                GetComponent<Animator>().SetTrigger("FinishRecipe");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
            GetGameObject(1).SetActive(false);
            GetGameObject(2).SetActive(true);
        }
    }
}