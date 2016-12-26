using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    public class DisplayStep : AStateMachineBehaviour
    {
        private bool mInit;

        private List<Step> StepList { get; set; }

        public override void Init()
        {
            mInit = false;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mInit) {
                StepList = GetComponent<RecipeBehaviour>().mRecipe.step;
                mInit = true;
                GetComponent<RecipeBehaviour>().StepIndex = 0;
            }
            GetGameObject(4).GetComponent<Animator>().SetTrigger("Open_WFullImage");
            GetGameObject(12).GetComponent<Text>().text = StepList[GetComponent<RecipeBehaviour>().StepIndex].sentence;
            GetGameObject(7).GetComponent<RawImage>().texture = Resources.Load(StepList[GetComponent<RecipeBehaviour>().StepIndex].media) as Texture;
            mTTS.Say(StepList[GetComponent<RecipeBehaviour>().StepIndex++].sentence);
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