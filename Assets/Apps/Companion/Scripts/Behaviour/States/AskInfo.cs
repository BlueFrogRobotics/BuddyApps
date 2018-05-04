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

	public enum QuestionToAsk
	{
		FIRSTNAME,
		LASTNAME,
		BIRTH,
		FAVORITECOLOR,
		FAVORITEMUSICBAND,
		OCCUPATION
	}

	public class AskInfo : AStateMachineBehaviour
	{
		private bool mNeedListen;
		private float mTime;
		private List<UserProfile> mProfiles;
		private QuestionToAsk mAskedQuestion;

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


				//TODO: make it random somehow...
				if (!AskMissingInfo()) {
					AskRandomInfo();

				}
			}


		}

		private bool AskMissingInfo()
		{
			// Ask corresponding missing info

			if (string.IsNullOrEmpty(mCompanion.mCurrentUser.LastName)) {
				Interaction.TextToSpeech.SayKey("asklastname");
				mAskedQuestion = QuestionToAsk.LASTNAME;
			} else if (string.IsNullOrEmpty(mCompanion.mCurrentUser.BirthDate)) {
				Interaction.TextToSpeech.SayKey("askbirth");
				mAskedQuestion = QuestionToAsk.BIRTH;
			} else if (string.IsNullOrEmpty(mCompanion.mCurrentUser.Tastes.Color)) {
				Interaction.TextToSpeech.Say(Dictionary.GetRandomString("askfavorite").Replace("[subject]", Dictionary.GetRandomString("color")));
				mAskedQuestion = QuestionToAsk.FAVORITECOLOR;
			} else if (string.IsNullOrEmpty(mCompanion.mCurrentUser.Tastes.MusicBand)) {
				Interaction.TextToSpeech.Say(Dictionary.GetRandomString("askfavorite").Replace("[subject]", Dictionary.GetRandomString("musicband")));
				mAskedQuestion = QuestionToAsk.FAVORITEMUSICBAND;
			} else if (string.IsNullOrEmpty(mCompanion.mCurrentUser.Occupation)) {
				Interaction.TextToSpeech.SayKey("askoccupation");
				mAskedQuestion = QuestionToAsk.OCCUPATION;
			} else return false;

			return true;
		}

		private void AskRandomInfo()
		{
			// Ask random info

			int lRand = UnityEngine.Random.Range(0, 5);

			if (lRand == 0) {
				Interaction.TextToSpeech.SayKey("asklastname");
				mAskedQuestion = QuestionToAsk.LASTNAME;
			} else if (lRand == 1) {
				Interaction.TextToSpeech.SayKey("askbirth");
				mAskedQuestion = QuestionToAsk.BIRTH;
			} else if (lRand == 2) {
				Interaction.TextToSpeech.Say(Dictionary.GetRandomString("askfavorite").Replace("[subject]", Dictionary.GetRandomString("color")));
				mAskedQuestion = QuestionToAsk.FAVORITECOLOR;
			} else if (lRand == 3) {
				Interaction.TextToSpeech.Say(Dictionary.GetRandomString("askfavorite").Replace("[subject]", Dictionary.GetRandomString("musicband")));
				mAskedQuestion = QuestionToAsk.FAVORITEMUSICBAND;
			} else if (lRand == 4) {
				Interaction.TextToSpeech.SayKey("askoccupation");
				mAskedQuestion = QuestionToAsk.OCCUPATION;
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

		//TODO refactor
		private void OnSpeechRecognition(string iMsg)
		{
			if (mAskedQuestion == QuestionToAsk.FIRSTNAME) {
				if (iMsg.ToLower() == mCompanion.mCurrentUser.LastName.ToLower()) {

					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ithoughitwas") + " " + Dictionary.GetRandomString("yourlastname"), true);
					mCompanion.mCurrentUser.FirstName = iMsg;
					Interaction.TextToSpeech.SayKey("askfirstname", true);
					mAskedQuestion = QuestionToAsk.LASTNAME;
					mNeedListen = true;
				} else if (!string.IsNullOrEmpty(mCompanion.mCurrentUser.FirstName) && iMsg.ToLower() != mCompanion.mCurrentUser.FirstName.ToLower()) {
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString(("ithoughitwas") + " " + mCompanion.mCurrentUser.FirstName));
					mCompanion.mCurrentUser.FirstName = iMsg;

				} else {
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString(("ok") + " " + iMsg));
					mCompanion.mCurrentUser.FirstName = iMsg;
				}

			} else if (mAskedQuestion == QuestionToAsk.LASTNAME) {
				if (iMsg.ToLower() == mCompanion.mCurrentUser.FirstName.ToLower()) {

					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ithoughitwas") + " " + Dictionary.GetRandomString("yourfirstname"), true);
					mCompanion.mCurrentUser.LastName = iMsg;
					Interaction.TextToSpeech.SayKey("askfirstname", true);
					mAskedQuestion = QuestionToAsk.FIRSTNAME;
					mNeedListen = true;
				} else if (!string.IsNullOrEmpty(mCompanion.mCurrentUser.LastName) && iMsg.ToLower() != mCompanion.mCurrentUser.LastName.ToLower()) {
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString(("ithoughitwas") + " " + mCompanion.mCurrentUser.LastName));
					mCompanion.mCurrentUser.LastName = iMsg;
				} else {
					Interaction.TextToSpeech.Say("ok" + " " + mCompanion.mCurrentUser.LastName);
					mCompanion.mCurrentUser.LastName = iMsg;
				}

			} else if (mAskedQuestion == QuestionToAsk.BIRTH) {
				if (!string.IsNullOrEmpty(mCompanion.mCurrentUser.BirthDate) && iMsg.ToLower() != mCompanion.mCurrentUser.BirthDate.ToLower())
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ithoughitwas") + " " + mCompanion.mCurrentUser.BirthDate, true);
				else
					Interaction.TextToSpeech.Say("ok" + " " + iMsg);


				mCompanion.mCurrentUser.BirthDate = iMsg;

			} else if (mAskedQuestion == QuestionToAsk.FAVORITECOLOR) {
				if (!string.IsNullOrEmpty(mCompanion.mCurrentUser.Tastes.Color) && iMsg.ToLower() != mCompanion.mCurrentUser.Tastes.Color.ToLower())
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ithoughitwas") + " " + mCompanion.mCurrentUser.Tastes.Color, true);
				else
					Interaction.TextToSpeech.Say("ok" + " " + iMsg);

				mCompanion.mCurrentUser.Tastes.Color = iMsg;

			} else if (mAskedQuestion == QuestionToAsk.FAVORITEMUSICBAND) {
				if (!string.IsNullOrEmpty(mCompanion.mCurrentUser.Tastes.MusicBand) && iMsg.ToLower() != mCompanion.mCurrentUser.Tastes.MusicBand.ToLower())
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ithoughitwas") + " " + mCompanion.mCurrentUser.Tastes.MusicBand, true);
				else
					Interaction.TextToSpeech.Say("ok" + " " + iMsg);

				mCompanion.mCurrentUser.Tastes.MusicBand = iMsg;


			} else if (mAskedQuestion == QuestionToAsk.OCCUPATION) {
				if (!string.IsNullOrEmpty(mCompanion.mCurrentUser.Occupation) && iMsg.ToLower() != mCompanion.mCurrentUser.Occupation.ToLower())
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ithoughitwas") + " " + mCompanion.mCurrentUser.Occupation, true);
				else
					Interaction.TextToSpeech.Say("ok" + " " + iMsg);


				mCompanion.mCurrentUser.Occupation = iMsg;
			}

			CompanionBehaviour.SaveProfiles();

			if (!mNeedListen)
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