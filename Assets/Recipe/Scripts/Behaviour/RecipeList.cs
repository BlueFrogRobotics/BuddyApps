﻿using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using BuddyTools;

namespace BuddyApp.Recipe
{
    public class Ingredient
    {
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("quantity")]
        public int quantity { get; set; }
        [XmlAttribute("unit")]
        public string unit { get; set; }
    }

    public class Recipe
    {
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("category")]
        public string category { get; set; }
        [XmlAttribute("illustration")]
        public string illustration { get; set; }
        [XmlAttribute("person")]
        public int person { get; set; }
        [XmlAttribute("time")]
        public string time { get; set; }
        [XmlAttribute("stars")]
        public int stars { get; set; }
        [XmlAttribute("xml")]
        public string xml { get; set; }
        [XmlElement("summary")]
        public string summary { get; set; }
        [XmlElement("ingredient")]
        public List<Ingredient> ingredient { get; set; }
    }

    public class RecipeList
    {
        [XmlElement("recipe")]
        public List<Recipe> recipe { get; set; }

        public static RecipeList Deserialize(string iPathToXml)
        {
            RecipeList lObject = new RecipeList();

            lObject = SerializeXML.DeSerializeObject<RecipeList>(iPathToXml);
            return lObject;
        }
    }
}