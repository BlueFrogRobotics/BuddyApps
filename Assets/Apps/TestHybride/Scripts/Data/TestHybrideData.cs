using BlueQuark;

namespace BuddyApp.TestHybride
{
    /* Data are stored in xml file for persistent data purpose */
    public class TestHybrideData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TestHybrideData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TestHybrideData>();
                return sInstance as TestHybrideData;
            }
        }
    }
}
