using BlueQuark;

namespace BuddyApp.FreezeDance
{
    /* Data are stored in xml file for persistent data purpose */
    public class FreezeDanceData : AAppData
    {
        /*
         * Number of music pauses before Buddy starts moving
         */
        public int NbPauseBeforeMvt { get; set; }

        /*
         * Number of music pauses before Buddy starts moving its head
         */
        public int NbPauseBeforeHeadMvt { get; set; }

        /*
         * Data singleton access
         */
        public static FreezeDanceData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<FreezeDanceData>();
                return sInstance as FreezeDanceData;
            }
        }
    }
}
