using BlueQuark;

namespace BuddyApp.ZohoTest
{
    /* Data are stored in xml file for persistent data purpose */
    public class ZohoTestData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static ZohoTestData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<ZohoTestData>();
                return sInstance as ZohoTestData;
            }
        }
    }
}
