using UnityEngine;
using System.Collections;
using BuddyOS.UI;
using BuddyFeature.Web;

namespace BuddyApp.IOT
{
    public class IOTSomfyThermostat : IOTSomfyDevice
    {
        private float mTemp;
        private float mTempEco;
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
            GameObject lName = InstanciateParam(ParamType.TEXTFIELD);
            TextField lNameText = lName.GetComponent<TextField>();

            lGaugeTemp.Label.text = "SET TEMPERATURE";
            lGaugeTemp.DisplayPercentage = false;
            lGaugeTemp.Suffix = "°";
            lGaugeTemp.Slider.minValue = 70F;
            lGaugeTemp.Slider.maxValue = 300F;
            lGaugeTemp.Slider.value = (float)System.Convert.ToDouble(states[8].value) * 10F;
            IOTDeviceCmdCmd lGaugeCmd = new IOTDeviceCmdCmd(this, 4);
            lGaugeTemp.UpdateCommands.Add(lGaugeCmd);

            lGaugeTempEco.Label.text = "SET ECO TEMPERATURE";
            lGaugeTempEco.DisplayPercentage = false;
            lGaugeTempEco.Suffix = "°";
            lGaugeTempEco.Slider.minValue = 70F;
            lGaugeTempEco.Slider.maxValue = 300F;
            lGaugeTempEco.Slider.value = (float)System.Convert.ToDouble(states[7].value) * 10F;
            IOTDeviceCmdCmd lGaugeEcoCmd = new IOTDeviceCmdCmd(this, 5);
            lGaugeTemp.UpdateCommands.Add(lGaugeEcoCmd);

            lNameText.Label.text = "NAME";
            IOTChangeNameCmd lChangeName = new IOTChangeNameCmd(this);
            lNameText.UpdateCommands.Add(lChangeName);
        }

        public void GetValue()
        {
            string url = "https://ha102-1.overkiz.com/enduser-mobile-web/enduserAPI/setup/devices/" + deviceURL.Replace(":", "%3A").Replace("/", "%252F").Replace("#", "%23");

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
            base.Command(iCommand);
            switch (iCommand)
            {
                case 4:
                    PostAction("setComfortTemperature", new float[] { iParam });
                    GetValue();
                    break;
                case 5:
                    PostAction("setComfortTemperature", new float[] { (float)System.Convert.ToDouble(states[8].value) + 0.5F });
                    GetValue();
                    break;
                case 6:
                    PostAction("setComfortTemperature", new float[] { (float)System.Convert.ToDouble(states[8].value) - 0.5F });
                    GetValue();
                    break;
            }
        }
    }
}