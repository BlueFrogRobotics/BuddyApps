using Buddy;
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
			mState.text = "ask user name";
			Debug.Log("ask user name state");

			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.ASK_USER_PROFILE;

			if (mCompanion.mCurrentUser != null && !string.IsNullOrEmpty(mCompanion.mCurrentUser.FirstName) ) {

				Debug.Log("We know the current user, ask info");
				Trigger("ASKINFO");
			}
			else {

				mNeedListen = true;


				//Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
				//Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);


				//TODO: ask a missing info on current user
				Debug.Log("asking name");
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
			if (!string.IsNullOrEmpty(iMsg)) {

				Debug.Log("answer name: " + iMsg);
				Debug.Log("answer Profiles: " + mProfiles.Count);
				for (int i = 0; i < mProfiles.Count; ++i) {
					if (iMsg == mProfiles[i].FirstName ||
						iMsg == mProfiles[i].LastName ||
						iMsg == mProfiles[i].FirstName + " " + mProfiles[i].LastName ||
						iMsg == mProfiles[i].LastName + " " + mProfiles[i].FirstName) {

						Debug.Log("found profile: " + mProfiles[i].FirstName);
						mCompanion.mCurrentUser = mProfiles[i];
						break;
					}
				}

				if (mCompanion.mCurrentUser != null) {

					Debug.Log("mCompanion.mCurrentUser " + mCompanion.mCurrentUser.FirstName);
					string lSentenceToSay = "Hello " + mCompanion.mCurrentUser.FirstName + " " + mCompanion.mCurrentUser.LastName;
					Interaction.TextToSpeech.Say(lSentenceToSay);
				} else {
					Debug.Log("mCompanion.mCurrentUser unknown ");
					Interaction.TextToSpeech.Say("Nice to meet you " + iMsg);
					mCompanion.mCurrentUser = mCompanion.AddProfile(iMsg);

					CompanionBehaviour.SaveProfiles();
				}

				Trigger("ASKINFO");
			} else
				mNeedListen = true;

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