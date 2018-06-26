using Buddy;
using Buddy.UI;
using Buddy.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace BuddyApp.Companion
{

	public enum QuestionToAsk
	{
		FIRSTNAME,
		LASTNAME,
		BIRTH,
		FAVORITECOLOUR,
		FAVORITEMUSICBAND,
		OCCUPATION,
		FAVORITESPORT
	}

	public class AskInfo : AStateMachineBehaviour
	{
		private bool mNeedListen;
		private bool mAskName;
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
			mAskName = false;
			mTime = 0F;
			mState.text = "ask user info";
			Debug.Log("ask info state");


			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.ASK_USER_PROFILE;

			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);


			if (mCompanion.mCurrentUser == null) {
				Debug.Log("We didn't recognized the curent user, ask his name");
				Trigger("ASKNAME");
				mAskName = true;
			} else if (string.IsNullOrEmpty(mCompanion.mCurrentUser.FirstName)) {
				Debug.Log("We don't know the current user name, ask him");
				Trigger("ASKNAME");
				mAskName = true;
			} else {
				Debug.Log("We know who is the curent user, ask some info: " + mCompanion.mCurrentUser.FirstName);

				mNeedListen = true;


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
			} else if (mCompanion.mCurrentUser.BirthDate.Year < 1900) {
				Interaction.TextToSpeech.SayKey("askbirth");
				mAskedQuestion = QuestionToAsk.BIRTH;
			} else if (mCompanion.mCurrentUser.Tastes.Colour == COLOUR.NONE) {
				Interaction.TextToSpeech.Say(Dictionary.GetRandomString("askfavorite").Replace("[subject]", Dictionary.GetRandomString("color")));
				mAskedQuestion = QuestionToAsk.FAVORITECOLOUR;
			} else if (mCompanion.mCurrentUser.Tastes.Sport == SPORT.NONE) {
				Interaction.TextToSpeech.Say(Dictionary.GetRandomString("askfavorite").Replace("[subject]", "sport"));
				mAskedQuestion = QuestionToAsk.FAVORITESPORT;
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

			int lRand = UnityEngine.Random.Range(0, 6);

			if (lRand == 0) {
				Interaction.TextToSpeech.SayKey("asklastname");
				mAskedQuestion = QuestionToAsk.LASTNAME;
			} else if (lRand == 1) {
				Interaction.TextToSpeech.SayKey("askbirth");
				mAskedQuestion = QuestionToAsk.BIRTH;
			} else if (lRand == 2) {
				Interaction.TextToSpeech.Say(Dictionary.GetRandomString("askfavorite").Replace("[subject]", Dictionary.GetRandomString("color")));
				mAskedQuestion = QuestionToAsk.FAVORITECOLOUR;
			} else if (lRand == 3) {
				Interaction.TextToSpeech.Say(Dictionary.GetRandomString("askfavorite").Replace("[subject]", Dictionary.GetRandomString("sport")));
				mAskedQuestion = QuestionToAsk.FAVORITESPORT;
			} else if (lRand == 4) {
				Interaction.TextToSpeech.Say(Dictionary.GetRandomString("askfavorite").Replace("[subject]", Dictionary.GetRandomString("musicband")));
				mAskedQuestion = QuestionToAsk.FAVORITEMUSICBAND;
			} else if (lRand == 5) {
				Interaction.TextToSpeech.SayKey("askoccupation");
				mAskedQuestion = QuestionToAsk.OCCUPATION;
			}
		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			if (mAskName) {
				Debug.Log("need to ask name!!!!");
				Trigger("ASKNAME");
			}

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
				DateTime lBirthDate = StringToDate(iMsg.ToLower());

				if (mCompanion.mCurrentUser.BirthDate.Year < 1900 && lBirthDate.Year > 1900 && lBirthDate != mCompanion.mCurrentUser.BirthDate)
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ithoughitwas") + " " + mCompanion.mCurrentUser.BirthDate, true);
				else if (lBirthDate.Year < 1900) {
					//TODO error
					Interaction.TextToSpeech.Say("error date:" + " " + iMsg);
					return;
				} else
					Interaction.TextToSpeech.Say("ok" + " " + iMsg);

				mCompanion.mCurrentUser.BirthDate = StringToDate(iMsg.ToLower());

			} else if (mAskedQuestion == QuestionToAsk.FAVORITECOLOUR) {
				COLOUR lColour = VocalTrigerred.StringToEnum<COLOUR>(iMsg.ToLower());
				if (!(mCompanion.mCurrentUser.Tastes.Colour == COLOUR.NONE) && lColour != COLOUR.NONE && lColour != mCompanion.mCurrentUser.Tastes.Colour)
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ithoughitwas") + " " + Dictionary.GetString(mCompanion.mCurrentUser.Tastes.Colour.ToString().ToLower()), true);
				else if (lColour == COLOUR.NONE) {
					//TODO error
					Interaction.TextToSpeech.Say("error color:" + " " + iMsg);
					return;
				} else
					Interaction.TextToSpeech.Say("ok" + " " + iMsg);

				mCompanion.mCurrentUser.Tastes.Colour = lColour;

			} else if (mAskedQuestion == QuestionToAsk.FAVORITESPORT) {
				SPORT lSport = VocalTrigerred.StringToEnum<SPORT>(iMsg.ToLower());
				if (!(mCompanion.mCurrentUser.Tastes.Colour == COLOUR.NONE) && lSport != SPORT.NONE && lSport != mCompanion.mCurrentUser.Tastes.Sport)
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ithoughitwas") + " " + Dictionary.GetString(mCompanion.mCurrentUser.Tastes.Colour.ToString().ToLower()), true);
				else if (lSport == SPORT.NONE) {
					//TODO error
					Interaction.TextToSpeech.Say("error sport:" + " " + iMsg);
					return;
				} else
					Interaction.TextToSpeech.Say("ok" + " " + iMsg);

				mCompanion.mCurrentUser.Tastes.Sport = lSport;

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


		//private COLOUR StringToColour(string iColor)
		//{
		//	if (VocalTrigerred.ContainsOneOf(iColor, "red"))
		//		return COLOUR.RED;
		//	else if (VocalTrigerred.ContainsOneOf(iColor, "blue"))
		//		return COLOUR.BLUE;
		//	else if (VocalTrigerred.ContainsOneOf(iColor, "black"))
		//		return COLOUR.BLACK;
		//	else if (VocalTrigerred.ContainsOneOf(iColor, "green"))
		//		return COLOUR.GREEN;
		//	else if (VocalTrigerred.ContainsOneOf(iColor, "grey"))
		//		return COLOUR.GREY;
		//	else if (VocalTrigerred.ContainsOneOf(iColor, "pink"))
		//		return COLOUR.PINK;
		//	else if (VocalTrigerred.ContainsOneOf(iColor, "yellow"))
		//		return COLOUR.YELLOW;
		//	else if (VocalTrigerred.ContainsOneOf(iColor, "purple"))
		//		return COLOUR.PURPLE;
		//	else if (VocalTrigerred.ContainsOneOf(iColor, "orange"))
		//		return COLOUR.ORANGE;
		//	else if (VocalTrigerred.ContainsOneOf(iColor, "brown"))
		//		return COLOUR.BROWN;

		//	else return COLOUR.NONE;
		//}


		private DateTime StringToDate(string iDate)
		{
			string lPattern;
			if (BYOS.Instance.Language.CurrentLang == Language.FR)
				lPattern = @"^((31(?!\ (Fév(rier)?|avr(il)?|juin?|(sep(?=\b|t)t?|nov)(embre)?)))|((30|29)(?!\ fév(rier)?))|(29(?=\ fév(rier)?\ (((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)))))|(0?[1-9])|1\d|2[0-8])\ (jan(vier)?|fev(rier)?|ma(r(s)?|i)|avr(il)?|ju((illet?)|(in?))|ao(ût)?|oct(obre)?|(sep(?=\b|t)t?|nov|déc)(embre)?)\ ((1[6-9]|[2-9]\d)\d{2})$";
			else
				lPattern = @"^((31(?!\ (feb(ruary)?|apr(il)?|june?|(sep(?=\b|t)t?|nov)(ember)?)))|((30|29)(?!\ feb(ruary)?))|(29(?=\ feb(ruary)?\ (((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)))))|(0?[1-9])|1\d|2[0-8])\ (jan(uary)?|feb(ruary)?|ma(r(ch)?|y)|apr(il)?|ju((ly?)|(ne?))|aug(ust)?|oct(ober)?|(sep(?=\b|t)t?|nov|dec)(ember)?)\ ((1[6-9]|[2-9]\d)\d{2})$";
			//Regex rgx = new Regex(pattern);
			Match lMatch = Regex.Match(iDate, lPattern);
			if (lMatch.Success) {
				string[] lWords = lMatch.Value.Split(' ');
				return new DateTime(int.Parse(lWords[2]), Month2Int(lWords[1].ToLower()), int.Parse(lWords[0]));
			} else return new DateTime();
		}

		//TODO: replace with StringToEnum
		private int Month2Int(string iMonth)
		{
			if (iMonth.Contains("ja"))
				return 1;
			else if (iMonth[0] == 'f')
				return 2;
			else if (iMonth.Contains("mar"))
				return 3;
			else if (iMonth[0] == 'a' && iMonth.Contains("il"))
				return 4;
			else if (iMonth.Contains("ma"))
				return 5;
			else if (iMonth.Contains("ju") && iMonth.Contains("n"))
				return 6;
			else if (iMonth.Contains("ju"))
				return 7;
			else if (iMonth[0] == 'a')
				return 8;
			else if (iMonth.Contains("sept"))
				return 9;
			else if (iMonth.Contains("octo"))
				return 10;
			else if (iMonth.Contains("nov"))
				return 11;
			else if (iMonth.Contains("decem"))
				return 12;

			else return 1;
		}

		private void ErrorSTT(STTError iError)
		{
			mNeedListen = true;
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Debug.Log("ask info exit!!!!!!!!");
			mState.text = "";
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.CHAT;

			Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Remove(ErrorSTT);
		}


	}
}