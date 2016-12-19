using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

        public void FillRecipe(GameObject iAiBehaviour, Recipe iRecipe)
        {
            mRecipe = iRecipe;
            aiBehaviour = iAiBehaviour;
            //transform.SetParent(aiBehaviour.GetComponent<RectTransform>());
            string lString = string.Empty;

            for(int i = 0; i < mRecipe.stars; i++)
            {
                maskStars[i].GetComponent<Image>().sprite = fullStar;
                stars[i].GetComponent<Image>().sprite = fullStar;
            }
            maskTime.GetComponent<Text>().text = mRecipe.time;
            maskText.GetComponent<Text>().text = mRecipe.summary;
            maskIngredient.GetComponent<Text>().text = "Ingredients (pour " + mRecipe.person + " personnes) :";
            foreach (Ingredient ingredient in mRecipe.ingredient)
                lString = lString + ingredient.name + ": " + ingredient.quantity + " " + ingredient.unit + '\n';
            maskDetail.GetComponent<Text>().text = lString;
            image.GetComponent<RawImage>().texture = Resources.Load(mRecipe.illustration) as Texture;
            time.GetComponent<Text>().text = mRecipe.time;
            //"..." only if summary > 90
            text.GetComponent<Text>().text = mRecipe.summary.Substring(0, 90) + "...";
            infosButton.GetComponent<Button>().onClick.AddListener(RecipeInfo);
            launchButton.GetComponent<Button>().onClick.AddListener(LaunchRecipe);
        }

        private void RecipeInfo()
        {
            GetComponent<Animator>().SetTrigger("Open_Recipe");
        }

        private void LaunchRecipe()
        {
            aiBehaviour.GetComponent<RecipeBehaviour>().mRecipe = mRecipe;
            aiBehaviour.GetComponent<Animator>().SetTrigger("LoadRecipe");
        }
    }
}