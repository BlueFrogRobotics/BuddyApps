using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using BuddyTools;

namespace BuddyApp.Recipe
{

    public class Content
    {
        [XmlAttribute("prep")]
        public int prep { get; set; }
        [XmlAttribute("cook")]
        public int cook { get; set; }
        [XmlAttribute("person")]
        public int person { get; set; }
        [XmlElement("ingredient")]
        public List<Ingredient> ingredient { get; set; }
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