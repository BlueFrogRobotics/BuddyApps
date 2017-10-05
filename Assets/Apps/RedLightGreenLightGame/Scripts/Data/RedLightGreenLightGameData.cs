using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    /* Data are stored in xml file for persistent data purpose */
    public class RedLightGreenLightGameData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static RedLightGreenLightGameData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RedLightGreenLightGameData>();
                return sInstance as RedLightGreenLightGameData;
            }
        }
    }
}
