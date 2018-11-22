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
        
        private string mLEDLocalisation;

        void OnEnable()
        {
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
            sliderUpSlope.maxValue = 255.0F;
            sliderUpSlope.value = sliderOffDuration.minValue;
            sliderUpSlope.onValueChanged.RemoveAllListeners();
            sliderUpSlope.onValueChanged.AddListener((iInput) => OnChangeUpSlope());

            sliderDownSlope.wholeNumbers = true;
            sliderDownSlope.minValue = 0.0F;
            sliderDownSlope.maxValue = 255.0F;
            sliderDownSlope.value = sliderDownSlope.minValue;
            sliderDownSlope.onValueChanged.RemoveAllListeners();
            sliderDownSlope.onValueChanged.AddListener((iInput) => OnChangeDownSlope());

            UpdateTexture();
            SetColor();
        }

        public void SetColor()
        {
            //A changer après avec la nouvelle méthode (il n'y a plus d'amplitude a proprement parler)
            //mLED.SetBodyLights((int)mH, (int)mS, (int)mV, mA, mF);

            switch (mDropDown.options[mDropDown.value].text)
            {
                case "HEARTH":
                    if (mToggle.isOn)
                        Buddy.Actuators.LEDs.SetHSVHeartLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value));
                    else
                        Buddy.Actuators.LEDs.SetHSVHeartLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value), FloatToByte(sliderLowLevel.value),
                            FloatToShort(sliderOnDuration.value), FloatToShort(sliderOffDuration.value), FloatToByte(sliderUpSlope.value), FloatToByte(sliderDownSlope.value));
                    break;

                case "LEFT_SHOULDER":
                    if (mToggle.isOn)
                        Buddy.Actuators.LEDs.SetHSVLeftShoulderLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value));
                    else
                        Buddy.Actuators.LEDs.SetHSVLeftShoulderLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value), FloatToByte(sliderLowLevel.value),
                            FloatToShort(sliderOnDuration.value), FloatToShort(sliderOffDuration.value), FloatToByte(sliderUpSlope.value), FloatToByte(sliderDownSlope.value));
                    break;

                case "RIGHT_SHOULDER":
                    if (mToggle.isOn)
                        Buddy.Actuators.LEDs.SetHSVRightShoulderLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value));
                    else
                        Buddy.Actuators.LEDs.SetHSVRightShoulderLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value), FloatToByte(sliderLowLevel.value),
                            FloatToShort(sliderOnDuration.value), FloatToShort(sliderOffDuration.value), FloatToByte(sliderUpSlope.value), FloatToByte(sliderDownSlope.value));
                    break;

                case "ALL":
                    if (mToggle.isOn) {
                        Buddy.Actuators.LEDs.SetHSVHeartLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value));
                        Buddy.Actuators.LEDs.SetHSVLeftShoulderLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value));
                        Buddy.Actuators.LEDs.SetHSVRightShoulderLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value));
                    } else {

                        Buddy.Actuators.LEDs.SetHSVHeartLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value), FloatToByte(sliderLowLevel.value),
                            FloatToShort(sliderOnDuration.value), FloatToShort(sliderOffDuration.value), FloatToByte(sliderUpSlope.value), FloatToByte(sliderDownSlope.value));
                        Buddy.Actuators.LEDs.SetHSVLeftShoulderLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value), FloatToByte(sliderLowLevel.value),
                            FloatToShort(sliderOnDuration.value), FloatToShort(sliderOffDuration.value), FloatToByte(sliderUpSlope.value), FloatToByte(sliderDownSlope.value));
                        Buddy.Actuators.LEDs.SetHSVRightShoulderLight(FloatToShort(sliderH.value), FloatToByte(sliderS.value), FloatToByte(sliderV.value), FloatToByte(sliderLowLevel.value),
                            FloatToShort(sliderOnDuration.value), FloatToShort(sliderOffDuration.value), FloatToByte(sliderUpSlope.value), FloatToByte(sliderDownSlope.value));
                    }
                    break;
            }
        }
        
        public void UpdateTexture()
        {
            rawImage.color = Color.HSVToRGB(sliderH.value / 360.0F, sliderS.value / 100.0F, sliderV.value / 100.0F);
        }
        
        public void ValueChanged()
        {
            UpdateTexture();
            SetColor();
        }

        public void SetFlash()
        {
            Buddy.Actuators.LEDs.Flash = true;
        }

        public void IsOnlyHSVChecked()
        {
            UpdateTexture();
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
            textH.text = "Hue " + sliderH.value.ToString();
            UpdateTexture();
            SetColor();
        }

        private void OnChangeS()
        {
            textS.text = "Sat " + sliderS.value.ToString();
            UpdateTexture();
            SetColor();
        }

        private void OnChangeV()
        {
            textV.text = "Val " + sliderV.value.ToString();
            UpdateTexture();
            SetColor();
        }

        private void OnChangeLowLevel()
        {
            textLowLevel.text = "LowLvl " + sliderLowLevel.value.ToString();
            UpdateTexture();
            SetColor();
        }

        private void OnChangeOnDuration()
        {
            textOnDuration.text = "OnDur " + sliderOnDuration.value.ToString();
            UpdateTexture();
            SetColor();
        }

        private void OnChangeOffDuration()
        {
            textOffDuration.text = "OffDur " + sliderOffDuration.value.ToString();
            UpdateTexture();
            SetColor();
        }

        private void OnChangeUpSlope()
        {
            textUpSlope.text = "UpSlope " + sliderUpSlope.value.ToString();
            UpdateTexture();
            SetColor();
        }

        private void OnChangeDownSlope()
        {
            textDownSlope.text = "DownSlope " + sliderDownSlope.value.ToString();
            UpdateTexture();
            SetColor();
        }
        
    }
}
