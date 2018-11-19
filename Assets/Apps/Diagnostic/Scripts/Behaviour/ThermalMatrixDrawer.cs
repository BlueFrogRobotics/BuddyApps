using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using BlueQuark;
using OpenCVUnity;

namespace BuddyApp.Diagnostic
{
    public sealed class ThermalMatrixDrawer : MonoBehaviour
    {
        [SerializeField]
        private List<ThermalPixel> pixels;

        [SerializeField]
        private Text AmbiantTemperature;

        [SerializeField]
        private Text AverageTemperature;

        private int mNbPixel;
        private float mTimeRefresh;
        private float[] mThermalSensorDataArray;
        private ThermalCamera mThermalCamera;

        private void Start()
        {
            mThermalCamera = Buddy.Sensors.ThermalCamera;
            
            //64
            mNbPixel = mThermalCamera.Width * mThermalCamera.Height;
            mThermalSensorDataArray = new float[mNbPixel];
            mTimeRefresh = 0F;
        }

        private void Update()
        {
            
            mTimeRefresh += Time.deltaTime;
            if(mTimeRefresh >= 0.2F)
            {
                Mat lMat = mThermalCamera.Frame.Mat.clone();
                Core.flip(lMat, lMat, 0);
                lMat.get(0, 0, mThermalSensorDataArray);
                for(int i = 0; i < mThermalSensorDataArray.Length; ++i)
                {
                    float lValuePixel = mThermalSensorDataArray[i];
                    if (lValuePixel != 0)
                        pixels[i].Value = mThermalSensorDataArray[i];
                }
                float oAverageTemp = 0;
                for (int i = 0; i < 64; ++i)
                    oAverageTemp += mThermalSensorDataArray[i];

                AverageTemperature.text = (oAverageTemp / 64).ToString();
                AmbiantTemperature.text = mThermalCamera.AmbiantTemperature.ToString();
                mTimeRefresh = 0F;
            }
        }
    }
}
