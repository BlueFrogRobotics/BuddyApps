using UnityEngine;
using System.Collections;
using BuddyFeature.Web;
using BuddyOS.UI;

namespace BuddyApp.IOT
{
    public class IOTPhilipsLightHUE : IOTDevices
    {
        private Hashtable mState;
        public Hashtable State { get { return mState; } set { mState = value; } }
        private int indice;
        public int Indice { get { return indice; } set { indice = value; } }

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

        public override void InitializeParams()
        {
            GameObject lOnOff = InstanciateParam(ParamType.ONOFF);
            GaugeOnOff lOnOffComponent = lOnOff.GetComponent<GaugeOnOff>();
            GameObject lIntensity = InstanciateParam(ParamType.GAUGE);
            Gauge lIntensityComponent = lIntensity.GetComponent<Gauge>();

            IOTSetIntensityCmd lCmd = new IOTSetIntensityCmd(this);
            lIntensityComponent.UpdateCommands.Add(lCmd);
        }

        private void SetValue(string[] iStr, object[] iVal)
        {
            Hashtable lLightSettings = new Hashtable();

            for (int i = 0; i < iStr.Length; i++)
            {
                string lStr = iStr[i];
                mState[lStr] = iVal[i];
                lLightSettings.Add(lStr, mState[lStr]);
            }

            string lPath = "http://" + mCredentials[0] + "/api/" + mCredentials[1] + "/lights/" + (indice + 1) + "/state";
            Request lRequest = new Request("PUT", lPath, lLightSettings);
            lRequest.Send((request) =>
            {
            });
        }

        public void GetValue()
        {
            string lPath = "http://" + mCredentials[0] + "/api/" + mCredentials[1] + "/lights/" + (indice + 1);
            Request lRequest = new Request("GET", lPath);
            lRequest.Send((request) =>
            {
                Hashtable lResult = request.response.Object;
                Hashtable lRealState = (Hashtable)lResult["state"];

                mState["on"] = lRealState["on"];
                mState["bri"] = lRealState["bri"];
                mState["hue"] = lRealState["hue"];
                mState["sat"] = lRealState["sat"];
                mState["effect"] = lRealState["effect"];
                mState["ct"] = lRealState["ct"];
                mState["alert"] = lRealState["alert"];
                mState["colormode"] = lRealState["colormode"];
                mState["reachable"] = lRealState["reachable"];

                if (lResult == null)
                {
                    return;
                }

            });
        }

        public void SetColor(Color iColor)
        {
            float lH = 0.0f, lS = 0.0f, lV = 0.0f;
            Color.RGBToHSV(iColor, out lH, out lS, out lV);
            string[] lKey = new string[3] { "bri", "hue", "sat" };
            object[] lValue = new object[3] {
            (int)(lV * 255.0f),
            (int)(lH * 65535.0f),
            (int)(lS * 255.0f)
        };
            SetValue(lKey, lValue);
        }

        public void SetIntensity(float iValue)
        {
            string[] lKey = new string[1] { "bri" };
            object[] lValue = new object[1] { (int)(iValue * 255.0f) };
            SetValue(lKey, lValue);
        }

        public void OnOff(bool iOnOff)
        {
            string[] lKey = new string[1] { "on" };
            object[] value = new object[1] { iOnOff };
            SetValue(lKey, value);
        }
    }
}
