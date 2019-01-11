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

        [SerializeField]
        private Text mCPUTemperature;
        [SerializeField]
        private Text mHEADTemperature;
        [SerializeField]
        private Text mBODDYTemperature;
        [SerializeField]
        private Button mToggleFan;

        private int mNbPixel;
        private float mTimeRefresh;
        private float[] mThermalSensorDataArray;
        private ThermalCamera mThermalCamera;

        private bool mIsFanActivated;

        private void Start()
        {
            mThermalCamera = Buddy.Sensors.ThermalCamera;
            
            //64
            mNbPixel = mThermalCamera.Width * mThermalCamera.Height;
            mThermalSensorDataArray = new float[mNbPixel];
            mTimeRefresh = 0F;

            mIsFanActivated = false;
            Buddy.Actuators.Fan.Stop();
            mToggleFan.onClick.AddListener(delegate {
                OnFanButtonClick();
            });
        }

        private void Update()
        {
            // UPDATE HEAT INFORMATIONS / COMING SOON :) ...
            //mCPUTemperature.text = Buddy.Sencors + " °";
            //mHEADTemperature.text = Buddy.Sencors + " °";
            //mBODDYTemperature.text = Buddy.Sencors + " °";


            mTimeRefresh += Time.deltaTime;
            if(mTimeRefresh >= 0.2F)
            {
                //Mat lMat = mThermalCamera.Frame.Mat.clone();
                //Core.flip(lMat, lMat, 0);
                mThermalCamera.Frame.Mat.get(0, 0, mThermalSensorDataArray);
                for(int i = 0; i < mThermalSensorDataArray.Length; ++i)
                {
                    float lValuePixel = mThermalSensorDataArray[i];
                    if (lValuePixel != 0)
                        pixels[i].Value = mThermalSensorDataArray[i];
                }
                float oAverageTemp = 0;
                for (int i = 0; i < 64; ++i)
                    oAverageTemp += mThermalSensorDataArray[i];

                AverageTemperature.text = "Average Temp:" + (oAverageTemp / 64).ToString() + " °";
                AmbiantTemperature.text = mThermalCamera.AmbiantTemperature + " °";
                mTimeRefresh = 0F;
            }
        }

        private void OnFanButtonClick ()
        {
            if (mIsFanActivated)
            {
                Buddy.Actuators.Fan.Stop();
                mToggleFan.GetComponentsInChildren<Text>()[0].text = "START FAN";
            }
            else
            {
                Buddy.Actuators.Fan.Start();
                mToggleFan.GetComponentsInChildren<Text>()[0].text = "STOP FAN";
            }

            mIsFanActivated = !mIsFanActivated;
        }
    }
}
