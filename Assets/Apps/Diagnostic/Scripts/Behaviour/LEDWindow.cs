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
        private DiagnosticBehaviour mDiagBehaviour = new DiagnosticBehaviour();

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
            sliderLowLevel.value = 1F;
            textLowLevel.text = sliderLowLevel.value.ToString();
            sliderLowLevel.onValueChanged.RemoveAllListeners();
            sliderLowLevel.onValueChanged.AddListener((iInput) => OnChangeLowLevel());

            sliderOnDuration.wholeNumbers = true;
            sliderOnDuration.minValue = 0.0F;
            sliderOnDuration.maxValue = 10000F;
            sliderOnDuration.value = 10F;
            textOnDuration.text = sliderOnDuration.value.ToString();
            sliderOnDuration.onValueChanged.RemoveAllListeners();
            sliderOnDuration.onValueChanged.AddListener((iInput) => OnChangeOnDuration(iInput));

            sliderOffDuration.wholeNumbers = true;
            sliderOffDuration.minValue = 0.0F;
            sliderOffDuration.maxValue = 10000F;
            sliderOffDuration.value = 300;
            textOffDuration.text = sliderOffDuration.value.ToString();
            sliderOffDuration.onValueChanged.RemoveAllListeners();
            sliderOffDuration.onValueChanged.AddListener((iInput) => OnChangeOffDuration(iInput));

            sliderUpSlope.wholeNumbers = true;
            sliderUpSlope.minValue = 0.0F;
            sliderUpSlope.maxValue = 100F;
            sliderUpSlope.value = 3F;
            textUpSlope.text = sliderUpSlope.value.ToString();
            sliderUpSlope.onValueChanged.RemoveAllListeners();
            sliderUpSlope.onValueChanged.AddListener((iInput) => OnChangeUpSlope(iInput));

            sliderDownSlope.wholeNumbers = true;
            sliderDownSlope.minValue = 0.0F;
            sliderDownSlope.maxValue = 100F;
            sliderDownSlope.value = 3F;
            textDownSlope.text = sliderDownSlope.value.ToString();
            sliderDownSlope.onValueChanged.RemoveAllListeners();
            sliderDownSlope.onValueChanged.AddListener((iInput) => OnChangeDownSlope(iInput));

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

                case "SHOULDER":
                    SetColorShoulder();
                    break;

                case "BOTH":
                    SetColorHeart();
                    SetColorShoulder();
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

        private void SetColorShoulder()
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

        //private void SetColorRightShoulder()
        //{
        //    if (mToggle.isOn)
        //    {
        //        Buddy.Actuators.LEDs.SetShouldersLights(
        //            FloatToUShort(sliderH.value),
        //            FloatToByte(sliderS.value),
        //            FloatToByte(sliderV.value));
        //    }
        //    else
        //    {
        //        Buddy.Actuators.LEDs.SetShouldersLights(
        //            FloatToUShort(sliderH.value),
        //            FloatToByte(sliderS.value),
        //            FloatToByte(sliderV.value));
        //        Buddy.Actuators.LEDs.SetShouldersPattern(
        //            FloatToByte(sliderLowLevel.value),
        //            FloatToUShort(sliderOnDuration.value),
        //            FloatToUShort(sliderOffDuration.value),
        //            FloatToByte(sliderUpSlope.value),
        //            FloatToByte(sliderDownSlope.value));
        //    }
        //}
        
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

        private double FloatToDouble(float iFloat)
        {
            return System.Convert.ToDouble(iFloat);
        }

        private float DoubleToFloat(double iDouble)
        {
            return (float)iDouble;
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

        private void OnChangeOnDuration(float iInput)
        {
            sliderOnDuration.value = iInput;
            //Debug.Log("IInput changed onduration slider duration : " + FloatToDouble(iInput / 32767f) + " input : " + iInput.ToString() + " test : " + sliderOnDuration.value.ToString());
            textOnDuration.text = mDiagBehaviour.ExpScale(FloatToDouble(iInput / 10000f) , 1000d, 10000d).ToString("0"); 
            SetColor();
        }

        private void OnChangeOffDuration(float iInput)
        {
            sliderOffDuration.value = iInput;
            textOffDuration.text = mDiagBehaviour.ExpScale(FloatToDouble(iInput / 10000f) , 1000d, 10000d).ToString("0");
            SetColor();
        }

        private void OnChangeUpSlope(float iInput)
        {
            sliderUpSlope.value = iInput;
            textUpSlope.text = mDiagBehaviour.ExpScale(FloatToDouble(iInput / 100f) , 25d, 100d).ToString("0");
            SetColor();
        }

        private void OnChangeDownSlope(float iInput)
        {
            sliderDownSlope.value = iInput;
            textDownSlope.text = mDiagBehaviour.ExpScale(FloatToDouble(iInput / 100f) , 25d, 100d).ToString("0"); ;
            SetColor();
        }
        
    }
}
