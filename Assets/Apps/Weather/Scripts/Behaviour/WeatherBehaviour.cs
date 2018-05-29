using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Weather
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class WeatherBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quittedq
         */
        private WeatherData mAppData;

        internal int mIndice;
        internal WeatherInfo[] mWeatherInfos;
        internal WeatherRequestError mRequestError = 0;

        internal enum WeatherRequestError
        {
            UNKNOWN,

            NONE,

            TOO_FAR
        }

        internal enum WeatherCommand : int
        {
            NONE = 0,
            MIN = 1,
            MAX = 2
        }

        internal enum WeatherMoment
        {
            NONE,
            MORNING,
            NOON,
            AFTERNOON,
            EVENING
        }

        internal string mVocalRequest;
        internal string mLocation;
        internal string mName;
        internal WeatherType mForecast;
        internal int mDate;
        internal int mHour;

        /// <summary>
        /// If the User ask with When keyword
        /// </summary>
        internal bool mWhen;
        internal CitiesData mCities = new CitiesData();
        internal bool mIsOk = true;

        /// <summary>
        /// If the User ask weather for the Weekend
        /// </summary>
        internal bool mWeekend;
        internal WeatherCommand mCommand;

        /// <summary>
        /// Define the Moment of the Weather : Morning, Noon, Afternoon or Evening
        /// </summary>
        internal WeatherMoment mWeatherTime;


        void Start()
        {
            /*
			* You can setup your App activity here.
			*/
            WeatherActivity.Init(null);

            /*
			* Init your app data
			*/
            mAppData = WeatherData.Instance;
        }
    }
}