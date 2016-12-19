using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;

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
        }

        private void GetAnswer(string iAnswer)
        {
            mNotManager.Display<SimpleNot>().With("J'ai entendu: " + iAnswer, mSpriteManager.GetSprite("Message"));
            GetComponent<RecipeBehaviour>().mAnswer = iAnswer;
            mAnimator.SetTrigger("AnswerRecipe");
        }

        private void NoAnswer(STTError error)
        {
            mAnimator.SetTrigger("NoAnswerRecipe");
            Debug.Log(error);
        }
    }
}