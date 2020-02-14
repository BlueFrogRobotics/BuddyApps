using BlueQuark;

namespace BuddyApp.Agoraio
{
    /* Data are stored in xml file for persistent data purpose */
    public class AgoraioData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static AgoraioData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<AgoraioData>();
                return sInstance as AgoraioData;
            }
        }
    }
}
