using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.HumanCounter
{
    enum observationTime : int
    {
        DEFAULT = 30,
        INCREMENT = 10,
    }

    public sealed class ObservationTimeSettingsState : AStateMachineBehaviour
    {
        private TText settingMessage;
        private string time;
        private float minimumTime = 10F;
        private float maximumTime = 3600F;

        /*
         *  Ajout de boutons et messages pour le reglage du temps. 
         *  (Code temporaire en attendant les carrousels)
         */
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.THINKING, true);

            // Custom Font (Not working because of a bug - [TODO] Have to wait for bug fix)
            Font headerFont = Buddy.Resources.Get<Font>("os_awesome");
            headerFont.material.color = new Color(0F, 0F, 0F, 1F);
            Buddy.GUI.Header.SetCustomLightTitle(headerFont); 
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("timertitle"));

            // Setup to 30 seconds by default
            HumanCounterData.Instance.observationTime = (float)observationTime.DEFAULT;

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                TButton incrementTime = iOnBuild.CreateWidget<TButton>();
                incrementTime.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                incrementTime.SetLabel(Buddy.Resources.GetString("incrementtimer"));
                incrementTime.OnClick.Add(() => 
                {
                    HumanCounterData.Instance.observationTime += (float)observationTime.INCREMENT;
                    updateMessageText();
                });

                time = ((int)(HumanCounterData.Instance.observationTime / 60)).ToString();
                time += "m:" + Mathf.RoundToInt(HumanCounterData.Instance.observationTime % 60) + "s";
                settingMessage = iOnBuild.CreateWidget<TText>();
                settingMessage.SetLabel(Buddy.Resources.GetString("timerinfo") + time);

                TButton decrementTime = iOnBuild.CreateWidget<TButton>();
                decrementTime.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                decrementTime.SetLabel(Buddy.Resources.GetString("decrementtimer"));
                decrementTime.OnClick.Add(() => 
                {
                    HumanCounterData.Instance.observationTime -= (float)observationTime.INCREMENT;
                    updateMessageText();
                });

                TButton resetTime = iOnBuild.CreateWidget<TButton>();
                resetTime.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_spinning"));
                resetTime.SetLabel(Buddy.Resources.GetString("resettimer"));
                resetTime.OnClick.Add(() => 
                {
                    HumanCounterData.Instance.observationTime = (float)observationTime.DEFAULT;
                    updateMessageText();
                });
            },
            // Click left
            () => { /* Back to next settings when available */ },
            // Left label
            Buddy.Resources.GetString("cancel"),
            // Click right
            () => { Trigger("ObservationView"); }
            // Right Label
            , Buddy.Resources.GetString("next"));
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }

        private void updateMessageText()
        {
            // Check time coherence
            if (HumanCounterData.Instance.observationTime < minimumTime)
                HumanCounterData.Instance.observationTime = minimumTime;
            if (HumanCounterData.Instance.observationTime > maximumTime)
                HumanCounterData.Instance.observationTime = maximumTime;

            // Update message TText
            time = ((int)(HumanCounterData.Instance.observationTime / 60)).ToString();
            time += "m:" + Mathf.RoundToInt(HumanCounterData.Instance.observationTime % 60) + "s";
            settingMessage.SetLabel(Buddy.Resources.GetString("timerinfo") + time);
        }
    }
}
