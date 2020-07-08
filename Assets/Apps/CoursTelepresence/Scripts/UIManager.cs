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
        // TODO : Check hour to know which display of the header we need to display
        // Check which message we need to send to the tablet to activate the call


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

        // TODO this is tmp for testing purpose
        [SerializeField]
        private RectTransform TopSection;

        //[SerializeField]
        //private Button EndCallButton;

        private RTMManager mRTMManager;

        private float mTime;

        //[SerializeField]
        //private Text BatteryLevel;

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
            if(mTime + 2F < Time.time)
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

        // Update is called once per frame
        //void Update()
        //{
        //}


        // TODO this is tmp for testing purpose
        public void BatteryButton()
        {
            Debug.Log("Change top section");
            if (Application.isEditor) {
                Debug.Log("Change top section editor");
                if (TopSection.pivot.y != 2.5F) {
                    Debug.Log("Change top section pre " + TopSection.pivot.y);
                    TopSection.pivot = new Vector2(TopSection.pivot.x, 2.5F);
                } else {
                    Debug.Log("Change top section pre " + TopSection.pivot.y);
                    TopSection.pivot = new Vector3(TopSection.pivot.x, 1F);
                }
            } else {
                Debug.Log("Change top section out of editor");
                if (TopSection.pivot.y != 2.5F) {
                    Debug.Log("Change top section pre " + TopSection.pivot.y);
                    TopSection.pivot = new Vector2(TopSection.pivot.x, 2.5F);
                } else {
                    Debug.Log("Change top section pre " + TopSection.pivot.y);
                    TopSection.pivot = new Vector3(TopSection.pivot.x, 1F);
                }

            }

            Debug.Log("Change top section post " + TopSection.pivot.y);
        }

        public void UpdateVolume(Slider lSlider)
        {
            if (Math.Abs(Buddy.Actuators.Speakers.Volume - lSlider.value) > 0.05) {
                Debug.Log("PRE Volume set to " + Buddy.Actuators.Speakers.Volume);
                Debug.Log("PRE slider set to " + lSlider.value);
                Buddy.Actuators.Speakers.Volume = lSlider.value;
                if (!Buddy.Actuators.Speakers.IsBusy)
                    Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
                Debug.Log("POST Volume set to " + Buddy.Actuators.Speakers.Volume);
            }
        }

        public void UpdateSlider(Slider lSlider)
        {
            Debug.Log("PRE slider value " + lSlider.value);
            lSlider.value = Buddy.Actuators.Speakers.Volume;
            Debug.Log("POST slider value " + lSlider.value);
        }

    }

}
