using UnityEngine;
using BuddyOS;

namespace BuddyApp.Basic
{
    /* Data are stored in xml file for persistent data purpose */
    /* Data base values are retrieve from the default_app_data.xml file in persistent data path. Streamingassets folder otherwise */
    /* Data are saved when you quit app. Data are saved in persistent data path */
    public class BasicAppData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int One { get; set; }
        public string Two { get; set; }
        public bool OneIsActive { get; set; }

        /*
         * Data singleton access
         */
        public static BasicAppData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BasicAppData>();
                return sInstance as BasicAppData;
            }
        }
    }
}
