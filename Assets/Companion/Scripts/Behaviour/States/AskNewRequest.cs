using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class AskNewRequest : AStateMachineBehaviour
	{
		private bool mNoRq;
		private bool mListenRq;

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
			mState.text = "Ask New Request";
			Debug.Log("state: Ask New Request");
			mTime = 0F;
			mNoRq = false;
			mListenRq = false;
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Interaction.TextToSpeech.Silence(500, true);
			BYOS.Instance.Interaction.DialogManager.Ask(OnAnswer, "newrq", 1, mKeyOptions, true);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;



			if (mNoRq) {
                //TODO propose a game or something if buddy wants to interact
                Interaction.TextToSpeech.Say("Ok... [500].", true);
				iAnimator.SetTrigger("DISENGAGE");
			} else if (mListenRq) {
                Interaction.TextToSpeech.Say("Que puis-je faire pour toi?", true);
				iAnimator.SetTrigger("VOCALTRIGGERED");
			} else if (mTime > 60F) {
				iAnimator.SetTrigger("DISENGAGE");
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		void OnAnswer(string iAnswer)
		{
			if (iAnswer.ToLower() == "yes") {
				//Debug.Log("Ask new rq yes " + iAnswer);
				mListenRq = true;
			} else {
				Debug.Log("Ask new rq no " + iAnswer + " dico: " + Dictionary.GetString("yes"));
				mNoRq = true;
			}
		}
	}
}