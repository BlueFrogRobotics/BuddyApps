using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public sealed class LEDWindow : MonoBehaviour
    {

        [SerializeField]
        private Dropdown mDropDown;

        [SerializeField]
        private Dropdown mDropDownSequence;

        private LEDPulsePattern mPattern;

        [SerializeField]
        private Toggle mToggle;
        
        [SerializeField]
        private Text textH;

        [SerializeField]
        private Text textS;

        [SerializeField]
        private Text textV;

        [SerializeField]
        private Text textLowLevel;

        [SerializeField]
        private Text textOnDuration;

        [SerializeField]
        private Text textOffDuration;

        [SerializeField]
        private Text textUpSlope;

        [SerializeField]
        private Text textDownSlope;



        [SerializeField]
        private Slider sliderH;

        [SerializeField]
        private Slider sliderS;

        [SerializeField]
        private Slider sliderV;

        [SerializeField]
        private Slider sliderLowLevel;

        [SerializeField]
        private Slider sliderOnDuration;

        [SerializeField]
        private Slider sliderOffDuration;

        [SerializeField]
        private Slider sliderUpSlope;

        [SerializeField]
        private Slider sliderDownSlope;

        [SerializeField]
        private RawImage rawImage;

        [SerializeField]
        private Button mFlash;

        private bool mStatus = false;
        private Sprite mStop;
        private Sprite mPlay;


        private void Start() 
        {
            mStop = Buddy.Resources.Get<Sprite>("os_icon_stop");
            mPlay = Buddy.Resources.Get<Sprite>("os_icon_play");
            mDropDown.onValueChanged.RemoveAllListeners();
            mDropDown.onValueChanged.AddListener((iInput) => SetColor());

            mDropDownSequence.onValueChanged.RemoveAllListeners();
            mDropDownSequence.onValueChanged.AddListener((iInput) => SetPattern());

            sliderH.wholeNumbers = true;
            sliderH.minValue = 0.0F;
            sliderH.maxValue = 360.0F;
            sliderH.value = sliderH.minValue;
            sliderH.onValueChanged.RemoveAllListeners();
            sliderH.onValueChanged.AddListener((iInput) => OnChangeH());

            sliderS.wholeNumbers = true;
            sliderS.minValue = 0.0F;
            sliderS.maxValue = 100.0F;
            sliderS.value = sliderS.maxValue;
            textS.text = sliderS.maxValue.ToString();
            sliderS.onValueChanged.RemoveAllListeners();
            sliderS.onValueChanged.AddListener((iInput) => OnChangeS());

            sliderV.wholeNumbers = true;
            sliderV.minValue = 0.0F;
            sliderV.maxValue = 100.0F;
            sliderV.value = sliderV.maxValue;
            textV.text = sliderV.maxValue.ToString();
            sliderV.onValueChanged.RemoveAllListeners();
            sliderV.onValueChanged.AddListener((iInput) => OnChangeV());

            sliderLowLevel.wholeNumbers = true;
            sliderLowLevel.minValue = 0.0F;
            sliderLowLevel.maxValue = 100.0F;
            sliderLowLevel.value = sliderLowLevel.minValue;
            sliderLowLevel.onValueChanged.RemoveAllListeners();
            sliderLowLevel.onValueChanged.AddListener((iInput) => OnChangeLowLevel());

            sliderOnDuration.wholeNumbers = true;
            sliderOnDuration.minValue = 0.0F;
            sliderOnDuration.maxValue = 32767F;
            sliderOnDuration.value = sliderOnDuration.minValue;
            sliderOnDuration.onValueChanged.RemoveAllListeners();
            sliderOnDuration.onValueChanged.AddListener((iInput) => OnChangeOnDuration());

            sliderOffDuration.wholeNumbers = true;
            sliderOffDuration.minValue = 0.0F;
            sliderOffDuration.maxValue = 32767F;
            sliderOffDuration.value = sliderOffDuration.minValue;
            sliderOffDuration.onValueChanged.RemoveAllListeners();
            sliderOffDuration.onValueChanged.AddListener((iInput) => OnChangeOffDuration());

            sliderUpSlope.wholeNumbers = true;
            sliderUpSlope.minValue = 0.0F;
            sliderUpSlope.maxValue = 255F;
            sliderUpSlope.value = sliderOffDuration.minValue;
            sliderUpSlope.onValueChanged.RemoveAllListeners();
            sliderUpSlope.onValueChanged.AddListener((iInput) => OnChangeUpSlope());

            sliderDownSlope.wholeNumbers = true;
            sliderDownSlope.minValue = 0.0F;
            sliderDownSlope.maxValue = 255F;
            sliderDownSlope.value = sliderDownSlope.minValue;
            sliderDownSlope.onValueChanged.RemoveAllListeners();
            sliderDownSlope.onValueChanged.AddListener((iInput) => OnChangeDownSlope());

            ValueChanged();
        }

        public void SetPattern()
        {
            switch (mDropDownSequence.options[mDropDownSequence.value].text)
            {
                case "DEFAULT":
                  
                    mPattern = LEDPulsePattern.DEFAULT;
                    break;

                case "BASIC_BLINK":
                    mPattern = LEDPulsePattern.BASIC_BLINK;
                    break;

                case "BREATHING":
                    mPattern = LEDPulsePattern.BREATHING;
                    break;

                case "DYNAMIC":
                    mPattern = LEDPulsePattern.DYNAMIC;
                    break;

                case "HEART_BEAT":
                    mPattern = LEDPulsePattern.HEART_BEAT;
                    break;

                case "LISTENING":
                    mPattern = LEDPulsePattern.LISTENING;
                    break;

                case "NOBLINK":
                    mPattern = LEDPulsePattern.NOBLINK;
                    break;

                case "PEACEFUL":
                    mPattern = LEDPulsePattern.PEACEFUL;
                    break;

                case "RECHARGE":
                    mPattern = LEDPulsePattern.RECHARGE;
                    break;
            }

        }

        private void SetColor()
        {
            switch (mDropDown.options[mDropDown.value].text)
            {
                case "HEART":
                    SetColorHeart();
                    break;

                case "LEFT_SHOULDER":
                    SetColorLeftShoulder();
                    break;

                case "RIGHT_SHOULDER":
                    SetColorRightShoulder();
                    break;

                case "ALL":
                    SetColorHeart();
                    SetColorLeftShoulder();
                    SetColorRightShoulder();
                    break;
            }

            // Update texture color
            rawImage.color = Color.HSVToRGB(sliderH.value / 360.0F, sliderS.value / 100.0F, sliderV.value / 100.0F);
        }

        private void SetColorHeart()
        {
            if (mToggle.isOn)
            {
                Buddy.Actuators.LEDs.SetHeartLight(
                    FloatToUShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value));
            }
            else
            {
                Buddy.Actuators.LEDs.SetHeartLight(
                    FloatToUShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value));
                Buddy.Actuators.LEDs.SetHeartPattern(
                    FloatToByte(sliderLowLevel.value),
                    FloatToUShort(sliderOnDuration.value),
                    FloatToUShort(sliderOffDuration.value),
                    FloatToByte(sliderUpSlope.value),
                    FloatToByte(sliderDownSlope.value));
            }
        }

        private void SetColorLeftShoulder()
        {
            if (mToggle.isOn)
            {
                Buddy.Actuators.LEDs.SetShouldersLights(
                    FloatToUShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value));
            }
            else
            {
                Buddy.Actuators.LEDs.SetShouldersLights(
                    FloatToUShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value));
                Buddy.Actuators.LEDs.SetShouldersPattern(
                    FloatToByte(sliderLowLevel.value),
                    FloatToUShort(sliderOnDuration.value),
                    FloatToUShort(sliderOffDuration.value),
                    FloatToByte(sliderUpSlope.value),
                    FloatToByte(sliderDownSlope.value));
            }
        }

        private void SetColorRightShoulder()
        {
            if (mToggle.isOn)
            {
                Buddy.Actuators.LEDs.SetShouldersLights(
                    FloatToUShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value));
            }
            else
            {
                Buddy.Actuators.LEDs.SetShouldersLights(
                    FloatToUShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value));
                Buddy.Actuators.LEDs.SetShouldersPattern(
                    FloatToByte(sliderLowLevel.value),
                    FloatToUShort(sliderOnDuration.value),
                    FloatToUShort(sliderOffDuration.value),
                    FloatToByte(sliderUpSlope.value),
                    FloatToByte(sliderDownSlope.value));
            }
        }
        
        public void ValueChanged()
        {
            SetColor();
        }

        //Waiting for CORE to do a function for flash
        private void SetFlash()
        {
            if (mStatus == false)
            {
                
                //Buddy.Actuators.LEDs.Flash = true;
                mStatus = true;
                mFlash.GetComponentsInChildren<Text>()[0].text = "TURN OFF FLASH";
                mFlash.GetComponentsInChildren<Image>()[1].sprite = mStop;
            }
            else
            {
                //Buddy.Actuators.LEDs.Flash = false;
                mStatus = false;
                mFlash.GetComponentsInChildren<Text>()[0].text = "TURN ON FLASH";
                mFlash.GetComponentsInChildren<Image>()[1].sprite = mPlay;
            }
        }

        public void LaunchSequence()
        {
            Buddy.Actuators.LEDs.SetBodyPattern(mPattern);
        }

        private void IsOnlyHSVChecked()
        {
            SetColor();
        }

        private byte FloatToByte(float iFloat)
        {
            return System.Convert.ToByte(iFloat);
        }

        private ushort FloatToUShort(float iFloat)
        {
            return System.Convert.ToUInt16(iFloat);
        }

        private short FloatToShort(float iFloat)
        {
            return System.Convert.ToInt16(iFloat);
        }

        private void OnChangeH()
        {
            textH.text = sliderH.value.ToString();
            SetColor();
        }

        private void OnChangeS()
        {
            textS.text = sliderS.value.ToString();
            SetColor();
        }

        private void OnChangeV()
        {
            textV.text = sliderV.value.ToString();
            SetColor();
        }

        private void OnChangeLowLevel()
        {
            textLowLevel.text = sliderLowLevel.value.ToString();
            SetColor();
        }

        private void OnChangeOnDuration()
        {
            textOnDuration.text = sliderOnDuration.value.ToString();
            SetColor();
        }

        private void OnChangeOffDuration()
        {
            textOffDuration.text = sliderOffDuration.value.ToString();
            SetColor();
        }

        private void OnChangeUpSlope()
        {
            textUpSlope.text = sliderUpSlope.value.ToString();
            SetColor();
        }

        private void OnChangeDownSlope()
        {
            textDownSlope.text = sliderDownSlope.value.ToString();
            SetColor();
        }
        
    }
}
