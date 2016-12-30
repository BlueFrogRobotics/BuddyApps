using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class FinishRecipe : AStateMachineBehaviour
    {
        private bool mDone;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mDone = false;
            mTTS.Say(mDictionary.GetString("finish"));
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mDone && mTTS.HasFinishedTalking)
            {
                mDone = true;
                //GetComponent<RecipeBehaviour>().Exit();
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}