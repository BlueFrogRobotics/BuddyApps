using BlueQuark;

namespace BuddyApp.Korian
{
    /* Data are stored in xml file for persistent data purpose */
    public class KorianData : AAppData
    {
        /*
         * Data getters / setters
         */
        public enum MailType
        {
            NONE,
            MAILA,
            MAILB
        }

        public MailType Mail { get; set; }
        /*
         * Data singleton access
         */
        public static KorianData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<KorianData>();
                return sInstance as KorianData;
            }
        }
    }
}
