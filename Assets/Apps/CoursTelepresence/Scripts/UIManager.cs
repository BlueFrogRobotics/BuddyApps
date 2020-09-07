using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using BlueQuark;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Text StudentName;

        [SerializeField]
        private Text StudentClass;

        [SerializeField]
        private Text PingHeader;

        [SerializeField]
        private Text Ping;

        [SerializeField]
        private Image BatteryIcon;

        [SerializeField]
        private Image BatteryIconHeader;

        [SerializeField]
        private Image NetworkIcon;

        [SerializeField]
        private RectTransform TopSection;

        private RTMManager mRTMManager;

        private float mTime;

        private static UIManager mInstance = null;
        private string mBatteryLevel;
        private string mNetworkLevel;

        public static UIManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new UIManager();
                return mInstance as UIManager;
            }
        }

        // Use this for initialization
        void Start()
        {
            // Update ping
            mRTMManager = GetComponent<RTMManager>();
            mRTMManager.OnPing = OnPingValue;
            mTime = Time.time;

            //init battery level
            if (Buddy.Sensors.Battery.Level > 0.9F)
                mBatteryLevel = "04";
            else if (Buddy.Sensors.Battery.Level > 0.7F)
                mBatteryLevel = "03";
            else if (Buddy.Sensors.Battery.Level > 0.5F)
                mBatteryLevel = "02";
            else if (Buddy.Sensors.Battery.Level > 0.15F)
                mBatteryLevel = "01";
            else
                mBatteryLevel = "00";

            // init Network level
            mNetworkLevel = "00";

            BatteryIconHeader.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
            BatteryIcon.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);

            Buddy.Sensors.Battery.OnLevelChange.Add(OnBatteryUpdate);

        }

        private void Update()
        {
            if(Time.time -  mTime > 6)
            {
                Ping.text = "";
                PingHeader.text = "";
            }
        }

        private void OnPingValue(int lValue)
        {
            mTime = Time.time;
            if (lValue < 500)
            {
                Ping.text = lValue.ToString() + "ms";
                PingHeader.text = lValue.ToString() + "ms";
            }
            else
            {
                Ping.text = "";
                PingHeader.text = "";
            }

            // Update icon
            if (lValue < 60) {
                if (mNetworkLevel != "04") {
                    mNetworkLevel = "04";
                    NetworkIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconSignal" + mNetworkLevel, Context.APP);
                }
            } else if (lValue < 100) {
                if (mNetworkLevel != "03") {
                    mNetworkLevel = "03";
                    NetworkIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconSignal" + mNetworkLevel, Context.APP);
                }
            } else if (lValue < 150) {
                if (mNetworkLevel != "02") {
                    mNetworkLevel = "02";
                    NetworkIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconSignal" + mNetworkLevel, Context.APP);
                }
            } else if (lValue < 200) {
                if (mNetworkLevel != "01") {
                    mNetworkLevel = "01";
                    NetworkIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconSignal" + mNetworkLevel, Context.APP);
                }
            } else if (mNetworkLevel != "00") {
                mNetworkLevel = "00";
                NetworkIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconSignal" + mNetworkLevel, Context.APP);
            }
        }

        private void OnBatteryUpdate(float lValue)
        {
            if (lValue > 0.9F) {
                if (mBatteryLevel != "04") {
                    mBatteryLevel = "04";
                    Debug.LogWarning("set battery to " + mBatteryLevel + " with image" + (
                        Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP) == null));
                    BatteryIcon.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
                    BatteryIconHeader.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
                }
            } else if (lValue > 0.7F) {
                if (mBatteryLevel != "03") {
                    mBatteryLevel = "03";
                    Debug.LogWarning("set battery to " + mBatteryLevel + " with image" + (
                        Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP) == null));
                    BatteryIcon.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
                    BatteryIconHeader.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
                }
            } else if (lValue > 0.5F) {
                if (mBatteryLevel != "02") {
                    mBatteryLevel = "02";
                    Debug.LogWarning("set battery to " + mBatteryLevel + " with image" + (
                        Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP) == null));
                    BatteryIcon.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
                    BatteryIconHeader.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
                }
            } else if (lValue > 0.15F) {
                if (mBatteryLevel != "01") {
                    mBatteryLevel = "01";
                    Debug.LogWarning("set battery to " + mBatteryLevel + " with image" + (
                        Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP).name == null));
                    BatteryIcon.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
                    BatteryIconHeader.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
                }
            } else if (mBatteryLevel != "00") {
                mBatteryLevel = "00";
                Debug.LogWarning("set battery to " + mBatteryLevel + " with image" + (
                    Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP) == null));
                BatteryIcon.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
                BatteryIconHeader.sprite = Buddy.Resources.Get<Sprite>("Education_IconBattery" + mBatteryLevel, Context.APP);
            }
        }

        public void UpdateVolume(Slider lSlider)
        {
            if (Math.Abs(Buddy.Actuators.Speakers.Volume - lSlider.value) > 0.05) {

                Buddy.Actuators.Speakers.Volume = lSlider.value;
                if (!Buddy.Actuators.Speakers.IsBusy)
                    Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
            }
        }

        public void UpdateSlider(Slider lSlider)
        {
            lSlider.value = Buddy.Actuators.Speakers.Volume;
        }

    }

}
