using UnityEngine;
using System.Collections;
using BuddyOS.UI;

namespace BuddyApp.IOT
{
    public class IOTSomfyThermometer : IOTSomfyDevice
    {
        private Label mTemp;

        public IOTSomfyThermometer(IOTSomfyDevice iObject) : base(iObject)
        {
            mType = DeviceType.THERMOSTAT;
            mName = iObject.label;
            mSpriteName = "IOT_Device_Thermometer";
        }

        public IOTSomfyThermometer(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mType = DeviceType.THERMOSTAT;
            mName = iObject.label;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermometer";
        }

        public IOTSomfyThermometer(string iName, string iURL, string iSessionID) : base(null)
        {
            mType = DeviceType.THERMOSTAT;
            mName = iName;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermometer";
        }

        public override void InitializeParams()
        {
            GameObject lTemp = InstanciateParam(ParamType.LABEL);
            mTemp = lTemp.GetComponent<Label>();

            mTemp.Text = "Temperature : " + states[1].value;

        }

        public override void UpdateSlow()
        {
            ChangeStateValue("core:TemperatureState");
            mTemp.Text = "Temperature : " + states[1].value;
        }

    }
}