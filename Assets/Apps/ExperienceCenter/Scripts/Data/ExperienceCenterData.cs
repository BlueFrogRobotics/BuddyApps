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

		// EnableMovement 
		public bool EnableHeadMovement { get; set; }
		public bool EnableBaseMovement { get; set; }
		public float StopDistance{ get; set; }
		public float NoiseTime{ get; set; }
		public float TableDistance{ get; set; }
		public float IOTDistance{ get; set; }
		public float HeadPoseTolerance{ get; set; }
		public float WelcomeTimeOut{ get; set; }
		public float MoveTimeOut{ get; set; }
		public float NoHingeAngle{ get; set; }
		public float NoHingeSpeed{ get; set; }
		public bool VoiceTrigger{ get; set; }
		public bool RunTrigger{ get; set; }
		public bool EnableBML{ get; set; }
		public float MaxSpeed{ get; set; }
		public float MinSpeed{ get; set; }
		public float MaxDistance{ get; set; }
		public float MinDistance{ get; set; }
		public bool CollisionDebug{ get; set; }


		// Scenario
		public string Scenario { get; set; }

		// TCP Server 
		public string IPAddress { get; set; }
		public string StatusTcp { get; set; }

        public string Language { get; set; }

        public bool ShouldSendCommand { get; set; }
		public ExperienceCenter.Command Command { get; set; }

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
