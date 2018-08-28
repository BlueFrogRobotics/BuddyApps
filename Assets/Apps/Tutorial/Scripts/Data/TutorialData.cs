using BlueQuark;

namespace BuddyApp.Tutorial
{
    /* Data are stored in xml file for persistent data purpose */
    public class TutorialData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TutorialData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TutorialData>();
                return sInstance as TutorialData;
            }
        }
    }
}
