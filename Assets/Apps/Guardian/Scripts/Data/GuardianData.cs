using Buddy;

namespace BuddyApp.Guardian
{
	//public enum GuardianMode : int
	//{
	//    FIXED,
	//    MOBILE
	//}

	public class GuardianData : AAppData
	{

		public GuardianData()
		{
			MovementDetectionThreshold = 50;
			SoundDetectionThreshold = 50;

            FirstRun = true;
            FirstRunParam = true;
            MobileDetection = true;
			MovementDetection = true;
			SoundDetection = true;
			FireDetection = true;
			KidnappingDetection = true;
			SendMail = true;
			Contact = new UserAccount();
			Contact.FirstName = "NONE";
        }

        /// <summary>
        /// Actual monitoring mode
        /// </summary>
        //public GuardianMode Mode { get; set; }

        /// <summary>
        /// Tells if param as already be launched
        /// </summary>
        public bool FirstRunParam { get; set; }

        /// <summary>
        /// Tells if app was already launched
        /// </summary>
        public bool FirstRun { get; set; }

		/// <summary>
		/// Tells if user want fix or mobile detection
		/// </summary>
		public bool MobileDetection { get; set; }


		/// <summary>
		/// Tells if user want to enable scan detection
		/// </summary>
		public bool ScanDetection { get; set; }


		/// <summary>
		/// Threshold of the movement dector
		/// </summary>
		public int MovementDetectionThreshold { get; set; }

		/// <summary>
		/// Threshold of the noise dector
		/// </summary>
		public int SoundDetectionThreshold { get; set; }

		/// <summary>
		/// Tells if movement detection is activated
		/// </summary>
		public bool MovementDetection { get; set; }

		/// <summary>
		/// Tells if noise detection is activated
		/// </summary>
		public bool SoundDetection { get; set; }

		/// <summary>
		/// Tells if fire detection is activated
		/// </summary>
		public bool FireDetection { get; set; }

		/// <summary>
		/// Tells if kidnapping detection is activated
		/// </summary>
		public bool KidnappingDetection { get; set; }

		/// <summary>
		/// Tells if mail sending is activated
		/// </summary>
		public bool SendMail { get; set; }

		/// <summary>
		/// Is true when the button head orientation setup in the parameter has been pressed
		/// </summary>
		public bool HeadOrientation { get; set; }

		/// <summary>
		/// Is true when the button movement detection test in the parameter has been pressed
		/// </summary>
		public bool MovementDebug { get; set; }

		/// <summary>
		/// Is true when the button noise detection test in the parameter has been pressed
		/// </summary>
		public bool SoundDebug { get; set; }

		/// <summary>
		/// Is true when the button fire detection test in the parameter has been pressed
		/// </summary>
		public bool FireDebug { get; set; }

		/// <summary>
		/// Is true when the button add contact in the parameter has been pressed
		/// </summary>
		public bool AddContact { get; set; }

		/// <summary>
		/// Contact who will receive the notifications
		/// </summary>
		public UserAccount Contact { get; set; }


		public static GuardianData Instance
		{
			get
			{
				if (sInstance == null)
					sInstance = GetInstance<GuardianData>();
				return sInstance as GuardianData;
			}
		}
	}
}
