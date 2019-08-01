using BlueQuark;

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
    public sealed class Restitution : AStateMachineBehaviour
    {
        private float mTimer;

        private List<WeatherPanel> mTrip;
        private WeatherMan mMan;

        private const int NB_DATA_IN_DAY = 8;
        private const int FIRST_HOUR = 8;

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
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWeatherB.mIsOk = true;

            mTrip = mMan.block;
            mTimer = 0F;

            if (!CheckDataForRequestedDate(mWeatherB.mWeatherInfos))
            {
                Buddy.Vocal.SayKey("unknowndate");
            }
            else
            {
                if (mWeatherB.mCommand == WeatherBehaviour.WeatherCommand.NONE)
                    StartCoroutine(DisplayToaster(mWeatherB.mWeatherInfos));
                else
                    ExecuteCommand(mWeatherB.mWeatherInfos);
            }
        }

        /// <summary>
        /// OnStateEnter is called at each frame
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            mTimer += Time.deltaTime;
            if (!Buddy.Vocal.IsSpeaking && mTimer > 3F && mWeatherB.mIsOk)
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
                        lAnswer = Buddy.Resources.GetRandomString((iWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + Buddy.Resources.GetRandomString("the") + " " + EnglishDate(iWeatherInfo.Day) + " " + Buddy.Resources.GetRandomString("inlocation") + " " + EnglishHour(iWeatherInfo.Hour) + " " + Buddy.Resources.GetRandomString("hour") + " ";
                    }
                    else
                    {
                        // Deny: There is no rain forecast for upcoming week
                        lAnswer = Buddy.Resources.GetRandomString("notexpect").Replace("[forecast]", Buddy.Resources.GetRandomString(mWeatherB.mForecast.ToString().ToLower() + "ing"));
                        if (mWeatherB.mDate == -1)
                            lAnswer += " " + Buddy.Resources.GetRandomString("week");
                        else
                            lAnswer += " " + lDayString;
                    }
                }
                else
                {
                    string lNoAnswer = Buddy.Resources.GetRandomString("no") + " "
                    + Buddy.Resources.GetRandomString((iWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + lDayString + " " + Buddy.Resources.GetRandomString("at") + " " + EnglishHour(iWeatherInfo.Hour);
                    string lYesAnswer = Buddy.Resources.GetRandomString("yes") + " "
                        + Buddy.Resources.GetRandomString((iWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + lDayString + " " + Buddy.Resources.GetRandomString("at") + " " + EnglishHour(iWeatherInfo.Hour);

                    if (mWeatherB.mForecast != iWeatherInfo.Type)
                    {
                        if (((mWeatherB.mForecast == WeatherType.CHANCE_OF_RAIN && iWeatherInfo.Type == WeatherType.RAIN) || (mWeatherB.mForecast == WeatherType.RAIN && iWeatherInfo.Type == WeatherType.CHANCE_OF_RAIN)) ||
                            ((mWeatherB.mForecast == WeatherType.CHANCE_FLURRIES && iWeatherInfo.Type == WeatherType.FLURRIES) || (mWeatherB.mForecast == WeatherType.FLURRIES && iWeatherInfo.Type == WeatherType.CHANCE_FLURRIES)) ||
                            ((mWeatherB.mForecast == WeatherType.CHANCE_SLEET && iWeatherInfo.Type == WeatherType.SLEET) || (mWeatherB.mForecast == WeatherType.SLEET && iWeatherInfo.Type == WeatherType.CHANCE_SLEET)) ||
                            ((mWeatherB.mForecast == WeatherType.CHANCE_SNOW && iWeatherInfo.Type == WeatherType.SNOW) || (mWeatherB.mForecast == WeatherType.SNOW && iWeatherInfo.Type == WeatherType.CHANCE_SNOW)))
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
            
            if (mWeatherB.mName != "" && !string.IsNullOrEmpty(lAnswer))
            {
                Debug.Log(lAnswer);
                Buddy.Vocal.Say(lAnswer);
            }
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
                lDayString = Buddy.Resources.GetString("today");
            else if (iSayDayOfWeek)
                lDayString = Buddy.Resources.GetString(iDateTime.DayOfWeek.ToString().ToLower().Trim());
            else if (mWeatherB.mDate == 1)
                lDayString = Buddy.Resources.GetString("tomorrow");
            else if (mWeatherB.mDate == 2)
                lDayString = Buddy.Resources.GetString("dayaftertomorrow");
            else if (mWeatherB.mWeekend)
                lDayString = Buddy.Resources.GetString("weekend");
            else
                lDayString = Buddy.Resources.GetString("intime") + " " + mWeatherB.mDate + " " + Buddy.Resources.GetString("days");

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
                lDayString = Buddy.Resources.GetString("today");
            else if (mWeatherB.mDate == 1)
                lDayString = Buddy.Resources.GetString("tomorrow");
            else if (mWeatherB.mDate == 2)
                lDayString = Buddy.Resources.GetString("dayaftertomorrow");
            else if (mWeatherB.mWeekend)
                lDayString = Buddy.Resources.GetString("weekend");
            else
                lDayString = Buddy.Resources.GetString("intime") + " " + mWeatherB.mDate + " " + Buddy.Resources.GetString("days");

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
                int hourMin, tempMin, hourMax, tempMax;
                if (!RecoverMinMaxTempOfADay(iWeatherInfo.Day, out hourMin, out tempMin, out hourMax, out tempMax))
                    return lAnswer;

                if (hourMin == hourMax)
                {
                    lAnswer = Buddy.Resources.GetRandomString("lightrestitution");
                    lAnswer = lAnswer.Replace("[date]", iDayString + " " + Buddy.Resources.GetRandomString("inlocation") + " " + mWeatherB.mName);
                    lAnswer = lAnswer.Replace("[degree]", tempMin.ToString());
                    lAnswer = lAnswer.Replace("[forecast]", Buddy.Resources.GetRandomString((iWeatherInfo.Type.ToString().ToLower()).Replace("_", "")));
                }
                else
                {
                    lAnswer = Buddy.Resources.GetRandomString("restitution");
                    lAnswer = lAnswer.Replace("[date]", iDayString + " " + Buddy.Resources.GetRandomString("inlocation") + " " + mWeatherB.mName);
                    lAnswer = lAnswer.Replace("[minTemp]", tempMin.ToString());
                    lAnswer = lAnswer.Replace("[hourMin]", EnglishHour(hourMin));
                    lAnswer = lAnswer.Replace("[maxTemp]", tempMax.ToString());
                    lAnswer = lAnswer.Replace("[hourMax]", EnglishHour(hourMax));
                    lAnswer = lAnswer.Replace("[forecast]", Buddy.Resources.GetRandomString((iWeatherInfo.Type.ToString().ToLower()).Replace("_", "")));
                }
            }
            else
            {
                lAnswer = Buddy.Resources.GetRandomString("restraintrestitution");
                lAnswer = lAnswer.Replace("[date]", iDayString + " " + Buddy.Resources.GetRandomString("inlocation") + " " + mWeatherB.mName);
                lAnswer = lAnswer.Replace("[time]", Buddy.Resources.GetRandomString(mWeatherB.mWeatherTime.ToString().ToLower()));
                lAnswer = lAnswer.Replace("[degree]", iWeatherInfo.MinTemperature.ToString());
            }            
            return (lAnswer);
        }

        private bool CheckDataForRequestedDate(WeatherInfo[] iWeatherInfo)
        {
            DateTime lTime = DateTime.Today;
            if (mWeatherB.mDate > 0)
                lTime = lTime.AddDays(mWeatherB.mDate);

            if (iWeatherInfo.Length == 0)
                return false;

            for (int i = 0; i < iWeatherInfo.Length; i++)
            {
                if (iWeatherInfo[i].Day == lTime.Day && iWeatherInfo[i].Hour > FIRST_HOUR)
                {
                    // There is weather info for at least one daily hour in the day requested
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Min / Max Temperature
        /// </summary>
        /// <param name="iWeatherInfo">weather info</param>
        /// <param name="iIsMin">if true, get the minimum. if false, get the maximum</param>
        /// <param name="iDayString">the day</param>
        /// <returns>Instance of weatherInfo</returns>
        private bool RecoverMinMaxTempOfADay(int day, out int hourMin, out int tempMin, out int hourMax, out int tempMax)
        {
            float lMinTemp = 200F;
            float lMaxTemp = -200F;
            hourMax = 0;
            hourMin = 0;

            for (int i = 0; i < mWeatherB.mWeatherInfos.Length; i++)
            {
                if (mWeatherB.mWeatherInfos[i].Day == day && mWeatherB.mWeatherInfos[i].Hour > FIRST_HOUR)
                {
                    if (mWeatherB.mWeatherInfos[i].MinTemperature < lMinTemp)
                    {
                        lMinTemp = mWeatherB.mWeatherInfos[i].MinTemperature;
                        hourMin = mWeatherB.mWeatherInfos[i].Hour;
                    }
                    if (mWeatherB.mWeatherInfos[i].MaxTemperature > lMaxTemp)
                    {
                        lMaxTemp = mWeatherB.mWeatherInfos[i].MaxTemperature;
                        hourMax = mWeatherB.mWeatherInfos[i].Hour;
                    }
                }
            }
            
            tempMin = (int)Math.Round(lMinTemp);
            tempMax = (int)Math.Round(lMaxTemp);

            if (lMinTemp == 200F)
                return false;

            return true;

            //if (mWeatherB.mDate >= 1)
            //{
            //    for (int k = 0; k < mWeatherB.mDate; k++)
            //    {
            //        j++;
            //        while (j < (iWeatherInfo.Length-1) && iWeatherInfo[j].Hour != START_HOUR_MINMAX_TEMP)
            //            j++;
            //    }
            //}

            //if (mWeatherB.mDate >= 1)
            //    lTime = lTime.AddDays(mWeatherB.mDate);
            //if (mWeatherB.mWeekend)
            //    if (iDayString.Equals("sunday"))
            //    {
            //        lTime = lTime.AddDays(1);
            //    };
            //int index = j;

            //for (int i = 0; i < 8; i++)
            //{
            //    if (lTime.Day.ToString().Equals(iWeatherInfo[j].Day.ToString()))
            //    {
            //        if (iWeatherInfo[j].MinTemperature < lMinTemp)
            //        {
            //            lMinTemp = iWeatherInfo[j].MinTemperature;
            //            hourMin = iWeatherInfo[j].Hour;
            //        }
            //        if (iWeatherInfo[j].MaxTemperature > lMaxTemp)
            //        {
            //            lMaxTemp = iWeatherInfo[j].MaxTemperature;
            //            hourMax = iWeatherInfo[j].Hour;
            //        }
            //    }
            //    j++;
            //    if (j >= iWeatherInfo.Length)
            //        break;
            //}
            //// If the we don't have any information about today's weather, we set the weather of tomorrow 

            //if (lMinTemp == 200F)
            //{
            //    lTime = lTime.AddDays(1);
            //    for (int i = 0; i < 8; i++)
            //    {
            //        if (lTime.Day.ToString().Equals(iWeatherInfo[index].Day.ToString()))
            //        {
            //            if (iWeatherInfo[index].MinTemperature < lMinTemp)
            //            {
            //                lMinTemp = iWeatherInfo[index].MinTemperature;
            //                hourMin = iWeatherInfo[index].Hour;
            //            }
            //            if (iWeatherInfo[index].MaxTemperature > lMaxTemp)
            //            {
            //                lMaxTemp = iWeatherInfo[index].MaxTemperature;
            //                hourMax = iWeatherInfo[index].Hour;
            //            }
            //        }
            //        index++;
            //        if (index >= iWeatherInfo.Length)
            //            break;
            //    }
            //}

            //WeatherInfo lWeatherInfoMin = new WeatherInfo()
            //{
            //    MinTemperature = (int)Math.Ceiling(lMinTemp),
            //    Hour = hourMin
            //};
            //WeatherInfo lWeatherInfoMax = new WeatherInfo()
            //{
            //    MaxTemperature = (int)Math.Ceiling(lMaxTemp),
            //    Hour = hourMax
            //};
            //if (iIsMin)
            //    return (lWeatherInfoMin);
            //else
            //    return (lWeatherInfoMax);
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
                    while (j < (iWeatherInfo.Length-1) && iWeatherInfo[j].Hour != 6)
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
                if (j >= iWeatherInfo.Length)
                    break;
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
            string lAnswer = Buddy.Resources.GetRandomString("saycommand");
            lAnswer = lAnswer.Replace("[date]", lDayString);
            lAnswer = lAnswer.Replace("[localization]", mWeatherB.mName);

            if (mWeatherB.mCommand == WeatherBehaviour.WeatherCommand.MIN)
            {
                lAnswer = lAnswer.Replace("[temperature]", RecoverMinMaxTemperature(iWeatherInfo, true).MinTemperature.ToString());
                lAnswer = lAnswer.Replace("[command]", Buddy.Resources.GetString("min"));
            }
            else if (mWeatherB.mCommand == WeatherBehaviour.WeatherCommand.MAX)
            {
                lAnswer = lAnswer.Replace("[temperature]", RecoverMinMaxTemperature(iWeatherInfo, false).MaxTemperature.ToString());
                lAnswer = lAnswer.Replace("[command]", Buddy.Resources.GetString("max"));
            }
            Buddy.Vocal.Say(lAnswer);
        }

        private string EnglishHour(int Hour)
        {
            string lHour;

            if (Buddy.Platform.Language.OutputLanguage == Buddy.Platform.Language.GetLanguageFromISOCode(ISO6391Code.FR))
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

            if (Buddy.Platform.Language.OutputLanguage == Buddy.Platform.Language.GetLanguageFromISOCode(ISO6391Code.FR))
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

            mWeatherB.mIsOk = false;

            for (int d = 0; d <= lNbDays; d++)
            {
                int indice = mWeatherB.mIndice + NB_DATA_IN_DAY * d;

                if (indice >= 0 && indice < mWeatherB.mWeatherInfos.Length)
                    SayWeather(mWeatherB.mWeatherInfos[indice], mWeatherB.mWeekend);
                else if (d==0 && mWeatherB.mWeatherInfos.Length > 0)
                    SayWeather(mWeatherB.mWeatherInfos[0]);

                yield return new WaitForSeconds(3f);

                while (Buddy.Vocal.IsSpeaking)
                    yield return null;

                //if (mWeatherB.mWeekend)
                //{
                //    SayWeather(mWeatherB.mWeatherInfos[mWeatherB.mIndice + 4 * d], true);
                //}
                //else
                //{
                //    if ((mWeatherB.mIndice + 4 * d) == -1)
                //        SayWeather(mWeatherB.mWeatherInfos[0]);
                //    else
                //        SayWeather(mWeatherB.mWeatherInfos[mWeatherB.mIndice + 4 * d]);
                //}

                //float num = 0.1f;
                //int j = 0;

                ////Toaster.Display<BackgroundToast>().With();
                ////Buddy.GUI.Toaster.Display<CustomToast>();

                //mWeatherB.mIsOk = false;
                //if (mWeatherB.mDate >= 1)
                //{
                //    for (int k = 0; k < mWeatherB.mDate + d; k++)
                //    {
                //        j++;
                //        while (iWeatherInfo[j].Hour != 9)
                //            j++;
                //    }
                //}

                //for (int i = 0; i < 8; i++)
                //{
                //    //if (i >= 4)
                //    //{
                //    //    j++;
                //    //    while (iWeatherInfo[j].Hour != 12)
                //    //        j++;
                //    //    SetPanel(mTrip[i], iWeatherInfo[j], true);

                //    //}

                //    if (i < 4)
                //    {
                //        SetPanel(mTrip[i], iWeatherInfo[j], false);
                //        j++;
                //    }
                //    yield return new WaitForSeconds(num);
                //}

                //if (lNbDays == 0)
                //    yield return new WaitForSeconds(6f);
                //else
                //    yield return new WaitForSeconds(3f);

                //while (Buddy.Vocal.IsSpeaking)
                //    yield return null;

                //for (int i = 3; i >= 0; i--)
                //{
                //    mTrip[i].Cancel();
                //    yield return new WaitForSeconds(num);
                //}
                //Buddy.GUI.Toaster.Hide();
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
            Debug.Log("SETPANEL 1");
            string[] date = iWeatherInfo.Date.Split('-');
            DateTime dt = new DateTime(Int32.Parse(date[0]), Int32.Parse(date[1]), Int32.Parse(date[2]));
            Debug.Log("SETPANEL 2");
            if (iWeatherInfo.MinTemperature <= 0)
                iTrip.SetFrost();
            Debug.Log("SETPANEL 3");
            iTrip.SetText(iWeatherInfo.MinTemperature.ToString() + "°C");
            Debug.Log("SETPANEL 3.1");
            if (iWeatherInfo.Hour >= 8 && iWeatherInfo.Hour < 12)
            {
                Debug.Log("SETPANEL 4");
                iTrip.Morning();
                Debug.Log("SETPANEL 4.1");
                iTrip.SetMoment(Buddy.Resources.GetRandomString("morning").ToUpper());
                Debug.Log("SETPANEL 4.2");
            }
            else if (iWeatherInfo.Hour >= 12 && iWeatherInfo.Hour < 16)
            {
                Debug.Log("SETPANEL 5");
                iTrip.Noon();
                if (iDisplayDayOfWeek)
                    iTrip.SetMoment(Buddy.Resources.GetRandomString(dt.DayOfWeek.ToString().ToLower()).ToUpper());
                else
                    iTrip.SetMoment(Buddy.Resources.GetRandomString("noon").ToUpper());
            }
            else if (iWeatherInfo.Hour >= 16 && iWeatherInfo.Hour < 20)
            {
                Debug.Log("SETPANEL 6");
                iTrip.After_Noon();
                iTrip.SetMoment(Buddy.Resources.GetRandomString("afternoon").ToUpper());
            }
            else
            {
                Debug.Log("SETPANEL 7");
                iTrip.Evening();
                iTrip.ChangeMomentColor(Color.white);
                iTrip.ChangeTextColor(Color.white);
                iTrip.SetMoment(Buddy.Resources.GetRandomString("evening").ToUpper());
            }
            if (iWeatherInfo.Hour >= 8 && iWeatherInfo.Hour <= 18)
                iTrip.SetSun();
            else
                iTrip.SetMoon();

            Debug.Log("SETPANEL 8");
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
            Debug.Log("SETPANEL 9");
            iTrip.Open();
        }

        #endregion

    }
}