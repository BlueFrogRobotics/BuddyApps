using BlueQuark;

namespace BuddyApp.Videoagoraio
{
    /* Data are stored in xml file for persistent data purpose */
    public class VideoagoraioData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static VideoagoraioData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<VideoagoraioData>();
                return sInstance as VideoagoraioData;
            }
        }
    }
}
