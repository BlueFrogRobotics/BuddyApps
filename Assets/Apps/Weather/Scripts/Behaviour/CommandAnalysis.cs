using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

//namespace BuddyApp.Weather
//{
//}

namespace BuddyApp.Weather
{

    public class CommandAnalysis : AStateMachineBehaviour
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

        override public void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("VocalRequest : " + WeatherData.Instance.VocalRequest);

            if (WeatherData.Instance.VocalRequest != "")
                StringAnalysis(WeatherData.Instance.VocalRequest);

            Trigger("Request");
        }

        private void StringAnalysis(string iVocalRequest)
        {
            // Analyse string to find parameters (place, date ...)
            ExtractLocation(iVocalRequest);
            iVocalRequest.ToLower();
            if (ContainsOneOf(iVocalRequest, Dictionary.GetPhoneticStrings("when")))
                mWeatherB.mWhen = true;
            mWeatherB.mWeekend = false;
            ExtractDate(iVocalRequest);
            ExtractHour(iVocalRequest);
            ExtractForecast(iVocalRequest);
        }



        private void ExtractDate(string iSpeech)
        {
            if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("today")))
                mWeatherB.mDate = 0;
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("dayaftertomorrow")))
                mWeatherB.mDate = 2;
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("tomorrow")))
                mWeatherB.mDate = 1;
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("weekend")))
            {
                mWeatherB.mDate = 6 - (int)DateTime.Now.DayOfWeek;
                mWeatherB.mWeekend = true;
            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("intime")) && ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("day")))
            {
                int nbDay = 0;
                string[] words = iSpeech.Split(' ');
                for (int iw = 0; iw < words.Length; ++iw)
                {
                    if (words[iw].ToLower() == Dictionary.GetString("intime") && iw + 2 < words.Length)
                    {
                        if (Int32.TryParse(words[iw + 1], out nbDay) && ContainsOneOf(words[iw + 2], Dictionary.GetPhoneticStrings("day")))
                        {
                            mWeatherB.mDate = nbDay;
                            break;
                        }
                    }
                }
            }

            if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("min")))
                mWeatherB.mCommand = WeatherBehaviour.WeatherCommand.MIN;
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("max")))
                mWeatherB.mCommand = WeatherBehaviour.WeatherCommand.MAX;
            else
            {
                mWeatherB.mCommand = WeatherBehaviour.WeatherCommand.NONE;
            }
        }


        private void ExtractForecast(string iSpeech)
        {
            iSpeech = iSpeech.ToLower();
            if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("snow")))
                mWeatherB.mForecast = WeatherType.SNOWY;
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("rainy")))
                mWeatherB.mForecast = WeatherType.RAIN;
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("fog")))
                mWeatherB.mForecast = WeatherType.OVERCAST;
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("sun")))
                mWeatherB.mForecast = WeatherType.SUNNY;
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("chanceofrainy")))
                mWeatherB.mForecast = WeatherType.CHANCE_OF_RAIN;
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("wind")))
            {
                // TODO
            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("thunder")))
                mWeatherB.mForecast = WeatherType.OVERCAST;
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("cloud")))
                mWeatherB.mForecast = WeatherType.CLOUDY;
        }

        private void ExtractLocation(string iSpeech)
        {
            string[] words = iSpeech.Split(' ');
            for (int iw = 0; iw < words.Length; ++iw)
            {
                if (words[iw].ToLower() == Dictionary.GetString("inlocation") && iw + 1 < words.Length)
                {
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

            if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("morning")))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 8;

                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.MORNING;
            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("noon")) || ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("pm")))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 12;
                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.NOON;

            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("afternoon")))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 16;
                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.AFTERNOON;
            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("midnight")))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 20;
                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.NONE;
            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("evening")))
            {
                if (mWeatherB.mHour == -1)
                    mWeatherB.mHour = 20;
                mWeatherB.mWeatherTime = WeatherBehaviour.WeatherMoment.EVENING;
            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("night")))
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
                for (int i = 0; i < 24; ++i)
                {
                    if (words[iw].ToLower() == i.ToString() + "h")
                        result.Add(i);
                    else if (ContainsOneOf(words[iw].ToLower(), Dictionary.GetPhoneticStrings("hour")))
                    {
                        if (words[iw - 1] == i.ToString())
                            result.Add(i);
                    }
                }
            }
            return result;
        }



        private bool ContainsOneOf(string iSpeech, string[] iListSpeech)
        {
            iSpeech = iSpeech.ToLower();
            for (int i = 0; i < iListSpeech.Length; ++i)
            {
                string[] words = iListSpeech[i].Split(' ');
                if (words.Length < 2)
                {
                    words = iSpeech.Split(' ');
                    foreach (string word in words)
                    {
                        if (word == iListSpeech[i].ToLower())
                        {
                            return true;
                        }
                    }
                }
                else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
                    return true;
            }
            return false;
        }
    }
}