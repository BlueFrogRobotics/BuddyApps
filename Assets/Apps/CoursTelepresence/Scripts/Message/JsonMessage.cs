using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.CoursTelepresence
{
    public class JsonMessage
    {
        public string propertyName;
        public string propertyValue;

        public JsonMessage(string iPropertyName, string iPropertyValue)
        {
            propertyName = iPropertyName;
            propertyValue = iPropertyValue;
        }
    }
}