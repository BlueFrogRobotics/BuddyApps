using Buddy;

namespace BuddyApp.BasicApp
{
    /* Data are stored in xml file for persistent data purpose */
    /* Data are saved when you quit app */
    public class BasicAppData : AAppData
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
        public static BasicAppData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BasicAppData>();
                return sInstance as BasicAppData;
            }
        }
    }
}
