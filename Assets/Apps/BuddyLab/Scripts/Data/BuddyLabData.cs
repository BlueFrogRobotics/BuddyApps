using BlueQuark;

namespace BuddyApp.BuddyLab
{
    /* Data are stored in xml file for persistent data purpose */
    public class BuddyLabData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static BuddyLabData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BuddyLabData>();
                return sInstance as BuddyLabData;
            }
        }
    }
}
