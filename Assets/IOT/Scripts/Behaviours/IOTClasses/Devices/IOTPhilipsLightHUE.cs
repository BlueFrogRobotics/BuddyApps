using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTPhilipsLightHUE : IOTDevices
    {
        public IOTPhilipsLightHUE()
        {
            mState = new Hashtable();
            mState.Add("on", false);
            mState.Add("bri", 254);
            mState.Add("hue", 14910);
            mState.Add("sat", 144);
            mState.Add("effect", "none");
            mState.Add("ct", 500);
            mState.Add("alert", "none");
            mState.Add("colormode", "xy");
            mState.Add("reachable", false);

            indice = 0;
        }

        private Hashtable mState;
        public Hashtable State { get { return mState; } set { mState = value; } }
        private int indice;
        public int Indice { get { return indice; } set { indice = value; } }

        private void setValue(string[] iStr, object[] iVal)
        {
            Hashtable lLightSettings = new Hashtable();

            for (int i = 0; i < iStr.Length; i++)
            {
                string lStr = iStr[i];
                mState[lStr] = iVal[i];
                lLightSettings.Add(lStr, mState[lStr]);
            }

            string lPath = "http://" + Credentials[0] + "/api/" + Credentials[1] + "/lights/" + (indice + 1) + "/state";
            HTTP.Request theRequest = new HTTP.Request("PUT", lPath, lLightSettings);
            theRequest.Send((request) =>
            {
            });
        }

        public void OnOff(bool iOnOff)
        {
            string[] lKey = new string[1] { "on" };
            object[] value = new object[1] { iOnOff };
            setValue(lKey, value);
        }
    }
}
