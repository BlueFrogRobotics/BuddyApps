using Buddy;
using Buddy.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class ProposeGame : AStateMachineBehaviour
	{
		private bool mNoGame;

		private float mTime;

		private List<string> mKeyOptions;


		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;
			mState = GetComponentInGameObject<Text>(0);
			mKeyOptions = new List<string>();
			mKeyOptions.Add("memory");
			mKeyOptions.Add("calcul");
			mKeyOptions.Add("quizz");
			mKeyOptions.Add("freezedance");
			mKeyOptions.Add("rlgl");
			mKeyOptions.Add("hideseek");
			mKeyOptions.Add("nogame");
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Propose Game";
			Debug.Log("state: Propose Game");
			mTime = 0F;
			mNoGame = false;
            Interaction.Mood.Set(MoodType.HAPPY);
            Interaction.DialogManager.Ask(OnAnswer, "askgame", 0, mKeyOptions);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;

			if (mTime > 60F || mNoGame) {
				iAnimator.SetTrigger("ASKNEWRQ");
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		void OnAnswer(string iAnswer)
		{
			switch (iAnswer) {
				case "calcul":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("CalculGameApp").Execute();
					break;

				case "freezedance":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("FreezeDanceApp").Execute();
					break;

				case "rlgl":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("RLGLApp").Execute();
					break;

				case "memory":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("MemoryGameApp").Execute();
					break;

				case "hideseek":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("HideAndSeek").Execute();
					break;

				case "quizz":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("QuizzGameApp").Execute();
					break;

				case "nogame":
					CompanionData.Instance.InteractDesire += 10;
					mNoGame = true;
					break;
			}
		}
	}
}