using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.DailyInfo
{
    [SerializeField]
    public class InfosData
    {
        /*
         * Type of info displayed as title
         */
        public string Title { get; set; }
        /*
         * List of icons displayed for each item - optional
         */
        public List<string> Icons { get; set; }
        /*
        * List of infos
        */
        public List<InfoData> Infos { get; set; }

    }
}