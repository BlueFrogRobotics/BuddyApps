﻿using UnityEngine;
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

        public string mAnswer { get; set; }
        public Recipe mRecipe { get; set; }
        public List<Recipe> mRecipeList { get; set; }
        public string mCategory { get; set; }
        public int IngredientIndex { get; set; }
        public int StepIndex { get; set; }
        public bool IsBackgroundActivated { get; set; }
        public int IngredientNbr { get; set; }

        //string lVal = mDictionary.GetString("prepare");
        void Start()
        {
            IsBackgroundActivated = false;
            mTTS = BYOS.Instance.TextToSpeech;
        }

        public void Exit()
        {
            BYOS.Instance.AppManager.Quit();
        }

        public void OnClickCategory(string category)
        {
            List<Recipe> lRecipeList = RecipeList.Deserialize(BuddyTools.Utils.GetStreamingAssetFilePath("recipe_list.xml")).recipe;
            mRecipeList = new List<Recipe>();

            //foreach (Recipe recipe in lRecipeList) {
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
            //foreach (Recipe recipe in mRecipeList) {
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
            for (int i = 0; i < mRecipePrefabList.Count; i++)
            //foreach (GameObject recipe in mRecipePrefabList)
                Destroy(mRecipePrefabList[i]);
            GetComponent<Animator>().SetTrigger("BackToCategory");
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
                    mTTS.Say(" [1000] " + lIngredient.name + " [700] " + lIngredient.quantity + " [200] " + lIngredient.unit, true);
                    mPrefabIngredientTextList[i].GetComponent<Text>().text = lIngredient.name + ": " + lIngredient.quantity + " " + lIngredient.unit;
                    if (lIngredient.icon != null)
                        mPrefabIngredientIconList[i].GetComponent<Image>().sprite = Resources.Load(lIngredient.icon) as Sprite;
                }
                else
                    mPrefabIngredientList[i].SetActive(false);
                IngredientIndex++;
            }
        }
    }
}