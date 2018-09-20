using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;

using System.IO;
using System;

namespace BuddyApp.Weather
{

    [Serializable]
    public sealed class CityData
    {
        public string Name { get; set; }

        public string Key { get; set; }
    }

}