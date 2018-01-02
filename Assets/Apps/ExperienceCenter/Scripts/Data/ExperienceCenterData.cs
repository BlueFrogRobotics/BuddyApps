using Buddy;

namespace BuddyApp.ExperienceCenter
{
    /* Data are stored in xml file for persistent data purpose */
    public class ExperienceCenterData : AAppData
    {
        // Somfy Rest API configuration
		public string API_URL { get; set; }
		public string UserID { get; set; }
		public string Password { get; set; }

		public bool ShouldTestIOT { get; set; }
        // IOT devices commands
		public bool LightState { get; set; }
		public bool SonosState { get; set; }
		public bool StoreState { get; set; }
        
        // IOT devices states
        public bool IsLightOn { get; set; }
        public bool IsStoreDeployed { get; set; }
        public bool IsMusicOn { get; set; }

        public string Language { get; set; }

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
