using BlueQuark;

namespace BuddyApp.TakePhoto
{
    /* Data are stored in xml file for persistent data purpose */
    public sealed class TakePhotoData : AAppData
    {
        /*
         * Data getters / setters
         */
        public bool Overlay { get; set; }
        private string mPhotoPath;
        public string PhotoPath { get { return mPhotoPath; } set { mPhotoPath = value; } }

        /*
         * Data singleton access
         */
        public static TakePhotoData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TakePhotoData>();
                return sInstance as TakePhotoData;
            }
        }
    }
}
