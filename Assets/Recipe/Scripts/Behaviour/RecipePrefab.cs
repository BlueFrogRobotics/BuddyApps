﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.Recipe
{
    public class RecipePrefab : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> maskStars;

        [SerializeField]
        private GameObject maskTime;

        [SerializeField]
        private GameObject maskText;

        [SerializeField]
        private GameObject maskIngredient;

        [SerializeField]
        private GameObject maskDetail;

        [SerializeField]
        private GameObject image;

        [SerializeField]
        private List<GameObject> stars;

        [SerializeField]
        private GameObject time;

        [SerializeField]
        private GameObject text;

        [SerializeField]
        private GameObject infosButton;

        [SerializeField]
        private GameObject launchButton;

        [SerializeField]
        private Sprite fullStar;

        private GameObject aiBehaviour;
        private Recipe mRecipe;
        private bool open = false;
        private TextToSpeech mTTS;

        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
        }

        public void FillRecipe(GameObject iAiBehaviour, Recipe iRecipe)
        {
            mRecipe = iRecipe;
            aiBehaviour = iAiBehaviour;
            string lString = string.Empty;

            for(int i = 0; i < mRecipe.stars; i++) {
                maskStars[i].GetComponent<Image>().sprite = fullStar;
                stars[i].GetComponent<Image>().sprite = fullStar;
            }
            maskTime.GetComponent<Text>().text = (mRecipe.prep + mRecipe.cook).ToString() + "MIN";
            if (mRecipe.summary != null && mRecipe.summary.Length > 350)
                maskText.GetComponent<Text>().text = mRecipe.summary.Substring(0, 347) + "...";
            else
                maskText.GetComponent<Text>().text = mRecipe.summary;
            maskIngredient.GetComponent<Text>().text = "Ingredients (pour " + mRecipe.person + " personnes) :";
            foreach (Ingredient ingredient in mRecipe.ingredient) {
                if (ingredient.unit == null) {
                    if (ingredient.quantity == 0)
                        lString = lString + ingredient.name + '\n';
                    else
                        lString = lString + ingredient.name + ": " + ingredient.quantity + '\n';
                }
                else
                    lString = lString + ingredient.name + ": " + ingredient.quantity + " " + ingredient.unit + '\n';
            }
            maskDetail.GetComponent<Text>().text = lString;
            image.GetComponent<RawImage>().texture = Resources.Load(mRecipe.illustration) as Texture;
            time.GetComponent<Text>().text = (mRecipe.prep + mRecipe.cook).ToString() + "MIN";
            if (mRecipe.summary != null && mRecipe.summary.Length > 93)
                text.GetComponent<Text>().text = mRecipe.summary.Substring(0, 90) + "...";
            else
                text.GetComponent<Text>().text = mRecipe.summary;
            infosButton.GetComponent<Button>().onClick.AddListener(RecipeInfo);
            launchButton.GetComponent<Button>().onClick.AddListener(LaunchRecipe);
        }

        private void RecipeInfo()
        {
            if (!open) {
                open = !open;
                GetComponent<Animator>().SetTrigger("Open_Recipe");
                if (mRecipe.summary != null)
                    mTTS.Say(mRecipe.summary);
            }
            else {
                open = !open;
                GetComponent<Animator>().SetTrigger("Close_Recipe");
                mTTS.Silence(0);
            }
        }

        private void LaunchRecipe()
        {
            mTTS.Silence(0);
            aiBehaviour.GetComponent<RecipeBehaviour>().mRecipe = mRecipe;
            aiBehaviour.GetComponent<Animator>().SetTrigger("StartRecipe");
        }
    }
}