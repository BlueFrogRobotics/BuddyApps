using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Weather
{
    /// <summary>
    /// Restition class is the final state of Weather
    /// </summary>
    public class Restitution : AStateMachineBehaviour
    {
        private float mTimer;

        private List<WeatherPanel> mTrip;
        private WeatherMan mMan;

        #region Unity Methods

        public override void Start()
        {
            mWeatherB = GetComponent<WeatherBehaviour>();
            mMan = GetComponent<WeatherMan>();
        }


        /// <summary>
        /// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mWeatherB.mIsOk = true;

            mTrip = mMan.block;
            mTimer = 0F;

            if (mWeatherB.mCommand == WeatherBehaviour.WeatherCommand.NONE)
                StartCoroutine(DisplayToaster(mWeatherB.mWeatherInfos));
            else
                ExecuteCommand(mWeatherB.mWeatherInfos);
        }

        /// <summary>
        /// OnStateEnter is called at each frame
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            mTimer += Time.deltaTime;
            if (Interaction.TextToSpeech.HasFinishedTalking && mTimer > 3F && mWeatherB.mIsOk)
            {
                QuitApp();
                mWeatherB.mDate = 0;
                mWeatherB.mForecast = WeatherType.UNKNOWN;
                mWeatherB.mLocation = "";
                mWeatherB.mName = "";
                WeatherData.Instance.VocalRequest = "";
                //Trigger("Restart");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tell the weather
        /// </summary>
        /// <param name="iWeatherInfo">Weather Info</param>
        /// <param name="iSayDayOfWeek"></param>
        private void SayWeather(WeatherInfo iWeatherInfo, bool iSayDayOfWeek = false)
        {
            string[] lDate = iWeatherInfo.Date.Split('-');
            DateTime lDateTime = new DateTime(Int32.Parse(lDate[0]), Int32.Parse(lDate[1]), Int32.Parse(lDate[2]));
            string lDayString = RecoverDay(iSayDayOfWeek, lDateTime);
            string lAnswer = string.Empty;

            if (mWeatherB.mForecast != WeatherType.UNKNOWN)
            {
                if (mWeatherB.mWhen)
                {
                    if (mWeatherB.mIndice != -1)
                    {

                        // Give date: It will rain ...
                        lAnswer = Dictionary.GetRandomString("itwillbe") + " " + Dictionary.GetRandomString((iWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + Dictionary.GetRandomString("the") + " " + EnglishDate(iWeatherInfo.Day) + " " + Dictionary.GetRandomString("inlocation") + " " + EnglishHour(iWeatherInfo.Hour) + " " + Dictionary.GetRandomString("hour") + " ";
                    }
                    else
                    {
                        // Deny: There is no rain forecast for upcoming week
                        lAnswer = Dictionary.GetRandomString("notexpect") + Dictionary.GetRandomString(mWeatherB.mForecast.ToString().ToLower() + "ing");
                        if (mWeatherB.mDate == -1)
                            lAnswer += " " + Dictionary.GetRandomString("week");
                        else
                            lAnswer += " " + lDayString;
                    }
                }
                else
                {
                    string lNoAnswer = Dictionary.GetRandomString("no") + " " + Dictionary.GetRandomString("itwillbe") + " "
                    + Dictionary.GetRandomString((iWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + lDayString + " " + Dictionary.GetRandomString("at") + " " + EnglishHour(iWeatherInfo.Hour);
                    string lYesAnswer = Dictionary.GetRandomString("yes") + " " + Dictionary.GetRandomString("itwillbe") + " "
                        + Dictionary.GetRandomString((iWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + lDayString + " " + Dictionary.GetRandomString("at") + " " + EnglishHour(iWeatherInfo.Hour);

                    if (mWeatherB.mForecast != iWeatherInfo.Type)
                    {
                        if ((mWeatherB.mForecast == WeatherType.CHANCE_OF_RAIN && iWeatherInfo.Type == WeatherType.RAIN) || (mWeatherB.mForecast == WeatherType.RAIN && iWeatherInfo.Type == WeatherType.CHANCE_OF_RAIN))
                            lAnswer = lYesAnswer;
                        else
                            lAnswer = lNoAnswer;
                    }
                    else
                        lAnswer = lYesAnswer;
                }
            }
            else
                lAnswer = InitAnswerRestitution(iWeatherInfo, lDayString);

            if (mWeatherB.mName != "")
                Interaction.TextToSpeech.Say(lAnswer/* + " " + Dictionary.GetRandomString("inlocation") + " " + mWeatherB.mName*/);
        }

        /// <summary>
        /// Get the day
        /// </summary>
        /// <param name="iSayDayOfWeek"></param>
        /// <param name="iDateTime"></param>
        /// <returns>the day</returns>
        private string RecoverDay(bool iSayDayOfWeek, DateTime iDateTime)
        {
            string lDayString = string.Empty;

            if (mWeatherB.mDate < 1)
                lDayString = Dictionary.GetString("today");
            else if (iSayDayOfWeek)
                lDayString = Dictionary.GetString(iDateTime.DayOfWeek.ToString().ToLower().Trim());
            else if (mWeatherB.mDate == 1)
                lDayString = Dictionary.GetString("tomorrow");
            else if (mWeatherB.mDate == 2)
                lDayString = Dictionary.GetString("dayaftertomorrow");
            else if (mWeatherB.mWeekend)
                lDayString = Dictionary.GetString("weekend");
            else
                lDayString = Dictionary.GetString("intime") + " " + mWeatherB.mDate + " " + Dictionary.GetString("days");

            return (lDayString);
        }

        /// <summary>
        /// Get the day without day of week
        /// </summary>
        /// <returns>the day</returns>
        private string RecoverDay()
        {
            string lDayString = string.Empty;

            if (mWeatherB.mDate < 1)
                lDayString = Dictionary.GetString("today");
            else if (mWeatherB.mDate == 1)
                lDayString = Dictionary.GetString("tomorrow");
            else if (mWeatherB.mDate == 2)
                lDayString = Dictionary.GetString("dayaftertomorrow");
            else if (mWeatherB.mWeekend)
                lDayString = Dictionary.GetString("weekend");
            else
                lDayString = Dictionary.GetString("intime") + " " + mWeatherB.mDate + " " + Dictionary.GetString("days");

            return (lDayString);
        }

        /// <summary>
        /// Initialize the Answer Restitution
        /// </summary>
        /// <param name="iWeatherInfo">weather info</param>
        /// <param name="iDayString">the day</param>
        /// <returns>the Answer</returns>
        private string InitAnswerRestitution(WeatherInfo iWeatherInfo, string iDayString)
        {
            string lAnswer = string.Empty;

            if (mWeatherB.mWeatherTime == WeatherBehaviour.WeatherMoment.NONE)
            {
                WeatherInfo lMinWeather = RecoverMinMaxTempOfADay(mWeatherB.mWeatherInfos, true, iDayString);
                WeatherInfo lMaxWeather = RecoverMinMaxTempOfADay(mWeatherB.mWeatherInfos, false, iDayString);

                if (lMinWeather.Hour == lMaxWeather.Hour)
                {
                    lAnswer = Dictionary.GetRandomString("lightrestitution");
                    lAnswer = lAnswer.Replace("[date]", iDayString + " " + Dictionary.GetRandomString("inlocation") + " " + mWeatherB.mName);
                    lAnswer = lAnswer.Replace("[degree]", lMinWeather.MinTemperature.ToString());
                    lAnswer = lAnswer.Replace("[forecast]", Dictionary.GetRandomString((iWeatherInfo.Type.ToString().ToLower()).Replace("_", "")));
                }
                else
                {
                    lAnswer = Dictionary.GetRandomString("restitution");
                    lAnswer = lAnswer.Replace("[date]", iDayString + " " + Dictionary.GetRandomString("inlocation") + " " + mWeatherB.mName);
                    lAnswer = lAnswer.Replace("[minTemp]", lMinWeather.MinTemperature.ToString());
                    lAnswer = lAnswer.Replace("[hourMin]", EnglishHour(lMinWeather.Hour));
                    lAnswer = lAnswer.Replace("[maxTemp]", lMaxWeather.MaxTemperature.ToString());
                    lAnswer = lAnswer.Replace("[hourMax]", EnglishHour(lMaxWeather.Hour));
                    lAnswer = lAnswer.Replace("[forecast]", Dictionary.GetRandomString((iWeatherInfo.Type.ToString().ToLower()).Replace("_", "")));
                }
            }
            else
            {
                lAnswer = Dictionary.GetRandomString("restraintrestitution");
                lAnswer = lAnswer.Replace("[date]", iDayString + " " + Dictionary.GetRandomString("inlocation") + " " + mWeatherB.mName);
                lAnswer = lAnswer.Replace("[time]", Dictionary.GetRandomString(mWeatherB.mWeatherTime.ToString().ToLower()));
                lAnswer = lAnswer.Replace("[degree]", iWeatherInfo.MinTemperature.ToString());
            }
            return (lAnswer);
        }

        /// <summary>
        /// Get Min / Max Temperature
        /// </summary>
        /// <param name="iWeatherInfo">weather info</param>
        /// <param name="iIsMin">if true, get the minimum. if false, get the maximum</param>
        /// <param name="iDayString">the day</param>
        /// <returns>Instance of weatherInfo</returns>
        private WeatherInfo RecoverMinMaxTempOfADay(WeatherInfo[] iWeatherInfo, bool iIsMin, string iDayString)
        {
            int j = 0;
            float lMinTemp = 200F;
            float lMaxTemp = -200F;
            int hourMax = 0;
            int hourMin = 0;

            if (mWeatherB.mDate >= 1)
            {
                for (int k = 0; k < mWeatherB.mDate; k++)
                {
                    j++;
                    while (iWeatherInfo[j].Hour != 8)
                        j++;
                }
            }

            DateTime lTime = DateTime.Today;

            if (mWeatherB.mDate >= 1)
                lTime = lTime.AddDays(mWeatherB.mDate);
            if (mWeatherB.mWeekend)
                if (iDayString.Equals("sunday"))
                {
                    lTime = lTime.AddDays(1);
                }

            for (int i = 0; i < 8; i++)
            {
                if (lTime.Day.ToString().Equals(iWeatherInfo[j].Day.ToString()))
                {
                    if (iWeatherInfo[j].MinTemperature < lMinTemp)
                    {
                        lMinTemp = iWeatherInfo[j].MinTemperature;
                        hourMin = iWeatherInfo[j].Hour;
                    }
                    if (iWeatherInfo[j].MaxTemperature > lMaxTemp)
                    {
                        lMaxTemp = iWeatherInfo[j].MaxTemperature;
                        hourMax = iWeatherInfo[j].Hour;
                    }
                }
                j++;
            }

            WeatherInfo lWeatherInfoMin = new WeatherInfo()
            {
                MinTemperature = (int)Math.Ceiling(lMinTemp),
                Hour = hourMin
            };
            WeatherInfo lWeatherInfoMax = new WeatherInfo()
            {
                MaxTemperature = (int)Math.Ceiling(lMaxTemp),
                Hour = hourMax
            };

            if (iIsMin)
                return (lWeatherInfoMin);
            else
                return (lWeatherInfoMax);
        }

        /// <summary>
        /// Get the Min or Max temperature
        /// </summary>
        /// <param name="iWeatherInfo">Information of weather</param>
        /// <param name="iIsMin">if true, recover minimum. if false, recover maximum</param>
        /// <returns>Instance of WeatherInfo</returns>
        private WeatherInfo RecoverMinMaxTemperature(WeatherInfo[] iWeatherInfo, bool iIsMin)
        {
            int j = 0;
            float lMinTemp = 200F;
            float lMaxTemp = -200F;
            int hourMax = 0;
            int hourMin = 0;

            if (mWeatherB.mDate >= 1)
            {
                for (int k = 0; k < mWeatherB.mDate; k++)
                {
                    j++;
                    while (iWeatherInfo[j].Hour != 8)
                        j++;
                }
            }

            for (int i = 0; i < 8; i++)
            {
                if (iWeatherInfo[j].MinTemperature < lMinTemp)
                {
                    lMinTemp = iWeatherInfo[j].MinTemperature;
                    hourMin = iWeatherInfo[j].Hour;
                }
                if (iWeatherInfo[j].MaxTemperature > lMaxTemp)
                {
                    lMaxTemp = iWeatherInfo[j].MaxTemperature;
                    hourMax = iWeatherInfo[j].Hour;
                }
                j++;
            }

            WeatherInfo lWeatherInfoMin = new WeatherInfo()
            {
                MinTemperature = (int)Math.Ceiling(lMinTemp),
                Hour = hourMin
            };
            WeatherInfo lWeatherInfoMax = new WeatherInfo()
            {
                MaxTemperature = (int)Math.Ceiling(lMaxTemp),
                Hour = hourMax
            };

            if (iIsMin)
                return (lWeatherInfoMin);
            else
                return (lWeatherInfoMax);
        }

        private void ExecuteCommand(WeatherInfo[] iWeatherInfo)
        {
            //mWeatherB.mIsOk = false;

            string lDayString = RecoverDay();

            string lAnswer = Dictionary.GetRandomString("saycommand");
            lAnswer = lAnswer.Replace("[date]", lDayString);
            lAnswer = lAnswer.Replace("[localization]", mWeatherB.mName);

            if (mWeatherB.mCommand == WeatherBehaviour.WeatherCommand.MIN)
            {
                lAnswer = lAnswer.Replace("[temperature]", RecoverMinMaxTemperature(iWeatherInfo, true).MinTemperature.ToString());
                lAnswer = lAnswer.Replace("[command]", Dictionary.GetString("min"));
            }
            else if (mWeatherB.mCommand == WeatherBehaviour.WeatherCommand.MAX)
            {
                lAnswer = lAnswer.Replace("[temperature]", RecoverMinMaxTemperature(iWeatherInfo, false).MaxTemperature.ToString());
                lAnswer = lAnswer.Replace("[command]", Dictionary.GetString("max"));
            }
            Interaction.TextToSpeech.Say(lAnswer);
        }

        private string EnglishHour(int Hour)
        {
            string lHour;

            if (BYOS.Instance.Language.CurrentLang == Language.FR)
                lHour = Hour.ToString() + " heure";
            else
            {
                if (Hour > 12 && Hour != 24)
                    lHour = (Hour - 12).ToString() + " pm";
                else if (Hour == 12)
                    lHour = Hour.ToString() + " pm";
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
                    lDate = Date.ToString();
            }
            else
            {
                if (Date == 1)
                    lDate = "first";
                else if (Date == 2)
                    lDate = "second";
                else if (Date == 3)
                    lDate = "third";
                else
                    lDate = Date.ToString() + "th";
            }
            return (lDate);
        }

        private IEnumerator DisplayToaster(WeatherInfo[] iWeatherInfo)
        {
            int lNbDays = 0;
            if (mWeatherB.mWeekend)
                lNbDays = 1;

            for (int d = 0; d <= lNbDays; d++)
            {
                if (mWeatherB.mWeekend)
                    SayWeather(mWeatherB.mWeatherInfos[mWeatherB.mIndice + 4 * d], true);
                else
                    SayWeather(mWeatherB.mWeatherInfos[mWeatherB.mIndice + 4 * d]);

                float num = 0.1f;
                int j = 0;

                Toaster.Display<BackgroundToast>().With();

                mWeatherB.mIsOk = false;
                if (mWeatherB.mDate >= 1)
                {
                    for (int k = 0; k < mWeatherB.mDate + d; k++)
                    {
                        j++;
                        while (iWeatherInfo[j].Hour != 8)
                            j++;
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    if (i >= 4)
                    {
                        j++;
                        while (iWeatherInfo[j].Hour != 12)
                            j++;
                        SetPanel(mTrip[i], iWeatherInfo[j], true);

                    }
                    if (i < 4)
                    {
                        SetPanel(mTrip[i], iWeatherInfo[j], false);
                        j++;
                    }
                    yield return new WaitForSeconds(num);
                }

                if (lNbDays == 0)
                    yield return new WaitForSeconds(6f);
                else
                    yield return new WaitForSeconds(3f);

                while (!Interaction.TextToSpeech.HasFinishedTalking)
                    yield return null;

                for (int i = 7; i >= 0; i--)
                {
                    mTrip[i].Cancel();
                    yield return new WaitForSeconds(num);
                }
                Toaster.Hide();
            }
            mWeatherB.mIsOk = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the Panel Text and Image
        /// </summary>
        /// <param name="iTrip">Panel to display</param>
        /// <param name="iWeatherInfo">weather info</param>
        /// <param name="iDisplayDayOfWeek">if true, display days; if false, opposite</param>
        public void SetPanel(WeatherPanel iTrip, WeatherInfo iWeatherInfo, bool iDisplayDayOfWeek)
        {
            string[] date = iWeatherInfo.Date.Split('-');
            DateTime dt = new DateTime(Int32.Parse(date[0]), Int32.Parse(date[1]), Int32.Parse(date[2]));

            if (iWeatherInfo.MinTemperature <= 0)
                iTrip.SetFrost();
            iTrip.SetText(iWeatherInfo.MinTemperature.ToString() + "°C");
            if (iWeatherInfo.Hour >= 8 && iWeatherInfo.Hour < 12)
            {
                iTrip.Morning();
                iTrip.SetMoment(Dictionary.GetRandomString("morning").ToUpper());
            }
            else if (iWeatherInfo.Hour >= 12 && iWeatherInfo.Hour < 16)
            {
                iTrip.Noon();
                if (iDisplayDayOfWeek)
                    iTrip.SetMoment(Dictionary.GetRandomString(dt.DayOfWeek.ToString().ToLower()).ToUpper());
                else
                    iTrip.SetMoment(Dictionary.GetRandomString("noon").ToUpper());
            }
            else if (iWeatherInfo.Hour >= 16 && iWeatherInfo.Hour < 20)
            {
                iTrip.After_Noon();
                iTrip.SetMoment(Dictionary.GetRandomString("afternoon").ToUpper());
            }
            else
            {
                iTrip.Evening();
                iTrip.ChangeMomentColor(Color.white);
                iTrip.ChangeTextColor(Color.white);
                iTrip.SetMoment(Dictionary.GetRandomString("evening").ToUpper());
            }
            if (iWeatherInfo.Hour >= 8 && iWeatherInfo.Hour <= 18)
                iTrip.SetSun();
            else
                iTrip.SetMoon();

            switch (iWeatherInfo.Type)
            {
                case WeatherType.CHANCE_FLURRIES:
                    iTrip.SetSnow();
                    iTrip.SetCloud3();
                    break;
                case WeatherType.CHANCE_OF_RAIN:
                    iTrip.SetRain();
                    iTrip.SetCloud3();
                    break;
                case WeatherType.CHANCE_SLEET:
                    iTrip.SetRain();
                    iTrip.SetCloud3();
                    break;
                case WeatherType.CHANCE_SNOW:
                    iTrip.SetSnow();
                    iTrip.SetCloud3();
                    break;
                case WeatherType.CHANCE_STORMS:
                    iTrip.SetCloud3();
                    break;
                case WeatherType.CLEAR:
                    if (iWeatherInfo.Hour > 6 && iWeatherInfo.Hour < 19)
                        iTrip.SetHalo();
                    break;
                case WeatherType.CLOUDY:
                    iTrip.SetCloud3();
                    break;
                case WeatherType.FLURRIES:
                    iTrip.SetSnow();
                    iTrip.SetCloud6();
                    break;
                case WeatherType.FOG:
                    iTrip.SetCloud3();
                    break;
                case WeatherType.HAZY:
                    iTrip.SetCloud3();
                    break;
                case WeatherType.MOSTLY_CLOUDY:
                    iTrip.SetCloud3();
                    break;
                case WeatherType.MOSTLY_SUNNY:
                    if (iWeatherInfo.Hour > 6 && iWeatherInfo.Hour < 19)
                        iTrip.SetHalo();
                    break;
                case WeatherType.OVERCAST:
                    iTrip.SetCloud6();
                    break;
                case WeatherType.PARTLY_CLOUDY:
                    iTrip.SetCloud1();
                    break;
                case WeatherType.PARTLY_SUNNY:
                    if (iWeatherInfo.Hour > 6 && iWeatherInfo.Hour < 19)
                        iTrip.SetHalo();
                    break;
                case WeatherType.RAIN:
                    iTrip.SetCloud6();
                    iTrip.SetRain();
                    break;
                case WeatherType.SLEET:
                    break;
                case WeatherType.SNOW:
                    iTrip.SetCloud6();
                    iTrip.SetSnow();
                    break;
                case WeatherType.SUNNY:
                    if (iWeatherInfo.Hour > 6 && iWeatherInfo.Hour < 19)
                        iTrip.SetHalo();
                    break;
                case WeatherType.THUNDERSTORMS:
                    iTrip.SetCloud6();
                    iTrip.SetRain();
                    break;
                default:
                    break;
            }

            iTrip.Open();
        }

        #endregion

    }
}