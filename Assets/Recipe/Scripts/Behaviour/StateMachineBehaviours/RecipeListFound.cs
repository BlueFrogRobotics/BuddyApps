using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class RecipeListFound : AStateMachineBehaviour
    {
        bool mCheck;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCheck = false;
            mMood.Set(MoodType.SURPRISED, false, true);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && mSpeaker.Voice.Status == SoundChannelStatus.FINISH)
            {
                mTTS.Say(mDictionary.GetString("listrecipefound"));
                mCheck = true;
            }
            if (mCheck && mTTS.HasFinishedTalking)
                iAnimator.SetTrigger("DisplayRecipeList");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}