using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BuddyApp.Diagnostic
{
    public class ThermalPixel : MonoBehaviour
    {
        private Image mImage;
        private Text mText;
        private float mDegrees;

        public float MinExpected { get; set; }
        public float MaxExpected { get; set; }
        public string Unite { get; set; }
        public string Form { get; set; }

        public ThermalPixel()
        {
            MinExpected = 20; 
            MaxExpected = 40; 
            Unite = "°";
            Form = "0.0";
        }

        // Use this for initialization
        void Awake()
        {
            mImage = GetComponentInChildren<Image>();
            mText = GetComponentInChildren<Text>();
        }

        public float Value
        {
            get
            {
                return mDegrees;
            }
            set
            {
                mDegrees = value;
                float lPurcent = DegreesToPurcent(mDegrees);
                mImage.color = new Color(lPurcent, 0, (1f - lPurcent), 0.85f);
                mText.text = mDegrees.ToString(Form) + Unite;
            }
        }

        public float DegreesToPurcent(float iValue)
        {
            float oV = (iValue - MinExpected) / (MaxExpected - MinExpected);
            if (oV > 1F) oV = 1F;
            if (oV < 0F) oV = 0F;
            return oV;
        }
    }
}
