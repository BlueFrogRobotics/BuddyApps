using BlueQuark;

namespace BuddyApp.TestHDCam
{
    /* Data are stored in xml file for persistent data purpose */
    public class TestHDCamData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TestHDCamData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TestHDCamData>();
                return sInstance as TestHDCamData;
            }
        }
    }
}
