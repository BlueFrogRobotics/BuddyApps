using UnityEngine;
using UnityEngine.UI;
using Buddy;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    public class DisplayStep : AStateMachineBehaviour
    {

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER DISPLAY STEP");
            string lText = GetComponent<RecipeBehaviour>().StepList[GetComponent<RecipeBehaviour>().StepIndex].sentence;
            if (GetGameObject(4).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Window_FullImage_Off"))
                GetGameObject(4).GetComponent<Animator>().SetTrigger("Open_WFullImage");
            GetGameObject(12).GetComponent<Text>().text = lText;
            GetGameObject(7).GetComponent<RawImage>().texture = Resources.Load<Texture>(GetComponent<RecipeBehaviour>().StepList[GetComponent<RecipeBehaviour>().StepIndex].media) as Texture;
            for (int i = 0; i < lText.Length; i++)
            {
                if (lText[i] == '.' && i < lText.Length - 1)
                    lText = lText.Insert(i + 1, "[1000]");
            }
			Interaction.TextToSpeech.Say(lText);
            GetComponent<RecipeBehaviour>().StepIndex++;
            GetComponent<Animator>().SetTrigger("TransitionStep");
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("EXIT DISPLAY STEP");
        }
    }
}