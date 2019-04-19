using BlueQuark;

namespace BuddyApp.AudioBehavior
{
    /* Data are stored in xml file for persistent data purpose */
    public class AudioBehaviorData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static AudioBehaviorData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<AudioBehaviorData>();
                return sInstance as AudioBehaviorData;
            }
        }
    }
}
