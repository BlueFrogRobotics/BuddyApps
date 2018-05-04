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
	public class AskInfo : AStateMachineBehaviour
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
			mState.text = "ask user info";

			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.ASK_USER_PROFILE;

			if (mCompanion.mCurrentUser == null)
				Trigger("ASKNAME");
			else {

				mNeedListen = true;


				Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
				Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);


				//TODO: ask a missing info on current user
				AskRandomMissingInfo(mCompanion.mCurrentUser);
			}


		}

		private void AskRandomMissingInfo(UserProfile mCurrentUser)
		{
			// Ask corresponding missing info
			if(string.IsNullOrEmpty(mCurrentUser.LastName) ) {
				Interaction.TextToSpeech.SayKey("asklastname");
			} else if(string.IsNullOrEmpty(mCurrentUser.BirthDate)) {
				Interaction.TextToSpeech.SayKey("askbirth");
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

			if (string.IsNullOrEmpty(mCompanion.mCurrentUser.LastName)) {
				mCompanion.mCurrentUser.LastName = iMsg;
			} else if (string.IsNullOrEmpty(mCompanion.mCurrentUser.BirthDate)) {
				// Collect missing info
				mCompanion.mCurrentUser.BirthDate = iMsg;
			}

				CompanionBehaviour.SaveProfiles();

				Trigger("VOCALCOMMAND");

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