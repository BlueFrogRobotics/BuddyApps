using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Radio
{
    public sealed class StreamLink
    {
        public string Url { get; set; }

        public string Protocol { get; set; }

        public int Port { get; set; }

        public string Format { get; set; }

        public string Codec { get; set; }

        public int Bitrate { get; set; }

        public int Frequency { get; set; }

        public int Channels { get; set; }

        override public string ToString()
        {
            string lDesc = "";
            lDesc += "Url: " + Url;
            lDesc += "\nProtocol: " + Protocol;
            lDesc += "\nFormat: " + Format;
            lDesc += "\nCodec: " + Codec;
            lDesc += "\nFrequency: " + Frequency;

            return lDesc;
        }

    }
}