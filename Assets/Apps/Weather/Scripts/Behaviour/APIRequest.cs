using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using System;
using System.IO;


namespace BuddyApp.Weather
{
    public class APIRequest : AStateMachineBehaviour
    {
        private int mDate;
        private int mNumberWeatherInfos;
        private bool mAnswerReceived;
        private bool mQuit;

        public override void Start()
        {
            mWeatherB = GetComponent<WeatherBehaviour>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mNumberWeatherInfos = 60;
            mAnswerReceived = false;
            mQuit = false;

            // Collect data and request the API accordingly
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Error. Check internet connection!");
            }

            if (mWeatherB.mLocation == "")
            {
                mWeatherB.mName = "Paris";
                mWeatherB.mLocation = "zmw:00000.45.07156";
            }
            BYOS.Instance.WebService.Weather.HourlyAt(mWeatherB.mLocation, WeatherProcessing, mNumberWeatherInfos);
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mAnswerReceived)
            {
                Trigger("Restitution");
            }
            else if (mQuit && Interaction.SpeechToText.HasFinished)
                QuitApp();
        }

        private void WeatherProcessing(WeatherInfo[] iWeather, WeatherError iError)
        {
            mWeatherB.mWeatherInfos = iWeather;

            Debug.Log("WeatherProcessing");
            if (iError != WeatherError.NONE)
            {
                if (iError == WeatherError.UNKNOWN_LOCATION)
                    Interaction.TextToSpeech.SayKey("locationissue");
                else if (iError == WeatherError.GEOLOCALIZATION_FAILED)
                    Interaction.TextToSpeech.SayKey("geolocfailed");
                else if (iError == WeatherError.GEOLOCALIZATION_DISABLED)
                    Interaction.TextToSpeech.SayKey("geolocdisable");
                else if (iError == WeatherError.MANY_LOCATIONS)
                {
                    CityData lNewCity = new CityData();
                    string[] lCitiesfile = Directory.GetFiles(BYOS.Instance.Resources.GetPathToRaw("Cities"));

                    lNewCity.Name = mWeatherB.mName;
                    lNewCity.Key = iWeather[0].Location.APICode;
                    mWeatherB.mLocation = lNewCity.Key;

                    mWeatherB.mCities.Cities.Add(lNewCity);

                    Utils.SerializeXML<CitiesData>(mWeatherB.mCities, lCitiesfile[0]);

                    Trigger("Reset");
                }
                mQuit = true;
                return;
            }
            // If error are handle correctly, this shouldn't happen!
            else if (iWeather == null || iWeather.Length == 0)
            {
                Interaction.TextToSpeech.SayKey("locationissue");
                mQuit = true;
                return;
            }
            else
            {
                GetWeatherInfos(iWeather, mWeatherB.mForecast);
            }
            mAnswerReceived = true;
        }

        private void GetWeatherInfos(WeatherInfo[] iWeather, WeatherType iWeatherType)
        {
            mWeatherB.mIndice = -1;
            mWeatherB.mRequestError = WeatherBehaviour.WeatherRequestError.UNKNOWN;
            bool lFound = false;
            int j = 0;

            if (iWeatherType != WeatherType.UNKNOWN)
            {
                if (mWeatherB.mWhen && mWeatherB.mDate == -1)
                {
                    for (int i = 0; i < iWeather.Length; ++i)
                    {
                        if ((iWeather[i].Type == WeatherType.CHANCE_OF_RAIN && iWeatherType == WeatherType.RAIN) || (iWeather[i].Type == WeatherType.RAIN && iWeatherType == WeatherType.CHANCE_OF_RAIN)
                            || iWeather[i].Type == iWeatherType)
                        {
                            mWeatherB.mIndice = i;
                            lFound = true;
                            break;
                        }

                    }
                    if (!lFound)
                    {
                        mWeatherB.mRequestError = WeatherBehaviour.WeatherRequestError.TOO_FAR;
                    }
                }
                else
                {

                    int lHour = 0;
                    int lDay = 0;

                    // Set the date
                    if (mWeatherB.mDate < 0)
                        lDay = DateTime.Now.Day;
                    else
                        lDay = mWeatherB.mDate;

                    // Set the hour
                    if (mWeatherB.mHour == -1)
                        lHour = DateTime.Now.Hour;
                    else
                        lHour = mWeatherB.mHour;

                    for (int i = 0; i < iWeather.Length; ++i)
                    {
                        if (iWeather[i].Day == lDay && ((iWeather[i].Type == WeatherType.CHANCE_OF_RAIN && iWeatherType == WeatherType.RAIN) || (iWeather[i].Type == WeatherType.RAIN && iWeatherType == WeatherType.CHANCE_OF_RAIN)
                            || iWeather[i].Type == iWeatherType))
                        {
                            mWeatherB.mIndice = i;
                            lFound = true;
                            break;
                        }
                    }
                    if (!lFound)
                    {
                        for (int i = 0; i < iWeather.Length; ++i)
                        {
                            if (iWeather[i].Hour > lHour)
                            {
                                mWeatherB.mIndice = i;
                                mWeatherB.mRequestError = WeatherBehaviour.WeatherRequestError.NONE;
                                break;
                            }
                        }
                    }
                }

            }
            else if (iWeatherType == WeatherType.UNKNOWN)
            {
                // How is the weather date/hour ?
                if (mWeatherB.mDate < 1)
                {
                    int lHour = DateTime.Now.Hour;
                    if (mWeatherB.mHour == -1)
                    {
                        mWeatherB.mIndice = 0;
                        lFound = true;
                    }
                    else if (mWeatherB.mHour >= 0)
                    {

                        for (int i = 0; i < iWeather.Length; ++i)
                        {
                            if (mWeatherB.mHour == iWeather[i].Hour)
                            {
                                mWeatherB.mIndice = i;
                                lFound = true;
                                break;
                            }
                        }

                    }
                }
                else if (mWeatherB.mDate > 0)
                {
                    int lDay = DateTime.Now.Day;
                    int k = lDay;
                    if (mWeatherB.mHour == -1)
                    {
                        for (int i = 0; i < iWeather.Length; ++i)
                        {
                            if (iWeather[i].Day != k)
                            {
                                k = iWeather[i].Day;
                                j++;
                            }
                            if (j == mWeatherB.mDate)
                            {
                                mWeatherB.mIndice = i;
                                k = 0;
                                j = 0;
                                lFound = true;
                                break;
                            }

                        }

                    }
                    else
                    {
                        for (int i = 0; i < iWeather.Length; ++i)
                        {
                            if ((mWeatherB.mHour == iWeather[i].Hour &&
                                iWeather[i].Day - lDay == mWeatherB.mDate) || ((iWeather[i].Day - lDay) - mWeatherB.mDate > 0))
                            {
                                mWeatherB.mIndice = i;
                                lFound = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}