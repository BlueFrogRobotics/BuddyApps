using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.UI;
namespace BuddyApp.IOT
{
    public class IOTSystems : IOTObjects
    {
        protected List<IOTDevices> mDevices = new List<IOTDevices>();
        public List<IOTDevices> Devices { get { return mDevices; } }

        public override void Connect()
        {
            Login();
            GetDevices();
        }

        public virtual void Login()
        {

        }

        public virtual void GetDevices()
        {

        }
    }
}
