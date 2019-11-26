using BlueQuark;

namespace BuddyApp.BabyPhone
{
    /* Data are stored in xml file for persistent data purpose */
    public class BabyPhoneData : AAppData
    {
        public BabyPhoneData()
        {
            MovementDetectionThreshold = 50;
            SoundDetectionThreshold = 50;

            FirstRun = true;
            FirstRunParam = true;
            MovementDetection = true;
            SoundDetection = true;
            SendMail = true;
            Contact = new UserAccount();
            Contact.FirstName = "NONE";
            ContactId = -1;
            BabyName = "";
            PlayLullaby = true;
            LullabyDuration = 1.0F;
            ReplayOnDetection = false;
            HeadPosition = 15;
        }

        public int Angle { get; set; }

        /// <summary>
        /// Tells if param as already be launched
        /// </summary>
        public bool FirstRunParam { get; set; }

        /// <summary>
        /// Tells if app was already launched
        /// </summary>
        public bool FirstRun { get; set; }

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
        /// Tells if mail sending is activated
        /// </summary>
        public bool SendMail { get; set; }

        /// <summary>
        /// Is true when the button add contact in the parameter has been pressed
        /// </summary>
        public bool AddContact { get; set; }

        /// <summary>
        /// Contact who will receive the notifications
        /// </summary>
        public UserAccount Contact { get; set; }

        /// <summary>
        /// Id of the selected contact that will receive the alert
        /// </summary>
        public int ContactId { get; set; }

        /// <summary>
        /// Name of the baby to personalize the notification email
        /// </summary>
        public string BabyName { get; set; }

        /// <summary>
        /// If true the lullaby is played before starting the detection
        /// </summary>
        public bool PlayLullaby { get; set; }

        /// <summary>
        /// Duration of the lullaby in seconds
        /// </summary>
        public float LullabyDuration { get; set; }

        /// <summary>
        /// If true the lullaby is played again after each detection of noise or movement
        /// </summary>
        public bool ReplayOnDetection { get; set; }

        /// <summary>
        /// Yes position of the head in degrees
        /// </summary>
        public float HeadPosition { get; set; }

        /*
         * Data singleton access
         */
        public static BabyPhoneData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BabyPhoneData>();
                return sInstance as BabyPhoneData;
            }
        }
    }
}
