using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.TeleBuddyQuatreDeux
{
    public class WheelsMotion
    {

        public float speed;
        public float angularVelocity;
        
        public WheelsMotion(float iSpeed, float iAngularVelocity)
        {
            speed = iSpeed;
            angularVelocity = iAngularVelocity;
        }
    }
}