using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using Buddy;

namespace BuddyApp.ShoppingList
{
    /// <summary>
    /// Item of a shopping list that will be serialized into an xml
    /// </summary>
    public class Item
    {
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("quantity")]
        public float quantity { get; set; }
        [XmlAttribute("prep")]
        public string prep { get; set; }
    }

    /// <summary>
    /// Class used to serialize and deserialize a list of items
    /// </summary>
    public class ListSerializer
    {

        [XmlElement("shopList")]
        public List<Item> shopList { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ListSerializer()
        {
            shopList = new List<Item>();
        }

        /// <summary>
        /// Serialize the list of items into an xml
        /// </summary>
        /// <param name="iPathToXml">path to where the xml should be</param>
        public void Serialize(string iPathToXml)
        {
            Utils.SerializeXML<ListSerializer>(this, iPathToXml);

        }

        /// <summary>
        /// Deserialize a list of items from an xml
        /// </summary>
        /// <param name="iPathToXml">path of the xml</param>
        /// <returns>the object that contains the list of items deserialized</returns>
        public static ListSerializer Deserialize(string iPathToXml)
        {
            ListSerializer lObject = new ListSerializer();

            lObject = Utils.UnserializeXML<ListSerializer>(iPathToXml);
            return lObject;
        }

        /// <summary>
        /// Clone this class into an other object
        /// </summary>
        /// <returns>the clone of this object</returns>
        public ListSerializer Clone()
        {
            ListSerializer lListSerializer = new ListSerializer();
            List<Item> lListItem = new List<Item>();
            for (int i = 0; i < shopList.Count; i++)
            {
                Item lItem = new Item();
                lItem.name = shopList[i].name;
                lItem.prep = shopList[i].prep;
                lItem.quantity = shopList[i].quantity;
                lListItem.Add(lItem);
            }
            lListSerializer.shopList = lListItem;
            return lListSerializer;
        }
    }
}