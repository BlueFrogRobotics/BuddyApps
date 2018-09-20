using BlueQuark;

namespace BuddyApp.Weather
{
    /* Data are stored in xml file for persistent data purpose */
    public sealed class WeatherData : AAppData
    {
        /*
         * Data getters / setters
         */
        public string VocalRequest { get; set; }

        /*
         * Data singleton access
         */
        public static WeatherData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<WeatherData>();
                return sInstance as WeatherData;
            }
        }
    }
}
