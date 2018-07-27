using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public class LEDWindow : MonoBehaviour
    {
        [SerializeField]
        private Text textH;

        [SerializeField]
        private Text textS;

        [SerializeField]
        private Text textV;

        [SerializeField]
        private Text textF;

        [SerializeField]
        private Text textA;

        [SerializeField]
        private Slider sliderH;

        [SerializeField]
        private Slider sliderS;

        [SerializeField]
        private Slider sliderV;

        [SerializeField]
        private Slider sliderF;

        [SerializeField]
        private Slider sliderA;

        [SerializeField]
        private RawImage rawImage;

        private LEDs mLED;

        private float mH;
        private float mS;
        private float mV;
        private float mF;
        private float mA;

        void Start()
        {
            mLED = Buddy.Actuators.LEDs;

            sliderH.wholeNumbers = true;
            sliderH.minValue = 0;
            sliderH.maxValue = 360;

            sliderS.wholeNumbers = true;
            sliderS.minValue = 0;
            sliderS.maxValue = 100;

            sliderV.wholeNumbers = true;
            sliderV.minValue = 0;
            sliderV.maxValue = 100;

            sliderF.wholeNumbers = false;
            sliderF.minValue = 0;
            sliderF.maxValue = 2;

            sliderA.wholeNumbers = false;
            sliderA.minValue = 0;
            sliderA.maxValue = 2;
        }

        void Update()
        {
            mH = sliderH.value;
            mS = sliderS.value;
            mV = sliderV.value;
            mF = sliderF.value;
            mA = sliderA.value;

            textH.text = "Hue " + mH.ToString();
            textS.text = "Sat " + mS.ToString();
            textV.text = "Val " + mV.ToString();
            textF.text = "Frq " + mF.ToString();
            textA.text = "Amp " + mA.ToString();
        }

        public void SetColor()
        {
            //A changer après avec la nouvelle méthode (il n'y a plus d'amplitude a proprement parler)
            //mLED.SetBodyLights((int)mH, (int)mS, (int)mV, mA, mF);
            //rawImage.color = Color.HSVToRGB(mH / 360F, mS / 100F, mV / 100F);
        }
    }
}
