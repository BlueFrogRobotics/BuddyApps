using Buddy;

namespace BuddyApp.ExperienceCenter
{
    /* Data are stored in xml file for persistent data purpose */
    public class ExperienceCenterData : AAppData
    {
        /*
         * Data getters / setters
         */
		public string API_URL{ get; set; }
		public string UserID { get; set; }
		public string Password { get; set; }

        /*
         * Data singleton access
         */
        public static ExperienceCenterData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<ExperienceCenterData>();
                return sInstance as ExperienceCenterData;
            }
        }
    }
}
