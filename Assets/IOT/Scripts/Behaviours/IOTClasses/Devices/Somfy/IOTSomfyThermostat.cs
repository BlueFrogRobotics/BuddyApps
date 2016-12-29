using UnityEngine;
using System.Collections;
using BuddyOS.UI;

namespace BuddyApp.IOT
{
    public class IOTSomfyThermostat : IOTSomfyDevice
    {
        public IOTSomfyThermostat(IOTSomfyDevice iObject) : base(iObject)
        {
            mType = DeviceType.SWITCH;
            mName = iObject.label;
            mSpriteName = "IOT_Device_Thermostat";
        }

        public IOTSomfyThermostat(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mType = DeviceType.SWITCH;
            mName = iObject.label;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermostat";
        }

        public IOTSomfyThermostat(string iName, string iURL, string iSessionID) : base(null)
        {
            mType = DeviceType.SWITCH;
            mName = iName;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermostat";
        }

        public override void InitializeParams()
        {
            base.InitializeParams();
            GameObject lOnOff = InstanciateParam(ParamType.ONOFF);
            OnOff lOnOffComponent = lOnOff.GetComponent<OnOff>();
            //GameObject lName = InstanciateParam(ParamType.TEXTFIELD);
            //TextField lNameComponent = lName.GetComponent<TextField>();

            lOnOffComponent.Label.text = "ON/OFF";
            lOnOffComponent.Label.resizeTextForBestFit = true;
            IOTOnOffCmd lCmdOnOff = new IOTOnOffCmd(this);
            lOnOffComponent.SwitchCommands.Add(lCmdOnOff);

            //lNameComponent.Label.text = "NAME";
            //lNameComponent.Label.resizeTextForBestFit = true;
            //IOTChangeNameCmd lCmdChangeName = new IOTChangeNameCmd(this);
            //lNameComponent.UpdateCommands.Add(lCmdChangeName);
        }
    }
}