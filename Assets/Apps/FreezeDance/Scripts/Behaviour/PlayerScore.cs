using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

namespace BuddyApp.FreezeDance
{
    public class PlayerScore 
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Score")]
        public int Score { get; set; }
    }

    public class PlayerList
    {
        [XmlElement("List")]
        public List<PlayerScore> List { get; set; }

        public PlayerList()
        {
            List = new List<PlayerScore>();
        }
    }
}