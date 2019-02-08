using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Radio
{
    public class RadioInfos
    {
        public string Permalink { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string LogoURL { get; set; }

        public string Country { get; set; }

        public string Language { get; set; }

        override public string ToString()
        {
            string lDesc = "";
            lDesc += "Permalink: " + Permalink;
            lDesc += "\nName: " + Name;
            lDesc += "\nDescription: " + Description;

            return lDesc;
        }
    }
}