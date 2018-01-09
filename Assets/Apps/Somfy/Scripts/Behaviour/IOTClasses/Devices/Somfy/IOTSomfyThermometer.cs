using UnityEngine;
using System.Text;
using Buddy;

namespace BuddyApp.Somfy
{
    public class IOTSomfyThermometer : IOTSomfyDevice
    {

        public IOTSomfyThermometer(IOTSomfyDevice iObject) : base(iObject)
        {
            mType = DeviceType.THERMOMETER;
            mName = iObject.label;
            mSpriteName = "IOT_Device_Thermometer";
        }

        public IOTSomfyThermometer(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mType = DeviceType.THERMOMETER;
            mName = iObject.label;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermometer";
        }

        public IOTSomfyThermometer(string iName, string iURL, string iSessionID) : base(null)
        {
            mType = DeviceType.THERMOMETER;
            mName = iName;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermometer";
        }


        public override void UpdateSlow()
        {
            Debug.Log("update la temperature");
            ChangeStateValue("core:TemperatureState");

        }

        public override void Command(int iCommand, float iParam = 0)
        {
            base.Command(iCommand, iParam);
            switch (iCommand)
            {
                case 7:
                    BYOS.Instance.Interaction.TextToSpeech.Say("The temperature is " + float.Parse(states[1].value).ToString().Replace(".", " point ") + " degrees");
                    break;
            }
        }
    }
}