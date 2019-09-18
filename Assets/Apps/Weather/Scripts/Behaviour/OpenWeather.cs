using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Net;

using UnityEngine;
using BlueQuark;

namespace BuddyApp.Weather
{

    public class OpenWeather : MonoBehaviour
    {
        private const string URL_WEATHER_OPENWEATHER = "http://api.openweathermap.org/data/2.5/forecast?APPID=ace9da3d335c3e0174e21a213e320943&mode=xml&units=metric";//&q=Paris";

        private Dictionary<int, WeatherType> mOWWeatherTypes;

        private bool mProcessing;

        // Use this for initialization
        void Awake()
        {
            mProcessing = false;

            mOWWeatherTypes = new Dictionary<int, WeatherType>();

            string weatherTypesFile = Buddy.Resources.GetRawFullPath("Weather_types.csv");
            if (!string.IsNullOrEmpty(weatherTypesFile))
            {

                // weather_types.csv contains weather types from openweather api
                // format : OW_ID;WeatherType_ID
                using (var reader = new StreamReader(weatherTypesFile))
                {
                    mOWWeatherTypes.Clear();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        if (values.Length >= 2)
                        {
                            string val = values[0];
                            int id = 0;
                            if (int.TryParse(val, out id))
                            {
                                if (mOWWeatherTypes.ContainsKey(id))
                                {
                                    Console.WriteLine("Weather type already registered " + id);
                                    continue;
                                }
                                string weatherType = values[1];
                                int weatherTypeId = 0;
                                if (int.TryParse(weatherType, out weatherTypeId))
                                {
                                    if (weatherTypeId <= 20)
                                        mOWWeatherTypes.Add(id, (WeatherType)weatherTypeId);
                                }

                            }
                        }
                    }
                }
            }
            Debug.Log("Number registered Weather types :" + mOWWeatherTypes.Count);
        }


        public WeatherType GetWeatherType(int OW_ID)
        {
            if (mOWWeatherTypes.ContainsKey(OW_ID))
            {
                return mOWWeatherTypes[OW_ID];
            }
            Debug.LogError("Unkwown weather type " + OW_ID);
            return WeatherType.UNKNOWN;
        }

        public void RetreiveWeather(string location, Action<WeatherInfo[], WeatherError> iOnEndRequest)
        {

            if (mProcessing)
            {
                //Log.E(LogModule.WEB_SERVICE, typeof(Weather),
                //    LogStatus.FAILURE, LogInfo.BUSY,
                //    "Already processing request");
                return;
            }

            mProcessing = true;
            StartCoroutine(RequestWeatherAsync(location, iOnEndRequest));
        }

        private IEnumerator RequestWeatherAsync(string location, Action<WeatherInfo[], WeatherError> iCallback)
        {
            string url_weather_localisation = URL_WEATHER_OPENWEATHER + "&q=" + location;
            Console.WriteLine(url_weather_localisation);

            string lXMLData = string.Empty;
            yield return StartCoroutine(RequestAsync(url_weather_localisation, value => lXMLData = value));

            if (string.IsNullOrEmpty(lXMLData))
            {
                iCallback(new WeatherInfo[0], WeatherError.REQUEST_FAILED);
                mProcessing = false;
                yield break;
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(lXMLData);

            // Get shift from UTC
            int secondsFromUTC = 0; // shift in seconds from UTC 
            XmlNodeList locationNodes = doc.DocumentElement.GetElementsByTagName("location");
            if (locationNodes.Count != 0)
            {
                foreach (XmlNode node in locationNodes[0].ChildNodes)
                {
                    if (node.Name == "timezone")
                    {
                        int.TryParse(node.InnerText, out secondsFromUTC);
                        Debug.Log("Shift in seconds from UTC time :" + secondsFromUTC);
                    }
                }
            }

            XmlNodeList list = doc.DocumentElement.GetElementsByTagName("forecast");
            if (list.Count == 0)
                yield break;

            List<WeatherInfo> lInfos = new List<WeatherInfo>();

            foreach (XmlNode node in list[0].ChildNodes)
            {
                if (node.Name == "time")
                {
                    DateTime dt = DateTime.Now;
                    double tempMin = 0.0f;
                    double tempMax = 0.0f;
                    double mm_rain = 0.0f;
                    double windSpeed = 0.0f; // meters/second
                    double windDirection = 0.0f; // degrees
                    WeatherType weatherType = WeatherType.UNKNOWN;

                    if (node.Attributes["to"] != null)
                    {
                        string dateStr = node.Attributes["to"].InnerText;
                        if (DateTime.TryParseExact(dateStr, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.AdjustToUniversal, out dt))
                        {
                            dt = dt.AddSeconds(secondsFromUTC);

                            Debug.Log("Date " + dateStr + " " + dt.ToShortDateString() + " " + dt.ToShortTimeString() + "\n");
                        }
                    }

                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        XmlNode subnode = node.ChildNodes[i];

                        if (subnode.Name == "temperature")
                        {
                            string unit = "";
                            ReadDoubleAttribute(subnode, "min", out tempMin);
                            ReadDoubleAttribute(subnode, "max", out tempMax);
                            if (subnode.Attributes["unit"] != null)
                                unit = subnode.Attributes["unit"].InnerText;

                            Debug.Log("Temperature unit " + unit + " temp " + tempMin);
                        }


                        if (subnode.Name == "symbol")
                        {
                            int OW_ID = 0;
                            if (subnode.Attributes["number"] != null
                                && int.TryParse(subnode.Attributes["number"].InnerText, out OW_ID))
                            {
                                weatherType = GetWeatherType(OW_ID);
                                Debug.Log("Type : " + weatherType);
                            }
                        }


                        if (subnode.Name == "windDirection ")
                        {
                            ReadDoubleAttribute(subnode, "deg", out windDirection);
                        }
                        if (subnode.Name == "windSpeed")
                        {
                            ReadDoubleAttribute(subnode, "mps", out windSpeed);
                        }
                        if (subnode.Name == "precipitation")
                        {
                            if (ReadDoubleAttribute(subnode, "value", out mm_rain))
                            {
                                if (subnode.Attributes["type"] != null)
                                {
                                    // Possible types : rain, snow
                                    string rainType = subnode.Attributes["type"].InnerText;
                                    Debug.Log(rainType + " : " + mm_rain);
                                }
                            }
                        }
                    }

                    WeatherLoc lLocation = new WeatherLoc()
                    {
                        City = location
                    };

                    lInfos.Add(new WeatherInfo
                    {
                        Location = lLocation,
                        Hour = dt.Hour,
                        Day = dt.Day,
                        Date = dt.ToString("yyyy-MM-dd"),
                        Type = weatherType,
                        Rain = (float)mm_rain,
                        MinTemperature = (int)Math.Round(tempMin),
                        MaxTemperature = (int)Math.Round(tempMax),
                        AverageWindSpeed = (float)windSpeed,
                        GustWindSpeed = (float)windSpeed,
                        DegreeWind = (float)windDirection
                    });
                }
            }

            iCallback(lInfos.ToArray(), WeatherError.NONE);

            mProcessing = false;

        }

        private bool ReadDoubleAttribute(XmlNode node, string attributeName, out double res)
        {
            res = 0.0f;
            if (node.Attributes[attributeName] != null)
            {
                if (double.TryParse(node.Attributes[attributeName].InnerText, NumberStyles.Any, new CultureInfo("en-US"), out res))
                {
                    return true;
                }
                Debug.Log("Error reading " + attributeName + " for " + node.Name + " : " + node.Attributes[attributeName].InnerText);
            }
            Debug.Log("No attribute " + attributeName + " for " + node.Name);
            return false;
        }


        private IEnumerator RequestAsync(string url, Action<string> iOnEndRequest)
        {
            string response = "";
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
                    Debug.Log(outputData);
                    count = streamRead.Read(readBuff, 0, 256);
                }
                // Close the Stream object.
                streamResponse.Close();
                streamRead.Close();
                // Release the resources held by response object.
                myHttpWebResponse1.Close();

                iOnEndRequest(response);
            }
            catch (ArgumentException e)
            {
                Debug.LogError("\nThe second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' " + e.Message);
            }
            catch (WebException e)
            {
                Debug.LogError("WebException raised " + e.Status + " " + e.Message);
            }
            catch (Exception e)
            {
                Debug.LogError("Exception raised " + e.Source + " " + e.Message);
            }

            yield break;
        }

    
    }
}
