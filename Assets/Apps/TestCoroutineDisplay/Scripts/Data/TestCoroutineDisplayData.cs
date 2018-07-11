using BlueQuark;

namespace BuddyApp.TestCoroutineDisplay
{
    /* Data are stored in xml file for persistent data purpose */
    public class TestCoroutineDisplayData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TestCoroutineDisplayData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TestCoroutineDisplayData>();
                return sInstance as TestCoroutineDisplayData;
            }
        }
    }
}
