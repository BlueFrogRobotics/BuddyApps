using UnityEngine.UI;
using UnityEngine;
using Buddy;
using Buddy.UI;
using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
	/// <summary>
	/// State where the user can set the detection sensibility, test them and set the head orientation
	/// </summary>
	public class ParametersState : AStateMachineBehaviour
	{
		private GuardianLayout mDetectionLayout;
		private bool mHasSwitchState = false;

		/// <summary>
		/// Enum of the different sub parameters windows
		/// </summary>
		private enum ParameterWindow : int
		{
			HEAD_ORIENTATION = 0,
			MOVEMENT = 1,
			SOUND = 2,
			FIRE = 3
		}

		public override void Start()
		{
			mDetectionLayout = new GuardianLayout();
			mHasSwitchState = false;
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			GuardianData.Instance.HeadOrientation = false;
			GuardianData.Instance.MovementDebug = false;
			GuardianData.Instance.SoundDebug = false;
			GuardianData.Instance.FireDebug = false;

			//mDetectionLayout = new GuardianLayout();
			mHasSwitchState = false;

			if (GuardianData.Instance.FirstRun)
				Interaction.TextToSpeech.SayKey("firstparam");



			BYOS.Instance.Toaster.Display<ParameterToast>().With(mDetectionLayout,
				() => { Trigger("NextStep"); },
				null);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (!mHasSwitchState) {
				if (GuardianData.Instance.HeadOrientation) {
					SwitchState(iAnimator, ParameterWindow.HEAD_ORIENTATION);
				} else if (GuardianData.Instance.MovementDebug) {
					SwitchState(iAnimator, ParameterWindow.MOVEMENT);
				} else if (GuardianData.Instance.SoundDebug) {
					SwitchState(iAnimator, ParameterWindow.SOUND);
				} else if (GuardianData.Instance.FireDebug) {
					SwitchState(iAnimator, ParameterWindow.FIRE);
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			//BYOS.Instance.Toaster.Hide();
			//GuardianData.Instance.FirstRun = false;
			//mDetectionLayout.mContacts.SetDefault(GuardianData.Instance.Contact.FirstName + " " + GuardianData.Instance.Contact.LastName);
		}

		private void SetThreshold()
		{
			//Detection.NoiseStimulus.Threshold = (100.0f - GuardianData.Instance.SoundDetectionThreshold) / 100.0f * 0.3f;
		}

		private void SwitchState(Animator iAnimator, ParameterWindow iParamWindow)
		{
			Toaster.Hide();
			Toaster.Display<BackgroundToast>().With();
			mHasSwitchState = true;
			//Detection.NoiseStimulus.enabled = true;
			SetThreshold();
			iAnimator.SetInteger("DebugMode", (int)iParamWindow);
		}

	}
}