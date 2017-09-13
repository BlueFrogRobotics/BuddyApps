using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Weather
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class WeatherBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private WeatherData mAppData;

		internal WeatherInfo mWeatherInfo;

		internal string mVocalRequest;
		internal string mLocation;
		internal WeatherType mForecast;
		internal int mDate;
		internal int mHour;
		internal bool mWhen;

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