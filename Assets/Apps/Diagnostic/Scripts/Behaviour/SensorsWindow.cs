﻿using System;
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
        /// <summary>
        /// MENU & WINDOWS LINKS
        /// </summary>
        //GET GAMEOBJECT WINDOW
        [SerializeField]
        private GameObject Window_TOF;
        [SerializeField]
        private GameObject Window_IR;
        [SerializeField]
        private GameObject Window_US;
        [SerializeField]
        private GameObject Window_CARESS;
        [SerializeField]
        private GameObject Window_PINCH;
        [SerializeField]
        private GameObject Window_CLIFF;
        [SerializeField]
        private GameObject Window_IMU;
        //GET TOGGLES NAV BUTTON
        [SerializeField]
        private Toggle BT_TOF;
        [SerializeField]
        private Toggle BT_IR;
        [SerializeField]
        private Toggle BT_US;
        [SerializeField]
        private Toggle BT_CARESS;
        [SerializeField]
        private Toggle BT_PINCH;
        [SerializeField]
        private Toggle BT_CLIFF;
        [SerializeField]
        private Toggle BT_IMU;

        /// <summary>
        /// ALL TOFF BUBBLES LINKS
        /// </summary>
        //GET TOF BUBBLES TEXT
        [SerializeField]
        private Text TOF_Text_00;
        [SerializeField]
        private Text TOF_Text_01;
        [SerializeField]
        private Text TOF_Text_02;
        [SerializeField]
        private Text TOF_Text_03;
        [SerializeField]
        private Text TOF_Text_04;
        [SerializeField]
        private Text TOF_Text_05;
        //GET TOF BUBBLES STATE
        [SerializeField]
        private Image TOF_OK_00;
        [SerializeField]
        private Image TOF_OK_01;
        [SerializeField]
        private Image TOF_OK_02;
        [SerializeField]
        private Image TOF_OK_03;
        [SerializeField]
        private Image TOF_OK_04;
        [SerializeField]
        private Image TOF_OK_05;
        //GET TOF BUBBLES STATE
        [SerializeField]
        private Text TOF_Error_00;
        [SerializeField]
        private Text TOF_Error_01;
        [SerializeField]
        private Text TOF_Error_02;
        [SerializeField]
        private Text TOF_Error_03;
        [SerializeField]
        private Text TOF_Error_04;
        [SerializeField]
        private Text TOF_Error_05;

        //LED IR RECEPTRICE
        [SerializeField]
        private Text IR_Text_00;
        [SerializeField]
        private Image IR_OK_00;
        [SerializeField]
        private Text IR_Error_00;

        //US SENSORS
        [SerializeField]
        private Text US_Text_00;
        [SerializeField]
        private Image US_OK_00;
        [SerializeField]
        private Text US_Error_00;
        [SerializeField]
        private Text US_Text_01;
        [SerializeField]
        private Image US_OK_01;
        [SerializeField]
        private Text US_Error_01;


        //CLIFF SENSORS
        [SerializeField]
        private Text CLIFF_Text_FrontFreeWheel;
        [SerializeField]
        private Image CLIFF_OK_FrontFreeWheel;
        [SerializeField]
        private Text CLIFF_Error_FrontFreeWheel;
        [SerializeField]
        private Image CLIFF_Icon_FrontFreeWheel;
        [SerializeField]
        private Text CLIFF_Text_FrontRightWheel;
        [SerializeField]
        private Image CLIFF_OK_FrontRightWheel;
        [SerializeField]
        private Text CLIFF_Error_FrontRightWheel;
        [SerializeField]
        private Image CLIFF_Icon_FrontRightWheel;
        [SerializeField]
        private Text CLIFF_Text_BackRightWheel;
        [SerializeField]
        private Image CLIFF_OK_BackRightWheel;
        [SerializeField]
        private Text CLIFF_Error_BackRightWheel;
        [SerializeField]
        private Image CLIFF_Icon_BackRightWheel;
        [SerializeField]
        private Text CLIFF_Text_BackRightFreeWheel;
        [SerializeField]
        private Image CLIFF_OK_BackRightFreeWheel;
        [SerializeField]
        private Text CLIFF_Error_BackRightFreeWheel;
        [SerializeField]
        private Image CLIFF_Icon_BackRightFreeWheel;
        [SerializeField]
        private Text CLIFF_Text_FrontLeftWheel;
        [SerializeField]
        private Image CLIFF_OK_FrontLeftWheel;
        [SerializeField]
        private Text CLIFF_Error_FrontLeftWheel;
        [SerializeField]
        private Image CLIFF_Icon_FrontLeftWheel;
        [SerializeField]
        private Text CLIFF_Text_BackLeftWheel;
        [SerializeField]
        private Image CLIFF_OK_BackLeftWheel;
        [SerializeField]
        private Text CLIFF_Error_BackLeftWheel;
        [SerializeField]
        private Image CLIFF_Icon_BackLeftWheel;
        [SerializeField]
        private Text CLIFF_Text_BackLeftFreeWheel;
        [SerializeField]
        private Image CLIFF_OK_BackLeftFreeWheel;
        [SerializeField]
        private Text CLIFF_Error_BackLeftFreeWheel;
        [SerializeField]
        private Image CLIFF_Icon_BackLeftFreeWheel;
        
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
        private Image IMU_OK_00;
        [SerializeField]
        private Text IMU_Error_00;


        //PINCH 
        [SerializeField]
        private Text PINCH_Text_00;
        [SerializeField]
        private Image PINCH_OK_00;
        [SerializeField]
        private Text PINCH_Error_00;
        [SerializeField]
        private Text PINCH_Text_01;
        [SerializeField]
        private Image PINCH_OK_01;
        [SerializeField]
        private Text PINCH_Error_01;
        [SerializeField]
        private Text PINCH_Text_02;
        [SerializeField]
        private Image PINCH_OK_02;
        [SerializeField]
        private Text PINCH_Error_02;
        //CARESS
        [SerializeField]
        private Text CARESS_Text_00;
        [SerializeField]
        private Image CARESS_OK_00;
        [SerializeField]
        private Text CARESS_Error_00;
        [SerializeField]
        private Text CARESS_Text_01;
        [SerializeField]
        private Image CARESS_OK_01;
        [SerializeField]
        private Text CARESS_Error_01;
        [SerializeField]
        private Text CARESS_Text_02;
        [SerializeField]
        private Image CARESS_OK_02;
        [SerializeField]
        private Text CARESS_Error_02;

        //Retrive All Sensors
        private UltrasonicSensor mLeftUSSensor;
        private UltrasonicSensor mRightUSSensor;
        private TimeOfFlightSensor mForeHeadTOFSensor;
        private TimeOfFlightSensor mChinTOFSensor;
        private TimeOfFlightSensor mFrontRightTOFSensor;
        private TimeOfFlightSensor mFrontMiddleTOFSensor;
        private TimeOfFlightSensor mFrontLeftTOFSensor;
        private TimeOfFlightSensor mBackTOFSensor;   
        private InfraredSensor mBackIRSensor;
        private CliffSensor mCliff_FrontFreeWheel;
        private CliffSensor mCliff_FrontRightWheel;
        private CliffSensor mCliff_BackRightWheel;
        private CliffSensor mCliff_BackRightFreeWheel;
        private CliffSensor mCliff_FrontLeftWheel;
        private CliffSensor mCliff_BackLeftWheel;
        private CliffSensor mCliff_BackLeftFreeWheel;

        private IMU mIMU;
        private TouchSensors mTouchSensor;

        // Set Colors
        private Color BuddyBlue = new Color(0.0f, 0.831f, 0.819f);
        private Color Red = new Color(1f, 0f, 0f);
        // List Objects Windows and Toggles
        private List<Toggle> mAllToggle;
        private List<GameObject> mAllWindow;

        void Start()
        {
            // Create Toggle Button list
            mAllToggle = new List<Toggle>();
            mAllToggle.Add(BT_TOF);
            mAllToggle.Add(BT_IR);
            mAllToggle.Add(BT_US);
            mAllToggle.Add(BT_CARESS);
            mAllToggle.Add(BT_PINCH);
            mAllToggle.Add(BT_CLIFF);
            mAllToggle.Add(BT_IMU);
            //Display all Toggle ON
            foreach (Toggle Tog in mAllToggle)
            {
                Tog.isOn = true;
            }
            // Create Window list
            mAllWindow = new List<GameObject>();
            mAllWindow.Add(Window_TOF);
            mAllWindow.Add(Window_IR);
            mAllWindow.Add(Window_US);
            mAllWindow.Add(Window_CARESS);
            mAllWindow.Add(Window_PINCH);
            mAllWindow.Add(Window_CLIFF);
            mAllWindow.Add(Window_IMU);
            //Display all window
            foreach (GameObject Window in mAllWindow){
                Window.SetActive(true);
            }
            // US
            mLeftUSSensor = Buddy.Sensors.UltrasonicSensors.Left;
            mRightUSSensor = Buddy.Sensors.UltrasonicSensors.Right;
            // TOF
            mForeHeadTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Forehead;
            mChinTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Chin;
            mFrontRightTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Right;
            mFrontMiddleTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Front;
            mFrontLeftTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Left;
            mBackTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Back;
            // IR
            mBackIRSensor = Buddy.Sensors.InfraredSensor;
            // CLIFF
            mCliff_FrontFreeWheel = Buddy.Sensors.CliffSensors.FrontFreeWheel;
            mCliff_FrontRightWheel = Buddy.Sensors.CliffSensors.FrontRightWheel;
            mCliff_BackRightWheel = Buddy.Sensors.CliffSensors.BackRightWheel;
            mCliff_BackRightFreeWheel = Buddy.Sensors.CliffSensors.BackRightFreeWheel;
            mCliff_FrontLeftWheel = Buddy.Sensors.CliffSensors.FrontLeftWheel;
            mCliff_BackLeftWheel = Buddy.Sensors.CliffSensors.BackLeftWheel;
            mCliff_BackLeftFreeWheel = Buddy.Sensors.CliffSensors.BackLeftFreeWheel;
            // IMU
            mIMU = Buddy.Sensors.IMU;
            //TouchSensor
            mTouchSensor = Buddy.Sensors.TouchSensors; 
        }

        void Update()
        {
            if (BT_TOF.isOn == true)
            {
                //TOF SENSOR FORE HEAD
                TOF_Error_00.text = mForeHeadTOFSensor.Error + "";
                TOF_Text_00.text = (mForeHeadTOFSensor.Value/1000) + "m";
                if (mForeHeadTOFSensor.Error == 0) { TOF_OK_00.color = BuddyBlue; }
                if (mForeHeadTOFSensor.Error != 0) { TOF_OK_00.color = Red; }
                //TOF SENSOR CHIN
                TOF_Error_01.text = mChinTOFSensor.Error + "";
                TOF_Text_01.text = (mChinTOFSensor.Value/1000) + "m";
                if (mChinTOFSensor.Error == 0) { TOF_OK_01.color = BuddyBlue; }
                if (mChinTOFSensor.Error != 0) { TOF_OK_01.color = Red; }
                //TOF SENSOR FRONT RIGHT
                TOF_Error_02.text = mFrontRightTOFSensor.Error + "";
                TOF_Text_02.text = (mFrontRightTOFSensor.Value/1000) + "m";
                if (mFrontRightTOFSensor.Error == 0) { TOF_OK_02.color = BuddyBlue; }
                if (mFrontRightTOFSensor.Error != 0) { TOF_OK_02.color = Red; }
                //TOF SENSOR FRONT MIDDLE
                TOF_Error_03.text = mFrontMiddleTOFSensor.Error + "";
                TOF_Text_03.text = (mFrontMiddleTOFSensor.Value/1000) + "m";
                if (mFrontMiddleTOFSensor.Error == 0) { TOF_OK_03.color = BuddyBlue; }
                if (mFrontMiddleTOFSensor.Error != 0) { TOF_OK_03.color = Red; }
                //TOF SENSOR FRONT LEFT
                TOF_Error_04.text = mFrontLeftTOFSensor.Error + "";
                TOF_Text_04.text = (mFrontLeftTOFSensor.Value/1000) + "m";
                if (mFrontLeftTOFSensor.Error == 0) { TOF_OK_04.color = BuddyBlue; }
                if (mFrontLeftTOFSensor.Error != 0) { TOF_OK_04.color = Red; }
                //TOF SENSOR BACK
                TOF_Error_05.text = mBackTOFSensor.Error + "";
                TOF_Text_05.text = (mBackTOFSensor.Value/1000) + "m";
                if (mBackTOFSensor.Error == 0) { TOF_OK_05.color = BuddyBlue; }
                if (mBackTOFSensor.Error != 0) { TOF_OK_05.color = Red; }
            }
            if (BT_IR.isOn == true)
            {
                //IR RECEPTEUR
                IR_Text_00.text = "#" + mBackIRSensor.Value;
                IR_OK_00.color = BuddyBlue;
                //IR_Error_00.text = mBackIRSensor.Error + "";
            }
            if (BT_US.isOn == true)
            {
                //US RECEPTEUR RIGHT
                US_Text_00.text = (mRightUSSensor.Value/1000) + "m";
                US_OK_00.color = BuddyBlue;
                US_Error_00.text = mRightUSSensor.Error + "";
                //US RECEPTEUR LEFT
                US_Text_01.text = (mLeftUSSensor.Value/1000) + "m";
                US_OK_01.color = BuddyBlue;
                US_Error_01.text = mLeftUSSensor.Error + "";
            }
            if (BT_CARESS.isOn == true)
            {
                //CARESS RIGHT
                CARESS_Text_00.text = "" + mTouchSensor.RightHead.Value;
                if (mTouchSensor.RightHead.Value == true) { CARESS_Text_00.color = BuddyBlue; }
                if (mTouchSensor.RightHead.Value == false) { CARESS_Text_00.color = Red; }
                CARESS_Error_00.text = mTouchSensor.RightHead.Error + "" ;
                if (mTouchSensor.RightHead.Error == 0) { CARESS_OK_00.color = BuddyBlue; }
                if (mTouchSensor.RightHead.Error != 0) { CARESS_OK_00.color = Red; }
                //CARESS BACK
                CARESS_Text_01.text = "" + mTouchSensor.BackHead.Value;
                if (mTouchSensor.BackHead.Value == true) { CARESS_Text_01.color = BuddyBlue; }
                if (mTouchSensor.BackHead.Value == false) { CARESS_Text_01.color = Red; }
                CARESS_Error_01.text =mTouchSensor.BackHead.Error + "";
                if (mTouchSensor.BackHead.Error == 0) { CARESS_OK_01.color = BuddyBlue; }
                if (mTouchSensor.BackHead.Error != 0) { CARESS_OK_01.color = Red; }
                //CARESS LEFT
                CARESS_Text_02.text = "" + mTouchSensor.LeftHead.Value;
                if(mTouchSensor.LeftHead.Value == true) { CARESS_Text_02.color = BuddyBlue; }
                if (mTouchSensor.LeftHead.Value == false) { CARESS_Text_02.color = Red; }
                CARESS_Error_02.text = mTouchSensor.LeftHead.Error + "";
                if (mTouchSensor.LeftHead.Error == 0) { CARESS_OK_02.color = BuddyBlue; }
                if (mTouchSensor.LeftHead.Error != 0) { CARESS_OK_02.color = Red; }
            }
            if (BT_PINCH.isOn == true)
            {
                //PINCH HEART
                PINCH_Text_02.text = "" + mTouchSensor.Heart.Value;
                if (mTouchSensor.Heart.Error == 0) { PINCH_OK_02.color = BuddyBlue; }
                if (mTouchSensor.Heart.Error != 0) { PINCH_OK_02.color = Red; }
                PINCH_Error_02.text = mTouchSensor.Heart.Error + "";
                //PINCH LEFT
                PINCH_Text_01.text = "" + mTouchSensor.LeftShoulder.Value;
                if (mTouchSensor.LeftShoulder.Error == 0) { PINCH_OK_01.color = BuddyBlue; }
                if (mTouchSensor.LeftShoulder.Error != 0) { PINCH_OK_01.color = Red; }
                PINCH_Error_01.text = mTouchSensor.LeftShoulder.Error + "";
                //PINCH RIGHT
                PINCH_Text_00.text = "" + mTouchSensor.RightShoulder.Value;
                if (mTouchSensor.RightShoulder.Error == 0) { PINCH_OK_00.color = BuddyBlue; }
                if (mTouchSensor.RightShoulder.Error != 0) { PINCH_OK_00.color = Red; }
                PINCH_Error_00.text = mTouchSensor.RightShoulder.Error + "";
            }
            if (BT_CLIFF.isOn == true)
            {
                // 00 Cliff Front Free Wheel
                CLIFF_Text_FrontFreeWheel.text = (mCliff_FrontFreeWheel.Value/10) + "cm";
                CLIFF_Error_FrontFreeWheel.text = mCliff_FrontFreeWheel.Error + "";
                if (mCliff_FrontFreeWheel.Error == 0) { CLIFF_OK_FrontFreeWheel.color = BuddyBlue; }
                if (mCliff_FrontFreeWheel.Error != 0) { CLIFF_OK_FrontFreeWheel.color = Red; }
                //if (mCliff_00.OnVoid == 0) { CLIFF_Icon_00.color = BuddyBlue; }
                //if (mCliff_00.OnVoid == 1) { CLIFF_Icon_00.color = Red; }

                // 01 Cliff Front Right Wheel
                CLIFF_Text_FrontRightWheel.text = (mCliff_FrontRightWheel.Value/10) + "cm";
                CLIFF_Error_FrontRightWheel.text = mCliff_FrontRightWheel.Error + "";
                if (mCliff_FrontRightWheel.Error == 0) { CLIFF_OK_FrontRightWheel.color = BuddyBlue; }
                if (mCliff_FrontRightWheel.Error != 0) { CLIFF_OK_FrontRightWheel.color = Red; }
                //if (mCliff_01.OnVoid == 0) { CLIFF_Icon_01.color = BuddyBlue; }
                //if (mCliff_01.OnVoid == 1) { CLIFF_Icon_01.color = Red; }

                // 02 Cliff Back Right Wheel
                CLIFF_Text_BackRightWheel.text = (mCliff_BackRightWheel.Value/10) + "cm";
                CLIFF_Error_BackRightWheel.text = mCliff_BackRightWheel.Error + "";
                if (mCliff_BackRightWheel.Error == 0) { CLIFF_OK_BackRightWheel.color = BuddyBlue; }
                if (mCliff_BackRightWheel.Error != 0) { CLIFF_OK_BackRightWheel.color = Red; }
                //if (mCliff_00.OnVoid == 0) { CLIFF_Icon_02.color = BuddyBlue; }
                //if (mCliff_00.OnVoid == 1) { CLIFF_Icon_02.color = Red; }

                // 03 Cliff Back Right free Wheel
                CLIFF_Text_BackRightFreeWheel.text = (mCliff_BackRightFreeWheel.Value/10) + "cm";
                CLIFF_Error_BackRightFreeWheel.text = mCliff_BackRightFreeWheel.Error + "";
                if (mCliff_BackRightFreeWheel.Error == 0) { CLIFF_OK_BackRightFreeWheel.color = BuddyBlue; }
                if (mCliff_BackRightFreeWheel.Error != 0) { CLIFF_OK_BackRightFreeWheel.color = Red; }
                //if (mCliff_03.OnVoid == 0) { CLIFF_Icon_03.color = BuddyBlue; }
                //if (mCliff_03.OnVoid == 1) { CLIFF_Icon_03.color = Red; }

                // 04 Cliff Front Left Wheel
                CLIFF_Text_FrontLeftWheel.text = (mCliff_FrontLeftWheel.Value/10) + "cm";
                CLIFF_Error_FrontLeftWheel.text = mCliff_FrontLeftWheel.Error + "";
                if (mCliff_FrontLeftWheel.Error == 0) { CLIFF_OK_FrontLeftWheel.color = BuddyBlue; }
                if (mCliff_FrontLeftWheel.Error != 0) { CLIFF_OK_FrontLeftWheel.color = Red; }
                //if (mCliff_04.OnVoid == 0) { CLIFF_Icon_04.color = BuddyBlue; }
                //if (mCliff_04.OnVoid == 1) { CLIFF_Icon_04.color = Red; }

                // 05 Cliff Back Left Wheel
                CLIFF_Text_BackLeftWheel.text = (mCliff_BackLeftWheel.Value/10) + "cm";
                CLIFF_Error_BackLeftWheel.text = mCliff_BackLeftWheel.Error + "";
                if (mCliff_BackLeftWheel.Error == 0) { CLIFF_OK_BackLeftWheel.color = BuddyBlue; }
                if (mCliff_BackLeftWheel.Error != 0) { CLIFF_OK_BackLeftWheel.color = Red; }
                //if (mCliff_05.OnVoid == 0) { CLIFF_Icon_05.color = BuddyBlue; }
                //if (mCliff_05.OnVoid == 1) { CLIFF_Icon_05.color = Red; }

                // 06 Cliff Back Left Free Wheel
                CLIFF_Text_BackLeftFreeWheel.text = (mCliff_BackLeftFreeWheel.Value/10) + "cm";
                CLIFF_Error_BackLeftFreeWheel.text = mCliff_BackLeftFreeWheel.Error + "";
                if (mCliff_BackLeftFreeWheel.Error == 0) { CLIFF_OK_BackLeftFreeWheel.color = BuddyBlue; }
                if (mCliff_BackLeftFreeWheel.Error != 0) { CLIFF_OK_BackLeftFreeWheel.color = Red; }
                //if (mCliff_06.OnVoid == 0) { CLIFF_Icon_06.color = BuddyBlue; }
                //if (mCliff_06.OnVoid == 1) { CLIFF_Icon_06.color = Red; }
            }
            if (BT_IMU.isOn == true)
            {
                Gyro_X.text = "X: " + mIMU.Gyroscope.x;
                Gyro_Y.text = "Y: " + mIMU.Gyroscope.y;
                Gyro_Z.text = "Z: " + mIMU.Gyroscope.z;
                Acc_X.text = "X: " + mIMU.Accelerometer.x;
                Acc_Y.text = "Y: " + mIMU.Accelerometer.y;
                Acc_Z.text = "Z: " + mIMU.Accelerometer.z;

                if (mIMU.Error == 0) { IMU_OK_00.color = BuddyBlue; }
                if (mIMU.Error != 0) { IMU_OK_00.color = Red; }
                IMU_Error_00.text = mIMU.Error + "";
            }
        }

        public void ToggleValueChanged(int iIndex)
        {
            Toggle mToggle = mAllToggle[iIndex];
            GameObject mWindow = mAllWindow[iIndex];
            bool mStatus = mToggle.isOn;
            if (mStatus == true){
                mWindow.SetActive(true);
            }
            else
            {
                mWindow.SetActive(false);
            }
        }
    }
}
