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

        public virtual void Creation()
        {
            mListIntantParams.Clear();
        }

        public override void Connect()
        {
            Login();
            GetDevices();
        }

        public override void OnOff(bool iOnOff)
        {
            foreach (IOTDevices lDevices in mDevices)
                lDevices.OnOff(iOnOff);
        }

        public virtual void Login()
        {

        }

        public virtual void GetDevices()
        {

        }
    }
}
