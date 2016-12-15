using UnityEngine;
using BuddyOS;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    public class RecipeBehaviour : MonoBehaviour
    {
        public string mAnswer { get; set; }
        public Recipe mRecipe { get; set; }
        public List<Recipe> mRecipeList { get; set; }
        public string mCategory { get; set; }

        void Start()
        {
        }

        public void Exit()
        {
            BYOS.Instance.AppManager.Quit();
        }

        public void OnClickCategory(string category)
        {
            List<Recipe> lRecipeList = RecipeList.Deserialize(BuddyTools.Utils.GetStreamingAssetFilePath("recipe_list.xml")).recipe;
            mRecipeList = new List<Recipe>();

            foreach (Recipe recipe in lRecipeList)
            {
                if (recipe.category == category)
                    mRecipeList.Add(recipe);
            }
            GetComponent<Animator>().SetTrigger("DisplayRecipeList");
        }
    }
}
