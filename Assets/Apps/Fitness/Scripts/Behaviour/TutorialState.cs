using System;
using BlueQuark;

using UnityEngine;

namespace BuddyApp.Fitness
{
	public class TutorialState : AStateMachineBehaviour
	{
		private readonly string STR_QUIT_COMMAND = "quit";
		private readonly string STR_NEXT_COMMAND = "next";
		private readonly string STR_PREVIOUS_COMMAND = "previous";

		private readonly float F_MAX_TIME_LISTENING = 10.0F;
		private float mFTimeListening;
		private SlideSet mSlider;
		private int mCurrentIndex;
		private float mSliderTime;


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On State Enter...");

			mFTimeListening = F_MAX_TIME_LISTENING;

			// New listening events
			Buddy.Vocal.OnEndListening.Add(OnEndListening);
			Buddy.GUI.Screen.OnTouch.Add(OnStopListening);
			Buddy.Vocal.OnEndSpeaking.Add(OnEndSpeaking);

			mSliderTime = 0.0F;
			mCurrentIndex = 0;
			Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + "a");


			mSlider = Buddy.GUI.Toaster.DisplaySlide();

			/// Initializing slider with image sorted
			
			mSlider.AddFirstDisplayedSlide<PictureToast>().With(Buddy.Resources.Get<Sprite>(FitnessData.Instance.Exercise + "a"));
			mSlider.AddSlide<PictureToast>().With(Buddy.Resources.Get<Sprite>(FitnessData.Instance.Exercise + "b"));
			mSlider.AddSlide<PictureToast>().With(Buddy.Resources.Get<Sprite>(FitnessData.Instance.Exercise + "c"));

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mSliderTime += Time.deltaTime;

			// Go to next slide after 10 seconds
			if (mSliderTime > 10F)
				NextSlide();

			if (Buddy.Vocal.IsListening) {
				mFTimeListening -= Time.deltaTime;
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "On State Exit...");
			Buddy.Vocal.OnEndListening.Clear();
			Buddy.GUI.Screen.OnTouch.Remove(OnStopListening);
			Buddy.Vocal.OnEndSpeaking.Clear();
			Buddy.GUI.Toaster.Hide();
		}

		private void OnEndSpeaking(SpeechOutput iSpeechOutput)
		{
			// We listen if timeout is not reached yet
			if (0.0F <= mFTimeListening)
				Buddy.Vocal.Listen();
		}

		private void OnEndListening(SpeechInput iSpeechInput)
		{
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "RULE : " + iSpeechInput.Rule);
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "UTTERANCE : " + iSpeechInput.Utterance);
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "CONFIDENCE : " + iSpeechInput.Confidence);

			if (iSpeechInput.IsInterrupted || -1 == iSpeechInput.Confidence) // Error during recognition or forced StopListening
			{
				// Do nothing
				return;
			}

			if (Utils.GetRealStartRule(iSpeechInput.Rule).EndsWith(STR_QUIT_COMMAND)) {
				QuitApp();
				return;
			}

			if (Utils.GetRealStartRule(iSpeechInput.Rule).EndsWith(STR_NEXT_COMMAND)) {
				NextSlide();
				return;
			}

			if (Utils.GetRealStartRule(iSpeechInput.Rule).EndsWith(STR_PREVIOUS_COMMAND)
				// Not first index
				&& mSlider.CurrentIndex > 0) {

				// Previous Photo
				mCurrentIndex--;
				mSlider.GoPrevious();
				mSliderTime = 0.0F;
				Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + (char)(97 + mCurrentIndex));
				return;
			}

			if (0.0F <= mFTimeListening) {
				Buddy.Vocal.Listen();
			}
		}

		private void NextSlide()
		{
			// Last photo
			if (mSlider.CurrentIndex + 1 == mSlider.Count) {
				Trigger("POSITIONING");
			} else {
				// Next Photo
				mCurrentIndex++;
				mSlider.GoNext();
				mSliderTime = 0.0F;
				Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + (char)(97 + mCurrentIndex));
			}
		}

		private void OnStopListening(Touch[] iTouch)
		{
			// If user changed slide with tactile
			if (mCurrentIndex != mSlider.CurrentIndex) {
				mCurrentIndex = mSlider.CurrentIndex;
				// Explain current slider
				mSliderTime = 0.0F;
				Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + (char)(97 + mCurrentIndex));
			}

			mFTimeListening = -1.0F;
		}
	}
}
