using System;
using System.Collections.Generic;
using BlueQuark;

using UnityEngine;

namespace BuddyApp.Fitness
{
	public class TrainingState : AStateMachineBehaviour
	{
		private SlideSet mSlider;
		private int mCurrentIndex;
		private float mSliderTime;
		private bool mQuit;


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On State Enter...");

			mSliderTime = 0.0F;
			mCurrentIndex = 0;
			mQuit = false;

			Buddy.Perception.SkeletonDetector.OnDetect.AddP(OnSkeletonDetect);

			Buddy.GUI.Screen.OnTouch.Add(OnStopListening);

			mSlider = Buddy.GUI.Toaster.DisplaySlide();

			while (mSlider.Count != 0) {
				Debug.LogError("training RemovingSlide!");
				mSlider.RemoveSlide(0);
			}

			Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + "a");
			/// Initializing slider with image sorted
			mSlider.AddFirstDisplayedSlide<PictureToast>().With(Buddy.Resources.Get<Sprite>(FitnessData.Instance.Exercise + "a"));
			mSlider.AddSlide<PictureToast>().With(Buddy.Resources.Get<Sprite>(FitnessData.Instance.Exercise + "b"));
			mSlider.AddSlide<PictureToast>().With(Buddy.Resources.Get<Sprite>(FitnessData.Instance.Exercise + "c"));

		}


		/*
		 *   On a skeleton detection this function is called.
		 *   mSkeletonDetectEnable: Enable or disable the code when WINDOWS is true.
		 *   Because the removeP function is in WIP on windows, we juste disable the code, for now.
		 */
		private bool OnSkeletonDetect(SkeletonEntity[] iSkeleton)
		{
			// We add each skeleton to a list, to display them later in OnNewFrame
			if(!mQuit)
			foreach (SkeletonEntity lSkeleton in iSkeleton) {
				// Look if one of the user is in correct position

				if (CheckPosition(lSkeleton.Joints, mCurrentIndex)) {
					// Bravo!
					Debug.Log("Position ok!!!!!!!!!!!!!!");
					NextSlide();
				}
			}
			return true;
		}

		private bool CheckPosition(SkeletonJoint[] joints, int mCurrentIndex)
		{
			if (FitnessData.Instance.Exercise == "armex") {

				// Create dictionnary with name to skeletonjoint
				Dictionary<SkeletonJointType, SkeletonJoint> lNameToJoint = new Dictionary<SkeletonJointType, SkeletonJoint>();

				// We browse all joints of the current skeleton
				foreach (var lJoint in joints)
					lNameToJoint[lJoint.Type] = lJoint;


				if (lNameToJoint.ContainsKey(SkeletonJointType.RIGHT_ELBOW) && lNameToJoint.ContainsKey(SkeletonJointType.LEFT_ELBOW) && lNameToJoint.ContainsKey(SkeletonJointType.RIGHT_WRIST) &&
					lNameToJoint.ContainsKey(SkeletonJointType.LEFT_WRIST)) {

					if (mCurrentIndex == 1) {
						// if right arm above elbow + 20cm
						if (lNameToJoint[SkeletonJointType.RIGHT_WRIST].WorldPosition.y - lNameToJoint[SkeletonJointType.RIGHT_ELBOW].WorldPosition.y > 200F)
							// if left arm above elbow
							if (lNameToJoint[SkeletonJointType.LEFT_WRIST].WorldPosition.y - lNameToJoint[SkeletonJointType.LEFT_ELBOW].WorldPosition.y > 200F)
								return true;

						// if right arm same height as elbow +- 4cm
					} else if (Math.Abs(lNameToJoint[SkeletonJointType.RIGHT_ELBOW].WorldPosition.y - lNameToJoint[SkeletonJointType.RIGHT_WRIST].WorldPosition.y) < 90F) {
						// if left arm above elbow


						if (Math.Abs(lNameToJoint[SkeletonJointType.LEFT_ELBOW].WorldPosition.y - lNameToJoint[SkeletonJointType.LEFT_WRIST].WorldPosition.y) < 90F) {
							return true;
						}
					}
				}
				return false;
			} else {
				// TODO
				return true;
			}
		}


		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mSliderTime += Time.deltaTime;
			// Cheer the guy from time to time?

		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "On State Exit...");
			Buddy.GUI.Screen.OnTouch.Remove(OnStopListening);
			Buddy.Perception.SkeletonDetector.OnDetect.Clear();
		}

		private void OnEndListening(SpeechInput iSpeechInput)
		{
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "RULE : " + iSpeechInput.Rule);
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "UTTERANCE : " + iSpeechInput.Utterance);
			ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "CONFIDENCE : " + iSpeechInput.Confidence);

			// Recognize current position

			Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + (char)(97 + mSlider.CurrentIndex));

		}

		private void NextSlide()
		{
			Debug.Log("NextSlide!!!");
			// Last photo
			if(!mQuit)
			if (mSlider.CurrentIndex + 1 == mSlider.Count) {
				mQuit = true;
				Debug.Log("finished!!!");
				Buddy.Vocal.SayKey("welldone");
				Buddy.GUI.Toaster.Hide();
				Buddy.Behaviour.SetMood(Mood.HAPPY);
				Trigger("MENU");

			} else {
				// Next Photo
				Debug.Log("NextPhoto!!!");
				mCurrentIndex++;
				Debug.Log("NextPhoto!!! " + FitnessData.Instance.Exercise + (char)(97 + mCurrentIndex));
				mSlider.GoNext();
				mSliderTime = 0.0F;
				Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + (char)(97 + mCurrentIndex));
			}
		}


		private void OnStopListening(Touch[] iTouch)
		{
			// If user tries to change slide with tactile
			if (mCurrentIndex != mSlider.CurrentIndex) {
				mSlider.GoTo(mCurrentIndex);
				// Explain current slider
				mSliderTime = 0.0F;
				Buddy.Behaviour.Face.PlayEvent(FacialEvent.WHAT);
				Buddy.Vocal.SayKey(FitnessData.Instance.Exercise + (char)(97 + mCurrentIndex));
			}
		}

	}
}
