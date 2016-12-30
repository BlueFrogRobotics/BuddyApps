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
            Debug.Log("Enter ListenRecipe");
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
            Debug.Log("ExitListenRecipe");
        }

        private void GetAnswer(string iAnswer)
        {
            Debug.Log("CallbackListenRecipe");
            GetComponent<RecipeBehaviour>().mAnswer = iAnswer.ToLower();
            mAnimator.SetTrigger("AnswerRecipe");
        }

        private void NoAnswer(STTError error)
        {
            Debug.Log("CallbackErrorListenRecipe");
            Debug.Log(error);
            mAnimator.SetTrigger("NoAnswerRecipe");
        }
    }
}