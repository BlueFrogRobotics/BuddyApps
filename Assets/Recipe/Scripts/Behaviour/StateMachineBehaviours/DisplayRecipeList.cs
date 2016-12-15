using UnityEngine;
using System.Collections.Generic;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.Recipe
{
    public class DisplayRecipeList : AStateMachineBehaviour
    {
        private List<Recipe> mRecipeList;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mRecipeList = GetComponent<RecipeBehaviour>().mRecipeList;
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
            GetGameObject(2).SetActive(false);
            GetGameObject(1).SetActive(true);
            GetGameObject(6).GetComponent<Animator>().SetTrigger("Open_WCategory");
            foreach (Recipe recipe in mRecipeList)
            {

            }
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mTTS.IsSpeaking())
                mTTS.Stop();
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
            GetGameObject(1).SetActive(false);
            GetGameObject(2).SetActive(true);
            GetGameObject(6).GetComponent<Animator>().SetTrigger("Close_WCategory");
        }
    }
}