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
            if (!check && mTTS.HasFinishedTalking && mSTT.HasFinished) {
                check = true;
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
            GetGameObject(10).GetComponent<Button>().onClick.RemoveAllListeners();
            GetGameObject(11).GetComponent<Button>().onClick.RemoveAllListeners();
            Debug.Log("EXIT TRANSITION STEP");
        }

        private void VocalProcessing(string answer)
        {
            answer = answer.ToLower();
            if (ContainKeyWord(answer, mDictionary.GetString("next").Split(' ')))
                NextStep();
            else if (ContainKeyWord(answer, mDictionary.GetString("last").Split(' ')))
                LastStep();
            else if (ContainKeyWord(answer, mDictionary.GetString("repeat").Split(' '))) {
                GetComponent<RecipeBehaviour>().StepIndex--;
                GetComponent<Animator>().SetTrigger("DisplayStep");
            } else
                mVocalManager.StartInstantReco();
        }

        private bool ContainKeyWord(string iAnswer, string[] iKeyWords)
        {
            bool oCheck = false;

            for (int i = 0; i < iKeyWords.Length; i++) {
                if (iAnswer.Contains(iKeyWords[i]))
                    oCheck = true;
            }
            return oCheck;
        }

        public void NextStep()
        {
            if (GetComponent<RecipeBehaviour>().StepIndex == GetComponent<RecipeBehaviour>().mRecipe.step.Count) {
                GetGameObject(4).GetComponent<Animator>().SetTrigger("Close_WFullImage");
                GetComponent<Animator>().SetTrigger("FinishStep");
            } else
                GetComponent<Animator>().SetTrigger("DisplayStep");
        }

        public void LastStep()
        {
            if (GetComponent<RecipeBehaviour>().StepIndex == 1) {
                GetComponent<RecipeBehaviour>().StepIndex = 0;
                GetComponent<RecipeBehaviour>().IngredientIndex -= 3;
                GetGameObject(4).GetComponent<Animator>().SetTrigger("Close_WFullImage");
                GetComponent<Animator>().SetTrigger("BackToIngredient");
            } else {
                if (GetComponent<RecipeBehaviour>().StepIndex > 1)
                    GetComponent<RecipeBehaviour>().StepIndex -= 2;
                else
                    GetComponent<RecipeBehaviour>().StepIndex = 0;
                GetComponent<Animator>().SetTrigger("DisplayStep");
            }
        }

        private void VocalError(STTError error)
        {
            mVocalManager.StartInstantReco();
        }
    }
}