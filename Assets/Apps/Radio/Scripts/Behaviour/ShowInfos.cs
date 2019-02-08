using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Radio
{
    public class ShowInfos
    {

        public string Name;

        public string Baseline;

        public string Start;

        public string End;

        public string Logo;

        public string Singer;

        public string Song;

        override public string ToString()
        {
            string lDesc = "";
            lDesc += "Name: " + Name;
            lDesc += "\nBaseline: " + Baseline;
            lDesc += "\nStart: " + Start;
            lDesc += "\nEnd: " + End;
            lDesc += "\nLogo: " + Logo;
            lDesc += "\nSinger: " + Singer;
            lDesc += "\nSong: " + Song;

            return lDesc;
        }
    }
}