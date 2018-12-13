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
        private Sprite mStop = Buddy.Resources.Get<Sprite>("os_icon_stop");
        private Sprite mPlay = Buddy.Resources.Get<Sprite>("os_icon_play");

        public void OnEnable()
        {
            mDropDown.onValueChanged.RemoveAllListeners();
            mDropDown.onValueChanged.AddListener((iInput) => SetColor());

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
            sliderS.onValueChanged.RemoveAllListeners();
            sliderS.onValueChanged.AddListener((iInput) => OnChangeS());

            sliderV.wholeNumbers = true;
            sliderV.minValue = 0.0F;
            sliderV.maxValue = 100.0F;
            sliderV.value = sliderV.maxValue;
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
            sliderOnDuration.maxValue = 5000.0F;
            sliderOnDuration.value = sliderOnDuration.minValue;
            sliderOnDuration.onValueChanged.RemoveAllListeners();
            sliderOnDuration.onValueChanged.AddListener((iInput) => OnChangeOnDuration());

            sliderOffDuration.wholeNumbers = true;
            sliderOffDuration.minValue = 0.0F;
            sliderOffDuration.maxValue = 5000.0F;
            sliderOffDuration.value = sliderOffDuration.minValue;
            sliderOffDuration.onValueChanged.RemoveAllListeners();
            sliderOffDuration.onValueChanged.AddListener((iInput) => OnChangeOffDuration());

            sliderUpSlope.wholeNumbers = true;
            sliderUpSlope.minValue = 0.0F;
            sliderUpSlope.maxValue = 5000.0F;
            sliderUpSlope.value = sliderOffDuration.minValue;
            sliderUpSlope.onValueChanged.RemoveAllListeners();
            sliderUpSlope.onValueChanged.AddListener((iInput) => OnChangeUpSlope());

            sliderDownSlope.wholeNumbers = true;
            sliderDownSlope.minValue = 0.0F;
            sliderDownSlope.maxValue = 5000.0F;
            sliderDownSlope.value = sliderDownSlope.minValue;
            sliderDownSlope.onValueChanged.RemoveAllListeners();
            sliderDownSlope.onValueChanged.AddListener((iInput) => OnChangeDownSlope());

            SetColor();
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
                Buddy.Actuators.LEDs.SetHSVHeartLight(
                    FloatToShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value));
            }
            else
            {
                Buddy.Actuators.LEDs.SetHSVHeartLight(
                    FloatToShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value),
                    FloatToByte(sliderLowLevel.value),
                    FloatToShort(sliderOnDuration.value),
                    FloatToShort(sliderOffDuration.value),
                    FloatToByte(sliderUpSlope.value),
                    FloatToByte(sliderDownSlope.value));
            }
        }

        private void SetColorLeftShoulder()
        {
            if (mToggle.isOn)
            {
                Buddy.Actuators.LEDs.SetHSVLeftShoulderLight(
                    FloatToShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value));
            }
            else
            {
                Buddy.Actuators.LEDs.SetHSVLeftShoulderLight(
                    FloatToShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value),
                    FloatToByte(sliderLowLevel.value),
                    FloatToShort(sliderOnDuration.value),
                    FloatToShort(sliderOffDuration.value),
                    FloatToByte(sliderUpSlope.value),
                    FloatToByte(sliderDownSlope.value));
            }
        }

        private void SetColorRightShoulder()
        {
            if (mToggle.isOn)
            {
                Buddy.Actuators.LEDs.SetHSVRightShoulderLight(
                    FloatToShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value));
            }
            else
            {
                Buddy.Actuators.LEDs.SetHSVRightShoulderLight(
                    FloatToShort(sliderH.value),
                    FloatToByte(sliderS.value),
                    FloatToByte(sliderV.value),
                    FloatToByte(sliderLowLevel.value),
                    FloatToShort(sliderOnDuration.value),
                    FloatToShort(sliderOffDuration.value),
                    FloatToByte(sliderUpSlope.value),
                    FloatToByte(sliderDownSlope.value));
            }
        }
        
        private void ValueChanged()
        {
            SetColor();
        }

        private void SetFlash()
        {
            if (mStatus == false)
            {
                Buddy.Actuators.LEDs.Flash = true;
                mStatus = true;
                mFlash.GetComponentsInChildren<Text>()[0].text = "TURN OFF FLASH";
                mFlash.GetComponentsInChildren<Image>()[1].sprite = mStop;
            }
            else
            {
                Buddy.Actuators.LEDs.Flash = false;
                mStatus = false;
                mFlash.GetComponentsInChildren<Text>()[0].text = "TURN ON FLASH";
                mFlash.GetComponentsInChildren<Image>()[1].sprite = mPlay;
            }
        }

        private void IsOnlyHSVChecked()
        {
            SetColor();
        }

        private byte FloatToByte(float iFloat)
        {
            return System.Convert.ToByte(iFloat);
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
