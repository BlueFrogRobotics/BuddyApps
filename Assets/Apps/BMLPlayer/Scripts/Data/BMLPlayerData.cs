using Buddy;

namespace BuddyApp.BMLPlayer
{
    /* Data are stored in xml file for persistent data purpose */
    public class BMLPlayerData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static BMLPlayerData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BMLPlayerData>();
                return sInstance as BMLPlayerData;
            }
        }
    }
}
