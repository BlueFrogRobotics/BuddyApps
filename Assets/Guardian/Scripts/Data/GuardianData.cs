using Buddy;

namespace BuddyApp.Guardian
{
    public enum GuardianMode : int
    {
        FIXED,
        MOBILE
    }

    public class GuardianData : AAppData
    {
        public enum Contacts : int
        {
            NOBODY,
            RODOLPHE,
            WALID,
            LEONARD,
            J2M,
            MAUD,
            FRANCK,
            BENOIT,
            MARC
        }

        public GuardianData()
        {
            MovementDetectionThreshold = 50;
            SoundDetectionThreshold = 50;

            MovementDetection = true;
            SoundDetection = true;
            FireDetection = true;
            KidnappingDetection = true;
            Contact = Contacts.NOBODY;
        }

        /// <summary>
        /// Actual monitoring mode
        /// </summary>
        public GuardianMode Mode { get; set; }

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
		public Contacts Contact { get; set; }

        public static GuardianData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<GuardianData>();
                return sInstance as GuardianData;
            }
        }

        /// <summary>
        /// Return the mail adress of the actual notifications receiver
        /// </summary>
        public string ContactAddress
        {
            get
            {
                switch (GuardianData.Instance.Contact) {

                    case GuardianData.Contacts.RODOLPHE:
                        return "rh@bluefrogrobotics.com";

                    case GuardianData.Contacts.WALID:
                        return "wa@bluefrogrobotics.com";

                    case GuardianData.Contacts.LEONARD:
                        return "leonardbrun@gmail.com";

                    case GuardianData.Contacts.J2M:
                        return "jmm@bluefrogrobotics.com";

                    case GuardianData.Contacts.MAUD:
                        return "mv@bluefrogrobotics.com";

                    case GuardianData.Contacts.FRANCK:
                        return "fd@bluefrogrobotics.com";

                    case GuardianData.Contacts.BENOIT:
                        return "bp@bluefrogrobotics.com";

                    case GuardianData.Contacts.MARC:
                        return "mg@bluefrogrobotics.com";

                    default:
                        return "";
                }
            }
        }
    }
}
