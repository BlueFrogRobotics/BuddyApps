using Buddy;

namespace BuddyApp.Radioplayer
{
    /* Data are stored in xml file for persistent data purpose */
    public class RadioplayerData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        public string DefaultRadio { get; set; }

        /*
         * Data singleton access
         */
        public static RadioplayerData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RadioplayerData>();
                return sInstance as RadioplayerData;
            }
        }
    }
}
