using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System.IO;

namespace BuddyApp.Weather
{
    public sealed class CitiesManager : MonoBehaviour
    {
        public CitiesData CitiesData { get; private set; }

        private List<CitiesData> ListCities;


        // Use this for initialization
        void Awake()
        {
            CitiesData = new CitiesData();
            ListCities = new List<CitiesData>();

            string[] lCitiesfile = Directory.GetFiles(Buddy.Resources.GetRawFullPath("Cities"));

            CitiesData lCdata = Utils.UnserializeXML<CitiesData>(lCitiesfile[0]);

            ListCities.Add(lCdata);

            CitiesData = ListCities[0];
        }

    }
}