using BlueQuark;

namespace BuddyApp.Wikipedia
{
    /* Data are stored in xml file for persistent data purpose */
    public class WikipediaData : AAppData
    {
        /*
         * Data getters / setters
         */
        public string Utterance { get; set; }

        /*
         * Data singleton access
         */
        public static WikipediaData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<WikipediaData>();
                return sInstance as WikipediaData;
            }
        }
    }
}
