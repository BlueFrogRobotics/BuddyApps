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
            if (GetComponent<RecipeBehaviour>().NoAnswerCount <= 1)
                mVocalManager.StartInstantReco();
            else
            {
                mVocalManager.EnableTrigger = true;
                GetComponent<RecipeBehaviour>().NoAnswerCount = 0;
            }
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
                mAnimator.SetTrigger("ChooseWithScreen");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocalManager.OnEndReco = null;
            mVocalManager.OnError = null;
            mVocalManager.StopListenBehaviour();
            mVocalManager.EnableTrigger = false;
            Debug.Log("EXIT LISTEN RECIPE");
        }

        private void GetAnswer(string iAnswer)
        {
            Debug.Log("GOT A ANSWER");
            GetComponent<RecipeBehaviour>().mAnswer = iAnswer.ToLower();
            mAnimator.SetTrigger("AnswerRecipe");
        }

        private void NoAnswer(STTError iError)
        {
            Debug.Log("GOT NO ANSWER");
            mAnimator.SetTrigger("NoAnswerRecipe");
        }
    }
}