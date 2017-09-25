using UnityEngine.UI;
using UnityEngine;
using Buddy;
using Buddy.UI;
using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
	public class StartTimerState : AStateMachineBehaviour
	{
		int mTimer;

		private bool mStartTimer;

		public override void Start()
		{
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			BYOS.Instance.Header.DisplayParameters = true;

			if (GuardianData.Instance.FirstRun) {
				Interaction.TextToSpeech.SayKey("firststartdetectiontimer");
				GuardianData.Instance.FirstRun = false;
				mTimer = 10;
            } else {
				Interaction.TextToSpeech.SayKey("startdetectiontimer");
				mTimer = 5;
			}

			mStartTimer = false;
			//Detection.SoundDetector.StartMic();
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (!Interaction.TextToSpeech.HasFinishedTalking || mStartTimer)
				return;

			BYOS.Instance.Toaster.Display<CountdownToast>().With(
				BYOS.Instance.Dictionary.GetString("startdetectiontimer"), mTimer,
				() => { Trigger("InitDetection"); },
				() => { Trigger("InitDetection"); },
				() => { Trigger("Cancel"); });

			mStartTimer = true;
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mStartTimer = false;
            BYOS.Instance.WebService.EMailSender.enabled = true;
			BYOS.Instance.Header.DisplayParameters = false;
		}
	}
}