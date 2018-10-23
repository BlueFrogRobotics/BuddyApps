using BlueQuark;

namespace BuddyApp.HumanCounter
{
    public enum DetectionOption
    {
        HUMAN_DETECT,
        FACE_DETECT,
        SKELETON_DETECT
    }

    /* Data are stored in xml file for persistent data purpose */
    public class HumanCounterData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int ObservationTime { get; set; }
        public DetectionOption DetectionOption { get; set; }

        //public bool humanDetectToggle { get; set; }

        
        /*
         * Data singleton access
         */
        public static HumanCounterData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<HumanCounterData>();
                return sInstance as HumanCounterData;
            }
        }
    }
}
