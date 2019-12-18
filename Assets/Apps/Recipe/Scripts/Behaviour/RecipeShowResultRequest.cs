using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Recipe
{
    public class RecipeShowResultRequest : AStateMachineBehaviour
    {
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            RecipeUtils.DebugColor("ON STATE ENTER SHOW RESULT : " + RecipeData.Instance.mRootObject.title);

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
            ShowOverallInfos();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {

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
                Debug.Log("Cancel");
                Buddy.GUI.Toaster.Hide();
            }, "CHOOSE ANOTHER RECIPE",
            () =>
            {
                Debug.Log("OK");
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
