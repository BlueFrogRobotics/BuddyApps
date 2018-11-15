using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public sealed class SensorsWindow : MonoBehaviour
    {
        [SerializeField]
        private Dropdown mDropDown;

        [SerializeField]
        private GameObject FirstPart;

        [SerializeField]
        private GameObject SecondPart;

        [SerializeField]
        private GameObject ThirdPart;

        [SerializeField]
        private GameObject FourthPart;

        //US SENSORS
        [SerializeField]
        private Text LeftUSError;
        [SerializeField]
        private Text LeftUSValue;

        [SerializeField]
        private Text RightUSError;
        [SerializeField]
        private Text RightUSValue;


        //TOF SENSORS
        [SerializeField]
        private Text ForeHeadTOFError;
        [SerializeField]
        private Text ForeHeadTOFValue;

        [SerializeField]
        private Text ChinTOFError;
        [SerializeField]
        private Text ChinTOFValue;

        [SerializeField]
        private Text FrontRightTOFError;
        [SerializeField]
        private Text FrontRightTOFValue;

        [SerializeField]
        private Text FrontMiddleTOFError;
        [SerializeField]
        private Text FrontMiddleTOFValue;

        [SerializeField]
        private Text FrontLeftTOFError;
        [SerializeField]
        private Text FrontLeftTOFValue;

        [SerializeField]
        private Text BackTOFError;
        [SerializeField]
        private Text BackTOFValue;

        //LED IR RECEPTRICE
        [SerializeField]
        private Text BackIRError;
        [SerializeField]
        private Text BackIRValue;

        //CLIFF SENSORS
        [SerializeField]
        private Text Cliff_1Error;
        [SerializeField]
        private Text Cliff_1Value;
        [SerializeField]
        private Text Cliff_1EmergencyStop;

        [SerializeField]
        private Text Cliff_2Error;
        [SerializeField]
        private Text Cliff_2Value;
        [SerializeField]
        private Text Cliff_2EmergencyStop;

        [SerializeField]
        private Text Cliff_3Error;
        [SerializeField]
        private Text Cliff_3Value;
        [SerializeField]
        private Text Cliff_3EmergencyStop;

        [SerializeField]
        private Text Cliff_4Error;
        [SerializeField]
        private Text Cliff_4Value;
        [SerializeField]
        private Text Cliff_4EmergencyStop;

        [SerializeField]
        private Text Cliff_5Error;
        [SerializeField]
        private Text Cliff_5Value;
        [SerializeField]
        private Text Cliff_5EmergencyStop;

        [SerializeField]
        private Text Cliff_6Error;
        [SerializeField]
        private Text Cliff_6Value;
        [SerializeField]
        private Text Cliff_6EmergencyStop;

        [SerializeField]
        private Text Cliff_7Error;
        [SerializeField]
        private Text Cliff_7Value;
        [SerializeField]
        private Text Cliff_7EmergencyStop;

        //IMU
        [SerializeField]
        private Text Gyro_X;
        [SerializeField]
        private Text Gyro_Y;
        [SerializeField]
        private Text Gyro_Z;

        [SerializeField]
        private Text Acc_X;
        [SerializeField]
        private Text Acc_Y;
        [SerializeField]
        private Text Acc_Z;

        [SerializeField]
        private Text IMU_Error;

        //PINCH / CARESS
        [SerializeField]
        private Text HeartValue;
        [SerializeField]
        private Text HeartError;
        [SerializeField]
        private Text LeftShoulderValue;
        [SerializeField]
        private Text LeftShoulderError;
        [SerializeField]
        private Text RightShoulderValue;
        [SerializeField]
        private Text RightShoulderError;

        [SerializeField]
        private Text BackHeadValue;
        [SerializeField]
        private Text BackHeadError;
        [SerializeField]
        private Text LeftHeadValue;
        [SerializeField]
        private Text LeftHeadError;
        [SerializeField]
        private Text RightHeadValue;
        [SerializeField]
        private Text RightHeadError;


        private UltrasonicSensor mLeftUSSensor;
        private UltrasonicSensor mRightUSSensor;

        private TimeOfFlightSensor mForeHeadTOFSensor;
        private TimeOfFlightSensor mChinTOFSensor;
        private TimeOfFlightSensor mFrontRightTOFSensor;
        private TimeOfFlightSensor mFrontMiddleTOFSensor;
        private TimeOfFlightSensor mFrontLeftTOFSensor;
        private TimeOfFlightSensor mBackTOFSensor;   
        
        private InfraredSensor mBackIRSensor;

        private CliffSensor mCliff_1;
        private CliffSensor mCliff_2;
        private CliffSensor mCliff_3;
        private CliffSensor mCliff_4;
        private CliffSensor mCliff_5;
        private CliffSensor mCliff_6;
        private CliffSensor mCliff_7;

        private IMU mIMU;

        private TouchSensors mTouchSensor;

        private List<GameObject> mAllGO;
        
        void Start()
        {
            
            mAllGO = new List<GameObject>();
            mAllGO.Add(FirstPart);
            mAllGO.Add(SecondPart);
            mAllGO.Add(ThirdPart);
            mAllGO.Add(FourthPart);

            foreach (GameObject go in mAllGO)
                go.SetActive(false);
            mAllGO[0].SetActive(true);

            //US / IR / TOF
            mLeftUSSensor = Buddy.Sensors.UltrasonicSensors.Left;
            mRightUSSensor = Buddy.Sensors.UltrasonicSensors.Right;

            mForeHeadTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Forehead;
            mChinTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Chin;
            mFrontRightTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Right;
            mFrontMiddleTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Front;
            mFrontLeftTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Left;
            mBackTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Back;

            mBackIRSensor = Buddy.Sensors.InfraredSensor;

            //Cliff Sensors
            mCliff_1 = Buddy.Sensors.CliffSensors.FrontLeftWheel;
            mCliff_2 = Buddy.Sensors.CliffSensors.BackLeftWheel;
            mCliff_3 = Buddy.Sensors.CliffSensors.BackLeftFreeWheel;
            mCliff_4 = Buddy.Sensors.CliffSensors.FrontFreeWheel;
            mCliff_5 = Buddy.Sensors.CliffSensors.BackRightFreeWheel;
            mCliff_6 = Buddy.Sensors.CliffSensors.BackRightWheel;
            mCliff_7 = Buddy.Sensors.CliffSensors.FrontRightWheel;

            //IMU
            mIMU = Buddy.Sensors.IMU;

            //TouchSensor
            mTouchSensor = Buddy.Sensors.TouchSensors;
            
        }

        void Update()
        {
            if(mDropDown.options[mDropDown.value].text == "TOF/IR/US")
            {
                //US
                LeftUSError.text = "" + mLeftUSSensor.Error;
                LeftUSValue.text = "" + mLeftUSSensor.Value;

                RightUSError.text = "" + mRightUSSensor.Error;
                RightUSValue.text = "" + mRightUSSensor.Value;


                //TOF
                ForeHeadTOFError.text = "" + mForeHeadTOFSensor.Error;
                ForeHeadTOFValue.text = "" + mForeHeadTOFSensor.Value;

                ChinTOFError.text = "" + mChinTOFSensor.Error;
                ChinTOFValue.text = "" + mChinTOFSensor.Value;

                FrontLeftTOFError.text = "" + mFrontLeftTOFSensor.Error;
                FrontLeftTOFValue.text = "" + mFrontLeftTOFSensor.Value;

                FrontMiddleTOFError.text = "" + mFrontMiddleTOFSensor.Error;
                FrontMiddleTOFValue.text = "" + mFrontMiddleTOFSensor.Value;

                FrontRightTOFError.text = "" + mFrontRightTOFSensor.Error;
                FrontRightTOFValue.text = "" + mFrontRightTOFSensor.Value;

                BackTOFError.text = "" + mBackTOFSensor.Error;
                BackTOFValue.text = "" + mBackTOFSensor.Value;

                //IR RECEPTEUR
                //BackIRError.text = "" + mBackIRSensor.Error;
                BackIRValue.text = "" + mBackIRSensor.Value;
            }
            else if (mDropDown.options[mDropDown.value].text == "CARESS/PINCH")
            {
                //CARESS
                BackHeadValue.text = "" + mTouchSensor.BackHead.Value;
                BackHeadError.text = "" + mTouchSensor.BackHead.Error;

                LeftHeadValue.text = "" + mTouchSensor.LeftHead.Value;
                LeftHeadError.text = "" + mTouchSensor.LeftHead.Value;

                RightHeadValue.text = "" + mTouchSensor.RightHead.Value;
                RightHeadError.text = "" + mTouchSensor.RightHead.Value;

                //PINCH
                HeartValue.text = "" + mTouchSensor.Heart.Value;
                HeartError.text = "" + mTouchSensor.Heart.Error;

                LeftShoulderValue.text = "" + mTouchSensor.LeftShoulder.Value;
                LeftShoulderError.text = "" + mTouchSensor.LeftShoulder.Error;

                RightShoulderValue.text = "" + mTouchSensor.RightShoulder.Value;
                RightShoulderError.text = "" + mTouchSensor.RightShoulder.Error;
            }
            else if (mDropDown.options[mDropDown.value].text == "CLIFF")
            {
                Cliff_1Value.text = "" + mCliff_1.Value;
                Cliff_1Error.text = "" + mCliff_1.Error;
                //Cliff_1EmergencyStop.text = "" + mCliff_1.

                Cliff_2Value.text = "" + mCliff_2.Value;
                Cliff_2Error.text = "" + mCliff_2.Error;
                //Cliff_2EmergencyStop.text = "" + mCliff_2.

                Cliff_3Value.text = "" + mCliff_3.Value;
                Cliff_3Error.text = "" + mCliff_3.Error;
                //Cliff_3EmergencyStop.text = "" + mCliff_3.

                Cliff_4Value.text = "" + mCliff_4.Value;
                Cliff_4Error.text = "" + mCliff_4.Error;
                //Cliff_4EmergencyStop.text = "" + mCliff_4.

                Cliff_5Value.text = "" + mCliff_5.Value;
                Cliff_5Error.text = "" + mCliff_5.Error;
                //Cliff_5EmergencyStop.text = "" + mCliff_5.

                Cliff_6Value.text = "" + mCliff_6.Value;
                Cliff_6Error.text = "" + mCliff_6.Error;
                //Cliff_6EmergencyStop.text = "" + mCliff_6.

                Cliff_7Value.text = "" + mCliff_7.Value;
                Cliff_7Error.text = "" + mCliff_7.Error;
                //Cliff_7EmergencyStop.text = "" + mCliff_7.
            }
            else if(mDropDown.options[mDropDown.value].text == "IMU")
            {
                Gyro_X.text = "" + mIMU.Gyroscope.x;
                Gyro_Y.text = "" + mIMU.Gyroscope.y;
                Gyro_Z.text = "" + mIMU.Gyroscope.z;
                Acc_X.text = "" + mIMU.Accelerometer.x;
                Acc_Y.text = "" + mIMU.Accelerometer.y;
                Acc_Z.text = "" + mIMU.Accelerometer.z;

                IMU_Error.text = "" + mIMU.Error;
            }
        }

        public void OnValueChanged()
        {
            if (mDropDown.options[mDropDown.value].text == "TOF/IR/US")
            {
                DisableAndEnableRightWindow(0);
            }
            else if (mDropDown.options[mDropDown.value].text == "CARESS/PINCH")
            {
                DisableAndEnableRightWindow(1);
            }
            else if(mDropDown.options[mDropDown.value].text == "CLIFF")
            {
                DisableAndEnableRightWindow(2);
            }
            else if (mDropDown.options[mDropDown.value].text == "IMU")
            {
                DisableAndEnableRightWindow(3);
            }
        }

        private void DisableAndEnableRightWindow(int iInput)
        {
            foreach (GameObject go in mAllGO)
                go.SetActive(false);
            mAllGO[iInput].SetActive(true);
        }
    }
}
