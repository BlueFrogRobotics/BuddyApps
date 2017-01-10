using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class ListenRecipe : AStateMachineBehaviour
    {
        private Animator mAnimator;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER LISTEN RECIPE");
            mAnimator = iAnimator;
            mVocalManager.OnEndReco = GetAnswer;
            mVocalManager.OnError = NoAnswer;
            mVocalManager.StartInstantReco();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocalManager.OnEndReco = null;
            mVocalManager.OnError = null;
            mVocalManager.StopListenBehaviour();
            Debug.Log("EXIT LISTEN RECIPE");
        }

        private void GetAnswer(string iAnswer)
        {
            GetComponent<RecipeBehaviour>().mAnswer = iAnswer.ToLower();
            mAnimator.SetTrigger("AnswerRecipe");
        }

        private void NoAnswer(STTError iError)
        {
            mAnimator.SetTrigger("NoAnswerRecipe");
        }
    }
}