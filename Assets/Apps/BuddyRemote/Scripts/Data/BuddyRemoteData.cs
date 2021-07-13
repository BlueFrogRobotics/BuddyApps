using BlueQuark;

namespace BuddyApp.BuddyRemote
{
    /* Data are stored in xml file for persistent data purpose */
    public class BuddyRemoteData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static BuddyRemoteData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BuddyRemoteData>();
                return sInstance as BuddyRemoteData;
            }
        }
    }
}
