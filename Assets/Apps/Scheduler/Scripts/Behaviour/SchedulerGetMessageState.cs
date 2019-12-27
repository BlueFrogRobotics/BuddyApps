﻿using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Scheduler
{
	/*
     *  REMAINING BUG:
     *  Impossible to launch vocon inside OnEndListenning callback ? not sure
     */
	public sealed class SchedulerGetMessageState : AStateMachineBehaviour
	{
		private enum MessageStatus
		{
			E_FIRST_LISTENING,
			E_SECOND_LISTENING,
			E_UI_DISPLAY
		}

		private MessageStatus mMsgStatus = MessageStatus.E_FIRST_LISTENING;

		private const int TRY_NUMBER = 2;
		private const float QUIT_TIMEOUT = 20;
		private const float FREESPEECH_TIMER = 15F;
		private const int RECOGNITION_SENSIBILITY = 5000;

		private const string CREDENTIAL_DEFAULT_URL = "http://bfr-dev.azurewebsites.net/dev/BuddyDev-cmfc3b05c071.txt";

		private float mTimer;
		private string mFreeSpeechCredentials;

		private bool mBIgnoreOnEndListening;
		private bool mBTouched;
		TTextBox mRecordedMsg;

		// TMP - Debug
		public void DebugColor(string msg, string color)
		{
			if (string.IsNullOrEmpty(color))
				Debug.Log(msg);
			else
				Debug.Log("<color=" + color + ">----" + msg + "----</color>");
		}

		/*
         *   This function wait for iFreeSpeechTimer seconds
         *   and then stop vocal listenning
         */
		public IEnumerator FreeSpeechLifeTime(float iFreeSpeechTimer)
		{
			yield return new WaitUntil(() => !Buddy.Vocal.IsListening);
			yield return new WaitForSeconds(iFreeSpeechTimer);
			Buddy.Vocal.StopListening();
		}

		public IEnumerator QuittingTimeout()
		{
			mTimer = 0;
			while (mTimer < QUIT_TIMEOUT)
			{
				yield return null;
				mTimer += Time.deltaTime;
			}
			QuitScheduler();
		}

		private IEnumerator GetCredentialsAndRunFreeSpeech()
		{
			WWW lWWW = new WWW(CREDENTIAL_DEFAULT_URL);
			yield return lWWW;

			mFreeSpeechCredentials = lWWW.text;

			// Setting for freespeech
			Buddy.Vocal.DefaultInputParameters.Credentials = mFreeSpeechCredentials;
			Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_THEN_FREESPEECH;
			Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "companion_commands" };

			Buddy.Vocal.OnEndListening.Clear();
			Buddy.Vocal.OnEndListening.Add(HybridResult);
			Buddy.Vocal.SayKeyAndListen("record");

			StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			SchedulerDateManager.GetInstance().AppState = SchedulerDateManager.E_SCHEDULER_STATE.E_MESSAGE_CHOICE;

			mTimer = -1;
			mBIgnoreOnEndListening = false;
			mRecordedMsg = null;

			// Setting of Vocon param
			Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "reminder", "common" };
			Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
			Buddy.Vocal.OnEndListening.Clear();

			// When touching the screen
			Buddy.GUI.Screen.OnTouch.Clear();
			Buddy.GUI.Screen.OnTouch.Add((iInput) => { mBTouched = true; Buddy.Vocal.StopListening(); });

			// Setting of Header
			Buddy.GUI.Header.DisplayParametersButton(false);
			Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
			lHeaderFont.material.color = new Color(0, 0, 0);
			Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);

			if (MessageStatus.E_UI_DISPLAY == mMsgStatus) // If was already displayed
			{
				SayMessageAndListen();

				DisplayMessageEntry();
			}
			else // First call freespeech
			{
				StartCoroutine(GetCredentialsAndRunFreeSpeech());
			}
		}

		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			// Reset the timeout timer, on touch
			if (Input.touchCount > 0)
			{
				mTimer = -1;
				if (Buddy.GUI.Toaster.IsBusy)
					mTimer = 0;
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			DebugColor("STATE EXIT", "red");
			Buddy.GUI.Header.HideTitle();
			Buddy.GUI.Toaster.Hide();
			Buddy.GUI.Footer.Hide();
			Buddy.Vocal.OnEndListening.Clear();
			Buddy.Vocal.StopAndClear();
			StopAllCoroutines();

			mRecordedMsg = null;
			mMsgStatus = MessageStatus.E_UI_DISPLAY;
		}

		private void HybridResult(SpeechInput iSpeechInput)
		{
			// Stop Coroutine if the vocal has stop because of end of users's speech
			StopAllCoroutines();

			string lRule = "";
			lRule = Utils.GetRule(iSpeechInput);
			DebugColor("Hybrid Rule SPEECH.ToString: " + lRule, "blue");
			DebugColor("Hybrid Msg SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");

			if (mBIgnoreOnEndListening)
			{
				Buddy.Vocal.OnEndListening.Clear();
				Buddy.Vocal.StopAndClear();
				return;
			}

			if (mBTouched)
			{
				mBTouched = false;
				Buddy.Vocal.OnEndListening.Clear();
				Buddy.Vocal.OnEndListening.Add(VoconResult);
				DisplayMessageEntry();
				return;
			}


			if (!string.IsNullOrEmpty(iSpeechInput.Utterance) && !string.IsNullOrEmpty(lRule))
			{
				SchedulerDateManager.GetInstance().SchedulerMsg = iSpeechInput.Utterance;
				SchedulerDateManager.GetInstance().SchedulerRule = lRule;

				SayMessageAndListen();

				DisplayMessageEntry();
				return;
			}

			if (MessageStatus.E_FIRST_LISTENING == mMsgStatus)
			{
				mMsgStatus = MessageStatus.E_SECOND_LISTENING;

				// Call freespeech
				Buddy.Vocal.OnEndListening.Clear();
				Buddy.Vocal.OnEndListening.Add(HybridResult);
				Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_THEN_FREESPEECH;
				Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "companion_commands" };
				Buddy.Vocal.SayKeyAndListen(SchedulerDateManager.STR_SORRY);
				StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
			}
			else if (MessageStatus.E_SECOND_LISTENING == mMsgStatus)
			{
				mMsgStatus = MessageStatus.E_UI_DISPLAY;

				SayMessageAndListen();

				DisplayMessageEntry();
			}
		}

		private void VoconResult(SpeechInput iSpeechInput)
		{
			if (iSpeechInput.IsInterrupted)// || -1 == iSpeechInput.Confidence)
			{
				Debug.LogError("Listening was interrupted!!");
				return;
			}

			DebugColor("Vocon Validate/Modif SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
			DebugColor("Vocon Validate/Modif SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
			DebugColor("Vocon Validate/Modif SPEECH RULE: " + iSpeechInput.Rule + " --- " + Utils.GetRealStartRule(iSpeechInput.Rule), "blue");

			if (Utils.GetRealStartRule(iSpeechInput.Rule) == SchedulerDateManager.STR_QUIT)
				QuitScheduler();

			// Reset timer if the user say something
			if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
			{
				mTimer = 0;
			}

			if (Utils.GetRealStartRule(iSpeechInput.Rule) == "yes")
			{
				// If the message is empty, warn the user
				if (string.IsNullOrEmpty(SchedulerDateManager.GetInstance().SchedulerMsg))
				{
					Buddy.Vocal.OnEndListening.Clear();
					Buddy.Vocal.OnEndListening.Add(VoconResult);
					Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "reminder", "common" };
					Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
					Buddy.Vocal.SayKeyAndListen("sryemptymsg");
				}
				else
				{
					ValidateMessage();
				}
			}
			else if (Utils.GetRealStartRule(iSpeechInput.Rule) == "modify")
			{
				ModifyMessage();
			}
			else if (mTimer < QUIT_TIMEOUT)
			{
				Buddy.Vocal.OnEndListening.Clear();
				Buddy.Vocal.OnEndListening.Add(VoconResult);
				Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "reminder", "common" };
				Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
				Buddy.Vocal.SayKeyAndListen(SchedulerDateManager.STR_SORRY);
			}
		}

		private void QuitScheduler()
		{
			mBIgnoreOnEndListening = true;
			Trigger("Exit");
		}

		private void SayMessageAndListen()
		{
			Buddy.Vocal.OnEndListening.Clear();
			Buddy.Vocal.StopAndClear();
			// Launch Vocon - Validation/or/Modify
			Buddy.Vocal.OnEndListening.Add(VoconResult);
			Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "reminder", "common" };
			Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;

			string msg = Buddy.Resources.GetString("hereisthemsg");
			if (!string.IsNullOrEmpty(SchedulerDateManager.GetInstance().SchedulerMsg))
				msg += "[50]" + SchedulerDateManager.GetInstance().SchedulerMsg;
			Buddy.Vocal.SayAndListen(msg + "[200]" + Buddy.Resources.GetString("validateormodify"));
		}

		private void DisplayMessageEntry()
		{
			mMsgStatus = MessageStatus.E_UI_DISPLAY;

			DebugColor("DISPLAY  MESSAGE", "blue");

			if (null == mRecordedMsg)
			{
				//  Display of the title
				Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("message"));

				// Create the top left button
				FButton lViewModeButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
				lViewModeButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
				lViewModeButton.OnClick.Add(() =>
				{
					mBIgnoreOnEndListening = true;
					// Replace by 'RecurrenceChoiceState' in future
					Trigger("HourChoiceState");
				});

				// Create Step view to the button
				FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
				lSteps.Dots = Enum.GetValues(typeof(SchedulerDateManager.E_SCHEDULER_STATE)).Length;
				lSteps.Select((int)SchedulerDateManager.GetInstance().AppState);
				lSteps.SetLabel(Buddy.Resources.GetString("steps"));

				// TMP - waiting for caroussel
				Buddy.GUI.Toaster.Display<ParameterToast>().With(
					(iOnBuild) =>
						{
							mRecordedMsg = iOnBuild.CreateWidget<TTextBox>();
							if (string.IsNullOrEmpty(SchedulerDateManager.GetInstance().SchedulerMsg))
								mRecordedMsg.SetPlaceHolder(Buddy.Resources.GetString("enteryourmsg"));
							else
								mRecordedMsg.SetText(SchedulerDateManager.GetInstance().SchedulerMsg);
							mRecordedMsg.OnChangeValue.Add(
								(iText) => {
									SchedulerDateManager.GetInstance().SchedulerMsg = iText;
									SchedulerDateManager.GetInstance().SchedulerRule = Utils.GetRuleFromUtterance(iText);
									mTimer = 0;
									Buddy.Vocal.StopListening(); });
						},
					() => ModifyMessage(),
					Buddy.Resources.GetString("modify"),
					() =>
					{
						if (!string.IsNullOrEmpty(SchedulerDateManager.GetInstance().SchedulerMsg) && !string.IsNullOrEmpty(SchedulerDateManager.GetInstance().SchedulerRule))
						ValidateMessage();
					},
					Buddy.Resources.GetString("validate")
					);

				StartCoroutine(QuittingTimeout());
			}
			else
			{
				if (string.IsNullOrEmpty(SchedulerDateManager.GetInstance().SchedulerMsg))
				{
					mRecordedMsg.SetPlaceHolder(Buddy.Resources.GetString("enteryourmsg"));
				}
				else
				{
					mRecordedMsg.SetText(SchedulerDateManager.GetInstance().SchedulerMsg);
				}
			}
		}

		private void ModifyMessage()
		{
			StopAllCoroutines();
			Buddy.Vocal.StopAndClear();

			SchedulerDateManager.GetInstance().SchedulerMsg = null;
			SchedulerDateManager.GetInstance().SchedulerRule = null;
			mTimer = -1;

			// Call freespeech
			Buddy.Vocal.OnEndListening.Clear();
			Buddy.Vocal.OnEndListening.Add(HybridResult);
			Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY;
			Buddy.Vocal.SayKeyAndListen("record");
			StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
		}

		private void ValidateMessage()
		{
			StopAllCoroutines();
			Buddy.Vocal.StopAndClear();

			DebugColor("mScheduledTask SAVED:" + SchedulerDateManager.GetInstance().SchedulerMsg, "green");
			DebugColor("mScheduledTask SAVED:" + SchedulerDateManager.GetInstance().SchedulerDate.ToShortDateString() + " at " + SchedulerDateManager.GetInstance().SchedulerDate.ToLongTimeString(), "green");
			DebugColor("mScheduledTask SAVED:" + SchedulerDateManager.GetInstance().RepetitionTime, "green");

			// Save the task
			ScheduledTask mScheduledTask = new ScheduledTask(SchedulerDateManager.GetInstance().SchedulerMsg, SchedulerDateManager.GetInstance().SchedulerRule, SchedulerDateManager.GetInstance().SchedulerDate);
			Buddy.Platform.Calendar.Add(mScheduledTask);

			Buddy.GUI.Header.HideTitle();
			Buddy.GUI.Toaster.Hide();
			Buddy.GUI.Footer.Hide();
			Buddy.Vocal.SayKey(SchedulerDateManager.STR_TASK_OK, (iOutput) => { QuitApp(); });
		}
	}
}