using BlueQuark;

namespace BuddyApp.ChangeXMLTeleBuddy
{
    /* Data are stored in xml file for persistent data purpose */
    public class ChangeXMLTeleBuddyData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static ChangeXMLTeleBuddyData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<ChangeXMLTeleBuddyData>();
                return sInstance as ChangeXMLTeleBuddyData;
            }
        }
    }
}
