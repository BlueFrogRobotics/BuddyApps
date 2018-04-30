﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using System.IO;

namespace BuddyApp.Weather
{
    public class CitiesManager : MonoBehaviour
    {
        public CitiesData CitiesData { get; private set; }

        private List<CitiesData> ListCities;


        // Use this for initialization
        void Awake()
        {
            CitiesData = new CitiesData();
            ListCities = new List<CitiesData>();

            string[] lCitiesfile = Directory.GetFiles(BYOS.Instance.Resources.GetPathToRaw("Cities"));

            CitiesData lCdata = Utils.UnserializeXML<CitiesData>(lCitiesfile[0]);

            ListCities.Add(lCdata);

            CitiesData = ListCities[0];
        }

    }
}