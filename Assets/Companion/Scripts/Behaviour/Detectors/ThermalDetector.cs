using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    public class ThermalDetector : MonoBehaviour
    {
        public bool ThermalDetected { get { return mThermalDetected; } }

        private const int THERMAL_THRESH = 22;

        private bool mThermalDetected;
        private ThermalSensor mSensor;

        void Start()
        {
            mSensor = BYOS.Instance.ThermalSensor;
        }
        
        void Update()
        {
            int[] lMatrix = mSensor.Matrix;
            int lMaxVal = 0;
            float lMean = 0.0F;

            for(int i=0; i< lMatrix.Length; i++) {
                lMean += lMatrix[i];
                if (lMatrix[i] > lMaxVal)
                    lMaxVal = lMatrix[i];
            }
            lMean /= lMatrix.Length;

            if (lMean > THERMAL_THRESH && lMaxVal > 24)
                mThermalDetected = true;
            else
                mThermalDetected = false;
        }
    }
}