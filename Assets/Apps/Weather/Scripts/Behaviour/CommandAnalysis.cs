using Buddy;
using Buddy.UI;

using System;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.Weather
{

    public class CommandAnalysis : AStateMachineBehaviour
    {

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            mWeatherB = GetComponent<WeatherBehaviour>();
            //Debug.Log("ENTER Command analysis: " + DateTime.Now.Hour);
            mWeatherB.mLocation = "";
            mWeatherB.mDate = -1;
            mWeatherB.mForecast = WeatherType.UNKNOWN;
            mWeatherB.mHour = -1;
            mWeatherB.mWhen = false;


            if (WeatherData.Instance.VocalRequest != "")
                StringAnalysis(WeatherData.Instance.VocalRequest.ToLower());

            Trigger("Reset");
        }


        private void StringAnalysis(string vocalRequest)
        {
            Debug.Log("Vocal request " + vocalRequest);

            string lFinalActype = mWeatherB.OnSpeechRecognition(vocalRequest);

            Debug.Log("FINAL ACTYPE : " + lFinalActype);

            ExtractParameters(lFinalActype);
        }

        private void ExtractParameters(string actype)
        {
            int lId = 0;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            while (actype[lId] != '(')
            {
                Debug.Log(actype[lId]);
                ++lId;
            }
            lId++;
            string actype2 = actype.Substring(lId, actype.Length - lId - 1);
            string[] acts = actype2.Split(',');
            foreach (string act in acts)
            {
                Debug.Log("$$$ actegale = " + act + " $$$");
                string[] tact = act.Split('=');
                dictionary.Add(tact[0], tact[1]);
            }

            if (dictionary.ContainsKey("when"))
            {
                if (dictionary["when"].Contains("true"))
                    mWeatherB.mWhen = true;
            }
            if (dictionary.ContainsKey("day"))
                mWeatherB.mDate = Convert.ToInt32(dictionary["day"]);

            if (dictionary.ContainsKey("hour"))
                mWeatherB.mHour = Convert.ToInt32(dictionary["hour"]);

            if (dictionary.ContainsKey("weatherPrediction"))
                ExtractForecast(dictionary["weatherPrediction"]);

            if (dictionary.ContainsKey("location"))
                mWeatherB.mLocation = dictionary["location"];

            Debug.Log("DATE : " + mWeatherB.mDate);
            Debug.Log("HOUR : " + mWeatherB.mHour);
            Debug.Log("FORCAST : " + mWeatherB.mForecast);
            Debug.Log("LOCATION : " + mWeatherB.mLocation);
            Debug.Log("WHEN : " + mWeatherB.mWhen);

        }

        private void ExtractForecast(string iSpeech)
        {
            iSpeech = iSpeech.ToLower();
            if (iSpeech.Contains("snow"))
            {
                mWeatherB.mForecast = WeatherType.SNOWY;
            }
            else if (iSpeech.Contains("rain"))
            {
                mWeatherB.mForecast = WeatherType.RAIN;
            }
            else if (iSpeech.Contains("overcast"))
            {
                mWeatherB.mForecast = WeatherType.OVERCAST;
            }
            else if (iSpeech.Contains("sun"))
            {
                mWeatherB.mForecast = WeatherType.SUNNY;
            }
            else if (iSpeech.Contains("nothing"))
            {
                // TODO
            }
            else if (iSpeech.Contains("cloud"))
            {
                mWeatherB.mForecast = WeatherType.CLOUDY;
            }
        }
    }
}