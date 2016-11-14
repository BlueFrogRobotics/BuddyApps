using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class SensorsWindow : MonoBehaviour
    {
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

        }
    }
}
