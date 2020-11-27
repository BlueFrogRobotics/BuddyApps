﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;
using System;

namespace BuddyApp.DiagnosticProd
{
    public class MiscWindow : MonoBehaviour
    {
        [Header("Battery")]
        [SerializeField]
        private Slider SliderBattery;
        [SerializeField]
        private Text BatteryPercent;

        [SerializeField]
        private Text StatusBatteryTop;

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

        private float mTimer;

        private float mBatteryLevel = 0F;
        private float mBatterySaved = 0F;

        // Use this for initialization
        void Start()
        {
            mTimer = 0F;
            // a tester
            AmplifierSlider.value = 0;

            mBatteryLevel = SystemInfo.batteryLevel * 100F;
            mBatterySaved = mBatteryLevel;
            SliderBattery.value = mBatteryLevel / 100F;
            //Version :
            HemiseVersion.text = Buddy.Boards.Body.BodyµC.Version;
            MotionVersion.text = Buddy.Boards.Body.WheelsµC.Version;
            HeadVersion.text = Buddy.Boards.Head.HeadµC.Version;
            RainetteVersion.text = Buddy.Boards.Head.Version;
            AudioVersion.text = Buddy.Boards.Head.AudioµC.Version;

            //Status : 
            HemiseStatus.text = "x" + Buddy.Boards.Body.BodyµC.Status.ToString("X");
            MotionStatus.text = "x" + Buddy.Boards.Body.WheelsµC.Status.ToString("X");
            HeadStatus.text = "x" + Buddy.Boards.Head.HeadµC.Status.ToString("X");
            RainetteStatus.text = "x" + Buddy.Boards.Head.Status.ToString("X");
            AudioStatus.text = "x" + Buddy.Boards.Head.AudioµC.Status.ToString("X");

            //ResetButton :
            ButtonResetHemise.onClick.AddListener(ResetHemise);
            ButtonResetMotion.onClick.AddListener(ResetMotion);
            ButtonResetHead.onClick.AddListener(ResetHead);
            ButtonResetRainette.onClick.AddListener(ResetRainette);
            ButtonResetAudio.onClick.AddListener(ResetAudio);

            AmplifierSlider.value = 2 - (int)Buddy.Actuators.Speakers.Gain;
            AmplifierSlider.onValueChanged.AddListener((iInput) => OnSliderAmplifierChanger(iInput));


            if (Buddy.Sensors.Battery.IsJackPlugged)
                StatusBatteryTop.text = "JACK PLUGGED";
            else
                StatusBatteryTop.text = "JACK UNPLUGGED";
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            if (mTimer > DiagnosticProdBehaviour.REFRESH_TIMER) {
                mTimer = 0F;
                TextVoltage.text = Buddy.Sensors.Battery.AverageValue.ToString("D") + "mV";

            }

            mBatteryLevel = SystemInfo.batteryLevel * 100F;
            if (mBatterySaved != mBatteryLevel) {
                mBatterySaved = mBatteryLevel;
                SliderBattery.value = mBatteryLevel / 100F;
            }
            BatteryPercent.text = (SystemInfo.batteryLevel * 100).ToString();

            UpdateBatteryStatus();
        }

        private void UpdateBatteryStatus()
        {
            if (Buddy.Sensors.Battery.ChargingStatus == BatteryChargingStatus.CHARGING)
                BatteryStatus.text = "Charge in progress";
            else if (Buddy.Sensors.Battery.ChargingStatus == BatteryChargingStatus.NOT_CHARGING)
                BatteryStatus.text = "Not charging";

            if (Buddy.Sensors.Battery.IsJackPlugged)
                StatusBatteryTop.text = "JACK PLUGGED";
            else
                StatusBatteryTop.text = "JACK UNPLUGGED";
        }

        public void ReadStatus()
        {
            HemiseVersion.text = "--";
            MotionVersion.text = "--";
            HeadVersion.text = "--";
            RainetteVersion.text = "--";
            AudioVersion.text = "--";

            StartCoroutine(WaitAndReset());
        }

        private IEnumerator WaitAndReset()
        {

            ResetHemise();
            ResetMotion();
            ResetHead();
            ResetRainette();
            ResetAudio();

            yield return new WaitForSeconds(0.4F);

            HemiseVersion.text = Buddy.Boards.Body.BodyµC.Version;
            MotionVersion.text = Buddy.Boards.Body.WheelsµC.Version;
            HeadVersion.text = Buddy.Boards.Head.HeadµC.Version;
            RainetteVersion.text = Buddy.Boards.Head.Version;
            AudioVersion.text = Buddy.Boards.Head.AudioµC.Version;
        }

        public void ResetHemise()
        {
            HemiseStatus.text = "--";
            StartCoroutine(WaitAndResetHemise());
        }

        private IEnumerator WaitAndResetHemise()
        {
            yield return new WaitForSeconds(0.4F);
            Buddy.Boards.Body.ResetStatus();
            HemiseStatus.text = "x" + Buddy.Boards.Body.Status.ToString("X");
        }

        public void ResetMotion()
        {
            MotionStatus.text = "--";
            StartCoroutine(WaitAndResetMotion());
        }

        private IEnumerator WaitAndResetMotion()
        {
            yield return new WaitForSeconds(0.4F);
            Buddy.Boards.Body.WheelsµC.ResetStatus();
            MotionStatus.text = "x" + Buddy.Boards.Body.WheelsµC.Status.ToString("X");
        }

        public void ResetHead()
        {
            HeadStatus.text = "--";
            StartCoroutine(WaitAndResetHead());
        }

        private IEnumerator WaitAndResetHead()
        {
            yield return new WaitForSeconds(0.4F);
            Buddy.Boards.Head.HeadµC.ResetStatus();
            HeadStatus.text = "x" + Buddy.Boards.Head.HeadµC.Status.ToString("X");
        }

        public void ResetRainette()
        {
            RainetteStatus.text = "--";
            StartCoroutine(WaitAndResetRainette());
        }

        private IEnumerator WaitAndResetRainette()
        {
            yield return new WaitForSeconds(0.4F);
            Buddy.Boards.Head.ResetStatus();
            RainetteStatus.text = "x" + Buddy.Boards.Head.Status.ToString("X");
        }

        public void ResetAudio()
        {
            AudioStatus.text = "--";
            StartCoroutine(WaitAndResetAudio());
        }

        private IEnumerator WaitAndResetAudio()
        {
            yield return new WaitForSeconds(0.4F);
            Buddy.Boards.Head.AudioµC.ResetStatus();
            AudioStatus.text = "x" + Buddy.Boards.Head.AudioµC.Status.ToString("X");
        }

        public void OnSliderAmplifierChanger(float iInput)
        {
            Buddy.Actuators.Speakers.Gain = (AudioGain) (2 - AmplifierSlider.value);
        }
    }

}
