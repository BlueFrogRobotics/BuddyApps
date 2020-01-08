using BlueQuark;

namespace BuddyApp.TestDepthNav
{
    /* Data are stored in xml file for persistent data purpose */
    public class TestDepthNavData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TestDepthNavData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TestDepthNavData>();
                return sInstance as TestDepthNavData;
            }
        }
    }
}
