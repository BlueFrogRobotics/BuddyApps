using BlueQuark;

namespace BuddyApp.HumanCounter
{
    /* Data are stored in xml file for persistent data purpose */
    public class HumanCounterData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int observationTime { get; set; }

        /*
         * Data singleton access
         */
        public static HumanCounterData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<HumanCounterData>();
                return sInstance as HumanCounterData;
            }
        }
    }
}
