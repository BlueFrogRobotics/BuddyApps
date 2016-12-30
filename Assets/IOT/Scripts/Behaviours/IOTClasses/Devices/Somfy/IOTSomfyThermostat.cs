﻿using UnityEngine;
using System.Collections;
using BuddyOS.UI;

namespace BuddyApp.IOT
{
    public class IOTSomfyThermostat : IOTSomfyDevice
    {
        public IOTSomfyThermostat(IOTSomfyDevice iObject) : base(iObject)
        {
            mType = DeviceType.THERMOSTAT;
            mName = iObject.label;
            mSpriteName = "IOT_Device_Thermostat";
        }

        public IOTSomfyThermostat(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mType = DeviceType.THERMOSTAT;
            mName = iObject.label;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermostat";
        }

        public IOTSomfyThermostat(string iName, string iURL, string iSessionID) : base(null)
        {
            mType = DeviceType.THERMOSTAT;
            mName = iName;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermostat";
        }

        public override void InitializeParams()
        {
            GameObject lTemp = InstanciateParam(ParamType.GAUGE);
            Gauge lGaugeTemp = lTemp.GetComponent<Gauge>();

            lGaugeTemp.Label.text = "Temperature";
            lGaugeTemp.Slider.minValue = 70F;
            lGaugeTemp.Slider.maxValue = 300F;
            IOTTempGaugeCmd lGaugeCmd = new IOTTempGaugeCmd(this);
            lGaugeTemp.UpdateCommands.Add(lGaugeCmd);
        }

        public void SetTemperature(float lTemp)
        {
            PostAction("setComfortTemperature", new string[] { System.Convert.ToString(lTemp) });
        }

        public override void Command(int iCommand)
        {
            base.Command(iCommand);
            switch (iCommand)
            {
                case 4:
                    break;
                case 5:
                    break;
            }
        }
    }
}