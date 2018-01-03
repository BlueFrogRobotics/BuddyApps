using UnityEngine;
using System.Collections;
using Buddy;

namespace BuddyApp.Somfy
{
    public class IOTSomfyThermostat : IOTSomfyDevice
    {
        private float mTemp;
        private float mTempEco;
        public IOTSomfyThermostat(IOTSomfyDevice iObject) : base(iObject)
        {
            mType = DeviceType.THERMOSTAT;
            mName = iObject.label;
            mSpriteName = "IOT_Device_Thermo";
        }

        public IOTSomfyThermostat(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mType = DeviceType.THERMOSTAT;
            mName = iObject.label;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermo";
        }

        public IOTSomfyThermostat(string iName, string iURL, string iSessionID) : base(null)
        {
            mType = DeviceType.THERMOSTAT;
            mName = iName;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermo";
        }

        public void GetValue()
        {
            string url = SomfyData.Instance.URL_API + "/setup/devices/" + deviceURL.Replace(":", "%3A").Replace("/", "%252F").Replace("#", "%23");

            Request lRequest = new Request("GET", url);
            lRequest.cookieJar = null;
            lRequest.SetHeader("cookie", mSessionID);

            lRequest.Send((lResult) =>
            {
                if (lResult == null)
                {
                    Debug.LogError("Somfy not connected");
                    return;
                }
                Debug.Log(lResult.response.Text);
                IOTSomfyDevice lDevices = JsonUtility.FromJson<IOTSomfyDevice>(lResult.response.Text);
                if (lDevices != null)
                {
                    creationTime = lDevices.creationTime;
                    lastUpdateTime = lDevices.lastUpdateTime;
                    label = lDevices.label;
                    deviceURL = lDevices.deviceURL;
                    shortcut = lDevices.shortcut;
                    controllableName = lDevices.controllableName;
                    definition = lDevices.definition;
                    states = lDevices.states;
                    attributes = lDevices.attributes;
                    available = lDevices.available;
                    enabled = lDevices.enabled;
                    placeOID = lDevices.placeOID;
                    widget = lDevices.widget;
                    type = lDevices.type;
                    oid = lDevices.oid;
                    uiClass = lDevices.uiClass;
                }
            });
        }

        public override void Command(int iCommand, float iParam = 0.0F)
        {
            Debug.Log("commande thermos");
            base.Command(iCommand);
            switch (iCommand)
            {
                case 3:
                    PostAction("setModeTemperature", new object[] { "manualMode", iParam });
                        break;
                case 4:
                    PostAction("setComfortTemperature", new object[] { iParam });
                    //GetValue();
                    break;
                case 5:
                    PostAction("setComfortTemperature", new object[] { (float)System.Convert.ToDouble(states[8].value) + 0.5F });
                    //GetValue();
                    break;
                case 6:
                    PostAction("setComfortTemperature", new object[] { (float)System.Convert.ToDouble(states[8].value) - 0.5F });
                    //GetValue();
                    break;
            }
        }
    }
}