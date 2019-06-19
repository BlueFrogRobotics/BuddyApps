using BlueQuark;

namespace BuddyApp.OutOfBox
{
    /* Data are stored in xml file for persistent data purpose */
    public class OutOfBoxData : AAppData
    {
        public enum PhaseId
        {
            PhaseOne,
            PhaseTwo,
            PhaseThree,
            PhaseFour,
            PhaseFive,
            PhaseSix
        }

        public PhaseId Phase { get; set; }

        /*
         * Data singleton access
         */
        public static OutOfBoxData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<OutOfBoxData>();
                return sInstance as OutOfBoxData;
            }
        }
    }
}
