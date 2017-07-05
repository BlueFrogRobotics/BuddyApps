using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class AskCharge : AStateMachineBehaviour
	{
		private bool mEngaged;
		private bool mGoToCharge;

		private float mTime;

		private List<string> mKeyOptions;


		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;
			mKeyOptions = new List<string>();
			mKeyOptions.Add("no");
			mKeyOptions.Add("yes");
			mState = GetComponentInGameObject<Text>(0);
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mState.text = "Ask Charge";
			Debug.Log("state: Ask Charge");
			mTime = 0F;
			mEngaged = false;
			mGoToCharge = false;
			Interaction.Mood.Set(MoodType.TIRED);
			BYOS.Instance.Interaction.DialogManager.Ask(OnAnswer, "askcharge", 1, mKeyOptions);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;



			if (mEngaged) {
                Interaction.TextToSpeech.Say("Ok... [500].", true);
                Interaction.Face.SetEvent(FaceEvent.YAWN);
                Interaction.TextToSpeech.Say("Que puis-je faire pour toi?", true);

				CompanionData.Instance.ChargeAsked = true;
				iAnimator.SetTrigger("VOCALTRIGGERED");
			} else if (mGoToCharge) {
                Interaction.TextToSpeech.Say("Ok, merci.", true);
                Interaction.Face.SetEvent(FaceEvent.YAWN);
				iAnimator.SetTrigger("CHARGE");
				CompanionData.Instance.ChargeAsked = true;
			} else if (mTime > 60F) {
				iAnimator.SetTrigger("CHARGE");
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		void OnAnswer(string iAnswer)
		{
			Debug.Log("callback");
			if (iAnswer.ToLower() == "yes") {
				Debug.Log("recharge answer yes");
				mGoToCharge = true;
			} else {
				Debug.Log("recharge answer " + iAnswer);
				mEngaged = true;
			}
		}
	}
}