using UnityEngine;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class ThermalMatrixDrawer : MonoBehaviour
    {
        private ThermalSensor mThermalSensor;

        private int mNbPixel;
        private float mTime;
        private int[] mThermalSensorDataArray;
        private List<ThermalPixel> mPixels;

        void Start()
        {
            mThermalSensor = BYOS.Instance.ThermalSensor;
            mNbPixel = mThermalSensor.Matrix.Length;
            mThermalSensorDataArray = new int[mNbPixel];

            mTime = 0F;

            foreach (ThermalPixel lPixel in mPixels)
                lPixel.Value = 0;

            if (mPixels.Count == 0)
                for (int i = 0; i < 4; ++i)
                    mPixels.AddRange(transform.GetChild(i).GetComponentsInChildren<ThermalPixel>());
        }

        void Update()
        {
            mTime += Time.deltaTime;

            // Avoid flashing
            if (mTime >= 0.2F) {
                /// get data from thermal sensor 
                mThermalSensorDataArray = mThermalSensor.Matrix;

                /// put the appripriate color to the image raw fo the scene
                for (int i = 0; i < mNbPixel; ++i) {
                    int lValue = mThermalSensorDataArray[i];
                    if (lValue != 0)
                        mPixels[i].Value = mThermalSensorDataArray[i];
                }
                mTime = 0F;
            }
        }
    }
}
