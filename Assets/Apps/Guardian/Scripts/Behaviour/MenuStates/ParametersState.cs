using UnityEngine.UI;
using UnityEngine;
using BlueQuark;
using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State that give the choice between the different parameters to set
    /// </summary>
    public sealed class ParametersState : AStateMachineBehaviour
    {
        private bool mHasSwitchState = false;

        private FButton mValidateButton;

        private bool mToastVisible;
        private float mTimer;

        private List<ButtonContent> mButtonContents = new List<ButtonContent>();

        private class ButtonContent
        {
            public string Label { get; set; }
            public string TriggerName { get; set; }
            public string SpriteName { get; set; }

            public ButtonContent(string iLabel, string iTriggerName, string iSpriteName)
            {
                Label = iLabel;
                TriggerName = iTriggerName;
                SpriteName = iSpriteName;
            }
        }

        public override void Start()
        {
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GuardianData.Instance.HeadOrientation = false;
            GuardianData.Instance.MovementDebug = false;
            GuardianData.Instance.SoundDebug = false;
            GuardianData.Instance.FireDebug = false;

            mHasSwitchState = false;
            mToastVisible = false;
            mTimer = 0.0F;

            if (GuardianData.Instance.FirstRunParam)
            {
                //Buddy.Vocal.SayKey("firstparam");
            }

            
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GuardianData.Instance.FirstRunParam = true;
            mTimer += Time.deltaTime;
            if(mTimer>1.0F && !mToastVisible)
            {
                ShowToast();
                mToastVisible = true;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Footer.Remove<FButton>(mValidateButton);
        }

        private void ShowToast()
        {
            mButtonContents.Clear();
            mButtonContents.Add(new ButtonContent(Buddy.Resources.GetString("motiondetection"), "MovementDetection", "os_icon_agent"));
            mButtonContents.Add(new ButtonContent(Buddy.Resources.GetString("noisedetection"), "SoundDetection", "os_icon_sound_on"));
            mButtonContents.Add(new ButtonContent(Buddy.Resources.GetString("heatdetection"), "FireDetection", "Fire_Alert"));
            mButtonContents.Add(new ButtonContent(Buddy.Resources.GetString("generalparameters"), "GeneralParameters", "os_icon_cog"));

            Buddy.GUI.Header.DisplayParametersButton(false);
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("detectionparameters"));

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                foreach (ButtonContent lButtonContent in mButtonContents)
                {
                    TVerticalListBox lBox = iBuilder.CreateBox();
                    
                    lBox.OnClick.Add(() => { iBuilder.Select(lBox); Debug.Log("Click " + lButtonContent.Label); Trigger(lButtonContent.TriggerName); });
                    lBox.SetLabel(lButtonContent.Label);
                    lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>(lButtonContent.SpriteName));
                    lBox.SetCenteredLabel(true);
                    lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                    if (lButtonContent == mButtonContents[0])
                        iBuilder.Select(lBox);
                }
            });

            mValidateButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            mValidateButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));
            mValidateButton.SetBackgroundColor(Utils.BUDDY_COLOR);
            mValidateButton.SetIconColor(Color.white);

            mValidateButton.OnClick.Add(() => { Trigger("NextStep"); });
        }

    }
}