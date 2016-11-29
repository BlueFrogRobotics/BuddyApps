using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class SensorsWindow : MonoBehaviour
    {
        [SerializeField]
        private Text leftUSError;

        [SerializeField]
        private Text rightUSError;

        [SerializeField]
        private Text backUSError;

        [SerializeField]
        private Text leftIRError;

        [SerializeField]
        private Text middleIRError;

        [SerializeField]
        private Text rightIRError;

        [SerializeField]
        private Text leftUSValue;

        [SerializeField]
        private Text rightUSValue;

        [SerializeField]
        private Text backUSValue;

        [SerializeField]
        private Text leftIRValue;

        [SerializeField]
        private Text middleIRValue;

        [SerializeField]
        private Text rightIRValue;

        private USSensor mLeftUSSensor;
        private USSensor mRightUSSensor;
        private USSensor mBackUSSensor;
        private IRSensor mLeftIRSensor;
        private IRSensor mMiddleIRSensor;
        private IRSensor mRightIRSensor;

        void Start()
        {
            mLeftUSSensor = BYOS.Instance.USSensors.Left;
            mRightUSSensor = BYOS.Instance.USSensors.Right;
            mBackUSSensor = BYOS.Instance.USSensors.Back;
            mLeftIRSensor = BYOS.Instance.IRSensors.Left;
            mMiddleIRSensor = BYOS.Instance.IRSensors.Middle;
            mRightIRSensor = BYOS.Instance.IRSensors.Right;
        }

        void Update()
        {
            leftUSError.text = "" + mLeftUSSensor.Error;
            rightUSError.text = "" + mRightUSSensor.Error;
            backUSError.text = "" + mBackUSSensor.Error;
            leftUSValue.text = "" + mLeftUSSensor.Value;
            rightUSValue.text = "" + mRightUSSensor.Value;
            backUSValue.text = "" + mBackUSSensor.Value;

            leftIRError.text = "" + mLeftIRSensor.Error;
            middleIRError.text = "" + mMiddleIRSensor.Error;
            rightIRError.text = "" + mRightIRSensor.Error;
            leftIRValue.text = "" + mLeftIRSensor.Value;
            middleIRValue.text = "" + mMiddleIRSensor.Value;
            rightIRValue.text = "" + mRightIRSensor.Value;
        }
    }
}
