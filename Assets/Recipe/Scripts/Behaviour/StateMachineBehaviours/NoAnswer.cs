using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class NoAnswer : AStateMachineBehaviour
    {

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (GetComponent<RecipeBehaviour>().NoAnswerCount >= 1)
                iAnimator.SetTrigger("ChooseWithScreen");
            else
                mTTS.Say(mDictionary.GetString("noanswerrecipe"));
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking && GetComponent<RecipeBehaviour>().NoAnswerCount < 1)
                iAnimator.SetTrigger("AskRecipeAgain");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetComponent<RecipeBehaviour>().NoAnswerCount++;
        }
    }
}