using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BuddySample.Basic
{
    public class ThermalPixel : MonoBehaviour
    {
        private Image mImage;
        private Text mText;

        private float mDegrees;

        public float mMinExpected { get; set; }
        public float mMaxExpected { get; set; }
        public string mUnite { get; set; }
        public string mForm { get; set; }

        public ThermalPixel()
        {
            mMinExpected = 20; 
            mMaxExpected = 40; 
            mUnite = "°";
            mForm = "0.0";
        }

        // Use this for initialization
        void Awake()
        {
            mImage = this.GetComponentInChildren<Image>();
            mText = this.GetComponentInChildren<Text>();
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
                mText.text = mDegrees.ToString(mForm) + mUnite;
            }
        }

        public float DegreesToPurcent(float iValue)
        {
            float lV = (iValue - mMinExpected) / (mMaxExpected - mMinExpected);
            if (lV > 1f) lV = 1f;
            if (lV < 0f) lV = 0f;
            return lV;
        }
    }
}
