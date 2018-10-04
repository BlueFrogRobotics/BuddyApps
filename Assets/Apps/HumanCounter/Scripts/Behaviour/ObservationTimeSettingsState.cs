using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.HumanCounter
{
    public sealed class ObservationTimeSettingsState : AStateMachineBehaviour
    {
        private const int MINIMUM_TIME = 10;
        private const int MAXIMUM_TIME = 3600;
        private const int DEFAULT_OBSERVATION_TIME = 30;
        private const int TIME_INCREMENT = 10;

        private TText mSettingMessage;
        private TToggle mToggleDetect;
        private string mTimeInfo;

        /*
         *  Temporary parameter toaster to set the observation time.
         *  This will be replace by carrousel toast when available.
         */
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.THINKING, true);
            // Custom Font (Not working because of a bug - wait for bug fix).
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0F, 0F, 0F, 1F);
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont); 
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("timertitle"));

            HumanCounterData.Instance.humanDetectToggle = false;
            // Setup to 30 seconds by default.
            HumanCounterData.Instance.observationTime = DEFAULT_OBSERVATION_TIME;

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                // Create a button to increment the time.
                TButton lIncrementTime = iOnBuild.CreateWidget<TButton>();
                lIncrementTime.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lIncrementTime.SetLabel(Buddy.Resources.GetString("incrementtimer"));
                // On click TIME_INCREMENT is add to the timer and the text is updated.
                lIncrementTime.OnClick.Add(() => 
                {
                    HumanCounterData.Instance.observationTime += TIME_INCREMENT;
                    UpdateTimeInfo();
                });

                // Create the text to inform the user about the timer value.
                mTimeInfo = (HumanCounterData.Instance.observationTime / 60).ToString();
                mTimeInfo += "m:" + (HumanCounterData.Instance.observationTime % 60) + "s";
                mSettingMessage = iOnBuild.CreateWidget<TText>();
                mSettingMessage.SetLabel(Buddy.Resources.GetString("timerinfo") + mTimeInfo);

                // Create a button to decrement the time.
                TButton lDecrementTime = iOnBuild.CreateWidget<TButton>();
                lDecrementTime.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                lDecrementTime.SetLabel(Buddy.Resources.GetString("decrementtimer"));
                // On click TIME_INCREMENT is substract to the timer and the text is updated.
                lDecrementTime.OnClick.Add(() => 
                {
                    HumanCounterData.Instance.observationTime -= TIME_INCREMENT;
                    UpdateTimeInfo();
                });

                // Create a button to reset to default observation time.
                TButton lResetTime = iOnBuild.CreateWidget<TButton>();
                lResetTime.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_spinning"));
                lResetTime.SetLabel(Buddy.Resources.GetString("resettimer"));
                // On click reset the timer to DEFAULT_OBSERVATION_TIME and the text is updated.
                lResetTime.OnClick.Add(() => 
                {
                    HumanCounterData.Instance.observationTime = DEFAULT_OBSERVATION_TIME;
                    UpdateTimeInfo();
                });

                // Create a toggle button to select face or human detect
                mToggleDetect = iOnBuild.CreateWidget<TToggle>();
                if (HumanCounterData.Instance.humanDetectToggle)
                    mToggleDetect.SetLabel(Buddy.Resources.GetString("toggledetect") + Buddy.Resources.GetString("human"));
                else
                    mToggleDetect.SetLabel(Buddy.Resources.GetString("toggledetect") + Buddy.Resources.GetString("face"));
                mToggleDetect.OnToggle.Add((iBool) =>
                {
                    HumanCounterData.Instance.humanDetectToggle = iBool;
                    UpdateToggleText();
                });
            },
            // Click left.
            () => { /* Back to next settings when available. */ },
            // Left label
            Buddy.Resources.GetString("cancel"),
            // Click right.
            () => { Trigger("ObservationView"); }
            // Right Label.
            , Buddy.Resources.GetString("next"));
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
        private void UpdateToggleText()
        {
            if (HumanCounterData.Instance.humanDetectToggle)
                mToggleDetect.SetLabel(Buddy.Resources.GetString("toggledetect") + Buddy.Resources.GetString("human"));
            else
                mToggleDetect.SetLabel(Buddy.Resources.GetString("toggledetect") + Buddy.Resources.GetString("face"));
        }

        private void UpdateTimeInfo()
        {
            // Check time consistency.
            if (HumanCounterData.Instance.observationTime < MINIMUM_TIME)
                HumanCounterData.Instance.observationTime = MINIMUM_TIME;
            if (HumanCounterData.Instance.observationTime > MAXIMUM_TIME)
                HumanCounterData.Instance.observationTime = MAXIMUM_TIME;

            // Update label TText.
            mTimeInfo = (HumanCounterData.Instance.observationTime / 60).ToString();
            mTimeInfo += "m:" + (HumanCounterData.Instance.observationTime % 60) + "s";
            mSettingMessage.SetLabel(Buddy.Resources.GetString("timerinfo") + mTimeInfo);
        }
    }
}
