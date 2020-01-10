using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using UnityEngine.UI;

namespace BuddyApp.Recipe
{
    public class RecipeShow : AStateMachineBehaviour {

        private string mBaseUrl = "https://spoonacular.com/cdn/ingredients_100x100/,";
        private Sprite mSprite;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
           
            ShowSteps(RecipeData.Instance.mIndexStep);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            //Buddy.GUI.Toaster.Hide();
            GetGameObject(1).transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            GetGameObject(1).transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        private void ShowSteps(int iIndexStep)
        {
            //if(!Buddy.Navigation.IsBusy && RecipeData.Instance.mUserWantMovingBuddy)
            //    Buddy.Navigation.Run<HumanTrackStrategy>().StaticTracking(tracking => true, null, BehaviourMovementPattern.HEAD | BehaviourMovementPattern.BODY_ROTATION);

            if (RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].equipment.Count > 0)
            {
                RecipeUtils.DebugColor("EQUIPEMENT IMG : " + RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].equipment[0].name, "red");
                StartCoroutine(DownloadImage(mBaseUrl + RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].equipment[0].image));
            }
            else if (RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].equipment.Count <= 0 && RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].ingredients.Count > 0)
            {
                RecipeUtils.DebugColor("INGREDIENT IMG : " + RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].ingredients[0].name, "red");
                StartCoroutine(DownloadImage(mBaseUrl + RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].ingredients[0].image));
            }

            //montrer etape avec texte + image + vocal qui dit la phrase et en meme temps avoir un listen pour si le user dit suivant / précédent / redire 
            Buddy.Vocal.Say(RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].step, false);
            Buddy.Vocal.ListenOnTrigger = true;
            Buddy.Vocal.OnEndListening.Add((iSpeechInput) => OnEndListening(iSpeechInput));

            if (!Buddy.GUI.Toaster.IsBusy)
            {
                Buddy.GUI.Toaster.Display<CustomToast>().With(GetGameObject(1));
            }
                
            GetGameObject(1).transform.GetChild(0).GetComponent<Text>().text = RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].step;
            GetGameObject(1).transform.GetChild(0).GetComponent<Text>().fontSize = 35;
            GetGameObject(1).transform.GetChild(0).GetComponent<Text>().color = new Color(1F, 1F, 1F);

            GetGameObject(1).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = Buddy.Resources.GetString("recipeprevious");
            GetGameObject(1).transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ButtonPrevious);
            GetGameObject(1).transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = Buddy.Resources.GetString("recipenext");
            GetGameObject(1).transform.GetChild(2).GetComponent<Button>().onClick.AddListener(ButtonNext);

            GetGameObject(1).transform.GetChild(3).GetComponent<Image>().sprite = mSprite;

            RecipeUtils.DebugColor("ENTER SHOW RESULT : " + RecipeData.Instance.mIndexStep, "red");

            //RecipeData.Instance.mRootObject.analyzedInstructions[0].steps[iIndexStep].equipment
        }

        private void OnEndListening(SpeechInput iSpeechInput)
        {
            RecipeUtils.DebugColor("ONENDLISTENING : " + iSpeechInput + " " + Buddy.Resources.GetString("recipeprevious") + " " + Buddy.Resources.GetString("recipenext"), "red");
            //if (!iSpeechInput.IsInterrupted)
            //{
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance) && Utils.ContainsOneOf(iSpeechInput.Utterance, Buddy.Resources.GetString("recipeprevious")))
            {
                ButtonPrevious();
            }
            else if ((!string.IsNullOrEmpty(iSpeechInput.Utterance) && Utils.ContainsOneOf(iSpeechInput.Utterance, Buddy.Resources.GetString("recipenext"))))
            {
                ButtonNext();
            }
            else if ((!string.IsNullOrEmpty(iSpeechInput.Utterance) && Utils.ContainsOneOf(iSpeechInput.Utterance, Buddy.Resources.GetString("reciperepeat"))))
            {
                
            }
            else if ((!string.IsNullOrEmpty(iSpeechInput.Utterance) && Utils.ContainsOneOf(iSpeechInput.Utterance, Buddy.Resources.GetString("recipestopmoving"))))
            {
                RecipeData.Instance.mUserWantMovingBuddy = false;
                Buddy.Navigation.Stop();
            }
            //}


        }

        private IEnumerator DownloadImage(string iUri)
        {
            using (WWW www = new WWW(iUri))
            {
                yield return www;

                if (www.error == null)
                {
                    mSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    RecipeUtils.DebugColor("WWW Error: " + www.error, "blue");
                }
            }
        }

        private void ButtonPrevious()
        {
            Buddy.Vocal.StopSpeaking();
            
            if (RecipeData.Instance.mIndexStep > 0)
                RecipeData.Instance.mIndexStep--;
            else
                RecipeData.Instance.mIndexStep = 0;
                
            RecipeUtils.DebugColor("PREVIOUS : " + RecipeData.Instance.mIndexStep, "red");
            //GetGameObject(1).transform.GetChild(1).GetComponent<Button>().onClick.RemoveListener(ButtonPrevious);
            Trigger("START_RECIPE");
        }

        private void ButtonNext()
        {
            Buddy.Vocal.StopSpeaking();
            
            if (RecipeData.Instance.mIndexStep < RecipeData.Instance.mRootObject.analyzedInstructions[0].steps.Count - 1)
                RecipeData.Instance.mIndexStep++;
            else
            {
                Buddy.Vocal.SayKey("recipedone", (iSpeechoutput) => { Trigger("RECO_QUESTION"); });
            }
            RecipeUtils.DebugColor("NEXT : " + RecipeData.Instance.mIndexStep, "red");
            //GetGameObject(1).transform.GetChild(2).GetComponent<Button>().onClick.RemoveListener(ButtonNext);
            Trigger("START_RECIPE");
        }
    }

}
