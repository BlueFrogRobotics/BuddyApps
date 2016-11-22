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
            leftUSError.text = "Left US err : " + mLeftUSSensor.Error;
            rightUSError.text = "Right US err : " + mRightUSSensor.Error;
            backUSError.text = "Back US err : " + mBackUSSensor.Error;
            leftUSValue.text = "Left US val : " + mLeftUSSensor.Value;
            rightUSValue.text = "Right US val : " + mRightUSSensor.Value;
            backUSValue.text = "Back US val : " + mBackUSSensor.Value;

            leftIRError.text = "Left IR err : " + mLeftIRSensor.Error;
            middleIRError.text = "Middle IR err : " + mMiddleIRSensor.Error;
            rightIRError.text = "Right IR err : " + mRightIRSensor.Error;
            leftIRValue.text = "Left IR val : " + mLeftIRSensor.Value;
            middleIRValue.text = "Middle IR val : " + mMiddleIRSensor.Value;
            rightIRValue.text = "Right IR val : " + mRightIRSensor.Value;
        }
    }
}
