using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTLights : IOTDevices
    {
        public IOTLights()
        {
            mType = DeviceType.LIGHT;
        }
    }
}