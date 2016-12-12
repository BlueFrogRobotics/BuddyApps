using UnityEngine;
using System.Collections;
using BuddyOS;
using System;

namespace BuddyApp.HideAndSeek
{
    public class HumanDetector : MonoBehaviour
    {

        bool mIsHumanDetected = false;
        public bool IsHumanDetected { get { return mIsHumanDetected; } }

        ThermalSensor mThermal;

        public event Action OnDetection;

        private float mMinThreshold = 20f;
        private float mMaxThreshold = 80f;
        private float mThreshold = 40.0f;
        private float mTimer = 0.0f;

        void Awake()
        {
            // mThermal = BuddyOS.BuddyOperatingSystem.Instance.BuddyAPI.ThermalSensor;
        }

        // Use this for initialization
        void Start()
        {
            mThermal = BYOS.Instance.ThermalSensor;
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            if (mTimer > 0.2f)
            {
                mIsHumanDetected = CheckHuman();
                mTimer = 0.0f;
            }
            if (mIsHumanDetected)
            {
                if (OnDetection != null)
                    OnDetection();
            }
        }

        bool CheckHuman()
        {
            int lMaxTemp = 0;
            int[] lLocalThermic = mThermal.Matrix;
            if (lLocalThermic != null)
            {
                string lText = "";
                for (int i = 0; i < lLocalThermic.Length; i++)
                {
                    lText += lLocalThermic[i] + " ";
                    if (lLocalThermic[i] > lMaxTemp)
                        lMaxTemp = lLocalThermic[i];
                }
            }
            //Debug.Log("max temp: "+lMaxTemp);
            //Debug.Log("temp: " + lText);
            if (lMaxTemp > 25 && lMaxTemp < 30)
                return true;
            else
                return false;
        }

        public float GetMinThreshold()
        {
            return mMinThreshold;
        }

        public float GetMaxThreshold()
        {
            return mMaxThreshold;
        }

        public float GetThreshold()
        {
            return mThreshold;
        }

        public void SetThreshold(float iThreshold)
        {
            if (iThreshold < mMinThreshold)
                mThreshold = mMinThreshold;
            else if (iThreshold > mMaxThreshold)
                mThreshold = mMaxThreshold;
            else
                mThreshold = iThreshold;
        }
    }
}
