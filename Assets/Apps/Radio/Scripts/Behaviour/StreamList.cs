using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Radio
{
    public class StreamList : MonoBehaviour
    {
        public List<StreamLink> StreamLinks;

        override public string ToString()
        {
            string lDesc = "*********************\n";
            if (StreamLinks != null) {
                int i = 0;
                foreach (StreamLink stream in StreamLinks) {
                    lDesc += "Stream " + i + "\n";
                    lDesc += stream.ToString();
                    lDesc += "\n*********************\n";
                    i++;
                }
            }
            return lDesc;
        }
    }
}