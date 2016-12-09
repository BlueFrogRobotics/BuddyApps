using System.Xml;
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
    }

    public class Recipe
    {
        [XmlAttribute("name")]
        public string name { get; set; }
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