using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class ListenRecipe : AStateMachineBehaviour
    {
        private int mErrorCount;
        private bool mNotListening;
        private float mTime;

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER LISTEN RECIPE");
            mNotListening = false;
            mErrorCount = 0;
            mTime = 0.0F;
            mAnimator = iAnimator;
			Interaction.VocalManager.OnEndReco = GetAnswer;
			Interaction.VocalManager.OnError = NoAnswer;
			Interaction.VocalManager.StopListenBehaviour = null;
			if (GetComponent<RecipeBehaviour>().NoAnswerCount == 0 && GetComponent<RecipeBehaviour>().RecipeNotFoundCount < 3) {
				//Interaction.VocalManager.StartInstantReco();
			} else {
				Debug.Log("Put Trigger On");
				Interaction.VocalManager.EnableTrigger = true;
				mNotListening = true;
				GetComponent<RecipeBehaviour>().NoAnswerCount = 0;
			}
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mTime += Time.deltaTime;
            //if (mNotListening && !Interaction.VocalManager.RecognitionTriggered && Input.GetTouch(0).tapCount > 1)
            //    iAnimator.SetTrigger("ChooseWithScreen");
            //if (mNotListening && !Interaction.VocalManager.RecognitionTriggered && mTime > 10.0F)
            //{
            //    mTime = 0.0F;
            //    Notifier.Display<Buddy.UI.SimpleNot>().With(Dictionary.GetString("hint1"), BYOS.Instance.Resources.GetSprite("LightOn"));
            //}
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
			//Interaction.VocalManager.OnEndReco = null;
			//Interaction.VocalManager.OnError = null;
			//Interaction.VocalManager.EnableTrigger = false;
            Debug.Log("EXIT LISTEN RECIPE");
        }

        private void GetAnswer(string iAnswer)
        {
            Debug.Log("GOT A ANSWER");
			//Interaction.VocalManager.StopListenBehaviour = Empty;
            GetComponent<RecipeBehaviour>().mAnswer = iAnswer.ToLower();
            mAnimator.SetTrigger("AnswerRecipe");
        }

        private void NoAnswer(STTError iError)
        {
            Debug.Log("GOT NO ANSWER");
            if (++mErrorCount == 3)
            {
				//Interaction.VocalManager.StopListenBehaviour = Empty;
                mAnimator.SetTrigger("NoAnswerRecipe");
            }
        }

        private void Empty()
        {
        }
    }
}
