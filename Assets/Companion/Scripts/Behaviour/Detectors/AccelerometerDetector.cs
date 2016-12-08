using UnityEngine;
using System;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.Companion
{
    public class AccelerometerDetector : ADetector
    {
        public bool LiftDetected { get { return mLiftDetected; } }

        private float mThreshold;
        private bool mLiftDetected;
        private Queue<float> mStack;
        private TabletParameters mParameters;

        public event Action OnDetection;

        void Start()
        {
            mLiftDetected = false;
            mThreshold = 5F;
            mStack = new Queue<float>();
            mParameters = BYOS.Instance.TabletParameters;
        }

        void Update()
        {
            float lAcceleroX = mParameters.GetXAccelerometer();
            //float lAcceleroY = mParameters.GetYAccelerometer();
            //float lAcceleroZ = mParameters.GetZAccelerometer();
            float lAcceleroTotal = lAcceleroX;// + lAcceleroY + lAcceleroZ;
            mStack.Enqueue(lAcceleroTotal);

            if (mStack.Count > 100) {
                mStack.Dequeue();
                float lMean = 0.0f;
                foreach (float lNumber in mStack)
                {
                    lMean += lNumber;
                }

                lMean /= mStack.Count;

                //Debug.Log("AcceleroZ mean : " + lMean);
                if (Mathf.Abs(lAcceleroTotal - lMean) > mThreshold)
                {
                    mLiftDetected = true;
                    if (OnDetection != null)
                        OnDetection();
                }
                else
                    mLiftDetected = false;
            }
        }
    }
}