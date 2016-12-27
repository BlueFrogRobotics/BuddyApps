using UnityEngine;
using BuddyOS.App;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    public class AnalyseAnswer : AStateMachineBehaviour
    {
        private string mAnswer;
        private List<Recipe> mRecipeList;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mAnswer = GetComponent<RecipeBehaviour>().mAnswer;
            mRecipeList = RecipeList.Deserialize(BuddyTools.Utils.GetStreamingAssetFilePath("recipe_list.xml")).recipe;
            SearchRecipe(iAnimator);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        private void SearchRecipe(Animator iAnimator)
        {
            string[] lWords = mAnswer.Split(' ');
            List<Recipe> lRecipeList = new List<Recipe>();
            int lIndex = 0;
            bool lFoundRecipe = false;

            while (lIndex < lWords.Length && SearchRecipe(mRecipeList, lWords[lIndex]).Count == 0)
                lIndex++;
            if (lIndex < lWords.Length) {
                lRecipeList = mRecipeList;
                for (; lIndex < lWords.Length; lIndex++) {
                    if (lWords[lIndex].Length > 3 && lWords[lIndex] != "avec")
                        lRecipeList = SearchRecipe(lRecipeList, lWords[lIndex]);
                }
            }
            if (lRecipeList.Count == 0)
                iAnimator.SetTrigger("RecipeNotFound");
            else if (lRecipeList.Count == 1) {
                GetComponent<RecipeBehaviour>().mRecipe = lRecipeList[0];
                iAnimator.SetTrigger("StartRecipe");
            }
            else {
                for (int i = 0; i < lRecipeList.Count; i++) {
                    lWords = lRecipeList[i].name.Split(' ');
                    for (int j = 0; j < lWords.Length; j++) {
                        lFoundRecipe = true;
                        if (lWords[j].Length > 3 && !mAnswer.Contains(lWords[j]))
                            lFoundRecipe = false;
                    }
                    if (lFoundRecipe) {
                        GetComponent<RecipeBehaviour>().mRecipe = lRecipeList[i];
                        iAnimator.SetTrigger("StartRecipe");
                        return;
                    }
                }
                GetComponent<RecipeBehaviour>().mRecipeList = lRecipeList;
                iAnimator.SetTrigger("ListRecipeFound");
            }
        }

        private List<Recipe> SearchRecipe(List<Recipe> iRecipeList, string iWord)
        {
            List<Recipe> oFoundRecipeList = new List<Recipe>();
            foreach (Recipe recipe in iRecipeList)
            {
                if (recipe.name.Contains(iWord) || recipe.name.Contains(iWord + "s") || recipe.name.Contains(iWord + "x"))
                    oFoundRecipeList.Add(recipe);
            }
                return oFoundRecipeList;
        }
    }
}