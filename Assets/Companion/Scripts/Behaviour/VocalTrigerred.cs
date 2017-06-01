using Buddy;
using Buddy.Command;
using Buddy.Features.Stimuli;
using Buddy.Features.Vocal;
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
		

		public override void Start()
		{

			//mSensorManager = BYOS.Instance.SensorManager;
			mSensorManager = GetComponent<StimuliManager>();
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
			mVocalManager.EnableTrigger = false;
			BYOS.Instance.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
			//mVocalChat.Activate();
			mVocalManager.EnableDefaultErrorHandling = true;
			//mVocalChat.WithNotification = true;
			mVocalChat.OnQuestionTypeFound = SortQuestionType;
			mNeedListen = true;
		}


		void OnSpeechRecognition(string iText)
		{
			mSpeechInput = true;
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (mTTS.HasFinishedTalking) {
				if (mNeedListen) {

					mVocalManager.StartInstantReco();
					mNeedListen = false;
				} else {
					//if (!mVocalChat.BuildingAnswer) {
					//	if (mVocalWanderOrder) {
					//		iAnimator.SetTrigger("WANDER");
					//	} else if (mOrderGiven || mVocalChat.AnswerGiven) {
					//		iAnimator.SetTrigger("ASKNEWRQ");
					//	} else if (mVocalManager.RecoProcessFinished) {
					//		if (!mSpeechInput) {
					//			//Mb this was a wrong trigger, back to IDLE
					//			iAnimator.SetTrigger("IDLE");
					//		}
					//	}
					//}
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mSpeechInput = false;
			BYOS.Instance.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
			//mVocalChat.DisActivate();
		}

		//Sort the type of the question returned by the Vocal Chat.
		//It either corresponds to orders on movement or launch applications
		private void SortQuestionType(string iType)
		{
			Debug.Log("Question Type found : " + iType);

			mOrderGiven = true;

			switch (iType) {
				case "Quit":
					mOrderGiven = true;
					break;
				case "Weather":
					mOrderGiven = true;
					break;
				case "Definition":
					mOrderGiven = true;
					break;
				case "Wander":
					mTTS.Say(mDictionary.GetString("wander"));
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
						mTTS.Say(mDictionary.GetString("follow"));
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
					BYOS.Instance.Speaker.VolumeUp();
					break;

				case "VolumeDown":
					BYOS.Instance.Speaker.VolumeDown();
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
	}
}