using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Weather
{

    public sealed class CommandAnalysis : AStateMachineBehaviour
    {
        private CitiesManager mCitiesManager;
        private CitiesData mCities;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void Start()
        {
            mCitiesManager = GetComponent<CitiesManager>();
            mCities = mCitiesManager.CitiesData;
            mWeatherB = GetComponent<WeatherBehaviour>();

            mWeatherB.mLocation = "";
            mWeatherB.mName = "";
            mWeatherB.mDate = -1;
            mWeatherB.mForecast = WeatherType.UNKNOWN;
            mWeatherB.mHour = -1;
            mWeatherB.mWhen = false;
            mWeatherB.mCities = mCities;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("VocalRequest : " + WeatherData.Instance.VocalRequest);

            if (WeatherData.Instance.VocalRequest != "")
                StringAnalysis(WeatherData.Instance.VocalRequest);
            else
            {
                string vocalRequest = "What's the weather like";
                if (Buddy.Platform.Language.InputLanguage.ISO6391Code == ISO6391Code.FR)
                    vocalRequest = "Quel temps fait-il";
                StringAnalysis(vocalRequest);
            }

            Trigger("Request");
        }

        private void StringAnalysis(string iVocalRequest)
        {
            // Analyse string to find parameters (place, date ...)
            ExtractLocation(iVocalRequest);
            iVocalRequest = iVocalRequest.ToLower();
            if (Buddy.Resources.ContainsPhonetic(iVocalRequest, "when"))
                mWeatherB.mWhen = true;
            mWeatherB.mWeekend = false;
            ExtractDate(iVocalRequest);
            ExtractHour(iVocalRequest);
            ExtractForecast(iVocalRequest);
        }



        private void ExtractDate(string iSpeech)
        {
            if (Buddy.Resources.ContainsPhonetic(iSpeech, "today"))
                mWeatherB.mDate = 0;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "dayaftertomorrow"))
                mWeatherB.mDate = 2;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "tomorrow"))
                mWeatherB.mDate = 1;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "weekend"))
            {
                mWeatherB.mDate = 6 - (int)DateTime.Now.DayOfWeek;
                mWeatherB.mWeekend = true;
            }
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "intime") && Buddy.Resources.ContainsPhonetic(iSpeech, "day"))
            {
                int nbDay = 0;
                string[] words = iSpeech.Split(' ');
                for (int iw = 0; iw < words.Length; ++iw)
                {
                    if (words[iw].ToLower() == Buddy.Resources.GetString("intime") && iw + 2 < words.Length)
                    {
                        if (Int32.TryParse(words[iw + 1], out nbDay) && Buddy.Resources.ContainsPhonetic(words[iw + 2], "day"))
                        {
                            mWeatherB.mDate = nbDay;
                            break;
                        }
                    }
                }
            }
            else
            {
                // Day of week in vocal request
                for (int i=0; i<7; i++)
                {
                    DayOfWeek d = (DayOfWeek)i;
                    if (Buddy.Resources.ContainsPhonetic(iSpeech, d.ToString().ToLower()))
                    {
                        int dnow = (int)DateTime.Now.DayOfWeek;
                        mWeatherB.mDate = (i >= dnow) ? i - dnow : (7 + i) - dnow;
                    }
                }

            }

            if (Buddy.Resources.ContainsPhonetic(iSpeech, "min"))
                mWeatherB.mCommand = WeatherBehaviour.WeatherCommand.MIN;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "max"))
                mWeatherB.mCommand = WeatherBehaviour.WeatherCommand.MAX;
            else
            {
                mWeatherB.mCommand = WeatherBehaviour.WeatherCommand.NONE;
            }
        }


        private void ExtractForecast(string iSpeech)
        {
            iSpeech = iSpeech.ToLower();
            if (Buddy.Resources.ContainsPhonetic(iSpeech, "chanceflurries"))
                mWeatherB.mForecast = WeatherType.CHANCE_FLURRIES;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "chanceofrain"))
                mWeatherB.mForecast = WeatherType.CHANCE_OF_RAIN;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "chancesleet"))
                mWeatherB.mForecast = WeatherType.CHANCE_SLEET;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "chancesnow"))
                mWeatherB.mForecast = WeatherType.CHANCE_SNOW;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "chancestorms"))
                mWeatherB.mForecast = WeatherType.CHANCE_STORMS;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "clear"))
                mWeatherB.mForecast = WeatherType.CLEAR;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "cloudy"))
                mWeatherB.mForecast = WeatherType.CLOUDY;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "flurries"))
                mWeatherB.mForecast = WeatherType.FLURRIES;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "fog"))
                mWeatherB.mForecast = WeatherType.FOG;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "hazy"))
                mWeatherB.mForecast = WeatherType.HAZY;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "mostlycloudy"))
                mWeatherB.mForecast = WeatherType.MOSTLY_CLOUDY;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "mostlysunny"))
                mWeatherB.mForecast = WeatherType.MOSTLY_SUNNY;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "overcast"))
                mWeatherB.mForecast = WeatherType.OVERCAST;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "partlycloudy"))
                mWeatherB.mForecast = WeatherType.PARTLY_CLOUDY;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "partlysunny"))
                mWeatherB.mForecast = WeatherType.PARTLY_SUNNY;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "rainy"))
                mWeatherB.mForecast = WeatherType.RAIN;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "sleet"))
                mWeatherB.mForecast = WeatherType.SLEET;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "snow"))
                mWeatherB.mForecast = WeatherType.SNOW;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "sun"))
                mWeatherB.mForecast = WeatherType.SUNNY;
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "thunder"))
                mWeatherB.mForecast = WeatherType.THUNDERSTORMS;

        }

        private void ExtractLocation(string iSpeech)
        {
            string[] words = iSpeech.Split(' ');
            for (int iw = 0; iw < words.Length; ++iw)
            {
                if (words[iw].ToLower() == Buddy.Resources.GetString("inlocation") && (iw + 1) < words.Length)
                {
                    if (!string.IsNullOrEmpty(words[iw + 1]))
                        if (char.IsUpper(words[iw + 1][0]))
                        {
                            string lDefinitionWord = "";

                            if (iw != -1 && iw != words.Length)
                            {
                                for (int j = iw + 1; j < words.Length; j++)
                                {
                                    if (!char.IsUpper(words[j][0]))
                                        break;                                    
                                    lDefinitionWord += words[j] + " ";
                                }
                            }
                            mWeatherB.mLocation = lDefinitionWord;
                        }
                }
            }

            mWeatherB.mName = mWeatherB.mLocation;

            foreach (CityData city in mCities.Cities)
            {
                if (mWeatherB.mLocation.Equals(city.Name))
                {
                    mWeatherB.mName = city.Name;
                    mWeatherB.mLocation = city.Key;
                    break;
                }
            }
        }


        /// <summary>
        /// Extract the hour from the text
        /// may it be from direct hour (8 in the morning)
        /// or words like morning / evening / night
        /// </summary>
        /// <param name="iSpeech"></param>
        private void ExtractHour(string iSpeech)
        {
            List<int> lHour = ContainsHour(iSpeech);
            if (lHour.Count == 1)
            {
                mWeatherB.mHour = lHour[0];
            }

            if (Buddy.Resources.ContainsPhonetic(iSpeech, "morning") || Buddy.Resources.ContainsPhonetic(iSpeech, "am"))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 8;

                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.MORNING;
            }
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "noon") || Buddy.Resources.ContainsPhonetic(iSpeech, "am"))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 12;
                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.NOON;

            }
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "afternoon") || Buddy.Resources.ContainsPhonetic(iSpeech, "pm"))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 16;
                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.AFTERNOON;
            }
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "midnight"))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 20;
                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.NONE;
            }
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "evening"))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 20;
                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.EVENING;
            }
            else if (Buddy.Resources.ContainsPhonetic(iSpeech, "night"))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 20;
                else if (mWeatherB.mHour < 13)
                {
                    // Do nothing?
                }
                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.EVENING;
            }
        }

        private List<int> ContainsHour(string iSpeech)
        {
            List<int> result = new List<int>();

            string[] words = iSpeech.Split(' ');
            for (int iw = 0; iw < words.Length; ++iw)
            {
                if (!string.IsNullOrEmpty(words[iw].Trim()))
                {
                    for (int i = 0; i < 24; ++i)
                    {
                        if (words[iw].ToLower() == i.ToString() + "h")
                            result.Add(i);
                        else if (Buddy.Resources.ContainsPhonetic(words[iw].ToLower(), "hour"))
                        {
                            if (words[iw - 1] == i.ToString())
                                result.Add(i);
                        }
                    }
                }
            }
            return result;
        }
    }
}