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
            HeadRotation = true;
            Contact = Contacts.NOBODY;
        }

        public GuardianMode Mode { get; set; }

        public int MovementDetectionThreshold { get; set; }

        public int SoundDetectionThreshold { get; set; }

        public bool MovementDetection { get; set; }

        public bool SoundDetection { get; set; }

        public bool FireDetection { get; set; }

        public bool KidnappingDetection { get; set; }

        public bool HeadRotation { get; set; }

        public bool HeadOrientation { get; set; }

        public bool MovementDebug { get; set; }

        public bool SoundDebug { get; set; }

        public bool FireDebug { get; set; }

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
