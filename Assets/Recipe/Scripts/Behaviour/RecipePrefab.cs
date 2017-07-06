using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Buddy;

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

        [SerializeField]
        private GameObject recipeTitle;

        private GameObject aiBehaviour;
        private Recipe mRecipe;
        private bool open = false;
        private TextToSpeech mTTS;
        private string mTextToSay;

        void Start()
        {
            mTTS = BYOS.Instance.Interaction.TextToSpeech;
        }

        public void FillRecipe(GameObject iAiBehaviour, Recipe iRecipe)
        {
            mRecipe = iRecipe;
            aiBehaviour = iAiBehaviour;
            string lString = string.Empty;
            int lTime = mRecipe.prep + mRecipe.cook;

            if ((mTextToSay = mRecipe.summary) != null)
            {
                for (int i = 0; i < mTextToSay.Length; i++)
                {
                    if (mTextToSay[i] == '.' && i < mTextToSay.Length - 1)
                        mTextToSay = mTextToSay.Insert(i + 1, "[800]");
                }
            }
            recipeTitle.GetComponent<Text>().text = mRecipe.name.ToUpper();
            for(int i = 0; i < mRecipe.stars; i++) {
                maskStars[i].GetComponent<Image>().sprite = fullStar;
                stars[i].GetComponent<Image>().sprite = fullStar;
            }
            if ((lTime > 60))
            {
                maskTime.GetComponent<Text>().text = (lTime / 60).ToString() + "H" + (lTime % 60).ToString() + "MIN";
                time.GetComponent<Text>().text = (lTime / 60).ToString() + "H" + (lTime % 60).ToString() + "MIN";
            }
            else
            {
                maskTime.GetComponent<Text>().text = (lTime).ToString() + "MIN";
                time.GetComponent<Text>().text = (lTime).ToString() + "MIN";
            }
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
                if (mTextToSay != null)
                    mTTS.Say(mTextToSay);
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