using Buddy;

namespace BuddyApp.PlayMath
{
    /* Data are stored in xml file for persistent data purpose */
    public class PlayMathData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static PlayMathData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<PlayMathData>();
                return sInstance as PlayMathData;
            }
        }
    }
}
