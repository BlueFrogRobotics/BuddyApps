using BlueQuark;

namespace BuddyApp.TestGridEye
{
    /* Data are stored in xml file for persistent data purpose */
    public class TestGridEyeData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TestGridEyeData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TestGridEyeData>();
                return sInstance as TestGridEyeData;
            }
        }
    }
}
