using UnityEngine;
using BuddyOS;
using BuddyOS.App;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    [RequireComponent(typeof(StateMachineAppLinker))]
    public class RecipeBehaviour : MonoBehaviour
    {
        [SerializeField]
        private GameObject RecipeListParent;
        [SerializeField]
        private GameObject prefabRecipe;
        private GameObject mRecipeInstance;
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
            if (mRecipeList.Count > 0)
                GetComponent<Animator>().SetTrigger("DisplayRecipeList");
            else
                GetComponent<Animator>().SetTrigger("NoRecipeFound");
        }

        public void DisplayRecipe()
        {
            foreach (Recipe recipe in mRecipeList)
            {
                mRecipeInstance = Instantiate(prefabRecipe);
                mRecipeInstance.transform.SetParent(RecipeListParent.transform);
                mRecipeInstance.GetComponent<RecipePrefab>().FillRecipe(gameObject, recipe);
            }
        }
    }
}
