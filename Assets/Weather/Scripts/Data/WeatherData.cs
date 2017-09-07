using Buddy;

namespace BuddyApp.Weather
{
    /* Data are stored in xml file for persistent data purpose */
    public class WeatherData : AAppData
    {
        /*
         * Data getters / setters
         */
        public string VocalRequest { get; set; }
		public string Location { get; set; }
		public string Forecast { get; set; }
		public int Date { get; set; }
		public bool When { get; set; }

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
