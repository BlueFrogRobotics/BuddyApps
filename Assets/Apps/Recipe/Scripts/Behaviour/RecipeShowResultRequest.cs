using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Recipe
{
    public class RecipeShowResultRequest : AStateMachineBehaviour
    {
        private Sprite mSprite;
        private float mTimer;
        private bool mHideDone;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            RecipeUtils.DebugColor("ON STATE ENTER SHOW RESULT : " + RecipeData.Instance.mRootObject.title);
            mTimer = 0F;
            mHideDone = false;
            RecipeUtils.DebugColor("ON STATE ENTER SHOW RESULT : " + RecipeData.Instance.mRootObject.analyzedInstructions[0].steps.Count);
            RecipeData.Instance.mListIngredient = new List<string>();
            //for (int i = 0; i < RecipeData.Instance.mRootObjectStep.result[0].steps.Count; ++i)
            //{
            //    RecipeUtils.DebugColor("i");
            //    for (int j = 0; j < RecipeData.Instance.mRootObjectStep.result[0].steps[i].ingredients.Count; ++j )
            //    {
            //        RecipeUtils.DebugColor("j");
            //        if (RecipeData.Instance.mRootObjectStep.result[0].steps[i].ingredients[j] != null)
            //        {
            //            RecipeData.Instance.mListIngredient.Add(RecipeData.Instance.mRootObjectStep.result[0].steps[i].ingredients[j].name);
            //        }
            //    }

            //}

            if (!string.IsNullOrEmpty(RecipeData.Instance.mRootObject.image))
            {
                StartCoroutine(DownloadImage(RecipeData.Instance.mRootObject.image));
                RecipeUtils.DebugColor("URL IMG : " + RecipeData.Instance.mRootObject.image, "blue");

            }
            else
                ShowOverallInfos();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if (mTimer > 5F && !Buddy.Vocal.IsBusy && !mHideDone)
            {
                mHideDone = true;
                Buddy.GUI.Toaster.Hide();

            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {

        }


        private IEnumerator DownloadImage(string iUri)
        {
            using (WWW www = new WWW(iUri))
            {
                yield return www;

                if (www.error == null)
                {
                    mSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
                    Buddy.GUI.Toaster.Display<CustomToast>().With(GetGameObject(2), () => { Buddy.Vocal.SayKey("recipeshowimg"); }, () => { ShowOverallInfos(); });
                    GetGameObject(2).GetComponent<Image>().sprite = mSprite;
                }
                else
                {
                    RecipeUtils.DebugColor("WWW Error: " + www.error, "blue");
                }
            }
        }

        private void ShowOverallInfos()
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                TText lTitle = iBuilder.CreateWidget<TText>();
                lTitle.SetLabel(Buddy.Resources.GetString("recipeshowresult") + RecipeData.Instance.mRootObjectList.results[RecipeData.Instance.mIndexRecipe].title);
                TText lPrepTime = iBuilder.CreateWidget<TText>();
                lPrepTime.SetLabel(Buddy.Resources.GetString("recipepreptime") + RecipeData.Instance.mRootObjectList.results[RecipeData.Instance.mIndexRecipe].readyInMinutes);
                TText lServing = iBuilder.CreateWidget<TText>();
                lServing.SetLabel(Buddy.Resources.GetString("recipeserving") + RecipeData.Instance.mRootObjectList.results[RecipeData.Instance.mIndexRecipe].servings);
                TText lNbStep = iBuilder.CreateWidget<TText>();
                lNbStep.SetLabel(Buddy.Resources.GetString("recipestep") + RecipeData.Instance.mRootObject.analyzedInstructions[0].steps.Count);
                TText lGluten = iBuilder.CreateWidget<TText>();
                lGluten.SetLabel(Buddy.Resources.GetString("recipegluten") + RecipeData.Instance.mRootObject.glutenFree);
                TText lVegetarian = iBuilder.CreateWidget<TText>();
                lVegetarian.SetLabel(Buddy.Resources.GetString("recipevegetarian")+ RecipeData.Instance.mRootObject.vegetarian);
                TText lVegan = iBuilder.CreateWidget<TText>();
                lVegan.SetLabel("Vegan : " + RecipeData.Instance.mRootObject.vegan);
            }, () =>
            {
                RecipeUtils.DebugColor("CANCELLLLL", "blue");
                Buddy.GUI.Toaster.Hide();
            }, "CHOOSE ANOTHER RECIPE",
            () =>
            {
                RecipeUtils.DebugColor("OKKKKKKKKK", "blue");
                Buddy.GUI.Toaster.Hide();
            }, "START",
            null,
            () =>
            {
                Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {

                    for (int i = 0; i < RecipeData.Instance.mRootObject.extendedIngredients.Count; ++i)
                    {
                        TVerticalListBox lBox = iBuilder.CreateBox();
                        lBox.SetLabel(RecipeData.Instance.mRootObject.extendedIngredients[i].name, RecipeData.Instance.mRootObject.extendedIngredients[i].amount.ToString() + " " + RecipeData.Instance.mRootObject.extendedIngredients[i].unit);
                        //lBox.LeftButton.Hide();
                        //Can't use i directly in lambda expression, see the closure problem
                    }
                });

                FLabeledButton lFooterButton = Buddy.GUI.Footer.CreateOnMiddle<FLabeledButton>();
                lFooterButton.SetLabel("Start");
                lFooterButton.OnClick.Add(() => 
                {
                    Debug.Log("Start Recipe ");
                    Buddy.GUI.Toaster.Hide();
                    Buddy.GUI.Footer.Hide();
                    Buddy.Vocal.SayKey("recipestartshow", (iSpeechOutput) => { RecipeData.Instance.mIndexStep = 0; Trigger("START_RECIPE"); });
                });
                
            });
        }

    }
}
