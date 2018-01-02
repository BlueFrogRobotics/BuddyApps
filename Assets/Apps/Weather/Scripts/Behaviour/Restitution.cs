using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Weather
{
    public class Restitution : AStateMachineBehaviour
    {
        private float mTimer;

        private List<WeatherPanel> trip;
        private WeatherMan mMan;
        //private WeatherPanel mPanel;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mWeatherB = GetComponent<WeatherBehaviour>();
            mWeatherB.mIsOk = true;

            mMan = GetComponent<WeatherMan>();
            //mPanel = GetComponent<WeatherPanel>();

            trip = mMan.block;
            Debug.Log("ENTER RESTITUTION");
            mTimer = 0F;
            Debug.Log("ENTER RESTITUTION: " + mWeatherB.mIndice + " Weather info size: " + mWeatherB.mWeatherInfos.Length);
            WeatherInfo lWeatherInfo = new WeatherInfo();
            if (mWeatherB.mIndice != -1)
                lWeatherInfo = mWeatherB.mWeatherInfos[mWeatherB.mIndice];

            // Tell the weather
            string lAnswer = "";
            string lDayString = "";
            if (mWeatherB.mDate < 1)
                lDayString = Dictionary.GetString("today");
            else if (mWeatherB.mDate == 1)
                lDayString = Dictionary.GetString("tomorrow");
            else if (mWeatherB.mDate == 2)
                lDayString = Dictionary.GetString("dayaftertomorrow");
            else
                lDayString = Dictionary.GetString("intime") + " " + mWeatherB.mDate + " " + Dictionary.GetString("days");

            //No info
            //if (mWeatherB.mVocalRequest == "") {
            //	lAnswer = Dictionary.GetString("today") + " " + Dictionary.GetRandomString("temperaturewillbe") + WeatherInfo.Temperature
            //		+ " " + Dictionary.GetRandomString("degreesanditisa") + " " + WeatherInfo.Type.ToString() + " " + Dictionary.GetString("day");

            //Forecast info
            //} else 

            if (mWeatherB.mForecast != WeatherType.UNKNOWN) {
                if (mWeatherB.mWhen) {
                    if (mWeatherB.mIndice != -1) {

                        // Give date: It will rain ...
                        lAnswer = Dictionary.GetRandomString("itwillbe") + " " + Dictionary.GetRandomString((lWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + Dictionary.GetRandomString("the") + " " + EnglishDate(lWeatherInfo.Day)  + " " + Dictionary.GetRandomString("inlocation") + " " + EnglishHour(lWeatherInfo.Hour) + " " + Dictionary.GetRandomString("hour") + " ";
                    } else {
                        // Deny: There is no rain forecast for upcoming week
                        lAnswer = Dictionary.GetRandomString("notexpect") + Dictionary.GetRandomString(mWeatherB.mForecast.ToString().ToLower() + "ing");

                        if (mWeatherB.mDate == -1) {
                            lAnswer += " " + Dictionary.GetRandomString("week");
                        } else {
                            lAnswer += " " + lDayString;
                        }
                    }
                } else {


                    string lNoAnswer = Dictionary.GetRandomString("no") + " " + Dictionary.GetRandomString("itwillbe") + " "
                    + Dictionary.GetRandomString((lWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + lDayString + " " + Dictionary.GetRandomString("at") + " " + EnglishHour(lWeatherInfo.Hour);
                    string lYesAnswer = Dictionary.GetRandomString("yes") + " " + Dictionary.GetRandomString("itwillbe") + " "
                        + Dictionary.GetRandomString((lWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + lDayString + " " + Dictionary.GetRandomString("at") + " " + EnglishHour(lWeatherInfo.Hour);

                    if (mWeatherB.mForecast != lWeatherInfo.Type)
                    {
                        if ((mWeatherB.mForecast == WeatherType.CHANCE_OF_RAIN && lWeatherInfo.Type == WeatherType.RAIN) || (mWeatherB.mForecast == WeatherType.RAIN && lWeatherInfo.Type == WeatherType.CHANCE_OF_RAIN))
                            lAnswer = lYesAnswer;
                        else
                            lAnswer = lNoAnswer;
                    }
                    else
                        lAnswer = lYesAnswer;

                    //if (mWeatherB.mForecast == WeatherType.SNOWY)
                    //	if (lWeatherInfo.Type == Buddy.WeatherType.CLOUDY || lWeatherInfo.Type == Buddy.WeatherType.SUNNY)
                    //		lAnswer = lNoAnswer;
                    //	else
                    //		lAnswer = Dictionary.GetRandomString("no") + " " + Dictionary.GetRandomString("but") + " " + Dictionary.GetRandomString("itwillbe") + " " + lWeatherInfo.Type.ToString() + " " + lDayString;

                    //else if (mWeatherB.mForecast == WeatherType.CLOUDY)
                    //	if (lWeatherInfo.Type == Buddy.WeatherType.CLOUDY)
                    //		lAnswer = lYesAnswer;
                    //	else
                    //		lAnswer = lNoAnswer;

                    //else if (mWeatherB.mForecast == WeatherType.SUNNY)
                    //	if (lWeatherInfo.Type == Buddy.WeatherType.SUNNY)
                    //		lAnswer = lYesAnswer;
                    //	else
                    //		lAnswer = lNoAnswer;

                    //else if (mWeatherB.mForecast == WeatherType.RAIN)
                    //	if (lWeatherInfo.Type == Buddy.WeatherType.RAIN)
                    //		lAnswer = lYesAnswer;
                    //	else
                    //		lAnswer = lNoAnswer;
                }
            } else {
                lAnswer = Dictionary.GetRandomString("restitution");
                lAnswer = lAnswer.Replace("[date]", lDayString);
                lAnswer = lAnswer.Replace("[hour]", EnglishHour(lWeatherInfo.Hour));
                lAnswer = lAnswer.Replace("[degree]", lWeatherInfo.MinTemperature.ToString());
                lAnswer = lAnswer.Replace("[forecast]", Dictionary.GetRandomString((lWeatherInfo.Type.ToString().ToLower()).Replace("_", "")));
            }

            if (mWeatherB.mName != "")
                Interaction.TextToSpeech.Say(lAnswer + " " + Dictionary.GetRandomString("inlocation") + " " + mWeatherB.mName);

            StartCoroutine(Example(mWeatherB.mWeatherInfos));
        }

        private string EnglishHour(int Hour)
        {
            string lHour;

            if (BYOS.Instance.Language.CurrentLang == Language.FR)
            {
                lHour = Hour.ToString() + " heure";
            }
            else
            {
                if (Hour > 12 && Hour != 24)
                {
                    lHour = (Hour - 12).ToString() + " pm";
                }
                else if (Hour == 12)
                {
                    lHour = Hour.ToString() + " pm";
                }
                else
                {
                    if (Hour == 24)
                        Hour = Hour - 12;
                    lHour = Hour.ToString() + " am ";
                }
            }
            return (lHour);
        }


        private string EnglishDate(int Date)
        {
            string lDate;

            if (BYOS.Instance.Language.CurrentLang == Language.FR)
            {
                if (Date == 1)
                    lDate = "premier";
                else
                {
                    lDate = Date.ToString();
                }
            }
            else
            {
                if (Date == 1)
                {
                    lDate = "first";
                }
                else if (Date == 2)
                {
                    lDate = "second";
                }
                else if (Date == 3)
                {
                    lDate = "third";
                }
                else
                {
                    lDate = Date.ToString() + "th";
                }
            }
            return (lDate);
        }

        IEnumerator Example(WeatherInfo[] lWeatherInfo)
        {
            float num = 0.1f;
            int j = 0;

            Toaster.Display<BackgroundToast>().With();

            mWeatherB.mIsOk = false;
            if (mWeatherB.mDate >= 1)
            {
                //if (lWeatherInfo[j].Hour != 20)
                    //j++;
                for (int k = 0; k < mWeatherB.mDate; k++)
                {
                    j++;
                    while (lWeatherInfo[j].Hour != 8)
                        j++;
                }
            }

            for (int i = 0; i < 8; i++)
            {
                if (i >= 4)
                {
                    j++;
                    while (lWeatherInfo[j].Hour != 12)
                        j++;
                    SetPanel(trip[i], lWeatherInfo[j], true);

                }
                if (i < 4)
                {
                    SetPanel(trip[i], lWeatherInfo[j], false);
                    j++;
                }
                yield return new WaitForSeconds(num);
            }

            yield return new WaitForSeconds(12f);

            for (int i = 7; i >= 0; i--)
            {
                trip[i].Cancel();
                yield return new WaitForSeconds(num);
            }
            mWeatherB.mIsOk = true;
            Toaster.Hide();
        }

        public void SetPanel(WeatherPanel trip, WeatherInfo Info, bool Pan)
        {
            string[] date = Info.Date.Split('-');
            DateTime dt = new DateTime(Int32.Parse(date[0]), Int32.Parse(date[1]), Int32.Parse(date[2]));

            if (Info.MinTemperature <= 0)
                trip.SetFrost();
            trip.SetText(Info.MinTemperature.ToString() + "°C");
            if (Info.Hour >= 8 && Info.Hour < 12)
            {
                trip.Morning();
                trip.SetMoment("MORNING");
            }
            else if (Info.Hour >= 12 && Info.Hour < 16)
            {
                trip.Noon();
                if (Pan)
                {
                    trip.SetMoment(dt.DayOfWeek.ToString());
                }
                else
                  trip.SetMoment("NOON");
            }
            else if (Info.Hour >= 16 && Info.Hour < 20)
            {
                trip.After_Noon();
                trip.SetMoment("AFTERNOON");
            }
            else
            {
                trip.Evening();
                trip.ChangeMomentColor(Color.white);
                trip.ChangeTextColor(Color.white);
                trip.SetMoment("EVENING");
            }
            if (Info.Hour >= 8 && Info.Hour <= 18)
                trip.SetSun();
            else
                trip.SetMoon();

            if (Info.Type == WeatherType.SUNNY || Info.Type == WeatherType.CLEAR)
            {
                Debug.Log("HALOOO");
                if (Info.Hour > 6 && Info.Hour < 19)
                    trip.SetHalo();
            }
            else if (Info.Type == WeatherType.OVERCAST)
            {
                Debug.Log("OVERCAST");

                trip.SetCloud6();
            }
            else if (Info.Type == WeatherType.CLOUDY)
            {
                Debug.Log("CLOUUUUD");
                trip.SetCloud3();
            }
            else if (Info.Type == WeatherType.PARTLY_CLOUDY)
            {
                Debug.Log("PARTLY CLOUUUUD");
                trip.SetCloud1();
            }
            else if (Info.Type == WeatherType.CHANCE_OF_RAIN)
            {
                Debug.Log("Chance of rain");
                trip.SetRain();
                trip.SetCloud3();
            }
            else if (Info.Type == WeatherType.RAIN)
            {
                Debug.Log("RAIIIN");
                trip.SetCloud6();
                trip.SetRain();
            }

            else if (Info.Type == WeatherType.SNOWY)
            {
                Debug.Log("SNOWWW");
                trip.SetCloud6();
                trip.SetSnow();
            }
            trip.Open();
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mTimer += Time.deltaTime;
			if (Interaction.TextToSpeech.HasFinishedTalking && mTimer > 3F && mWeatherB.mIsOk) {
                QuitApp();
                Debug.Log("Restart test");
                mWeatherB.mDate = 0;
                mWeatherB.mForecast = WeatherType.UNKNOWN;
                mWeatherB.mLocation = "";
                mWeatherB.mName = "";
                WeatherData.Instance.VocalRequest = "";
                //Trigger("Restart");
            }
        }

	}
}