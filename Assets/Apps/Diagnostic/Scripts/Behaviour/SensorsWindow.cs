using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public class SensorsWindow : MonoBehaviour
    {
        [SerializeField]
        private Text leftUSError;
        [SerializeField]
        private Text leftUSValue;

        [SerializeField]
        private Text rightUSError;
        [SerializeField]
        private Text rightUSValue;

        [SerializeField]
        private Text leftTOLError;
        [SerializeField]
        private Text leftTOLValue;

        [SerializeField]
        private Text backTOLError;
        [SerializeField]
        private Text backTOLValue;

        [SerializeField]
        private Text rightTOLError;
        [SerializeField]
        private Text rightTOLValue;

        [SerializeField]
        private Text chinTOLError;
        [SerializeField]
        private Text chinTOLValue;

        [SerializeField]
        private Text foreHeadTOLError;
        [SerializeField]
        private Text foreHeadTOLValue;

        [SerializeField]
        private Text frontTOLError;
        [SerializeField]
        private Text frontTOLValue;


        private UltrasonicSensor mLeftUSSensor;
        private UltrasonicSensor mRightUSSensor;
        //private UltrasonicSensor mBackUSSensor;
        private TimeOfFlightSensor mLeftTOLSensor;
        private TimeOfFlightSensor mBackTOLSensor;
        private TimeOfFlightSensor mRightTOLSensor;
        private TimeOfFlightSensor mChinTOLSensor;
        private TimeOfFlightSensor mForeHeadTOLSensor;
        private TimeOfFlightSensor mFrontTOLSensor;

        void Start()
        {
            mLeftUSSensor = Buddy.Sensors.UltrasonicSensors.Left;
            mRightUSSensor = Buddy.Sensors.UltrasonicSensors.Right;
            //mBackUSSensor = BYOS.Instance.Primitive.USSensors.Back;
            mLeftTOLSensor = Buddy.Sensors.TimeOfFlightSensors.Left;
            mBackTOLSensor = Buddy.Sensors.TimeOfFlightSensors.Back;
            mRightTOLSensor = Buddy.Sensors.TimeOfFlightSensors.Right;
            mFrontTOLSensor = Buddy.Sensors.TimeOfFlightSensors.Front;
            mForeHeadTOLSensor = Buddy.Sensors.TimeOfFlightSensors.Forehead;
            mChinTOLSensor = Buddy.Sensors.TimeOfFlightSensors.Chin;
        }

        void Update()
        {
            leftUSError.text = "" + mLeftUSSensor.Error;
            leftUSValue.text = "" + mLeftUSSensor.Value;/*.Distance;*/

            rightUSError.text = "" + mRightUSSensor.Error;
            rightUSValue.text = "" + mRightUSSensor.Value;/*.Distance;*/

            leftTOLError.text = "" + mLeftTOLSensor.Error;
            leftTOLValue.text = "" + mLeftTOLSensor.Value;

            backTOLError.text = "" + mBackTOLSensor.Error;
            backTOLValue.text = "" + mBackTOLSensor.Value;

            rightTOLError.text = "" + mRightTOLSensor.Error;
            rightTOLValue.text = "" + mRightTOLSensor.Value;

            chinTOLError.text = "" + mChinTOLSensor.Error;
            chinTOLValue.text = "" + mChinTOLSensor.Value;

            foreHeadTOLError.text = "" + mForeHeadTOLSensor.Error;
            foreHeadTOLValue.text = "" + mForeHeadTOLSensor.Value;

            frontTOLError.text = "" + mFrontTOLSensor.Error;
            frontTOLValue.text = "" + mFrontTOLSensor.Value;

			//Debug.Log("RightIRSensor " + mRightIRSensor.Distance);
        }
    }
}
