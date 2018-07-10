using Buddy;

namespace BuddyApp.TakePose
{
    /* Data are stored in xml file for persistent data purpose */
    public class TakePoseData : AAppData
    {
        /*
         * Data getters / setters
         */



        /*
         * Data singleton access
         */
        public static TakePoseData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TakePoseData>();
                return sInstance as TakePoseData;
            }
        }
    }
}
