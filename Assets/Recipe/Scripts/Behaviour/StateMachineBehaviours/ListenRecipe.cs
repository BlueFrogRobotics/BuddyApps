using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class ListenRecipe : AStateMachineBehaviour
    {
        private Animator mAnimator;
        private int mErrorCount;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER LISTEN RECIPE");
            mErrorCount = 0;
            mAnimator = iAnimator;
            mVocalManager.OnEndReco = GetAnswer;
            mVocalManager.OnError = NoAnswer;
            mVocalManager.StopListenBehaviour = null;
            if (GetComponent<RecipeBehaviour>().NoAnswerCount == 0)
                mVocalManager.StartInstantReco();
            else
            {
                Debug.Log("Put Trigger On");
                mVocalManager.EnableTrigger = true;
                GetComponent<RecipeBehaviour>().NoAnswerCount = 0;
            }
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mVocalManager.RecognitionTriggered && Input.GetTouch(0).tapCount > 1)
                iAnimator.SetTrigger("ChooseWithScreen");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocalManager.OnEndReco = null;
            mVocalManager.OnError = null;
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
            if (++mErrorCount == 3)
            {
                mVocalManager.StopListenBehaviour = Empty;
                mAnimator.SetTrigger("NoAnswerRecipe");
            }
        }

        private void Empty()
        {
        }
    }
}
