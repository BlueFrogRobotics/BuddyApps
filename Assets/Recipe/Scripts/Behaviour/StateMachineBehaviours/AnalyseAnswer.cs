﻿using UnityEngine;
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
            Debug.Log("EnterAnalyseAnswer");
            mAnswer = GetComponent<RecipeBehaviour>().mAnswer;
            mRecipeList = RecipeList.Deserialize(BuddyTools.Utils.GetStreamingAssetFilePath("recipe_list.xml")).recipe;
            SearchRecipe(iAnimator);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ExitAnalyseAnswer");
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
                for (; lIndex < lWords.Length; lIndex++)
                    lRecipeList = SearchRecipe(lRecipeList, lWords[lIndex]);
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
                        if (lWords[j].Length > 2 && !mAnswer.Contains(lWords[j].Substring(0, lWords[j].Length - 1)))
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
            bool lFound;

            for (int i = 0; i < iRecipeList.Count; i++)
            {
                lFound = false;
                string[] recipeWords = iRecipeList[i].name.Split(' ');
                for (int j = 0; j < recipeWords.Length; j++)
                {
                    if (recipeWords[j] == iWord || recipeWords[j] + "s" == iWord || recipeWords[j] + "x" == iWord
                        || recipeWords[j] == iWord + "s" || recipeWords[j] == iWord + "x")
                        lFound = true;
                }
                if (lFound || iWord.Length < 4)
                    oFoundRecipeList.Add(iRecipeList[i]);
            }
            return oFoundRecipeList;
        }
    }
}