﻿using Buddy;
using Buddy.UI;
using Buddy.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class AskName : AStateMachineBehaviour
	{
		private bool mNeedListen;
		private float mTime;
		private List<UserProfile> mProfiles;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
			mCompanion = GetComponent<CompanionBehaviour>();
			mProfiles = mCompanion.Profiles;

		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime = 0F;
			mState.text = "ask user profile";

			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.ASK_USER_PROFILE;

			if (mCompanion.mCurrentUser != null)
				Trigger("ASKINFO");
			else {

				mNeedListen = true;


				Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
				Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);


				//TODO: ask a missing info on current user
				Interaction.TextToSpeech.SayKey("askname");
			}


		}



		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;
			if (mTime > 20F) {
				iAnimator.SetTrigger("INTERACT");
				CompanionData.Instance.mLearnDesire -= 30;
			}

			if (Interaction.TextToSpeech.HasFinishedTalking && mNeedListen) {
				Interaction.VocalManager.StartInstantReco();
				mNeedListen = false;
			}
		}


		private void OnSpeechRecognition(string iMsg)
		{


			for (int i = 0; i < mProfiles.Count; ++i) {
				if (iMsg == mProfiles[i].FirstName ||
					iMsg == mProfiles[i].LastName ||
					iMsg == mProfiles[i].FirstName + " " + mProfiles[i].LastName ||
					iMsg == mProfiles[i].LastName + " " + mProfiles[i].FirstName) {
					mCompanion.mCurrentUser = mProfiles[i];
					break;
				}
			}

			if (mCompanion.mCurrentUser != null) {
				string lSentenceToSay = "Hello " + mCompanion.mCurrentUser.FirstName + " " + mCompanion.mCurrentUser.LastName;
				Interaction.TextToSpeech.Say(lSentenceToSay);
			} else {
				Interaction.TextToSpeech.Say("Nice to meet you " + iMsg);
				mCompanion.mCurrentUser = mCompanion.AddProfile(iMsg);
			}

			Trigger("ASKINFO");

		}

		private void ErrorSTT(STTError iError)
		{
			mNeedListen = true;
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "";
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.CHAT;

			Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Remove(ErrorSTT);
		}


	}
}