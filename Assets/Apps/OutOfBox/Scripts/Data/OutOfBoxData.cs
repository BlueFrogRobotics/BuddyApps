using BlueQuark;

namespace BuddyApp.OutOfBox
{
    /* Data are stored in xml file for persistent data purpose */
    public class OutOfBoxData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static OutOfBoxData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<OutOfBoxData>();
                return sInstance as OutOfBoxData;
            }
        }
    }
}
