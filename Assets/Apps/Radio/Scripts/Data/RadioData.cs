using BlueQuark;

namespace BuddyApp.Radio
{
    /* Data are stored in xml file for persistent data purpose */
    public class RadioData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static RadioData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RadioData>();
                return sInstance as RadioData;
            }
        }
    }
}
