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
        /// <summary>
        /// MENU & WINDOWS LINKS
        /// </summary>
        //GET GAMEOBJECT WINDOW
        [SerializeField]
        private GameObject Window_TOF;
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
        private Toggle BT_US;
        [SerializeField]
        private Toggle BT_CARESS;
        [SerializeField]
        private Toggle BT_PINCH;
        [SerializeField]
        private Toggle BT_CLIFF;
        [SerializeField]
        private Toggle BT_FILTERED;
        [SerializeField]
        private Toggle BT_IMU;

        /// <summary>
        /// ALL TOFF BUBBLES LINKS
        /// </summary>
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
        private Text PINCH_Text_HEART;
        [SerializeField]
        private Image PINCH_OK_HEART;
        [SerializeField]
        private Text PINCH_Error_HEART;
        [SerializeField]
        private Text PINCH_Text_RIGHT;
        [SerializeField]
        private Image PINCH_OK_RIGHT;
        [SerializeField]
        private Text PINCH_Error_RIGHT;
        [SerializeField]
        private Text PINCH_Text_LEFT;
        [SerializeField]
        private Image PINCH_OK_LEFT;
        [SerializeField]
        private Text PINCH_Error_LEFT;

        //CARESS
        [SerializeField]
        private Text CARESS_Text_LEFT;
        [SerializeField]
        private Image CARESS_OK_LEFT;
        [SerializeField]
        private Text CARESS_Error_LEFT;
        [SerializeField]
        private Text CARESS_Text_BACK;
        [SerializeField]
        private Image CARESS_OK_BACK;
        [SerializeField]
        private Text CARESS_Error_BACK;
        [SerializeField]
        private Text CARESS_Text_RIGHT;
        [SerializeField]
        private Image CARESS_OK_RIGHT;
        [SerializeField]
        private Text CARESS_Error_RIGHT;

        //Retrive All Sensors
        private UltrasonicSensor mLeftUSSensor;
        private UltrasonicSensor mRightUSSensor;
        private TimeOfFlightSensor mForeHeadTOFSensor;
        private TimeOfFlightSensor mChinTOFSensor;
        private TimeOfFlightSensor mFrontRightTOFSensor;
        private TimeOfFlightSensor mFrontMiddleTOFSensor;
        private TimeOfFlightSensor mFrontLeftTOFSensor;
        private TimeOfFlightSensor mBackTOFSensor;
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

        private float mTimerRefresh;

        private bool mFilteredValue;

        void Start()
        {
            mTimerRefresh = 0F;

            // Create Toggle Button list
            mAllToggle = new List<Toggle>();
            mAllToggle.Add(BT_TOF);
            mAllToggle.Add(BT_US);
            mAllToggle.Add(BT_CARESS);
            mAllToggle.Add(BT_PINCH);
            mAllToggle.Add(BT_CLIFF);
            mAllToggle.Add(BT_IMU);

            //Display all Toggle ON
            foreach (Toggle Tog in mAllToggle) {
                Tog.isOn = true;
            }

            // Filtered value?
            //mFilteredValue = BT_FILTERED.isOn;
            //BT_FILTERED.onValueChanged.AddListener((iInput) => mFilteredValue = iInput);
            mFilteredValue = false;

            // Create Window list
            mAllWindow = new List<GameObject>();
            mAllWindow.Add(Window_TOF);
            mAllWindow.Add(Window_US);
            mAllWindow.Add(Window_CARESS);
            mAllWindow.Add(Window_PINCH);
            mAllWindow.Add(Window_CLIFF);
            mAllWindow.Add(Window_IMU);

            //Display all window
            foreach (GameObject Window in mAllWindow) {
                Window.SetActive(true);
            }

            // US
            mLeftUSSensor = Buddy.Sensors.UltrasonicSensors.Left;
            mRightUSSensor = Buddy.Sensors.UltrasonicSensors.Right;

            // TOF
            mForeHeadTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Head;
            //mChinTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Back;
            mFrontRightTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Right;
            mFrontMiddleTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Center;
            mFrontLeftTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Left;
            mBackTOFSensor = Buddy.Sensors.TimeOfFlightSensors.Back;
            
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
            mTimerRefresh += Time.deltaTime;
            //A tester les filtered value avec dropdown et value des US / TOF
            if (mTimerRefresh > DiagnosticBehaviour.REFRESH_TIMER) {
                mTimerRefresh = 0F;
                if (BT_TOF.isOn) {

                    //TOF SENSOR FORE HEAD
                    TOF_Error_00.text = mForeHeadTOFSensor.Error.ToString();
                    if (mFilteredValue)
                        TOF_Text_00.text = (mForeHeadTOFSensor.Value / 1000) + "m";
                    else
                    {
                        if (mForeHeadTOFSensor.Value > 0)
                            TOF_Text_00.text = (mForeHeadTOFSensor.ListDistanceObjectTOF[0] / 1000) + "m";
                    }
                    TOF_OK_00.color = mForeHeadTOFSensor.Error == 0 ? BuddyBlue : Red;

                    //TOF SENSOR CHIN
                    //TOF_Error_01.text = mChinTOFSensor.Error.ToString();
                    //if (mFilteredValue)
                    //    TOF_Text_01.text = (mChinTOFSensor.Value / 1000) + "m";
                    //else
                    //    TOF_Text_01.text = (mChinTOFSensor.Value / 1000) + "m";
                    //TOF_OK_01.color = mChinTOFSensor.Error == 0 ? BuddyBlue : Red;

                    //TOF SENSOR FRONT RIGHT
                    TOF_Error_02.text = mFrontRightTOFSensor.Error.ToString();
                    if (mFilteredValue)
                        TOF_Text_02.text = (mFrontRightTOFSensor.Value / 1000) + "m";
                    else
                    {
                        if (mFrontRightTOFSensor.Value > 0)
                            TOF_Text_02.text = (mFrontRightTOFSensor.ListDistanceObjectTOF[0] / 1000) + "m";
                    }
                    TOF_OK_02.color = mFrontRightTOFSensor.Error == 0 ? BuddyBlue : Red;

                    //TOF SENSOR FRONT MIDDLE
                    TOF_Error_03.text = mFrontMiddleTOFSensor.Error.ToString();
                    if (mFilteredValue)
                        TOF_Text_03.text = (mFrontMiddleTOFSensor.Value / 1000) + "m";
                    else
                    {
                        if (mFrontMiddleTOFSensor.Value > 0)
                            TOF_Text_03.text = (mFrontMiddleTOFSensor.ListDistanceObjectTOF[0] / 1000) + "m";
                    }
                    TOF_OK_03.color = mFrontMiddleTOFSensor.Error == 0 ? BuddyBlue : Red;

                    //TOF SENSOR FRONT LEFT
                    TOF_Error_04.text = mFrontLeftTOFSensor.Error.ToString();
                    if (mFilteredValue)
                        TOF_Text_04.text = (mFrontLeftTOFSensor.Value / 1000) + "m";
                    else
                    {
                        if (mFrontLeftTOFSensor.Value > 0)
                            TOF_Text_04.text = (mFrontLeftTOFSensor.ListDistanceObjectTOF[0] / 1000) + "m";
                    }
                    TOF_OK_04.color = mFrontLeftTOFSensor.Error == 0 ? BuddyBlue : Red;

                    //TOF SENSOR BACK
                    TOF_Error_05.text = mBackTOFSensor.Error.ToString();
                    if (mFilteredValue)
                        TOF_Text_05.text = (mBackTOFSensor.Value / 1000) + "m";
                    else
                    {
                        if(mBackTOFSensor.Value > 0)
                            TOF_Text_05.text = (mBackTOFSensor.ListDistanceObjectTOF[0] / 1000) + "m";
                    }
                    TOF_OK_05.color = mBackTOFSensor.Error == 0 ? BuddyBlue : Red;
                }

                if (BT_US.isOn) {
                    //US RECEPTEUR RIGHT
                    if (mFilteredValue)
                        US_Text_00.text = (mRightUSSensor.Value / 1000) + "m";
                    else
                        US_Text_00.text = (mRightUSSensor.Value / 1000) + "m";
                    US_OK_00.color = BuddyBlue;
                    US_Error_00.text = mRightUSSensor.Error.ToString();

                    //US RECEPTEUR LEFT
                    if (mFilteredValue)
                        US_Text_01.text = (mLeftUSSensor.Value / 1000) + "m";
                    else
                        US_Text_01.text = (mLeftUSSensor.Value / 1000) + "m";
                    US_OK_01.color = BuddyBlue;
                    US_Error_01.text = mLeftUSSensor.Error.ToString();
                }

                if (BT_CARESS.isOn) {
                    //CARESS RIGHT
                    CARESS_Text_RIGHT.text = mTouchSensor.RightHead.Value.ToString();
                    CARESS_Error_RIGHT.text = mTouchSensor.RightHead.Error.ToString();
                    CARESS_Text_RIGHT.color = mTouchSensor.RightHead.Value ? BuddyBlue : Red;
                    CARESS_OK_RIGHT.color = mTouchSensor.RightHead.Error == 0 ? BuddyBlue : Red;

                    //CARESS BACK
                    CARESS_Text_BACK.text = mTouchSensor.BackHead.Value.ToString();
                    CARESS_Error_BACK.text = mTouchSensor.BackHead.Error.ToString();
                    CARESS_Text_BACK.color = mTouchSensor.BackHead.Value ? BuddyBlue : Red;
                    CARESS_OK_BACK.color = mTouchSensor.BackHead.Error == 0 ? BuddyBlue : Red;

                    //CARESS LEFT
                    CARESS_Text_LEFT.text = mTouchSensor.LeftHead.Value.ToString();
                    CARESS_Error_LEFT.text = mTouchSensor.LeftHead.Error.ToString();
                    CARESS_Text_LEFT.color = mTouchSensor.LeftHead.Value ? BuddyBlue : Red;
                    CARESS_OK_LEFT.color = mTouchSensor.LeftHead.Error == 0 ? BuddyBlue : Red;
                }

                if (BT_PINCH.isOn) {
                    // PINCH HEART
                    PINCH_Text_HEART.text = mTouchSensor.Heart.Value.ToString();
                    PINCH_Error_HEART.text = mTouchSensor.Heart.Error.ToString();
                    PINCH_Text_HEART.color = mTouchSensor.Heart.Value ? BuddyBlue : Red;
                    PINCH_OK_HEART.color = mTouchSensor.Heart.Error == 0 ? BuddyBlue : Red;

                    // PINCH LEFT
                    PINCH_Text_LEFT.text = mTouchSensor.LeftShoulder.Value.ToString();
                    PINCH_Error_LEFT.text = mTouchSensor.LeftShoulder.Error.ToString();
                    PINCH_Text_LEFT.color = mTouchSensor.LeftShoulder.Value ? BuddyBlue : Red;
                    PINCH_OK_LEFT.color = mTouchSensor.LeftShoulder.Error == 0 ? BuddyBlue : Red;

                    // PINCH RIGHT
                    PINCH_Text_RIGHT.text = mTouchSensor.RightShoulder.Value.ToString();
                    PINCH_Error_RIGHT.text = mTouchSensor.RightShoulder.Error.ToString();
                    PINCH_Text_RIGHT.color = mTouchSensor.RightShoulder.Value ? BuddyBlue : Red;
                    PINCH_OK_RIGHT.color = mTouchSensor.RightShoulder.Error == 0 ? BuddyBlue : Red;
                }

                if (BT_CLIFF.isOn) {
                    // Cliff Front Free Wheel
                    CLIFF_Text_FrontFreeWheel.text = (mCliff_FrontFreeWheel.Value) + "mm";
                    CLIFF_Error_FrontFreeWheel.text = mCliff_FrontFreeWheel.Error.ToString();
                    CLIFF_OK_FrontFreeWheel.color = mCliff_FrontFreeWheel.Error == 0 ? BuddyBlue : Red;

                    // Cliff Front Right Wheel
                    CLIFF_Text_FrontRightWheel.text = (mCliff_FrontRightWheel.Value) + "mm";
                    CLIFF_Error_FrontRightWheel.text = mCliff_FrontRightWheel.Error.ToString();
                    CLIFF_OK_FrontRightWheel.color = mCliff_FrontRightWheel.Error == 0 ? BuddyBlue : Red;

                    // Cliff Back Right Wheel
                    CLIFF_Text_BackRightWheel.text = (mCliff_BackRightWheel.Value) + "mm";
                    CLIFF_Error_BackRightWheel.text = mCliff_BackRightWheel.Error.ToString();
                    CLIFF_OK_BackRightWheel.color = mCliff_BackRightWheel.Error == 0 ? BuddyBlue : Red;

                    // Cliff Back Right free Wheel
                    CLIFF_Text_BackRightFreeWheel.text = (mCliff_BackRightFreeWheel.Value) + "mm";
                    CLIFF_Error_BackRightFreeWheel.text = mCliff_BackRightFreeWheel.Error.ToString();
                    CLIFF_OK_BackRightFreeWheel.color = mCliff_BackRightFreeWheel.Error == 0 ? BuddyBlue : Red;

                    // Cliff Front Left Wheel
                    CLIFF_Text_FrontLeftWheel.text = (mCliff_FrontLeftWheel.Value) + "mm";
                    CLIFF_Error_FrontLeftWheel.text = mCliff_FrontLeftWheel.Error.ToString();
                    CLIFF_OK_FrontLeftWheel.color = mCliff_FrontLeftWheel.Error == 0 ? BuddyBlue : Red;

                    // Cliff Back Left Wheel
                    CLIFF_Text_BackLeftWheel.text = (mCliff_BackLeftWheel.Value) + "mm";
                    CLIFF_Error_BackLeftWheel.text = mCliff_BackLeftWheel.Error.ToString();
                    CLIFF_OK_BackLeftWheel.color = mCliff_BackLeftWheel.Error == 0 ? BuddyBlue : Red;

                    // Cliff Back Left Free Wheel
                    CLIFF_Text_BackLeftFreeWheel.text = (mCliff_BackLeftFreeWheel.Value) + "mm";
                    CLIFF_Error_BackLeftFreeWheel.text = mCliff_BackLeftFreeWheel.Error.ToString();
                    CLIFF_OK_BackLeftFreeWheel.color = mCliff_BackLeftFreeWheel.Error == 0 ? BuddyBlue : Red;
                }

                if (BT_IMU.isOn) {
                    Gyro_X.text = mIMU.Gyroscope.x.ToString("F0");
                    Gyro_Y.text = mIMU.Gyroscope.y.ToString("F0");
                    Gyro_Z.text = mIMU.Gyroscope.z.ToString("F0");
                    Acc_X.text = mIMU.Accelerometer.x.ToString("F0");
                    Acc_Y.text = mIMU.Accelerometer.y.ToString("F0");
                    Acc_Z.text = mIMU.Accelerometer.z.ToString("F0");

                    IMU_OK_00.color = mIMU.Error == 0 ? BuddyBlue : Red;
                    IMU_Error_00.text = mIMU.Error.ToString();
                }
            }

        }

        public void OnChangeDropdownValue()
        {
            mFilteredValue = !mFilteredValue;
        }

        public void ToggleValueChanged(int iIndex)
        {
            Toggle mToggle = mAllToggle[iIndex];
            GameObject mWindow = mAllWindow[iIndex];
            bool mStatus = mToggle.isOn;
            if (mStatus == true) {
                mWindow.SetActive(true);
            } else {
                mWindow.SetActive(false);
            }
        }
    }
}
