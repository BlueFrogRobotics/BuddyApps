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

        // mail adress to send picture to
        private string mmailtoshare;
        public string mailtoshare { get { return mmailtoshare; } set { mmailtoshare = value; } }

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
