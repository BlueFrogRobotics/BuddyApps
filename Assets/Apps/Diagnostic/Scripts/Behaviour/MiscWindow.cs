using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public class MiscWindow : MonoBehaviour
    {
        [Header("Battery")]
        [SerializeField]
        private Slider SliderBattery;
        [SerializeField]
        private Text BatteryPercent;
        [SerializeField]
        private Text BatteryStatus;

        [Header("Main Board Voltage")]
        [SerializeField]
        private Text TextVoltage;

        [Header("Version")]
        [SerializeField]
        private Text HemiseVersion;
        [SerializeField]
        private Text MotionVersion;
        [SerializeField]
        private Text HeadVersion;
        [SerializeField]
        private Text RainetteVersion;
        [SerializeField]
        private Text AudioVersion;

        [Header("Status")]
        [SerializeField]
        private Text HemiseStatus;
        [SerializeField]
        private Text MotionStatus;
        [SerializeField]
        private Text HeadStatus;
        [SerializeField]
        private Text RainetteStatus;
        [SerializeField]
        private Text AudioStatus;

        [Header("Amplifier")]
        [SerializeField]
        private Slider AmplifierSlider;

        [Header("Button reset")]
        [SerializeField]
        private Button ButtonResetHemise;
        [SerializeField]
        private Button ButtonResetMotion;
        [SerializeField]
        private Button ButtonResetHead;
        [SerializeField]
        private Button ButtonResetRainette;
        [SerializeField]
        private Button ButtonResetAudio;


        private float mBatteryLevel = 0F;
        private float mBatterySaved = 0F;

        // Use this for initialization
        void Start()
        {

            mBatteryLevel = SystemInfo.batteryLevel*100F;
            mBatterySaved = mBatteryLevel;
            SliderBattery.value = mBatteryLevel / 100F;
            //Version :
            HemiseVersion.text = Buddy.Boards.Body.Version;
            MotionVersion.text = Buddy.Boards.Body.WheelsµC.Version;
            HeadVersion.text = Buddy.Boards.Head.HeadµC.Version;
            RainetteVersion.text = Buddy.Boards.Head.Version;
            AudioVersion.text = Buddy.Boards.Head.AudioµC.Version;

            //Status : 
            HemiseStatus.text = Buddy.Boards.Body.Status.ToString();
            MotionStatus.text = Buddy.Boards.Body.WheelsµC.Status.ToString();
            HeadStatus.text = Buddy.Boards.Head.HeadµC.Status.ToString();
            RainetteStatus.text = Buddy.Boards.Head.Status.ToString();
            AudioStatus.text = Buddy.Boards.Head.AudioµC.Status.ToString();

            //ResetButton :
            ButtonResetHemise.onClick.AddListener(ResetHemise);
            ButtonResetMotion.onClick.AddListener(ResetMotion);
            ButtonResetHead.onClick.AddListener(ResetHead);
            ButtonResetRainette.onClick.AddListener(ResetRainette);
            ButtonResetAudio.onClick.AddListener(ResetAudio);

            AmplifierSlider.onValueChanged.AddListener((iInput) => OnSliderAmplifierChanger(iInput));

        }

        // Update is called once per frame
        void Update()
        {
            mBatteryLevel = SystemInfo.batteryLevel*100F;
            if(mBatterySaved != mBatteryLevel)
            {
                mBatterySaved = mBatteryLevel;
                SliderBattery.value = mBatteryLevel/100F;
            }
            BatteryPercent.text = (SystemInfo.batteryLevel * 100).ToString();
            TextVoltage.text = Buddy.Sensors.Battery.AverageLevel.ToString("D") + " - " + Buddy.Sensors.Battery.Level.ToString("D");
        }

        public void ResetHemise()
        {
            Buddy.Boards.Body.ResetStatus();
            HemiseStatus.text = Buddy.Boards.Body.Status.ToString();
        }

        public void ResetMotion()
        {
            Buddy.Boards.Body.WheelsµC.ResetStatus();
            MotionStatus.text = Buddy.Boards.Body.WheelsµC.Status.ToString();

        }

        public void ResetHead()
        {
            Buddy.Boards.Head.HeadµC.ResetStatus();
            HeadStatus.text = Buddy.Boards.Head.HeadµC.Status.ToString();

        }

        public void ResetRainette()
        {
            Buddy.Boards.Head.ResetStatus();
            RainetteStatus.text = Buddy.Boards.Head.Status.ToString();

        }

        public void ResetAudio()
        {
            Buddy.Boards.Head.AudioµC.ResetStatus();
            AudioStatus.text = Buddy.Boards.Head.AudioµC.Status.ToString();

        }

        public void OnSliderAmplifierChanger(float iInput)
        {
            int mSliderValue = (int)iInput;
            if(mSliderValue == 0)
                Buddy.Actuators.Speakers.Gain = 20;
            else if(mSliderValue == 1)
                Buddy.Actuators.Speakers.Gain = 32;
            else if (mSliderValue == 2)
                Buddy.Actuators.Speakers.Gain = 36;
        }
    }

}
