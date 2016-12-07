using UnityEngine;
using BuddyOS;
using BuddyOS.App;

namespace BuddyApp.IOT
{
    /* Data are stored in xml file for persistent data purpose */
    /* Data base values are retrieve from the basic_app_data.xml file in persistent data path. Streamingassets folder otherwise */
    /* Data are saved when you quit app. Data are saved in persistent data path */
    public class IOTData : AAppData
    {
        /*
         * Data getters / setters
         */

        /*
         * Data singleton access
         */
        public static IOTData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<IOTData>();
                return sInstance as IOTData;
            }
        }
    }
}
