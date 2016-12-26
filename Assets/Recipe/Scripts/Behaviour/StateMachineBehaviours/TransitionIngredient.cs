using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class TransitionIngredient : AStateMachineBehaviour
    {
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(8).GetComponent<Button>().onClick.AddListener(LastIngredient);
            GetGameObject(9).GetComponent<Button>().onClick.AddListener(NextIngredient);
            mVocalActivation.VocalProcessing = VocalProcessing;
            mVocalActivation.VocalError = VocalError;
            mVocalActivation.StartInstantReco();
        }

        private void VocalProcessing(string answer)
        {

            if (answer.Contains("suivant") || answer.Contains("suivante"))
                NextIngredient();
            else if (answer.Contains("précédent") || answer.Contains("précédente") || answer.Contains("avant"))
                LastIngredient();
            else if (answer.Contains("répète") || answer.Contains("répéter"))
            {
                GetComponent<RecipeBehaviour>().IngredientIndex -= 3;
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
            }
        }

        public void NextIngredient()
        {
            if (GetComponent<RecipeBehaviour>().IngredientIndex >= GetComponent<RecipeBehaviour>().mRecipe.ingredient.Count)
            {
                GetGameObject(3).GetComponent<Animator>().SetTrigger("Close_WList");
                GetComponent<Animator>().SetTrigger("DisplayStep");
            }
            else
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
        }

        public void LastIngredient()
        {
            if (GetComponent<RecipeBehaviour>().IngredientIndex >= 6)
                GetComponent<RecipeBehaviour>().IngredientIndex -= 6;
            else
                GetComponent<RecipeBehaviour>().IngredientIndex = 0;
            GetComponent<Animator>().SetTrigger("DisplayIngredient");
        }

        private void VocalError(STTError error)
        {
            mVocalActivation.StartInstantReco();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTTS.Silence(1);
            GetGameObject(8).GetComponent<Button>().onClick.RemoveAllListeners();
            GetGameObject(9).GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}