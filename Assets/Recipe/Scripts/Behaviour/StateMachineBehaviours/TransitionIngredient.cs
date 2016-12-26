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
            check = false;
            GetGameObject(8).GetComponent<Button>().onClick.AddListener(LastIngredient);
            GetGameObject(9).GetComponent<Button>().onClick.AddListener(NextIngredient);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!check && mTTS.HasFinishedTalking) {
                check = true;
                mVocalActivation.VocalProcessing = VocalProcessing;
                mVocalActivation.VocalError = VocalError;
                mVocalActivation.StartInstantReco();
            }
            //Sprite lol = mSpriteManager.GetSprite("Icon", "AtlasRecipe");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocalActivation.VocalProcessing = null;
            mVocalActivation.VocalError = null;
            mTTS.Silence(0);
            GetGameObject(8).GetComponent<Button>().onClick.RemoveAllListeners();
            GetGameObject(9).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        private void VocalProcessing(string answer)
        {
            if (answer.Contains("suivant") || answer.Contains("suivante"))
                NextIngredient();
            else if (answer.Contains("précédent") || answer.Contains("précédente") || answer.Contains("avant"))
                LastIngredient();
            else if (answer.Contains("répète") || answer.Contains("répéter")) {
                GetComponent<RecipeBehaviour>().IngredientIndex -= 3;
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
            }
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
    }
}