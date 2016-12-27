using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BuddyOS;
using UnityEngine.UI;

namespace BuddyApp.TakePose
{
	public class TakePose : SpeechStateBehaviour
	{
		private bool mDisplayedTimer;
		private bool mSpeechDone;

		private int mRand = 0;
		private int mSpeechId = 0;
		private float mTimer = 0f;
		private float mExitTimer = 5.0f;

		private string mSpeech1 = "takePose";
		private string mSpeech2 = "3";
		private string mSpeech3 = "2";
		private string mSpeech4 = "1";

		private Canvas mCanvasTimer;
		private AnimManager mAnimationManager;

		private List<string> mTakePoseSpeech;

		public override void Init()
		{
			mCanvasTimer = GetComponentInGameObject<Canvas>(0);
			mAnimationManager = GetComponentInGameObject<AnimManager>(1);
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			// init
			BYOS.Instance.VocalActivation.enabled = false;
			animator.ResetTrigger("AskPoseAgain");
			SayInLang(mSpeech1);
			mSpeechDone = false;
			mDisplayedTimer = false;
			mRand = UnityEngine.Random.Range(0, 9);
			mSpeechId = 0;
			mTimer = 0f;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!mSpeechDone && !mTTS.IsSpeaking) {
				mTimer += Time.deltaTime;

				if (mTimer > 0.5f) {
					if (mDisplayedTimer) {
						HideCanvasTimer();
						mDisplayedTimer = false;
					}
				}

				if (mTimer > 1.0f) {

					if (mSpeechId == 0) {
						DisplayCanvasTimer(mSpeech2);
						mDisplayedTimer = true;
						mTTS.Say(mSpeech2);
					} else if (mSpeechId == 1) {
						DisplayCanvasTimer(mSpeech3);
						mDisplayedTimer = true;
						mTTS.Say(mSpeech3);
					} else if (mSpeechId == 2) {
						DisplayCanvasTimer(mSpeech4);
						mDisplayedTimer = true;
						mTTS.Say(mSpeech4);
					} else if (mSpeechId == 3) {
						SetFace();
					} else {
						mSpeechDone = true;
					}

					mSpeechId += 1;
					mTimer = 0f;
				}
			}

			if (mSpeechDone) {
				mTimer += Time.deltaTime;

				Debug.Log("mTimer " + mTimer);

				if (mTimer > mExitTimer)
					animator.SetTrigger("AskPoseAgain");
			}
		}


		private void SetFace()
		{
			switch (mRand) {
				case 0:

					BYOS.Instance.Speaker.Voice.Play(VoiceSound.SIGH);
					mMood.Set(MoodType.ANGRY);
					switch (UnityEngine.Random.Range(0, 1)) {
						case 0:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
						case 1:
							mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
							mAnimationManager.Blink();
							break;
						default:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
					}

					break;
				case 1:

					BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
					mMood.Set(MoodType.SURPRISED);
					mAnimationManager.Sigh();
					break;
				case 2:
					BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
					mMood.Set(MoodType.SCARED);
					mAnimationManager.Swallow();
					break;
				case 3:
					BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
					mMood.Set(MoodType.HAPPY);

					switch (UnityEngine.Random.Range(0, 1)) {
						case 0:
							mFace.SetEvent(FaceEvent.SMILE);
							mAnimationManager.Laugh();
							break;
						case 1:
							mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
							mAnimationManager.Blink();
							break;
						default:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
					}
					break;
				case 4:

					BYOS.Instance.Speaker.Voice.Play(VoiceSound.SIGH);
					mMood.Set(MoodType.SICK);
					mAnimationManager.Shivers();
					break;
				case 5:
					BYOS.Instance.Speaker.Voice.Play(VoiceSound.SIGH);
					mMood.Set(MoodType.TIRED);
					switch (UnityEngine.Random.Range(0, 1)) {
						case 0:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
						case 1:
							mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
							mAnimationManager.Blink();
							break;
						default:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
					}

					break;
				case 6:
					BYOS.Instance.Speaker.Voice.Play(VoiceSound.SIGH);
					mMood.Set(MoodType.THINKING);
					switch (UnityEngine.Random.Range(0, 1)) {
						case 0:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
						case 1:
							mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
							mAnimationManager.Blink();
							break;
						default:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
					}

					break;

				case 7:
					BYOS.Instance.Speaker.Voice.Play(VoiceSound.SIGH);
					mMood.Set(MoodType.GRUMPY);
					switch (UnityEngine.Random.Range(0, 1)) {
						case 0:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
						case 1:
							mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
							mAnimationManager.Blink();
							break;
						default:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
					}

					break;


				case 8:
					BYOS.Instance.Speaker.FX.Play(FXSound.BEEP_2);
					mMood.Set(MoodType.LOVE);
					switch (UnityEngine.Random.Range(0, 1)) {
						case 0:
							mFace.SetEvent(FaceEvent.SMILE);
							mAnimationManager.Scream();
							break;
						case 1:
							mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
							mAnimationManager.Blink();
							break;
						default:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
					}

					break;
				default:
					mMood.Set(MoodType.GRUMPY);
					switch (UnityEngine.Random.Range(0, 1)) {
						case 0:
							mFace.SetEvent(FaceEvent.SMILE);
							mAnimationManager.Laugh();
							break;
						case 1:
							mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
							mAnimationManager.Blink();
							break;
						default:
							mFace.SetEvent(FaceEvent.SCREAM);
							mAnimationManager.Scream();
							break;
					}
					break;
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("exit take Pose");
			mMood.Set(MoodType.NEUTRAL);
		}



		/********************** TIMER CANVAS **********************/

		private void DisplayCanvasTimer(string text)
		{
			Debug.Log("Display canvas Timer " + text);
			mCanvasTimer.GetComponent<Animator>().ResetTrigger("Open_WTimer");
			mCanvasTimer.GetComponent<Animator>().ResetTrigger("Close_WTimer");
			mCanvasTimer.GetComponent<Animator>().SetTrigger("Open_WTimer");
			mCanvasTimer.GetComponentInChildren<Text>().text = text;
		}

		private void UpdateCanvasTimer(string text)
		{
			Debug.Log("Update canvas Timer " + text);
			mCanvasTimer.GetComponentInChildren<Text>().text = text;
		}

		private void HideCanvasTimer()
		{
			Debug.Log("Hide canvas Timer");
			mCanvasTimer.GetComponent<Animator>().ResetTrigger("Open_WTimer");
			mCanvasTimer.GetComponent<Animator>().ResetTrigger("Close_WTimer");
			mCanvasTimer.GetComponent<Animator>().SetTrigger("Close_WTimer");
			Debug.Log("Hide canvas Timer2");

		}

	}
}