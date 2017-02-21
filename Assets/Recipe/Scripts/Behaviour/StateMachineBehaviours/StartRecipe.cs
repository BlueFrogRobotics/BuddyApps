using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class StartRecipe : AStateMachineBehaviour
    {
        private bool mCheck;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
            GetGameObject(2).SetActive(true);
            GetGameObject(1).SetActive(false);
            GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
            mCheck = false;
            mMood.Set(MoodType.HAPPY, false, true);
            GetComponent<RecipeBehaviour>().StepList = GetComponent<RecipeBehaviour>().mRecipe.step;
            GetComponent<RecipeBehaviour>().StepIndex = 0;
            GetComponent<RecipeBehaviour>().IngredientIndex = 0;
            GetComponent<RecipeBehaviour>().IngredientNbr = GetComponent<RecipeBehaviour>().mRecipe.ingredient.Count;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && mSpeaker.Voice.Status == SoundChannelStatus.FINISH)
            {
                mTTS.Say(mDictionary.GetRandomString("startrecipe") + " " + GetComponent<RecipeBehaviour>().mRecipe.name + "[800]");
                if (GetComponent<RecipeBehaviour>().mRecipe.person > 1)
                    mTTS.Say(mDictionary.GetString("startingredient") + GetComponent<RecipeBehaviour>().mRecipe.person + mDictionary.GetString("person") + "s:[500]", true);
                else
                    mTTS.Say(mDictionary.GetString("startingredient") + GetComponent<RecipeBehaviour>().mRecipe.person + mDictionary.GetString("person") + ":[500]", true);
                mCheck = true;
            }
            if (mCheck && mTTS.HasFinishedTalking)
                iAnimator.SetTrigger("DisplayIngredient");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocalManager.StopListenBehaviour = null;
            mMood.Set(MoodType.NEUTRAL);
            if (!GetComponent<RecipeBehaviour>().IsBackgroundActivated)
            {
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
                GetGameObject(2).SetActive(false);
                GetGameObject(1).SetActive(true);
                GetComponent<RecipeBehaviour>().IsBackgroundActivated = true;
            }
        }
    }
}