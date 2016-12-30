using UnityEngine;
using System.Collections;
using BuddyOS.UI;

namespace BuddyApp.IOT
{
    public class IOTSomfyThermometer : IOTSomfyDevice
    {
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
            base.InitializeParams();
            GameObject lTemp = InstanciateParam(ParamType.LABEL);
            Label lTempComponent = lTemp.GetComponent<Label>();

            lTempComponent.Text = "Temperature : ";

        }



    }
}