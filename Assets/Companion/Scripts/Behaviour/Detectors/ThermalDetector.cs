using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    public class ThermalDetector : MonoBehaviour
    {
        public bool ThermalDetected { get { return mThermalDetected; } }

        private bool mThermalDetected;
        private ThermalSensor mSensor;

        void Start()
        {
            mSensor = BYOS.Instance.ThermalSensor;
        }
        
        void Update()
        {
            int lActivePixel = 0;
            int[] lMatrix = mSensor.Matrix;

            for(int i=0; i< lMatrix.Length; i++) {
                if (lMatrix[i] > 27)
                    lActivePixel++;
            }

            if (lActivePixel > 4)
                mThermalDetected = true;
            else
                mThermalDetected = false;
        }
    }
}