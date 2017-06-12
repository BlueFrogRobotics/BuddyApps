using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Buddy;

namespace BuddyApp.Diagnostic
{
    public class ThermalWindow : MonoBehaviour
    {
        private LED mLED;
        private Face mFace;
        private Wheels mWheels;
        private Hinge mYesHinge;
        private Hinge mNoHinge;

        private ThermalSensor mThermalSensor;

        void Start()
        {
            mThermalSensor = BYOS.Instance.ThermalSensor;
        }

        void Update()
        {
        }
    }
}
