using UnityEngine;
using System.Text;
using BuddyOS.UI;
using BuddyOS;

namespace BuddyApp.IOT
{
    public class IOTSomfyThermometer : IOTSomfyDevice
    {
        private Label mTemp;
        private TextToSpeech mTTS;

        public IOTSomfyThermometer(IOTSomfyDevice iObject) : base(iObject)
        {
            mType = DeviceType.THERMOMETER;
            mName = iObject.label;
            mSpriteName = "IOT_Device_Thermometer";
            mTTS = BYOS.Instance.TextToSpeech;
        }

        public IOTSomfyThermometer(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mType = DeviceType.THERMOMETER;
            mName = iObject.label;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermometer";
            mTTS = BYOS.Instance.TextToSpeech;
        }

        public IOTSomfyThermometer(string iName, string iURL, string iSessionID) : base(null)
        {
            mType = DeviceType.THERMOMETER;
            mName = iName;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Thermometer";
            mTTS = BYOS.Instance.TextToSpeech;
        }

        public override void InitializeParams()
        {
            base.InitializeParams();
            GameObject lTemp = InstanciateParam(ParamType.LABEL);
            mTemp = lTemp.GetComponent<Label>();
            GameObject lName = InstanciateParam(ParamType.TEXTFIELD);
            TextField lNameText = lName.GetComponent<TextField>();

            mTemp.Text = "TEMPERATURE : " + states[1].value;
            
            lNameText.Label.text = "NAME";
            IOTChangeNameCmd lChangeName = new IOTChangeNameCmd(this);
            lNameText.UpdateCommands.Add(lChangeName);
        }

        public override void UpdateSlow()
        {
            ChangeStateValue("core:TemperatureState");
            mTemp.Text = "TEMPERATURE : " + states[1].value;
        }

        public override void Command(int iCommand, float iParam = 0)
        {
            base.Command(iCommand, iParam);
            switch (iCommand)
            {
                case 7:
                    mTTS.Say("The temperature is " + float.Parse(states[1].value).ToString().Replace(".", " point ") + " degrees");
                    break;
            }
        }
    }
}