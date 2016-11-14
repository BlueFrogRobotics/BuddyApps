using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class MotorsWindow : MonoBehaviour
    {
        private Wheels mWheels;
        private Hinge mYesHinge;
        private Hinge mNoHinge;

        void Start()
        {
            mWheels = BYOS.Instance.Motors.Wheels;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
        }

        void Update()
        {
        }
    }
}
