using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;

using System.IO;
using System;

namespace BuddyApp.Weather
{
    public class CityCoordinates
    {
        private List<string> mNames;
        private double mLatitude;
        private double mLongitude;
        private string mCountryCode;

        public CityCoordinates(string countryCode, double latitude, double longitude)
        {
            mNames = new List<string>();
            mCountryCode = countryCode;
            mLatitude = latitude;
            mLongitude = longitude;
        }

        public void AddName(string name)
        {
            mNames.Add(name);
        }

        public bool GetCoordinates(string name, out double latitude, out double longitude)
        {
            latitude = 0.0;
            longitude = 0.0;
            foreach (string n in mNames)
            {
                if (n.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    latitude = mLatitude;
                    longitude = mLongitude;
                    return true;
                }
            }
            return false;
        }

    }

}