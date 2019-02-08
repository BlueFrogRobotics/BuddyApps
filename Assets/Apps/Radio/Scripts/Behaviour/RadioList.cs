using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Radio
{
    public class RadioList
    {
        public List<RadioInfos> Radios { get; set; }

        override public string ToString()
        {
            string lDesc = "*********************\n";
            if (Radios != null) {
                int i = 0;
                foreach (RadioInfos radio in Radios) {
                    lDesc += "Radio " + i + "\n";
                    lDesc += radio.ToString();
                    lDesc += "\n*********************\n";
                    i++;
                }
            }
            return lDesc;
        }
    }
}