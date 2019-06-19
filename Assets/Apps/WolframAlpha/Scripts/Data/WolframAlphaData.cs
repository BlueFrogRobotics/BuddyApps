using BlueQuark;

namespace BuddyApp.WolframAlpha
{
    /* Data are stored in xml file for persistent data purpose */
    public class WolframAlphaData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static WolframAlphaData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<WolframAlphaData>();
                return sInstance as WolframAlphaData;
            }
        }
    }
}
