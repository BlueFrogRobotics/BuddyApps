using Buddy;

namespace BuddyApp.SharedWIP
{
    /* Data are stored in xml file for persistent data purpose */
    public class SharedWIPData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static SharedWIPData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<SharedWIPData>();
                return sInstance as SharedWIPData;
            }
        }
    }
}
