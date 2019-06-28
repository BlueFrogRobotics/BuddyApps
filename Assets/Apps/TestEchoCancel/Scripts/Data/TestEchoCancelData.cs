using BlueQuark;

namespace BuddyApp.TestEchoCancel
{
    /* Data are stored in xml file for persistent data purpose */
    public class TestEchoCancelData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TestEchoCancelData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TestEchoCancelData>();
                return sInstance as TestEchoCancelData;
            }
        }
    }
}
