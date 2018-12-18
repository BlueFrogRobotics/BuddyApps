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

		private SlideSet mSlider;
		private int mCurrentIndex;
		private bool mSpeechInterrupted;


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On State Enter...");


			mCurrentIndex = 0;
			mSpeechInterrupted = false;
			Buddy.Vocal.SayKey("tuto");
			Buddy.Vocal.Say("[400]");
			Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + "a", OnEndSpeaking);
			
			mSlider = Buddy.GUI.Toaster.DisplaySlide();
			
			while (mSlider.Count != 0) {
				Debug.LogError("tuto RemovingSlide!");
				mSlider.RemoveSlide(0);
			}

			/// Initializing slider with image sorted
			mSlider.AddFirstDisplayedSlide<PictureToast>().With(Buddy.Resources.Get<Sprite>(FitnessData.Instance.Exercise + "a"));
			mSlider.AddSlide<PictureToast>().With(Buddy.Resources.Get<Sprite>(FitnessData.Instance.Exercise + "b"));
			mSlider.AddSlide<PictureToast>().With(Buddy.Resources.Get<Sprite>(FitnessData.Instance.Exercise + "c"));

		}


		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "On State Exit...");
			Buddy.GUI.Screen.OnTouch.Remove(OnTouch);
			Buddy.GUI.Toaster.Hide();
		}

		private void OnEndSpeaking(SpeechOutput iSpeechOutput)
		{

			//if(iSpeechOutput.IsInterrupted)

			// If speech interrupted, don't go next slide
			if (mSpeechInterrupted)
				mSpeechInterrupted = false;
			else
				NextSlide();
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
				Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + (char)(97 + mCurrentIndex), OnEndSpeaking);
			}
		}

		private void OnTouch(Touch[] iTouch)
		{
			// If user changed slide with tactile
			if (mCurrentIndex != mSlider.CurrentIndex) {
				mSpeechInterrupted = true;
				Buddy.Vocal.StopSpeaking();
				mCurrentIndex = mSlider.CurrentIndex;
				// Explain current slider
				Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + (char)(97 + mCurrentIndex), OnEndSpeaking);
			}

		}
	}
}
