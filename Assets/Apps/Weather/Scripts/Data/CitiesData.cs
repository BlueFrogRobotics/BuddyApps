﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Buddy;

using System.IO;

namespace BuddyApp.Weather
{
    [Serializable]
    public class CitiesData
    {
        public List<CityData> Cities { get; set; }
    }

}