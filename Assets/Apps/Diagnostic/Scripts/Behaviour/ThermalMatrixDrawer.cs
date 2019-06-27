﻿using UnityEngine;
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
        private Text CPUTemperature;
        [SerializeField]
        private Text HEADTemperature;
        [SerializeField]
        private Text BODYTemperature;
        [SerializeField]
        private Button ToggleFan;

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
            ToggleFan.onClick.AddListener(delegate {
                OnFanButtonClick();
            });
        }

        private void Update()
        {
            // UPDATE HEAT INFORMATIONS
            //CPUTemperature.text = Buddy.Sensors. + " °";

            mTimeRefresh += Time.deltaTime;
            if(mTimeRefresh >= DiagnosticBehaviour.REFRESH_TIMER)
            {
                BODYTemperature.text = Buddy.Sensors.IMU.Temperature + " °";
                HEADTemperature.text = Buddy.Boards.Main.Temperature + " °";
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

                AverageTemperature.text = "Average Temp:" + (oAverageTemp / 64).ToString("0.00") + " °";
                AmbiantTemperature.text = mThermalCamera.AmbiantTemperature + " °";
                mTimeRefresh = 0F;
            }
        }

        private void OnFanButtonClick ()
        {
            if (mIsFanActivated)
            {
                Buddy.Actuators.Fan.Stop();
                ToggleFan.GetComponentsInChildren<Text>()[0].text = "START FAN";
            }
            else
            {
                Buddy.Actuators.Fan.Mode = FanMode.ON;
                ToggleFan.GetComponentsInChildren<Text>()[0].text = "STOP FAN";
            }

            mIsFanActivated = !mIsFanActivated;
        }
    }
}
