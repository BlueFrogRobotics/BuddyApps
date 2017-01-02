﻿using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class AskRecipe : AStateMachineBehaviour
    {

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(2).SetActive(true);
            GetComponent<RecipeBehaviour>().NoAnswerCount = 0;
            GetComponent<RecipeBehaviour>().RecipeNotFoundCount = 0;
            mTTS.Say(mDictionary.GetString("askprepare"));
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking && !mVocalActivation.RecognitionTriggered)
                iAnimator.SetTrigger("QuestionFinished");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}