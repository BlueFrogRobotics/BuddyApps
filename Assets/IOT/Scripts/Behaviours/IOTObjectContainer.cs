using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTObjectContainer : MonoBehaviour
    {
        private IOTObjects mObject;
        public IOTObjects Object { get { return mObject; } set { mObject = value; } }
    }
}
