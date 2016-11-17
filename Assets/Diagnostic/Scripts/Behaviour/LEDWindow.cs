using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class LEDWindow : MonoBehaviour
    {
        private LED mLED;

        void Start()
        {
            mLED = BYOS.Instance.LED;
        }

        void Update()
        {

        }
    }
}
