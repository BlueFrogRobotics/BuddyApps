using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class RecipeNotFound : AStateMachineBehaviour
    {
        bool mCheck;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCheck = false;
            mMood.Set(MoodType.THINKING, false, true);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && mSpeaker.Voice.Status == SoundChannelStatus.FINISH)
            {
                if (GetComponent<RecipeBehaviour>().RecipeNotFoundCount < 2)
                    mTTS.Say(mDictionary.GetRandomString("recipenotfound"));
                else
                    mMood.Set(MoodType.NEUTRAL);
                mCheck = true;
            }
            else if (mCheck && mTTS.HasFinishedTalking)
                iAnimator.SetTrigger("AskRecipeAgain");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetComponent<RecipeBehaviour>().RecipeNotFoundCount++;
        }
    }
}