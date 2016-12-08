using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using BuddyTools;

namespace BuddyApp.Recipe
{

    public class Position
    {
        [XmlAttribute("x")]
        public int x { get; set; }
        [XmlAttribute("y")]
        public int y { get; set; }
        [XmlAttribute("width")]
        public int width { get; set; }
        [XmlAttribute("height")]
        public int height { get; set; }
    }

    public class Transition
    {
        [XmlAttribute("type")]
        public string type { get; set; }
        [XmlAttribute("value")]
        public int value { get; set; }
        [XmlAttribute("target")]
        public int target { get; set; }
        [XmlElement("position")]
        public Position position { get; set; }
    }

    public class TransitionList
    {
        [XmlElement("transition")]
        public List<Transition> transition { get; set; }
    }

    public class DefaultNextStep
    {
        [XmlAttribute("target")]
        public int target { get; set; }
    }

    public class Media
    {
        [XmlAttribute("type")]
        public string type { get; set; }
        [XmlAttribute("path")]
        public string path { get; set; }
        [XmlElement("position")]
        public Position position { get; set; }
    }

    public class SentenceToSay
    {
        [XmlAttribute("sentence")]
        public string sentence { get; set; }
    }

    public class SentenceToDisplay
    {
        [XmlAttribute("sentence")]
        public string sentence { get; set; }
        [XmlElement("position")]
        public Position position { get; set; }
    }

    public class Step
    {
        [XmlAttribute("id")]
        public int id { get; set; }
        [XmlAttribute("typeStep")]
        public string typeStep { get; set; }
        [XmlElement("media")]
        public Media media { get; set; }
        [XmlElement("sentenceToSay")]
        public SentenceToSay sentenceToSay { get; set; }
        [XmlElement("sentenceToDisplay")]
        public SentenceToDisplay sentenceToDisplay { get; set; }
        [XmlElement("transitionList")]
        public TransitionList transitionList { get; set; }
        [XmlElement("defaultNextStep")]
        public DefaultNextStep defaultNextStep { get; set; }
    }

    public class Content
    {
        [XmlAttribute("type")]
        public string type { get; set; }
        [XmlElement("step")]
        public List<Step> step { get; set; }

        public static Content Deserialize(string iPathToXml)
        {
            Content lObject = new Content();

            lObject = SerializeXML.DeSerializeObject<Content>(iPathToXml);
            return lObject;
        }
    }

}