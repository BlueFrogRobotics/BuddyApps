using UnityEngine;
using System.Collections.Generic;
using BuddyOS;

namespace BuddySample.Basic
{
    public class ThermalMatrixDrawer : MonoBehaviour
    {
        private ThermalSensor mThermalSensor;

        private int[] mThermalSensorDataArray;

        public List<ThermalPixel> mPixels;

        private int mSizeOfThermalSensorDataArray;

        private float mTime;

        void Start()
        {
            mThermalSensor = BYOS.Instance.ThermalSensor;
            mSizeOfThermalSensorDataArray = mThermalSensor.Matrix.Length;
            mThermalSensorDataArray = new int[mSizeOfThermalSensorDataArray];

            mTime = 0;
            foreach (ThermalPixel tp in mPixels) tp.Value = 0f;

            if (mPixels.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Transform lRow = transform.GetChild(i);

                    foreach (ThermalPixel tp in lRow.GetComponentsInChildren<ThermalPixel>())
                    {
                        mPixels.Add(tp);
                    }
                }
            }
        }

        void Update()
        {
            mTime += Time.deltaTime;

            //sans ça, l'affichage clignotte 
            if (mTime >= 0.2f)
            {
                /// get data from thermal sensor 
                mThermalSensorDataArray = mThermalSensor.Matrix;

                /// put the appripriate color to the image raw fo the scene
                for (int i = 0; i < mSizeOfThermalSensorDataArray; i++)
                {
                    int lValue = mThermalSensorDataArray[i];
                    if (lValue != 0)
                        mPixels[i].Value = mThermalSensorDataArray[i];
                }
                mTime = 0;
            }

        }
    } 
}
