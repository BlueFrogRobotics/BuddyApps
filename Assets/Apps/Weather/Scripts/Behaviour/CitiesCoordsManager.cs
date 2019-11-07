using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using BlueQuark;
using System.IO;

namespace BuddyApp.Weather
{
    public sealed class CitiesCoordManager : MonoBehaviour
    {
        private List<CityCoordinates> ListCitiesCoord;


        // Use this for initialization
        void Awake()
        {
            ListCitiesCoord = new List<CityCoordinates>();

            string citiesCoordsFile = Buddy.Resources.GetRawFullPath("Cities_coordinates.csv");
            if (!string.IsNullOrEmpty(citiesCoordsFile)
                && File.Exists(citiesCoordsFile))
            {
                // Cities_coordinates.csv contains coordinates for main cities in the world
                // format : Latitude;Longitude;CountryCode;CountryNameInEnglish;CountryNameInAnotherLanguage;etc.
                using (var reader = new StreamReader(citiesCoordsFile))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        if (values.Length >= 4)
                        {
                            double latitude = 0.0f;
                            double longitude= 0.0f;
                            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
                            if  (!double.TryParse(values[0], NumberStyles.Number, culture, out latitude)
                            || !double.TryParse(values[1], NumberStyles.Number, culture, out longitude))
                            {
                                continue;
                            }
                            string countryCode = values[2];
                            CityCoordinates cityCoords = new CityCoordinates(countryCode, latitude, longitude);

                            for (int i = 3; i < values.Length; i++)
                                if (!string.IsNullOrEmpty(values[i].Trim()))
                                    cityCoords.AddName(values[i].Trim());

                            ListCitiesCoord.Add(cityCoords);
                        }
                    }
                }
            }
            Debug.Log("Cities number " + ListCitiesCoord.Count);
        }

        // Return true if the location has been found
        public bool GetCoordinates(string location, out double latitude, out double longitude)
        {
            latitude = 0.0;
            longitude = 0.0;

            foreach (CityCoordinates cityCoords in ListCitiesCoord)
            {
                if (cityCoords.GetCoordinates(location, out latitude, out longitude))
                    return true;
            }

            return false;
        }

    }
}