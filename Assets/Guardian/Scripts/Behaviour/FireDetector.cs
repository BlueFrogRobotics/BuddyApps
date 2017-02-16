using UnityEngine;
using System.Collections;
using BuddyOS;
using System;
using Ionic.Zip;
using System.IO;

namespace BuddyApp.Guardian
{
    public class FireDetector : MonoBehaviour, IDetector
    {

        public bool IsFireDetected { get { return mIsFireDetected; } }

        private ThermalSensor mThermal;
        private bool mIsFireDetected = false;

        public event Action OnDetection;

        public ShowTemperature mShowTemp;

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
            //     using (ZipFile zip = new ZipFile())
            //{
            //    // add this map file into the "images" directory in the zip archive
            //    string filepath = Path.Combine(Application.persistentDataPath, "livetest.avi");
            //    zip.AddFile(filepath, "images");
            //    // add the report into a different directory in the archive
            //    //zip.AddFile("c:\\Reports\\2008-Regional-Sales-Report.pdf", "files");
            //    //zip.AddFile("ReadMe.txt");
            //    zip.Save(Path.Combine(Application.persistentDataPath, "testcomp.zip"));
            //}


        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            if (mTimer > 0.2f)
            {
                mIsFireDetected = CheckFire();
                mTimer = 0.0f;
            }
            if (mIsFireDetected)
            {
                if (OnDetection != null)
                    OnDetection();
            }
        }

        bool CheckFire()
        {
            int lMaxTemp = 0;
            int[] lLocalThermic = mThermal.Matrix;
            if (lLocalThermic != null)
            {
                mShowTemp.FillTemperature(lLocalThermic);
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
            if (lMaxTemp > mThreshold && lMaxTemp < 100)
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
