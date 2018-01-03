using Buddy;

namespace BuddyApp.Timer
{
    /* Data are stored in xml file for persistent data purpose */
    public class TimerData : AAppData
    {
        /*
         * Data getters / setters
         */
        public string VocalRequest { get; set; }

        /*
         * Data singleton access
         */
        public static TimerData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TimerData>();
                return sInstance as TimerData;
            }
        }
    }
}
