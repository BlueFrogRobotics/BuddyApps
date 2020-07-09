using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.CoursTelepresence
{
    public class WheelsMessage
    {
        public string propertyName;
        public string speed;
        public string angularVelocity;

        public WheelsMessage(string iPropertyName, string iSpeed, string iAngularVelocity)
        {
            propertyName = iPropertyName;
            speed = iSpeed;
            angularVelocity = iAngularVelocity;
        }
    }
}