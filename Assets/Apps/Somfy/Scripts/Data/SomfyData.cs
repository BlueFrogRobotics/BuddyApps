using Buddy;

namespace BuddyApp.Somfy
{
    /* Data are stored in xml file for persistent data purpose */
    public class SomfyData : AAppData
    {
        /*
         * Data getters / setters
         */
        public string Login { get; set; }
        public string Password { get; set; }

        public string URL_API { get; set; }
        public string VocalRequest { get; set; }

        /*
         * Data singleton access
         */
        public static SomfyData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<SomfyData>();
                return sInstance as SomfyData;
            }
        }
    }
}
