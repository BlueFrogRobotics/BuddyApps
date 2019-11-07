using BlueQuark;


using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.Weather
{

    public sealed class PanelInfo : MonoBehaviour
    {
        [SerializeField]
        internal Text TimeStart;
        [SerializeField]
        internal Text TimeEnd;

        public void SetTime(int value)
        {
            if (TimeStart)
                TimeStart.text = value.ToString("00");
            if (TimeEnd)
            {
                int endValue = value + 3;
                if (endValue > 24) endValue = 24;
                TimeEnd.text = endValue.ToString("00");
            }
        }
    }
}