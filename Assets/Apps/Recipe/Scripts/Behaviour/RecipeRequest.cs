using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using UnityEngine.UI;

namespace BuddyApp.Recipe
{
    public class RecipeRequest : AStateMachineBehaviour
    {

        public const string mURLCore = "https://api.spoonacular.com/recipes/";
        public const string mURLRequestID = "search?query=";
        public const string mULRRequestAllInformations = "information";
        private const string mKeyDev = "apiKey=b1b1ab62bea04b7a9c2541ef80aa24d0";

        private List<string> ListStep;
        private Image mImageIcon;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            RecipeUtils.DebugColor("RECIPE REQUEST : " + RecipeData.Instance.mRecipeString, "blue");
            
            StartCoroutine(GetRequest(mURLCore + mURLRequestID + RecipeData.Instance.mRecipeString + "&" +  mKeyDev));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.GUI.Toaster.Hide();
        }

        private IEnumerator GetRequest(string iUri)
        {
            using (WWW www = new WWW(iUri))
            {
                yield return www;
                if (www.error == null)
                {
                    RecipeUtils.DebugColor("SALUT GREGOIRE 1 : " ,"blue");
                    RecipeData.Instance.mRootObjectList = JsonUtility.FromJson<RootObjectList>(www.text.Trim());
                    RecipeUtils.DebugColor("SALUT GREGOIRE 2 : ", "blue");
                    //RootObject lRootOjectJson = JsonUtility.FromJson<RootObject>(www.text);
                    if (RecipeData.Instance.mRootObjectList.results.Count == 0)
                    {
                        //N'a rien trouvé
                        Buddy.Vocal.SayAndListen("reciperequestfailed", null, OnEndListenning, null, SpeechRecognitionMode.FREESPEECH_ONLY);
                    }
                    else
                    {
                        if(RecipeData.Instance.mRootObjectList.results.Count == 1)
                        {
                            Buddy.Vocal.SayKey("recipeonefound");
                        }
                        else
                        {
                            //voici ce que j'ai trouvé
                            Buddy.Vocal.SayKey("recipelistfound");
                        }
                        
                        Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {

                            for(int i = 0; i < RecipeData.Instance.mRootObjectList.results.Count; ++i)
                            {
                                TVerticalListBox lBox = iBuilder.CreateBox();
                                lBox.SetLabel(RecipeData.Instance.mRootObjectList.results[i].title, Buddy.Resources.GetString( "recipepreparationtime") + RecipeData.Instance.mRootObjectList.results[i].readyInMinutes + "min / " +  Buddy.Resources.GetString("recipeserving") + RecipeData.Instance.mRootObjectList.results[i].servings);
                                TRightSideButton lRammsteinPlayButton = lBox.CreateRightButton();
                                lRammsteinPlayButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_play", Context.OS));
                                //lBox.LeftButton.Hide();
                                //Can't use i directly in lambda expression, see the closure problem
                                int lindex = i;
                                lRammsteinPlayButton.OnClick.Add(() => {
                                    Buddy.GUI.Toaster.Hide();
                                    RecipeData.Instance.mIndexRecipe = lindex;
                                    ButtonClicked(RecipeData.Instance.mRootObjectList.results[lindex].id);
                                    Debug.Log("Launch Recipe " + RecipeData.Instance.mRootObjectList.results[lindex].title);
                                });
                            }

                            TVerticalListBox lBoxBack = iBuilder.CreateBox();
                            lBoxBack.SetLabel(Buddy.Resources.GetString("recipeback"));
                            lBoxBack.OnClick.Add(() =>
                            {
                                Buddy.GUI.Toaster.Hide();
                                Trigger("REQUEST_FAILED");
                            });
                        });
                    }
                }
                else
                {
                    RecipeUtils.DebugColor("WWW Error: " + www.error, "blue");
                }
            }
        }

        private IEnumerator GetStepFromRequest(string iId)
        {
            RecipeUtils.DebugColor(mURLCore + iId + "/analyzedInstructions?" + mKeyDev, "red");
            using (WWW www = new WWW(mURLCore + iId + "/" + mULRRequestAllInformations + "?" + mKeyDev))
            {
                yield return www;
                if (www.error == null)
                {
                    RecipeData.Instance.mRootObject = JsonUtility.FromJson<RootObject>(www.text);
                    RecipeUtils.DebugColor("BEFORE REQUEST DONE 1 : " + RecipeData.Instance.mRootObject.title);
                    //RecipeUtils.DebugColor("BEFORE REQUEST DONE : " + RecipeData.Instance.mRootObjectStep.result[0].steps[0].ingredients.Count.ToString());
                    //RecipeUtils.DebugColor("DEUXIEME REQUEST : " + RecipeData.Instance.mRootObjectStep.result[0].steps[0].step, "blue");
                    Trigger("REQUEST_DONE");
                }
                else
                {
                    RecipeUtils.DebugColor("SECOND REQUEST Error: " + www.error, "blue");
                }
            }
        }

        private void OnEndListenning(SpeechInput iSpeechInput)
        {
            Debug.Log("OnEndListenning " + iSpeechInput.Utterance);

            if (!iSpeechInput.IsInterrupted)
            {
                if (!string.IsNullOrEmpty(iSpeechInput.Utterance) && Utils.ContainsOneOf(iSpeechInput.Utterance, "recipeyes"))
                {
                    Trigger("REQUEST_FAILED");
                }
                else if (!string.IsNullOrEmpty(iSpeechInput.Utterance) && Utils.ContainsOneOf(iSpeechInput.Utterance, "recipeno"))
                {
                    QuitApp();
                }
            }
        }

        private void ButtonClicked(int iIndexRecipe)
        {

            RecipeUtils.DebugColor("Button clicked :  " + iIndexRecipe, "blue");
            //RecipeData.Instance.mIdRecipeWanted = iIndexRecipe;
            StartCoroutine(GetStepFromRequest(iIndexRecipe.ToString()));
        }

    }

}

