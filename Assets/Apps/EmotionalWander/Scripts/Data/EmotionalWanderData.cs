using Buddy;

namespace BuddyApp.EmotionalWander
{
    /* Data are stored in xml file for persistent data purpose */
    public class EmotionalWanderData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static EmotionalWanderData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<EmotionalWanderData>();
                return sInstance as EmotionalWanderData;
            }
        }
    }
}
