using Buddy;

namespace BuddyApp.Tutorial
{
    /* Data are stored in xml file for persistent data purpose */
    /* Data are saved when you quit app */
    public class TutorialData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int One { get; set; }
        public string Two { get; set; }
        public bool OneIsActive { get; set; }

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
