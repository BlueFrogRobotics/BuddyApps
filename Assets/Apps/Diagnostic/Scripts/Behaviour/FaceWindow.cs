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
        
        void OnEnable()
        {
            BackgroundImage.SetActive(false);

            MoodDropdown.ClearOptions();
            EventDropdown.ClearOptions();
            LEDBehaviourDropdown.ClearOptions();

            MoodDropdown.onValueChanged.RemoveAllListeners();
            EventDropdown.onValueChanged.RemoveAllListeners();
            LEDBehaviourDropdown.onValueChanged.RemoveAllListeners();

            MoodDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(Mood))));
            EventDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(FacialEvent))));
            LEDBehaviourDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(LEDPulsePattern))));

            MoodDropdown.onValueChanged.AddListener((iInput) => OnMoodChanged());
            EventDropdown.onValueChanged.AddListener((iInput) => OnEventChanged());
            LEDBehaviourDropdown.onValueChanged.AddListener((iInput) => OnLEDBehaviourChanged());
        }

        void OnDisable()
        {
            BackgroundImage.SetActive(true);
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
    }
}
