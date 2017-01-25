using UnityEngine;
using System;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.Companion
{
    /// <summary>
    /// Detects if Buddy is being lifted
    /// </summary>
    public class AccelerometerDetector : ADetector
    {
        public bool LiftDetected { get { return mLiftDetected; } }

        private const float ACC_THRESHOLD = 6F;

        private bool mLiftDetected;
        private Queue<float> mStack;
        private TabletParameters mParameters;

        // Callback when a strong vertical acceleration is detected
        public event Action OnDetection;

        void Start()
        {
            mLiftDetected = false;
            mStack = new Queue<float>();
            mParameters = BYOS.Instance.TabletParameters;
        }

        void Update()
        {
            float lAcceleroX = mParameters.GetXAccelerometer();
            //float lAcceleroY = mParameters.GetYAccelerometer();
            //float lAcceleroZ = mParameters.GetZAccelerometer();

            //Compute an average on acceleration over time for better precision
            float lAcceleroTotal = lAcceleroX;
            mStack.Enqueue(lAcceleroTotal);

            //If too much data, dequeue it and make some computation
            if (mStack.Count > 100) {
                mStack.Dequeue();
                float lMean = 0.0f;
                //Compute the mean of values
                foreach (float lNumber in mStack)
                {
                    lMean += lNumber;
                }

                lMean /= mStack.Count;
                
                //If mean goes over our set threshold
                if (lAcceleroTotal - lMean > ACC_THRESHOLD)
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