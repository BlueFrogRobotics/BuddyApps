using UnityEngine;
using BuddyOS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BuddyApp.Recipe
{
    public class Cooking : MonoBehaviour
    {
        [SerializeField]
        private List<Button> mButtons;
        [SerializeField]
        private RawImage mMedia;
        [SerializeField]
        private Text mText;
        [SerializeField]
        private GameObject loadingScreen;
        [SerializeField]
        private Animator animator;
        private TextToSpeech mTTS;
        private SpeechToText mSTT;
        private VocalActivation mVocalActivation;
        private List<Step> mListStep;
        private int mIndex = 0;
        private List<Recipe> mRecipeList;

        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mSTT = BYOS.Instance.SpeechToText;
            mVocalActivation = BYOS.Instance.VocalActivation;
        }

        private IEnumerator LoadingScreen()
        {
            yield return new WaitForSeconds(3F);

            animator.SetBool("Close_WLoading", true);
            yield return new WaitForSeconds(1F);
            loadingScreen.SetActive(false);
            mRecipeList = RecipeList.Deserialize(BuddyTools.Utils.GetStreamingAssetFilePath("recipe_list.xml")).recipe;
            ChooseRecipe();
            mListStep = Content.Deserialize(BuddyTools.Utils.GetStreamingAssetFilePath("crêpes.xml")).step;
            if (mListStep != null)
                DisplayStep();
        }

        private void ChooseRecipe()
        {
            mTTS.Say("Alors qu'est-ce que l'on prépare ?");
            while (mTTS.IsSpeaking()) { }
            mVocalActivation.VocalProcessing = AnswerRecipeName;
            mVocalActivation.StartInstantReco();
        }

        private void AnswerRecipeName(string iName)
        {
            mText.gameObject.SetActive(true);
            mText.text = iName;
            string[] lWords = iName.Split(' ');
            List<Recipe> lRecipeList = new List<Recipe>();
            int lIndex;
            while (lWords[lIndex] && !SearchRecipe(mRecipeList, lWords[lIndex]))
                lIndex++;
            if (lWords[lIndex])
            {
                lRecipeList = mRecipeList;
                while (lWords[lIndex])
                    lRecipeList = SearchRecipe(lRecipeList, lWords[lIndex]);
            }
            if (!lRecipeList)
                RecipeNotFound();
            else
                RecipeFound(lRecipeList);
        }

        private void RecipeNotFound()
        {
            mTTS.Say("Désolé je n'ai pas trouvée cette recette, veux tu que la recherche sur internet ?");
            while (mTTS.IsSpeaking()) { }
            mVocalActivation.VocalProcessing = AnswerRecipeNotFound;
            mVocalActivation.StartInstantReco();
        }

        private void AnswerRecipeNotFound(string iAnswer)
        {
            if (iAnswer.Contains("oui"))
                mTTS.Say("Fiture non implémentée, menu principal");
            else
                mTTS.Say("menu principal");
            while (mTTS.IsSpeaking()) { }
            ChooseRecipe();
        }

        private void RecipeFound(List<Recipe> iRecipeList)
        {
            if (iRecipeList.Count == 1 || )
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

        private void DisplayStep()
        {
            if (mListStep[mIndex] != null)
            {
                if (mListStep[mIndex].media != null)
                    DisplayMedia(mListStep[mIndex].media);
                if (mListStep[mIndex].sentenceToSay != null && mListStep[mIndex].sentenceToSay.sentence != null)
                    mTTS.Say(mListStep[mIndex].sentenceToSay.sentence);
                if (mListStep[mIndex].sentenceToDisplay != null && mListStep[mIndex].sentenceToDisplay.sentence != null)
                    DisplayText(mListStep[mIndex].sentenceToDisplay);
                if (mListStep[mIndex].transitionList != null && mListStep[mIndex].transitionList.transition != null &&
                    mListStep[mIndex].transitionList.transition.Count > 0)
                {
                    foreach (Transition transition in mListStep[mIndex].transitionList.transition)
                    {
                        if (transition.type == "timer")
                            StartCoroutine(ExecuteAfterTime(transition.target, transition.value));
                        else if (transition.type == "button")
                            TransitionByButton(transition);
                    }
                }
                else
                    FinishStep(mIndex + 1);
            }
        }

        private void FinishStep(int iNextId)
        {
            StopCoroutine("ExecuteAfterTime");
            mTTS.Silence(1);
            mMedia.gameObject.SetActive(false);
            mText.gameObject.SetActive(false);
            foreach (Button button in mButtons)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
            mIndex = iNextId;
            DisplayStep();
        }

        private void DisplayMedia(Media iMedia)
        {
            Position lMediaPosition = iMedia.position;
            RectTransform lMediaTransform = mMedia.GetComponent<RectTransform>();

            mMedia.gameObject.SetActive(true);
            mMedia.texture = Resources.Load(iMedia.path) as Texture; 
            if (lMediaPosition != null)
            {
                lMediaTransform.position = new Vector3(lMediaPosition.x, lMediaPosition.y, 0);
                lMediaTransform.sizeDelta = new Vector2(lMediaPosition.width, lMediaPosition.height);
            }
        }

        private void DisplayText(SentenceToDisplay iText)
        {
            Position lTextPosition = iText.position;
            RectTransform lTextTransform = mText.GetComponent<RectTransform>();

            mText.gameObject.SetActive(true);
            mText.text = iText.sentence;
            if (lTextPosition != null)
            {
                lTextTransform.position = new Vector3(lTextPosition.x, lTextPosition.y, 0);
                lTextTransform.sizeDelta = new Vector2(lTextPosition.width, lTextPosition.height);
            }
        }

        private void TransitionByButton(Transition transition)
        {
            Position lPositionButton = transition.position;
            RectTransform lButtonTransform;

            foreach (Button button in mButtons)
            {
                if (!button.gameObject.activeSelf)
                {
                    button.gameObject.SetActive(true);
                    lButtonTransform = button.GetComponent<RectTransform>();
                    if (lPositionButton != null)
                    {
                        lButtonTransform.localPosition = new Vector3(lPositionButton.x, lPositionButton.y, 0);
                        lButtonTransform.sizeDelta = new Vector2(lPositionButton.width, lPositionButton.height);
                    }
                    button.onClick.AddListener(() => { FinishStep(transition.target); });
                    break;
                }
            }
        }

        IEnumerator ExecuteAfterTime(int iTarget, int iDelay)
        {
            yield return new WaitForSeconds(100);
            FinishStep(iTarget);
        }
    }
}