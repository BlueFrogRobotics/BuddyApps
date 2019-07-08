using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Net;


namespace BuddyApp.Weather
{
    public sealed class APIRequest : AStateMachineBehaviour
    {
        private int mDate;
        private int mNumberWeatherInfos;
        private bool mAnswerReceived;
        private bool mQuit;
        private int mTimeout;
        private OpenWeather mOpenWeather;

        public override void Start()
        {
            mWeatherB = GetComponent<WeatherBehaviour>();
            mTimeout = 0;
            mOpenWeather = GetComponent<OpenWeather>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mNumberWeatherInfos = 60;
            mAnswerReceived = false;
            mQuit = false;

            // Collect data and request the API accordingly
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Error. Check internet connection!");
            }
            Debug.Log("mWeatherB.mLocation " + mWeatherB.mLocation);
            if (mWeatherB.mLocation == "")
            {
                mWeatherB.mName = "Paris";
                mWeatherB.mLocation = "zmw:00000.45.07156";
            }
            Debug.Log("mWeatherB.mName " + mWeatherB.mName);
            mOpenWeather.RetreiveWeather(mWeatherB.mName, WeatherProcessing);

            //Buddy.WebServices.Weather.HourlyAt(mWeatherB.mLocation, WeatherProcessing, mNumberWeatherInfos);
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mAnswerReceived)
            {
                Trigger("Restitution");
            }
            else if (mQuit && !Buddy.Vocal.IsListening && !Buddy.Vocal.IsSpeaking )
                QuitApp();
        }

        private void WeatherProcessing(WeatherInfo[] iWeather, WeatherError iError)
        {
            mWeatherB.mWeatherInfos = iWeather;

            if (mTimeout >= 3)
            {
                Buddy.Vocal.SayKey("requestfailed");
                mTimeout = 0;
                mQuit = true;
                return;
            }
            Debug.Log("WeatherProcessing");
            if (iError != WeatherError.NONE)
            {
                switch (iError) 
                {
                    case WeatherError.UNKNOWN_LOCATION:
                        Buddy.Vocal.SayKey("locationissue");
                        mTimeout = 0;
                        break;
                    case WeatherError.GEOLOCALIZATION_FAILED:
                        Buddy.Vocal.SayKey("geolocfailed");
                        mTimeout = 0;
                        break;
                    case WeatherError.GEOLOCALIZATION_DISABLED:
                        Buddy.Vocal.SayKey("geolocdisable");
                        mTimeout = 0;
                        break;
                    case WeatherError.MANY_LOCATIONS:
                        CityData lNewCity = new CityData();
                        string[] lCitiesfile = Directory.GetFiles(Buddy.Resources.GetRawFullPath("Cities"));

                        lNewCity.Name = mWeatherB.mName;
                        lNewCity.Key = iWeather[0].Location.APICode;
                        mWeatherB.mLocation = lNewCity.Key;

                        mWeatherB.mCities.Cities.Add(lNewCity);

                        Utils.SerializeXML<CitiesData>(mWeatherB.mCities, lCitiesfile[0]);

                        Trigger("Reset");
                        mTimeout = 0;
                        break;
                    case WeatherError.REQUEST_FAILED:
                        mTimeout++;
                        Trigger("Reset");
                        break;
                }
                mQuit = true;
                return;
            }
            // If error are handle correctly, this shouldn't happen!
            else if (iWeather == null || iWeather.Length == 0)
            {
                Buddy.Vocal.SayKey("locationissue");
                mQuit = true;
                return;
            }
            else
            {
                GetWeatherInfos(iWeather, mWeatherB.mForecast);
                mTimeout = 0;
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
                        if (((iWeather[i].Type == WeatherType.CHANCE_OF_RAIN && iWeatherType == WeatherType.RAIN) || (iWeather[i].Type == WeatherType.RAIN && iWeatherType == WeatherType.CHANCE_OF_RAIN)) ||
                            ((iWeather[i].Type == WeatherType.CHANCE_FLURRIES && iWeatherType == WeatherType.FLURRIES) || (iWeather[i].Type == WeatherType.FLURRIES && iWeatherType == WeatherType.CHANCE_FLURRIES)) ||
                            ((iWeather[i].Type == WeatherType.CHANCE_SNOW && iWeatherType == WeatherType.SNOW) || (iWeather[i].Type == WeatherType.SNOW && iWeatherType == WeatherType.CHANCE_SNOW)) ||
                            ((iWeather[i].Type == WeatherType.CHANCE_SLEET && iWeatherType == WeatherType.SLEET) || (iWeather[i].Type == WeatherType.SLEET && iWeatherType == WeatherType.CHANCE_SLEET)) ||
                            iWeather[i].Type == iWeatherType)
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

        private bool GetResponse(string url, out string response)
        {
            response = "";
            try
            {
                // Create a new HttpWebRequest object.Make sure that 
                // a default proxy is set if you are behind a firewall.
                HttpWebRequest myHttpWebRequest1 =
                  (HttpWebRequest)WebRequest.Create(url);

                myHttpWebRequest1.KeepAlive = false;
                // Assign the response object of HttpWebRequest to a HttpWebResponse variable.
                HttpWebResponse myHttpWebResponse1 =
                  (HttpWebResponse)myHttpWebRequest1.GetResponse();

                Stream streamResponse = myHttpWebResponse1.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                Char[] readBuff = new Char[256];
                int count = streamRead.Read(readBuff, 0, 256);
                while (count > 0)
                {
                    String outputData = new String(readBuff, 0, count);
                    response += outputData;
                    count = streamRead.Read(readBuff, 0, 256);
                }
                // Close the Stream object.
                streamResponse.Close();
                streamRead.Close();
                // Release the resources held by response object.
                myHttpWebResponse1.Close();

                return true;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\nThe second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close'");
                Console.WriteLine("\n{0}", e.Message);
            }
            catch (WebException e)
            {
                Console.WriteLine("WebException raised!");
                Console.WriteLine("\n{0}", e.Message);
                Console.WriteLine("\n{0}", e.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception raised!");
                Console.WriteLine("Source :{0} ", e.Source);
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return false;
        }

        private string ReadLevelNode(XmlNode node, string nodeName, string level)
        {
            string res = "";
            if (node.Name == nodeName)
            {
                for (int j = 0; j < node.ChildNodes.Count; j++)
                {
                    XmlNode subsnode = node.ChildNodes[j];
                    if (subsnode.Attributes["val"] != null &&
                        (subsnode.Attributes["val"].InnerText == level))
                    {
                        return subsnode.InnerText;
                    }
                }
            }
            return res;
        }

        private bool ReadDoubleLevelNode(XmlNode node, string nodeName, string level, out double res)
        {
            res = 0.0f;
            string val = ReadLevelNode(node, nodeName, level);
            if (!string.IsNullOrEmpty(val))
            {
                if (double.TryParse(val, NumberStyles.Any, CultureInfo.CurrentCulture, out res))
                {
                    return true;
                }
            }
            return false;
        }

        private void ParseWeatherInfo(string lXMLData)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(lXMLData);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.Name == "echeance")
                {
                    if (node.Attributes["timestamp"] != null)
                    {
                        DateTime dt = DateTime.Now;
                        if (DateTime.TryParseExact(node.Attributes["timestamp"].InnerText, "yyyy-MM-dd HH:mm:ss UTC", null, System.Globalization.DateTimeStyles.None, out dt))
                        {
                            Console.Write("Date " + node.Attributes["timestamp"].InnerText + " " + dt.ToShortDateString() + " " + dt.ToShortTimeString() + "\n");
                        }
                    }

                    double tempMin = 100f;
                    double tempMax = -100f;
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        XmlNode subnode = node.ChildNodes[i];

                        double temp = 0.0f;
                        if (ReadDoubleLevelNode(subnode, "temperature", "2m", out temp))
                        {
                            temp -= 273.15f;
                            if (temp > tempMax) tempMax = temp;
                            if (temp < tempMin) tempMin = temp;
                            int itemp = (int)Math.Round(temp);
                            Console.Write("temperature " + temp + "\n");
                        }

                        double nebulosite = 0.0f;
                        if (ReadDoubleLevelNode(subnode, "nebulosite", "totale", out nebulosite))
                        {
                            Console.Write("nebulosite " + nebulosite + "\n");
                        }
                        double min_wind = 0.0f;
                        if (ReadDoubleLevelNode(subnode, "vent_moyen", "10m", out min_wind))
                        {
                            Console.Write("min_wind " + min_wind + "\n");
                        }
                        double max_wind = 0.0f;
                        if (ReadDoubleLevelNode(subnode, "vent_rafales", "10m", out max_wind))
                        {
                            Console.Write("max_wind " + max_wind + "\n");
                        }
                        if (subnode.Name == "pluie")
                        {
                            double mm_rain = 0.0f;
                            if (double.TryParse(subnode.InnerText, NumberStyles.Any, CultureInfo.CurrentCulture, out mm_rain))
                            {
                                Console.Write("rain " + mm_rain);
                            }
                        }
                        if (subnode.Name == "neige")
                        {
                            if (subnode.InnerText == "oui")
                            {
                                //oType = WeatherType.SNOW;
                            }
                        }

                        //double tr = 13.12f + 0.6215f * temp + (0.3965f * temp - 11.37f) * Math.Pow(vkm, 0.16f);

                    }

                    Console.WriteLine();
                }
            }

            return;

        }
    }
}