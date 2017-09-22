using Buddy;

namespace BuddyApp.Jukebox
{
    /* Data are stored in xml file for persistent data purpose */
    public class JukeboxData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static JukeboxData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<JukeboxData>();
                return sInstance as JukeboxData;
            }
        }
    }
}
