using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public class ThermalWindow : MonoBehaviour
    {
        private LEDs mLED;
        private Face mFace;
        private Wheels mWheels;
        private YesHeadHinge mYesHinge;
        private NoHeadHinge mNoHinge;

        //private ThermalSensor mThermalSensor;

        void Start()
        {
            //mThermalSensor = BYOS.Instance.Primitive.ThermalSensor;
        }

        void Update()
        {
        }
    }
}
