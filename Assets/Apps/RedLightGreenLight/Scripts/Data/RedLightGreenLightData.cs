using Buddy;

namespace BuddyApp.RedLightGreenLight
{
    /* Data are stored in xml file for persistent data purpose */
    public class RedLightGreenLightData : AAppData
    {

        /*
         * Data getters / setters
         */
        public int Difficulty { get; set; }

        /*
         * Data singleton access
         */
        public static RedLightGreenLightData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RedLightGreenLightData>();
                return sInstance as RedLightGreenLightData;
            }
        }
    }
}
