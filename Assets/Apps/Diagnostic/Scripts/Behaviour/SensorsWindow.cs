using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Buddy;

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
            mLeftUSSensor = BYOS.Instance.Primitive.USSensors.Left;
            mRightUSSensor = BYOS.Instance.Primitive.USSensors.Right;
            mBackUSSensor = BYOS.Instance.Primitive.USSensors.Back;
            mLeftIRSensor = BYOS.Instance.Primitive.IRSensors.Left;
            mMiddleIRSensor = BYOS.Instance.Primitive.IRSensors.Middle;
            mRightIRSensor = BYOS.Instance.Primitive.IRSensors.Right;
        }

        void Update()
        {
            leftUSError.text = "" + mLeftUSSensor.Error;
            rightUSError.text = "" + mRightUSSensor.Error;
            backUSError.text = "" + mBackUSSensor.Error;
            leftUSValue.text = "" + mLeftUSSensor.Distance;
            rightUSValue.text = "" + mRightUSSensor.Distance;
            backUSValue.text = "" + mBackUSSensor.Distance;

            leftIRError.text = "" + mLeftIRSensor.Error;
            middleIRError.text = "" + mMiddleIRSensor.Error;
            rightIRError.text = "" + mRightIRSensor.Error;
            leftIRValue.text = "" + mLeftIRSensor.Distance;
            middleIRValue.text = "" + mMiddleIRSensor.Distance;
            rightIRValue.text = "" + mRightIRSensor.Distance;
			Debug.Log("RightIRSensor " + mRightIRSensor.Distance);
        }
    }
}
