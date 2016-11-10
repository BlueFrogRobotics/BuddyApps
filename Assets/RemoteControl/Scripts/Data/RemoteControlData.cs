using UnityEngine;
using BuddyOS;

namespace BuddyApp.Remote
{
    /* Data are stored in xml file for persistent data purpose */
    /* Data base values are retrieve from the default_app_data.xml file in persistent data path. Streamingassets folder otherwise */
    /* Data are saved when you quit app. Data are saved in persistent data path */
    public class RemoteControlData : AAppData
    {
        /*
         * Data getters / setters
         */
        public bool IsActive { get; set; }

        /*
         * Data singleton access
         */
        public static RemoteControlData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RemoteControlData>();
                return sInstance as RemoteControlData;
            }
        }
    }
}
