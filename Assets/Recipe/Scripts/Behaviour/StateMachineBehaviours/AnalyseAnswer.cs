using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;
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

            //mNotManager.Display<SimpleNot>().With("Hello !! ", BYOS.Instance.SpriteManager.SpriteFromAtlas(BuddyOS.UI.SpriteManager.DEFAULT_ATLAS, "Message"));
            //mNotManager.Display<SimpleNot>().With("Hello !! ", mSpriteManager.GetSprite("Message"));
            //BYOS.Instance.AppManager.Quit();
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
            List<Recipe> lRecipeList = null;
            int lIndex = 0;
            while (lWords[lIndex] != null && SearchRecipe(mRecipeList, lWords[lIndex]) == null)
                lIndex++;
            if (lWords[lIndex] != null)
            {
                lRecipeList = mRecipeList;
                while (lWords[lIndex] != null)
                    lRecipeList = SearchRecipe(lRecipeList, lWords[lIndex]);
            }
            if (lRecipeList == null)
                iAnimator.SetTrigger("RecipeNotFound");
            else if (lRecipeList.Count == 1)
            {
                GetComponent<RecipeBehaviour>().mRecipe = lRecipeList[0];
                iAnimator.SetTrigger("RecipeFound");
            }
            else
            {
                GetComponent<RecipeBehaviour>().mRecipeList = lRecipeList;
                iAnimator.SetTrigger("ListRecipeFound");
            }
        }

        private List<Recipe> SearchRecipe(List<Recipe> iRecipeList, string word)
        {
            List<Recipe> lFoundRecipeList = new List<Recipe>();
            foreach (Recipe recipe in iRecipeList)
            {
                if (recipe.name.Contains(word))
                    lFoundRecipeList.Add(recipe);
            }
            if (lFoundRecipeList.Count > 0)
                return lFoundRecipeList;
            else
                return null;
        }
    }
}