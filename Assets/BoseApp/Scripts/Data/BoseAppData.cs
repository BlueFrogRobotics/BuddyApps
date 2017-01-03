using BuddyOS;
using BuddyOS.App;

namespace BuddyApp.BoseApp
{
    /* Data are stored in xml file for persistent data purpose */
    /* Data are saved when you quit app inside the persistent data path */
    public class BoseAppData : AAppData
    {
        /*
         * Data getters / setters
         */
        public string recastToken { get; set; }
        public string recastLangage { get; set; }
        public string boseAddr { get; set; }
        public string playlistOne { get; set; }
        public string playlistTwo { get; set; }
        public string playlistThree { get; set; }
        public string playlistFour { get; set; }
        public string playlistFive { get; set; }
        public string playlistSix { get; set; }
        public string refresh_token { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }


        /*
         * Data singleton access
         */
        public static BoseAppData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BoseAppData>();
                return sInstance as BoseAppData;
            }
        }
    }
}