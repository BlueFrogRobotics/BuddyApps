using BlueQuark;

namespace BuddyApp.Gallery
{
    /* Data are stored in xml file for persistent data purpose */
    public class GalleryData : AAppData
    {
        /*
         * Data getters / setters
         */
        public string mailshare { get; set; }

        /*
         * Data singleton access
         */
        public static GalleryData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<GalleryData>();
                return sInstance as GalleryData;
            }
        }
    }
}
