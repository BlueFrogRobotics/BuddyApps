using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public sealed class FaceWindow : MonoBehaviour
    {
        [SerializeField]
        private GameObject BackgroundImage;

        [SerializeField]
        private Dropdown MoodDropdown;

        [SerializeField]
        private Dropdown EventDropdown;

        [SerializeField]
        private Dropdown LEDBehaviourDropdown;

        [SerializeField]
        private Slider EnergySlider;
        [SerializeField]
        private Text EnergyText;

        [SerializeField]
        private Slider PositivenessSlider;
        [SerializeField]
        private Text PositivenessText;

        [SerializeField]
        private Dropdown LabialExpression;

        private bool mIsDone;

        void OnEnable()
        {
            BackgroundImage.SetActive(false);

            MoodDropdown.ClearOptions();
            EventDropdown.ClearOptions();
            LEDBehaviourDropdown.ClearOptions();

            MoodDropdown.onValueChanged.RemoveAllListeners();
            EventDropdown.onValueChanged.RemoveAllListeners();
            LEDBehaviourDropdown.onValueChanged.RemoveAllListeners();
            LabialExpression.onValueChanged.RemoveAllListeners();

            MoodDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(Mood))));
            EventDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(FacialEvent))));
            LEDBehaviourDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(LEDPulsePattern))));

            MoodDropdown.onValueChanged.AddListener((iInput) => OnMoodChanged());
            EventDropdown.onValueChanged.AddListener((iInput) => OnEventChanged());
            LEDBehaviourDropdown.onValueChanged.AddListener((iInput) => OnLEDBehaviourChanged());


            EnergySlider.onValueChanged.AddListener((iInput) => OnChangeEnergy(iInput));
            PositivenessSlider.onValueChanged.AddListener((iInput) => OnChangePositiveness(iInput));
            EnergySlider.value = 1F;
            PositivenessSlider.value = 1F;

            LabialExpression.onValueChanged.AddListener((iInput) => OnChangeLabialExpression(iInput));
        }

        void OnDisable()
        {
            BackgroundImage.SetActive(true);
        }

        public void OnChangeLabialExpression(int iLabialValue)
        {
            if (iLabialValue == 1) {
                Buddy.Behaviour.Face.SetLabialExpression(BlueQuark.LabialExpression.NEUTRAL);
                Buddy.Vocal.Say("Neutral expression");
                iLabialValue = 0;
            } else if (iLabialValue == 2) {
                Buddy.Behaviour.Face.SetLabialExpression(BlueQuark.LabialExpression.HAPPY);
                Buddy.Vocal.Say("happy expression");
                iLabialValue = 0;
            } else if (iLabialValue == 3) {
                Buddy.Behaviour.Face.SetLabialExpression(BlueQuark.LabialExpression.ANGRY);
                Buddy.Vocal.Say("angry expression");
                iLabialValue = 0;
            }
        }

        public void OnChangePositiveness(float iInput)
        {
            float mPositiveness = 0F;
            mPositiveness = iInput - 1F;
            PositivenessText.text = mPositiveness.ToString("0.00");
            try {
                Buddy.Behaviour.Face.Pleasure = mPositiveness;
            } catch {
            }
        }

        public void OnChangeEnergy(float iInput)
        {
            float mEnergy = 0F;
            mEnergy = iInput - 1F;
            EnergyText.text = mEnergy.ToString("0.00");
            try {
                Buddy.Behaviour.Face.Arousal = mEnergy;
            } catch {
            }
        }

        public void OnMoodChanged()
        {
            Buddy.Behaviour.SetMood((Mood)Enum.Parse(typeof(Mood), MoodDropdown.captionText.text));
        }

        public void OnEventChanged()
        {
            Buddy.Behaviour.Face.PlayEvent((FacialEvent)Enum.Parse(typeof(FacialEvent), EventDropdown.captionText.text));
        }

        public void OnLEDBehaviourChanged()
        {
            Buddy.Actuators.LEDs.SetBodyPattern((LEDPulsePattern)Enum.Parse(typeof(LEDPulsePattern), LEDBehaviourDropdown.captionText.text));
        }

        public void OnResetFace()
        {
            EnergySlider.value = 1F;
            PositivenessSlider.value = 1F;
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        }
    }
}
