using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class NoAnswer : AStateMachineBehaviour
    {
        private int mCount;

        public override void Init()
        {
            mCount = 1;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("Enter NoAnswer");
            if (mCount >= 1)
                iAnimator.SetTrigger("ChooseWithScreen");
            else
                mTTS.Say(mDictionary.GetString("noanswerrecipe"));
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking && mCount < 1)
                iAnimator.SetTrigger("AskRecipeAgain");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ExitNoAnswer");
            mCount++;
        }
    }
}