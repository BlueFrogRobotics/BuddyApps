using UnityEngine.UI;
using UnityEngine;
using BlueQuark;
using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
	/// <summary>
	/// State where the user can set the detection sensibility, test them and set the head orientation
	/// </summary>
	public class ParametersState : AStateMachineBehaviour
	{
		//private GuardianLayout mDetectionLayout;
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
			//mDetectionLayout = new GuardianLayout();
			mHasSwitchState = false;
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			GuardianData.Instance.HeadOrientation = false;
			GuardianData.Instance.MovementDebug = false;
			GuardianData.Instance.SoundDebug = false;
			GuardianData.Instance.FireDebug = false;
            
			mHasSwitchState = false;

            if (GuardianData.Instance.FirstRunParam)
            {
                Buddy.Vocal.SayKey("firstparam");
               
            }

			//Buddy.GUI.Toaster.Display<ParameterToast>().With(mDetectionLayout,
			//	() => { Trigger("NextStep"); },
			//	null);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            GuardianData.Instance.FirstRunParam = true;
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
            
        }

		private void SwitchState(Animator iAnimator, ParameterWindow iParamWindow)
		{
			Buddy.GUI.Toaster.Hide();
            //Add the custom toast with background
			//Toaster.Display<BackgroundToast>().With();
			mHasSwitchState = true;
			iAnimator.SetInteger("DebugMode", (int)iParamWindow);
		}

	}
}