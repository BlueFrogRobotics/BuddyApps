using UnityEngine;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.Recipe
{
    public class ListenRecipe : AStateMachineBehaviour
    {
        private Animator mAnimator;
        private int mErrorCount;
        private bool mNotListening;
        private float mTime;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER LISTEN RECIPE");
            mNotListening = false;
            mErrorCount = 0;
            mTime = 0.0F;
            mAnimator = iAnimator;
            mVocalManager.OnEndReco = GetAnswer;
            mVocalManager.OnError = NoAnswer;
            mVocalManager.StopListenBehaviour = null;
            if (GetComponent<RecipeBehaviour>().NoAnswerCount == 0 && GetComponent<RecipeBehaviour>().RecipeNotFoundCount < 3)
                mVocalManager.StartInstantReco();
            else
            {
                Debug.Log("Put Trigger On");
                mVocalManager.EnableTrigger = true;
                mNotListening = true;
                GetComponent<RecipeBehaviour>().NoAnswerCount = 0;
            }
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTime += Time.deltaTime;
            if (mNotListening && !mVocalManager.RecognitionTriggered && Input.GetTouch(0).tapCount > 1)
                iAnimator.SetTrigger("ChooseWithScreen");
            if (mNotListening && !mVocalManager.RecognitionTriggered && mTime > 10.0F)
            {
                mTime = 0.0F;
                mNotManager.Display<SimpleNot>().With(mDictionary.GetString("hint1"), mSpriteManager.GetSprite("LightOn"));
            }
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
            mVocalManager.StopListenBehaviour = Empty;
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
