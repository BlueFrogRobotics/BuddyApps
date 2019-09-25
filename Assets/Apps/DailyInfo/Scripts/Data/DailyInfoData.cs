using BlueQuark;

namespace BuddyApp.DailyInfo
{
    /* Data are stored in xml file for persistent data purpose */
    public class DailyInfoData : AAppData
    {
        /*
         * Name of the infos file
         */
        public string InfosFileName { get; set; }
        /*
         * Vocal request
         */
        public string VocalRequest { get; set; }

        /*
         * Data singleton access
         */
        public static DailyInfoData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<DailyInfoData>();
                return sInstance as DailyInfoData;
            }
        }
    }
}
