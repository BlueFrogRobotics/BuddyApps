﻿using Buddy;
using Buddy.Command;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	//[RequireComponent(typeof(Reaction))]
	[RequireComponent(typeof(VocalChat))]
	public class VocalTrigerred : AStateMachineBehaviour
	{
		private VocalChat mVocalChat;
		//private Reaction mReaction;
		private bool mVocalWanderOrder;
		private bool mRobotIsTrackingSomeone;
		private bool mOrderGiven;
		private bool mSpeechInput;
		private bool mNeedListen;
		private float mTime;

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;
			mVocalChat = GetComponent<VocalChat>();
			mState = GetComponentInGameObject<Text>(0);
			//mReaction = GetComponent<Reaction>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Vocal Triggered";
			Debug.Log("state: Vocal Triggered");

			mSpeechInput = false;
			mOrderGiven = false;
			mVocalWanderOrder = false;
			mRobotIsTrackingSomeone = false;
			Interaction.VocalManager.EnableTrigger = false;
			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
			mVocalChat.Activate();
			Interaction.VocalManager.EnableDefaultErrorHandling = true;
			mVocalChat.WithNotification = true;
			mVocalChat.OnQuestionTypeFound = SortQuestionType;
			mNeedListen = true;
			mTime = 0F;
		}


		void OnSpeechRecognition(string iText)
		{
			mTime = 0F;
			mSpeechInput = true;

			// Compensate VocalChat doing nothing if accept:
			if(ContainsOneOf(iText, Dictionary.GetPhoneticStrings("accept"))){
				mNeedListen = true;
			}
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;
            if (Interaction.TextToSpeech.HasFinishedTalking) {
				Debug.Log("STATE onenter done: " + " needListen: " + mNeedListen);
				if (mNeedListen) {
					Debug.Log("VocalTriggered Start Listen");

					Interaction.VocalManager.StartInstantReco();
					mNeedListen = false;
					mTime = 0F;
                } else {
					Debug.Log("pre pre Back to IDLE: ");
					if (!mVocalChat.BuildingAnswer) {
						if (mVocalWanderOrder) {
							Trigger("WANDER");
						} else if (mOrderGiven || mVocalChat.AnswerGiven) {
							Trigger("ASKNEWRQ");
						} else if (Interaction.VocalManager.RecognitionFinished && mTime > 15F) {
							Debug.Log("pre Back to IDLE: ");
							if (!mSpeechInput) {
								//Mb this was a wrong trigger, back to IDLE
								Debug.Log("Back to IDLE: ");
								Trigger("IDLE");
							}
						}
					}
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			Debug.Log("VOCAL TRIGGERED STATE EXIT");
			mNeedListen = true;
			mSpeechInput = false;
			Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
			mVocalChat.DisActivate();
		}

		//Sort the type of the question returned by the Vocal Chat.
		//It either corresponds to orders on movement or launch applications
		private void SortQuestionType(string iType)
		{
			Debug.Log("Question Type found : " + iType);

			mOrderGiven = true;

			switch (iType) {
				case "Quit":
					break;
				case "Weather":
					break;
				case "Definition":
					break;
				case "Wander":
					Interaction.TextToSpeech.Say(Dictionary.GetString("wander"));
					mVocalWanderOrder = true;
					//TODO, maybe ask for interaction instead if Buddy really wants to interact
					CompanionData.Instance.InteractDesire -= 10;
					if (CompanionData.Instance.MovingDesire < 50)
						CompanionData.Instance.MovingDesire = 50;

					Debug.Log("Start wanderring by voice");
					break;

				case "CanMove":
					CompanionData.Instance.CanMoveBody = true;
					break;

				case "DontMove":
					CompanionData.Instance.MovingDesire -= 20;
					mVocalWanderOrder = false;
					CompanionData.Instance.CanMoveBody = false;
					//mReaction.StopMoving();
					mRobotIsTrackingSomeone = false;
					//GetComponent<FollowWanderReaction>().enabled = false;
					break;

				case "FollowMe":
					if (!mRobotIsTrackingSomeone) {
						Interaction.TextToSpeech.Say(Dictionary.GetString("follow"));
						CompanionData.Instance.InteractDesire -= 10;
						//GetComponent<FollowWanderReaction>().enabled = true;
						mRobotIsTrackingSomeone = true;
					}
					break;

				case "HeadUp":
					//GetComponent<IdleReaction>().HeadPosition = -5F;
					//GetComponent<WanderReaction>().HeadPosition = -5F;
					break;

				case "HeadDown":
					//GetComponent<IdleReaction>().HeadPosition = 15F;
					//GetComponent<WanderReaction>().HeadPosition = 15F;
					break;

				case "VolumeUp":
					BYOS.Instance.Primitive.Speaker.VolumeUp();
					break;

				case "VolumeDown":
					BYOS.Instance.Primitive.Speaker.VolumeDown();
					break;

				case "LookAtMe":
					//mReaction.SearchFace();
					break;

				case "Alarm":
					new StartAppCmd("Alarm").Execute();
					break;

				case "Photo":
					CompanionData.Instance.InteractDesire -= 30;
					new StartAppCmd("TakePhotoApp").Execute();
					break;

				case "Pose":
					CompanionData.Instance.InteractDesire -= 30;
					new StartAppCmd("TakePoseApp").Execute();
					break;

				case "Calcul":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("CalculGameApp").Execute();
					break;

				case "Babyphone":
					CompanionData.Instance.InteractDesire -= 10;
					new StartAppCmd("BabyApp").Execute();
					break;

				case "FreezeDance":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("FreezeDanceApp").Execute();
					break;

				case "Guardian":
					CompanionData.Instance.InteractDesire -= 20;
					new StartAppCmd("Guardian").Execute();
					break;

				case "IOT":
					CompanionData.Instance.InteractDesire -= 10;
					new StartAppCmd("IOT").Execute();
					break;

				case "Jukebox":
					CompanionData.Instance.InteractDesire -= 20;
					new StartAppCmd("JukeboxApp_V2").Execute();
					break;

				case "Recipe":
					CompanionData.Instance.InteractDesire -= 20;
					new StartAppCmd("Recipe").Execute();
					break;

				case "RLGL":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("RLGLApp").Execute();
					break;

				case "Memory":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("MemoryGameApp").Execute();
					break;

				case "HideSeek":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("HideAndSeek").Execute();
					break;

				case "Quizz":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("QuizzGameApp").Execute();
					break;

				case "Colors":
					break;

				default:
					break;

			}

		}

		private bool ContainsOneOf(string iSpeech, string[] iListSpeech)
		{
			iSpeech = iSpeech.ToLower();
			for (int i = 0; i < iListSpeech.Length; ++i) {
				string[] words = iListSpeech[i].Split(' ');
				if (words.Length < 2) {
					words = iSpeech.Split(' ');
					foreach (string word in words) {
						if (word == iListSpeech[i].ToLower()) {
							return true;
						}
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
					return true;
			}
			return false;
		}

	}
}