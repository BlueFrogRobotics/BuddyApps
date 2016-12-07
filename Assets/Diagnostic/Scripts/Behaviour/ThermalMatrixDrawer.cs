using UnityEngine;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class ThermalMatrixDrawer : MonoBehaviour
    {
        private ThermalSensor mThermalSensor;

        [SerializeField]
        private List<ThermalPixel> pixels;

        private int mNbPixel;
        private float mTime;
        private int[] mThermalSensorDataArray;

        void Start()
        {
            mThermalSensor = BYOS.Instance.ThermalSensor;
            mNbPixel = mThermalSensor.Matrix.Length;
            mThermalSensorDataArray = new int[mNbPixel];

            mTime = 0F;

        }

        void Update()
        {
            mTime += Time.deltaTime;

            // Avoid flashing
            if (mTime >= 0.2F) {
                // get data from thermal sensor 
                mThermalSensorDataArray = mThermalSensor.Matrix;

                // put the appropriate color to the image raw fo the scene
                for (int i = 0; i < mNbPixel; ++i) {
                    int lValue = mThermalSensorDataArray[i];
                    if (lValue != 0)
                        pixels[i].Value = mThermalSensorDataArray[i];
                }
                mTime = 0F;
            }
        }
    }
}
