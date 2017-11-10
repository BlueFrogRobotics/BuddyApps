using Buddy;

namespace BuddyApp.RecolLoc
{
    /* Data are stored in xml file for persistent data purpose */
    public class RecolLocData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static RecolLocData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RecolLocData>();
                return sInstance as RecolLocData;
            }
        }
    }
}
