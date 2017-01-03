using UnityEngine;
using BuddyOS;
using BuddyOS.App;
using UnityEngine.UI;
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

        [SerializeField]
        private List<GameObject> mPrefabIngredientList;

        [SerializeField]
        private List<GameObject> mPrefabIngredientTextList;

        [SerializeField]
        private List<GameObject> mPrefabIngredientIconList;

        private GameObject mRecipeInstance;
        private TextToSpeech mTTS;
        private List<GameObject> mRecipePrefabList;
        private SpriteManager mSpriteManager;
        private Dictionary mDictionary;

        public int NoAnswerCount { get; set; }
        public int RecipeNotFoundCount { get; set; }
        public string mAnswer { get; set; }
        public Recipe mRecipe { get; set; }
        public List<Recipe> mRecipeList { get; set; }
        public string mCategory { get; set; }
        public int IngredientIndex { get; set; }
        public int StepIndex { get; set; }
        public List<Step> StepList { get; set; }
        public bool IsBackgroundActivated { get; set; }
        public int IngredientNbr { get; set; }

        //string lVal = mDictionary.GetString("prepare");
        void Start()
        {
            IsBackgroundActivated = false;
            mTTS = BYOS.Instance.TextToSpeech;
            mSpriteManager = BYOS.Instance.SpriteManager;
            mDictionary = BYOS.Instance.Dictionary;
        }

        public void Exit()
        {
            mTTS.Silence(0);
            BYOS.Instance.AppManager.Quit();
        }

        public void OnClickCategory(string category)
        {
            //List<Recipe> lRecipeList = RecipeList.Deserialize(BuddyTools.Utils.GetStreamingAssetFilePath("tt.xml")).recipe;
            List<Recipe> lRecipeList = RecipeList.Deserialize(BuddyTools.Utils.GetStreamingAssetFilePath(mDictionary.GetString("pathtoxml"))).recipe;
            mRecipeList = new List<Recipe>();

            for (int i = 0; i < lRecipeList.Count; i++)
            {
                if (lRecipeList[i].category == category)
                    mRecipeList.Add(lRecipeList[i]);
            }
            if (mRecipeList.Count > 0)
                GetComponent<Animator>().SetTrigger("DisplayRecipeList");
            else
                GetComponent<Animator>().SetTrigger("NoRecipeFound");
        }

        public void DisplayRecipe()
        {
            mRecipePrefabList = new List<GameObject>();
            for (int i = 0; i < mRecipeList.Count; i++)
            {
                mRecipeInstance = Instantiate(prefabRecipe);
                mRecipePrefabList.Add(mRecipeInstance);
                mRecipeInstance.GetComponent<RectTransform>().SetParent(RecipeListParent.GetComponent<RectTransform>(), false);
                mRecipeInstance.GetComponent<RecipePrefab>().FillRecipe(gameObject, mRecipeList[i]);
            }
        }

        public void OnClickBackToCategory()
        {
            DestroyRecipePrefab();
            GetComponent<Animator>().SetTrigger("BackToCategory");
        }

        public void DestroyRecipePrefab()
        {
            for (int i = 0; i < mRecipePrefabList.Count; i++)
                Destroy(mRecipePrefabList[i]);
        }

        public void DisplayIngredient()
        {
            Ingredient lIngredient;

            for (int i = 0; i < 3; i++)
            {
                if (IngredientIndex < IngredientNbr)
                {
                    lIngredient = mRecipe.ingredient[IngredientIndex];
                    mPrefabIngredientList[i].SetActive(true);
                    if (lIngredient.unit == null)
                    {
                        if (lIngredient.quantity == 0)
                        {
                            mTTS.Say(" [1000] " + lIngredient.name, true);
                            mPrefabIngredientTextList[i].GetComponent<Text>().text = lIngredient.name;
                        }
                        else
                        {
                            mTTS.Say(" [1000] " + lIngredient.name + " [100] " + lIngredient.quantity, true);
                            mPrefabIngredientTextList[i].GetComponent<Text>().text = lIngredient.name + ": " + lIngredient.quantity;
                        }
                    }
                    else
                    {
                        mTTS.Say(" [1000] " + lIngredient.name + " [100] " + lIngredient.quantity + lIngredient.unit, true);
                        mPrefabIngredientTextList[i].GetComponent<Text>().text = lIngredient.name + ": " + lIngredient.quantity + " " + lIngredient.unit;
                    }
                    if (lIngredient.icon != null)
                        mPrefabIngredientIconList[i].GetComponent<Image>().sprite = mSpriteManager.GetSprite(lIngredient.icon, "AtlasRecipe");
                   }
                else
                    mPrefabIngredientList[i].SetActive(false);
                IngredientIndex++;
            }
        }
    }
}