using Buddy;

namespace BuddyApp.Boot
{
    /* Data are stored in xml file for persistent data purpose */
    public class BootData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static BootData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BootData>();
                return sInstance as BootData;
            }
        }
    }
}
