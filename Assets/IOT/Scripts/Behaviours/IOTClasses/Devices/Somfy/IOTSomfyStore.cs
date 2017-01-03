using UnityEngine;
using System.Collections;
using BuddyOS.UI;
using BuddyFeature.Web;
using System.Text;

namespace BuddyApp.IOT
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

        public override void InitializeParams()
        {
            base.InitializeParams();
            GameObject lButtonClose = InstanciateParam(ParamType.BUTTON);
            Button lButtonCloseComponent = lButtonClose.GetComponent<Button>();
            GameObject lButtonOpen = InstanciateParam(ParamType.BUTTON);
            Button lButtonOpenComponent = lButtonOpen.GetComponent<Button>();
            GameObject lName = InstanciateParam(ParamType.TEXTFIELD);
            TextField lNameComponent = lName.GetComponent<TextField>();

            lButtonCloseComponent.Label.text = "CLOSE";
            IOTDeviceCmdCmd lClose = new IOTDeviceCmdCmd(this, 2);
            lButtonCloseComponent.ClickCommands.Add(lClose);

            lButtonOpenComponent.Label.text = "OPEN";
            IOTDeviceCmdCmd lOpen = new IOTDeviceCmdCmd(this, 3);
            lButtonOpenComponent.ClickCommands.Add(lOpen);

            lNameComponent.Label.text = "NAME";
            IOTChangeNameCmd lCmdChangeName = new IOTChangeNameCmd(this);
            lNameComponent.EndEditCommands.Add(lCmdChangeName);
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