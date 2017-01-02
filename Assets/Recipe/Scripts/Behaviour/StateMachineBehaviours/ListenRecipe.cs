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
            mAnimator = iAnimator;
            mVocalActivation.VocalProcessing = GetAnswer;
            mVocalActivation.VocalError = NoAnswer;
            mVocalActivation.StartInstantReco();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocalActivation.VocalProcessing = null;
            mVocalActivation.VocalError = null;
            mVocalActivation.StopListenBehaviour();
        }

        private void GetAnswer(string iAnswer)
        {
            GetComponent<RecipeBehaviour>().mAnswer = iAnswer.ToLower();
            mAnimator.SetTrigger("AnswerRecipe");
        }

        private void NoAnswer(STTError error)
        {
            mAnimator.SetTrigger("NoAnswerRecipe");
        }
    }
}