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
    }
}
