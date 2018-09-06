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

        private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

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
            Debug.Log("ON STATE ENTER PARAMETER STATE !!!!!!!!!!!!!!!!!!!!!!");
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

            //PARAMETER OF GUARDIAN : need to wait for the discussion between Antoine Marc and Delphine 
            //Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            //{
            //    iBuilder.CreateWidget<TButton>().SetLabel("move");
            //    //iBuilder.CreateWidget<TText>().SetLabel("test");
            //    iBuilder.CreateWidget<TSlider>().OnSlide.Add((iVal) => Debug.Log(iVal));
            //    iBuilder.CreateWidget<TToggle>().OnToggle.Add((iVal) => Debug.Log(iVal));
            //    //iBuilder.CreateWidget<TText>().SetLabel("test2");
            //},
            //() => { Debug.Log("Click cancel"); }, "Cancel",
            //() => { Debug.Log("Click next"); Buddy.GUI.Toaster.Hide(); }, "Next"
            //);

            mButtonContent.Clear();
            mButtonContent.Add(Buddy.Resources.GetString("movementdetection"), "MovementDetection");
            mButtonContent.Add(Buddy.Resources.GetString("sounddetection"), "SoundDetection");
            mButtonContent.Add(Buddy.Resources.GetString("firedetection"), "FireDetection");
            mButtonContent.Add(Buddy.Resources.GetString("generalparameters"), "GeneralParameters");


            Buddy.GUI.Header.DisplayParametersButton(false);
            Buddy.GUI.Header.DisplayLightTitle("Menu");

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                foreach (KeyValuePair<string, string> lButtonContent in mButtonContent)
                {
                    //We create the container
                    TVerticalListBox lBox = iBuilder.CreateBox();
                    //We create en event OnClick so we can trigger en event when we click on the box
                    lBox.OnClick.Add(() => { Debug.Log("Click " + lButtonContent.Key); Trigger(lButtonContent.Value); });
                    //We label our button with our informations in the dictionary
                    lBox.SetLabel(lButtonContent.Key);
                    //You can set a left button if you need to add en event or an icon at the left
                    //lBox.LeftButton.Hide();
                    lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("Fire_Alert"));
                    //We place the text of the button in the center of the box
                    lBox.SetCenteredLabel(true);
                    lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                }
            });
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
            Buddy.GUI.Toaster.Hide();
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