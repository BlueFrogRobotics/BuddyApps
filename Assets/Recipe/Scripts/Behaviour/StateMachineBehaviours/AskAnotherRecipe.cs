using UnityEngine;
using UnityEngine.UI;
using Buddy;
using Buddy.UI;

namespace BuddyApp.Recipe
{
    public class AskAnotherRecipe : AStateMachineBehaviour
    {
        private bool mCheck;

        public override void Start()
        {
			
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER ASK ANOTHER RECIPE");
            mCheck = false;
            BYOS.Instance.Toaster.Display<BinaryQuestionToast>().With(Dictionary.GetString("another"), AnswerYes, AnswerNo);
			Interaction.TextToSpeech.Say(Dictionary.GetString("another"));
        }

        private void AnswerYes()
		{
			BYOS.Instance.Toaster.Hide();
			GetComponent<Animator>().SetTrigger("BackToStart");
        }

		private void AnswerNo()
		{
			BYOS.Instance.Toaster.Hide();
			GetComponent<RecipeBehaviour>().Exit();
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && Interaction.TextToSpeech.HasFinishedTalking && !Interaction.VocalManager.RecognitionTriggered) {
                mCheck = true;
				Interaction.VocalManager.OnEndReco = VocalProcessing;
				Interaction.VocalManager.OnError = VocalError;
				Interaction.VocalManager.StartInstantReco();
            }
        }

        private void VocalProcessing(string iAnswer)
        {
            iAnswer = iAnswer.ToLower();
			if (iAnswer.Contains(Dictionary.GetString("yes")))
				AnswerYes();
			else if (iAnswer.Contains(Dictionary.GetString("no")))
				AnswerNo();
        }

        private void VocalError(STTError error)
        {
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
			Interaction.VocalManager.OnEndReco = null;
			Interaction.VocalManager.OnError = null;
			Interaction.VocalManager.StopListenBehaviour();
			Interaction.TextToSpeech.Silence(0);
            Debug.Log("EXIT ASK ANOTHER RECIPE");
        }
    }
}