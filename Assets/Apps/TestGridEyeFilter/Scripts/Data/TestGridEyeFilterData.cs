using BlueQuark;

namespace BuddyApp.TestGridEyeFilter
{
    /* Data are stored in xml file for persistent data purpose */
    public class TestGridEyeFilterData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TestGridEyeFilterData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TestGridEyeFilterData>();
                return sInstance as TestGridEyeFilterData;
            }
        }
    }
}
