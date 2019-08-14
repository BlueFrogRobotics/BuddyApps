using BlueQuark;

namespace BuddyApp.OutOfBoxV3
{
    /* Data are stored in xml file for persistent data purpose */
    public class OutOfBoxV3Data : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static OutOfBoxV3Data Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<OutOfBoxV3Data>();
                return sInstance as OutOfBoxV3Data;
            }
        }
    }
}
