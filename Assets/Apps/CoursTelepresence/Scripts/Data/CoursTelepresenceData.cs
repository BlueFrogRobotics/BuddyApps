using BlueQuark;

namespace BuddyApp.CoursTelepresence
{
    /* Data are stored in xml file for persistent data purpose */
    public class CoursTelepresenceData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        public string Ping { get; set; }
        /*
         * Data singleton access
         */
        public static CoursTelepresenceData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<CoursTelepresenceData>();
                return sInstance as CoursTelepresenceData;
            }
        }
    }
}
