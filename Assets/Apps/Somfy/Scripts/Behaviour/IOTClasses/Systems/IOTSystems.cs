using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BlueQuark;

namespace BuddyApp.Somfy
{
    public class IOTSystems : IOTObjects
    {
        protected List<IOTDevices> mDevices = new List<IOTDevices>();
        public List<IOTDevices> Devices { get { return mDevices; } }

        protected Dictionary<string, string> mScenarios = new Dictionary<string, string>();
        public Dictionary<string, string> Scenarios { get { return mScenarios; } }

        //public virtual void Creation()
        //{
        //    mListIntantParams.Clear();
        //}

        //public override void Connect()
        //{
        //    Login();
        //    GetDevices();
        //}

        public override void OnOff(bool iOnOff)
        {
            foreach (IOTDevices lDevices in mDevices)
                lDevices.OnOff(iOnOff);
        }

        public virtual IEnumerator Login()
        {
            yield return null;
        }

        //public virtual void GetDevices()
        //{

        //}

        public virtual IEnumerator GetTheDevices()
        {
            yield return null;
        }

        public virtual IEnumerator GetScenarios()
        {
            yield return null;
        }

        public virtual IEnumerator LaunchScenario(string oid)
        {
            yield return null;
        }
    }
}
