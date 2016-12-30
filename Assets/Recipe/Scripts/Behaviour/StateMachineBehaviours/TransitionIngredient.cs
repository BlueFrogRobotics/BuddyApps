using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class TransitionIngredient : AStateMachineBehaviour
    {
        private bool check;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER TRANSITION INGREDIENT");
            check = false;
            GetGameObject(8).GetComponent<Button>().onClick.AddListener(LastIngredient);
            GetGameObject(9).GetComponent<Button>().onClick.AddListener(NextIngredient);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!check && mTTS.HasFinishedTalking && !mVocalActivation.RecognitionTriggered) {
                check = true;
                mVocalActivation.VocalProcessing = VocalProcessing;
                mVocalActivation.VocalError = VocalError;
                mVocalActivation.StartInstantReco();
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocalActivation.VocalProcessing = null;
            mVocalActivation.VocalError = null;
            mVocalActivation.StopListenBehaviour();
            mTTS.Silence(0);
            GetGameObject(8).GetComponent<Button>().onClick.RemoveAllListeners();
            GetGameObject(9).GetComponent<Button>().onClick.RemoveAllListeners();
            Debug.Log("EXIT TRANSITION INGREDIENT");
        }

        private void VocalProcessing(string answer)
        {
            answer = answer.ToLower();
            if (ContainKeyWord(answer, mDictionary.GetString("next").Split(' ')))
                NextIngredient();
            else if (ContainKeyWord(answer, mDictionary.GetString("last").Split(' ')))
                LastIngredient();
            else if (ContainKeyWord(answer, mDictionary.GetString("repeat").Split(' ')))
            {
                GetComponent<RecipeBehaviour>().IngredientIndex -= 3;
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
            }
            else
                mVocalActivation.StartInstantReco();
        }

        private bool ContainKeyWord(string iAnswer, string[] iKeyWords)
        {
            bool lCheck = false;

            for (int i = 0; i < iKeyWords.Length; i++)
            {
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
            }
            else
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
        }

        public void LastIngredient()
        {
            if (GetComponent<RecipeBehaviour>().IngredientIndex == 3 && GetComponent<RecipeBehaviour>().mRecipeList != null)
            {
                GetGameObject(3).GetComponent<Animator>().SetTrigger("Close_WList");
                GetComponent<Animator>().SetTrigger("BackToList");
            }
            else
            {
                if (GetComponent<RecipeBehaviour>().IngredientIndex >= 6)
                    GetComponent<RecipeBehaviour>().IngredientIndex -= 6;
                else
                    GetComponent<RecipeBehaviour>().IngredientIndex = 0;
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
            }
        }

        private void VocalError(STTError error)
        {
            mVocalActivation.StartInstantReco();
        }
    }
}