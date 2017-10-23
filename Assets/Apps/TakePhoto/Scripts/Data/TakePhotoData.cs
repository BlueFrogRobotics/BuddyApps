using Buddy;

namespace BuddyApp.TakePhoto
{
    /* Data are stored in xml file for persistent data purpose */
    public class TakePhotoData : AAppData
    {
        /*
         * Data getters / setters
         */
        public bool Overlay { get; set; }

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
