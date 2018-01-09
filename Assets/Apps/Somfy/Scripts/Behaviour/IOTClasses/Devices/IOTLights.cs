using UnityEngine;
using System.Collections;

namespace BuddyApp.Somfy
{
    public class IOTLights : IOTDevices
    {
        public IOTLights()
        {
            mType = DeviceType.LIGHT;
        }
    }
}