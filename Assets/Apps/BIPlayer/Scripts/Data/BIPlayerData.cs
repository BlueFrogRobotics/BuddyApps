using BlueQuark;

namespace BuddyApp.BIPlayer
{
    /* Data are stored in xml file for persistent data purpose */
    public class BIPlayerData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static BIPlayerData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BIPlayerData>();
                return sInstance as BIPlayerData;
            }
        }
    }
}
