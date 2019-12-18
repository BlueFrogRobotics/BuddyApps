using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using UnityEngine.UI;

namespace BuddyApp.Recipe
{
    public class RecipeShow : AStateMachineBehaviour {

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
           ShowSteps(RecipeData.Instance.mIndexStep);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if(!Buddy.Vocal.IsListening && !Buddy.Vocal.IsSpeaking)
            {
                Buddy.Vocal.Listen((iSpeechInput) => OnEndListening(iSpeechInput));
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            
        }

        private void ShowSteps(int iIndexStep)
        {
            //montrer etape avec texte + image + vocal qui dit la phrase et en meme temps avoir un listen pour si le user dit suivant / précédent / redire 
            Buddy.Vocal.Say(RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].step);
            Buddy.GUI.Toaster.Display<CustomToast>().With(GetGameObject(1));
            GetGameObject(1).transform.GetChild(0).GetComponent<Text>().text = RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].step;
            GetGameObject(1).transform.GetChild(0).GetComponent<Text>().fontSize = 40;
            GetGameObject(1).transform.GetChild(0).GetComponent<Text>().color = new Color(1F, 1F, 1F);
        }

        private void OnEndListening(SpeechInput iSpeechInput)
        {

        }
    }

}
