using UnityEngine;
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
            base.InitializeParams();
            GameObject lTemp = InstanciateParam(ParamType.GAUGE);
            Gauge lGaugeTemp = lTemp.GetComponent<Gauge>();
            GameObject lTempEco = InstanciateParam(ParamType.GAUGE);
            Gauge lGaugeTempEco = lTempEco.GetComponent<Gauge>();

            lGaugeTemp.Label.text = "SET TEMPERATURE";
            lGaugeTemp.Slider.minValue = 70F;
            lGaugeTemp.Slider.maxValue = 300F;
            IOTDeviceCmdCmd lGaugeCmd = new IOTDeviceCmdCmd(this, 4);
            lGaugeTemp.UpdateCommands.Add(lGaugeCmd);

            lGaugeTempEco.Label.text = "SET ECO TEMPERATURE";
            lGaugeTempEco.Slider.minValue = 70F;
            lGaugeTempEco.Slider.maxValue = 300F;
            IOTDeviceCmdCmd lGaugeEcoCmd = new IOTDeviceCmdCmd(this, 5);
            lGaugeTemp.UpdateCommands.Add(lGaugeEcoCmd);
        }
        public override void Command(int iCommand, float iParam = 0.0F)
        {
            base.Command(iCommand);
            switch (iCommand)
            {
                case 4:
                    PostAction("setComfortTemperature", new float[] { iParam });
                    break;
                case 5:
                case 6:
                    PostAction("setComfortTemperature", new float[] { iParam });
                    break;
            }
        }
    }
}