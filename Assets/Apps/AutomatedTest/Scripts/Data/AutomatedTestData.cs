using BlueQuark;
using System.Collections.Generic;

namespace BuddyApp.AutomatedTest
{
    /* Data are stored in xml file for persistent data purpose */
    public class AutomatedTestData : AAppData
    {
        /*
         * Data singleton access
         */
        public static AutomatedTestData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<AutomatedTestData>();
                return sInstance as AutomatedTestData;
            }
        }
    }
}
