﻿using System;
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
        
        private float mH;
        private float mS;
        private float mV;
        private float mLowLevel;
        private float mOnDuration;
        private float mOffDuration;
        private float mUpSlope;
        private float mDownSlope;

        private bool mIsOnlyHSV;

        private string mLEDLocalisation;
        void Start()
        {

            mIsOnlyHSV = false;

            mLEDLocalisation = "ALL";
            sliderH.wholeNumbers = true;
            sliderH.minValue = 0;
            sliderH.maxValue = 360;

            sliderS.wholeNumbers = true;
            sliderS.minValue = 0;
            sliderS.maxValue = 100;
            sliderS.value = sliderS.maxValue;

            sliderV.wholeNumbers = true;
            sliderV.minValue = 0;
            sliderV.maxValue = 100;
            sliderV.value = sliderV.maxValue;

            sliderLowLevel.wholeNumbers = true;
            sliderLowLevel.minValue = 0;
            sliderLowLevel.maxValue = 100;

            sliderOnDuration.wholeNumbers = true;
            sliderOnDuration.minValue = 0;
            sliderOnDuration.maxValue = 5000;

            sliderOffDuration.wholeNumbers = true;
            sliderOffDuration.minValue = 0;
            sliderOffDuration.maxValue = 5000;

            sliderUpSlope.wholeNumbers = true;
            sliderUpSlope.minValue = 0;
            sliderUpSlope.maxValue = 255;

            sliderDownSlope.wholeNumbers = true;
            sliderDownSlope.minValue = 0;
            sliderDownSlope.maxValue = 255;
        }

        void Update()
        {
            //Buddy.Actuators.LEDs.Flash
            mH = sliderH.value;
            mS = sliderS.value;
            mV = sliderV.value;
            mLowLevel = sliderLowLevel.value;
            mOnDuration = sliderOnDuration.value;
            mOffDuration = sliderOffDuration.value;
            mUpSlope = sliderUpSlope.value;
            mDownSlope = sliderDownSlope.value;

            textH.text = "Hue " + mH.ToString();
            textS.text = "Sat " + mS.ToString();
            textV.text = "Val " + mV.ToString();
            textLowLevel.text = "LowLvl " + mLowLevel.ToString();
            textOnDuration.text = "OnDur " + mOnDuration.ToString();
            textOffDuration.text = "OffDur " + mOffDuration.ToString();
            textUpSlope.text = "UpSlope " + mUpSlope.ToString();
            textDownSlope.text = "DownSlope " + mDownSlope.ToString();

        }

        public void SetColor()
        {
            //A changer après avec la nouvelle méthode (il n'y a plus d'amplitude a proprement parler)
            //mLED.SetBodyLights((int)mH, (int)mS, (int)mV, mA, mF);
            
            if(mLEDLocalisation == "ALL")
            {
                if(mIsOnlyHSV)
                {
                    Buddy.Actuators.LEDs.SetHSVHeartLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV));
                    Buddy.Actuators.LEDs.SetHSVLeftShoulderLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV));
                    Buddy.Actuators.LEDs.SetHSVRightShoulderLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV));
                }
                else
                {
                    
                    Buddy.Actuators.LEDs.SetHSVHeartLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV), FloatToByte(mLowLevel), 
                        FloatToShort(mOnDuration), FloatToShort(mOffDuration), FloatToByte(mUpSlope), FloatToByte(mDownSlope));
                    Buddy.Actuators.LEDs.SetHSVLeftShoulderLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV), FloatToByte(mLowLevel),
                        FloatToShort(mOnDuration), FloatToShort(mOffDuration), FloatToByte(mUpSlope), FloatToByte(mDownSlope));
                    Buddy.Actuators.LEDs.SetHSVRightShoulderLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV), FloatToByte(mLowLevel),
                        FloatToShort(mOnDuration), FloatToShort(mOffDuration), FloatToByte(mUpSlope), FloatToByte(mDownSlope));
                }
                
            }
            else if(mLEDLocalisation == "HEARTH")
            {
                if(mIsOnlyHSV)
                    Buddy.Actuators.LEDs.SetHSVHeartLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV));
                else
                    Buddy.Actuators.LEDs.SetHSVHeartLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV), FloatToByte(mLowLevel),
                        FloatToShort(mOnDuration), FloatToShort(mOffDuration), FloatToByte(mUpSlope), FloatToByte(mDownSlope));
            }
            else if (mLEDLocalisation == "LEFT_SHOULDER")
            {
                if(mIsOnlyHSV)
                    Buddy.Actuators.LEDs.SetHSVLeftShoulderLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV));
                else
                    Buddy.Actuators.LEDs.SetHSVLeftShoulderLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV), FloatToByte(mLowLevel),
                        FloatToShort(mOnDuration), FloatToShort(mOffDuration), FloatToByte(mUpSlope), FloatToByte(mDownSlope));
            }
            else if (mLEDLocalisation == "RIGHT_SHOULDER")
            {
                if(mIsOnlyHSV)
                    Buddy.Actuators.LEDs.SetHSVRightShoulderLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV));
                else
                    Buddy.Actuators.LEDs.SetHSVRightShoulderLight(FloatToShort(mH), FloatToByte(mS), FloatToByte(mV), FloatToByte(mLowLevel),
                        FloatToShort(mOnDuration), FloatToShort(mOffDuration), FloatToByte(mUpSlope), FloatToByte(mDownSlope));
            }
        }
        
        public void UpdateTexture()
        {
            rawImage.color = Color.HSVToRGB(mH / 360F, mS / 100F, mV / 100F);
        }
        
        public void ValueChanged()
        {
            if (mDropDown.options[mDropDown.value].text =="HEARTH")
            {
                mLEDLocalisation = "HEARTH";
                Debug.Log("HEARTH");
            }
            else if (mDropDown.options[mDropDown.value].text == "LEFT_SHOULDER")
            {
                mLEDLocalisation = "LEFT_SHOULDER";
                Debug.Log("LEFT SHOULDER");
            }
            else if (mDropDown.options[mDropDown.value].text == "RIGHT_SHOULDER")
            {
                mLEDLocalisation = "RIGHT_SHOULDER";
                Debug.Log("RIGHT SHOULDER");
            }
            else if(mDropDown.options[mDropDown.value].text == "ALL")
            {
                mLEDLocalisation = "ALL";
                Debug.Log("ALL");
            }
        }

        public void IsChecked()
        {

        }

        public void SetFlash()
        {
            Buddy.Actuators.LEDs.Flash = true;
        }

        private byte FloatToByte(float iFloat)
        {
            return System.Convert.ToByte(iFloat);
        }

        private short FloatToShort(float iFloat)
        {
            return System.Convert.ToInt16(iFloat);
        }
    }
}
