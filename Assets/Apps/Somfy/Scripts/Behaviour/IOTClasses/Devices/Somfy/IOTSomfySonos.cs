using UnityEngine;
using System.Text;
using Buddy;

namespace BuddyApp.Somfy
{
    public class IOTSomfySonos : IOTSomfyDevice
    {

        public IOTSomfySonos(IOTSomfyDevice iObject) : base(iObject)
        {
            mType = DeviceType.SPEAKER;
            mName = iObject.label;
            mSpriteName = "IOT_Device_Sonos";
        }

        public IOTSomfySonos(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mType = DeviceType.SPEAKER;
            mName = iObject.label;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Sonos";
        }

        public IOTSomfySonos(string iName, string iURL, string iSessionID) : base(null)
        {
            mType = DeviceType.SPEAKER;
            mName = iName;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Sonos";
        }


        //public override void UpdateSlow()
        //{
        //    Debug.Log("update la temperature");
        //    ChangeStateValue("core:TemperatureState");

        //}

        public override void Command(int iCommand, float iParam = 0)
        {
            base.Command(iCommand);
            switch (iCommand)
            {
                case 2:
                    PostAction("play");
                    break;
                case 3:
                    PostAction("stop");
                    break;
            }
        }
    }
}