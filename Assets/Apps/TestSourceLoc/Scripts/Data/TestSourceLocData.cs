using BlueQuark;

namespace BuddyApp.TestSourceLoc
{
    /* Data are stored in xml file for persistent data purpose */
    public class TestSourceLocData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TestSourceLocData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TestSourceLocData>();
                return sInstance as TestSourceLocData;
            }
        }
    }
}
