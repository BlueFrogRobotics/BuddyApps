using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace BuddyApp.DailyInfo
{

    [SerializeField]
    public class InfoData
    {
        /*
         * Day of the info
         */
        public DateTime Day { get; set; }
        /*
         * Specific part of day like lunch, morning, etc - optional
         */
        public string DayPart { get; set; }
        /*
         * List of items isplayed in the menu and said by Buddy
         */
        public List<string> Items { get; set; }
    }
}