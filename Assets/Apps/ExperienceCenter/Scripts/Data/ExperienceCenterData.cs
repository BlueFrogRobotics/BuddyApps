using Buddy;

namespace BuddyApp.ExperienceCenter
{
    /* Data are stored in xml file for persistent data purpose */
    public class ExperienceCenterData : AAppData
    {
		public ExperienceCenterData()
		{
			ShouldTestIOT = false;
			Command = "";
		}

        /*
         * Data getters / setters
         */
		public string API_URL{ get; set; }
		public string UserID { get; set; }
		public string Password { get; set; }
		public bool ShouldTestIOT { get; set; }
		public bool LightState { get; set; }
		public bool SonosState { get; set; }
		public bool StoreState { get; set; }

		public string Command { get; set; }

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
