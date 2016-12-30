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
            Debug.Log("ENTER DISPLAY STEP");
            if (!mInit) {
                StepList = GetComponent<RecipeBehaviour>().mRecipe.step;
                mInit = true;
                GetComponent<RecipeBehaviour>().StepIndex = 0;
            }
            string lText = StepList[GetComponent<RecipeBehaviour>().StepIndex].sentence;
            if (GetGameObject(4).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Window_FullImage_Off"))
                GetGameObject(4).GetComponent<Animator>().SetTrigger("Open_WFullImage");
            GetGameObject(12).GetComponent<Text>().text = lText;
            GetGameObject(7).GetComponent<RawImage>().texture = Resources.Load(StepList[GetComponent<RecipeBehaviour>().StepIndex].media) as Texture;
            for (int i = 0; i < lText.Length; i++)
            {
                if (lText[i] == '.' && i < lText.Length - 1)
                    lText = lText.Insert(i + 1, "[1000]");
            }
            mTTS.Say(lText);
            GetComponent<RecipeBehaviour>().StepIndex++;
            GetComponent<Animator>().SetTrigger("TransitionStep");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("EXIT DISPLAY STEP");
        }
    }
}