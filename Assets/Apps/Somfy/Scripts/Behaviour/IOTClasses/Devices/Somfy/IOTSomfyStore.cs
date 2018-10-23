using UnityEngine;
using System.Collections;
using BlueQuark;
using System.Text;

namespace BuddyApp.Somfy
{
    public class IOTSomfyStore : IOTSomfyDevice
    {

        public IOTSomfyStore(IOTSomfyDevice iObject) : base(iObject)
        {
            mType = DeviceType.STORE;
            mName = iObject.label;
            mSpriteName = "IOT_Device_Store";
        }

        public IOTSomfyStore(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mType = DeviceType.STORE;
            mName = iObject.label;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Store";
        }

        public IOTSomfyStore(string iName, string iURL, string iSessionID) : base(null)
        {
            mType = DeviceType.STORE;
            mName = iName;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Store";

        }

        public override void Command(int iCommand, float iParam = 0.0F)
        {
            base.Command(iCommand);
            switch (iCommand)
            {
                case 2:
                    PostAction("close");
                    break;
                case 3:
                    PostAction("open");
                    break;
            }
        }
    }
}