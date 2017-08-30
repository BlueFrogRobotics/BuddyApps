using UnityEngine;
using Buddy;

namespace BuddyApp.Weather
{
	public class Test : AStateMachineBehaviour
	{

		public override void Start()
		{
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Debug.Log("ENTER LISTEN test");
			Interaction.VocalManager.OnEndReco = GetAnswer;
			Interaction.VocalManager.OnError = NoAnswer;
			Interaction.VocalManager.StopListenBehaviour = null;

		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Debug.Log("EXIT LISTEN");
		}

		private void GetAnswer(string iAnswer)
		{
			Utils.LogI(LogContext.APP, "GOT AN ANSWER: " + iAnswer);
			Interaction.VocalManager.StopListenBehaviour = Empty;
			WeatherData.Instance.VocalRequest = iAnswer.ToLower();
			Trigger("Test");
		}

		private void NoAnswer(STTError iError)
		{
			Utils.LogI(LogContext.APP, "On loading...");
			Debug.Log("GOT NO ANSWER");
			Interaction.VocalManager.StopListenBehaviour = Empty;
			Trigger("Test");
		}

		private void Empty()
		{
		}
	}
}
