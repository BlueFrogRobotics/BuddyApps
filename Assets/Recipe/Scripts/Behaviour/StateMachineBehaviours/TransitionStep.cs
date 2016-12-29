using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class TransitionStep : AStateMachineBehaviour
    {
        private bool check;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER TRANSITION STEP");
            check = false;
            GetGameObject(10).GetComponent<Button>().onClick.AddListener(LastStep);
            GetGameObject(11).GetComponent<Button>().onClick.AddListener(NextStep);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!check && mTTS.HasFinishedTalking)
            {
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
            GetGameObject(10).GetComponent<Button>().onClick.RemoveAllListeners();
            GetGameObject(11).GetComponent<Button>().onClick.RemoveAllListeners();
            Debug.Log("EXIT TRANSITION STEP");
        }

        private void VocalProcessing(string answer)
        {
            if (answer.Contains("suivant") || answer.Contains("suivante"))
                NextStep();
            else if (answer.Contains("précédent") || answer.Contains("précédente") || answer.Contains("avant"))
                LastStep();
            else if (answer.Contains("répète") || answer.Contains("répéter"))
            {
                GetComponent<RecipeBehaviour>().StepIndex--;
                GetComponent<Animator>().SetTrigger("DisplayStep");
            }
            else
                mVocalActivation.StartInstantReco();
        }

        public void NextStep()
        {
            if (GetComponent<RecipeBehaviour>().StepIndex == GetComponent<RecipeBehaviour>().mRecipe.step.Count)
                GetComponent<RecipeBehaviour>().Exit();
            else
                GetComponent<Animator>().SetTrigger("DisplayStep");
        }

        public void LastStep()
        {
            if (GetComponent<RecipeBehaviour>().StepIndex == 1) {
                GetComponent<RecipeBehaviour>().StepIndex = 0;
                GetComponent<RecipeBehaviour>().IngredientIndex -= 3;
                GetGameObject(4).GetComponent<Animator>().SetTrigger("Close_WFullImage");
                GetComponent<Animator>().SetTrigger("BackToIngredient");
            }
            else {
                if (GetComponent<RecipeBehaviour>().StepIndex > 1)
                    GetComponent<RecipeBehaviour>().StepIndex -= 2;
                else
                    GetComponent<RecipeBehaviour>().StepIndex = 0;
                GetComponent<Animator>().SetTrigger("DisplayStep");
            }
        }

        private void VocalError(STTError error)
        {
            mVocalActivation.StartInstantReco();
        }

        
    }
}