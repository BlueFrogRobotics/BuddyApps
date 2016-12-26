using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    public class DisplayStep : AStateMachineBehaviour
    {
        private bool mInit = false;
        private List<Step> mStepList { get; set; }

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mInit)
            {
                mStepList = GetComponent<RecipeBehaviour>().mRecipe.step;
                mInit = true;
                GetComponent<RecipeBehaviour>().StepIndex = 0;
            }
            /*if (mStepList[GetComponent<RecipeBehaviour>().StepIndex].media != null)
            {
                GetGameObject(7).GetComponent<RawImage>().texture = Resources.Load(mStepList[GetComponent<RecipeBehaviour>().StepIndex].media) as Texture;
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
                GetGameObject(1).SetActive(true);
                GetGameObject(2).SetActive(false);
                GetGameObject(4).GetComponent<Animator>().SetTrigger("Open_WFullImage");
            }
            else
            {
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
                GetGameObject(1).SetActive(false);
                GetGameObject(2).SetActive(true);
                GetGameObject(4).GetComponent<Animator>().SetTrigger("Close_WFullImage");
            }*/
            GetGameObject(4).GetComponent<Animator>().SetTrigger("Open_WFullImage");
            mTTS.Say(mStepList[GetComponent<RecipeBehaviour>().StepIndex++].sentence);
            GetComponent<Animator>().SetTrigger("TransitionStep");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}