using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class TransitionIngredient : AStateMachineBehaviour
    {
        private bool mCheck;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCheck = false;
            GetGameObject(8).GetComponent<Button>().onClick.AddListener(LastIngredient);
            GetGameObject(9).GetComponent<Button>().onClick.AddListener(NextIngredient);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && mTTS.HasFinishedTalking && mSTT.HasFinished) {
                mCheck = true;
                mVocalManager.OnEndReco = VocalProcessing;
                mVocalManager.OnError = VocalError;
                mVocalManager.StartInstantReco();
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocalManager.OnEndReco = null;
            mVocalManager.OnError = null;
            mVocalManager.StopListenBehaviour();
            mTTS.Silence(0);
            GetGameObject(8).GetComponent<Button>().onClick.RemoveAllListeners();
            GetGameObject(9).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        private void VocalProcessing(string answer)
        {
            answer = answer.ToLower();
            if (ContainKeyWord(answer, mDictionary.GetString("next").Split(' ')))
                NextIngredient();
            else if (ContainKeyWord(answer, mDictionary.GetString("last").Split(' ')))
                LastIngredient();
            else if (ContainKeyWord(answer, mDictionary.GetString("repeat").Split(' '))) {
                GetComponent<RecipeBehaviour>().IngredientIndex -= 3;
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
            } else
                mVocalManager.StartInstantReco();
        }

        private bool ContainKeyWord(string iAnswer, string[] iKeyWords)
        {
            bool lCheck = false;

            for (int i = 0; i < iKeyWords.Length; i++) {
                if (iAnswer.Contains(iKeyWords[i]))
                    lCheck = true;
            }
            return lCheck;
        }

        public void NextIngredient()
        {
            if (GetComponent<RecipeBehaviour>().IngredientIndex >= GetComponent<RecipeBehaviour>().mRecipe.ingredient.Count) {
                GetGameObject(3).GetComponent<Animator>().SetTrigger("Close_WList");
                GetComponent<Animator>().SetTrigger("DisplayStep");
            } else
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
        }

        public void LastIngredient()
        {
            if (GetComponent<RecipeBehaviour>().IngredientIndex == 3) {
                GetGameObject(3).GetComponent<Animator>().SetTrigger("Close_WList");
                if (GetComponent<RecipeBehaviour>().mRecipeList != null)
                    GetComponent<Animator>().SetTrigger("BackToList");
                else {
                    GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
                    GetGameObject(1).SetActive(false);
                    GetGameObject(2).SetActive(true);
                    GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
                    GetComponent<Animator>().SetTrigger("BackToStart");
                }
            } else {
                if (GetComponent<RecipeBehaviour>().IngredientIndex >= 6)
                    GetComponent<RecipeBehaviour>().IngredientIndex -= 6;
                else
                    GetComponent<RecipeBehaviour>().IngredientIndex = 0;
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
            }
        }

        private void VocalError(STTError iError)
        {
            mVocalManager.StartInstantReco();
        }
    }
}