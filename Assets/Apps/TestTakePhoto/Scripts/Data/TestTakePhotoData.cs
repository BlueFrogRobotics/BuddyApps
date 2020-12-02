using BlueQuark;

namespace BuddyApp.TestTakePhoto
{
    /* Data are stored in xml file for persistent data purpose */
    public class TestTakePhotoData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TestTakePhotoData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TestTakePhotoData>();
                return sInstance as TestTakePhotoData;
            }
        }
    }
}
