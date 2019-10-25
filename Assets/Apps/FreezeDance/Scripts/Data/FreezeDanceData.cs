using BlueQuark;

namespace BuddyApp.FreezeDance
{
    /* Data are stored in xml file for persistent data purpose */
    public class FreezeDanceData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static FreezeDanceData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<FreezeDanceData>();
                return sInstance as FreezeDanceData;
            }
        }
    }
}
