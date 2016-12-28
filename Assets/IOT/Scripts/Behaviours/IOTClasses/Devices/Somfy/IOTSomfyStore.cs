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
        }

        public IOTSomfyStore(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mType = DeviceType.STORE;
            mName = iObject.label;
            mSessionID = iSessionID;
        }

        public IOTSomfyStore(string iName, string iURL, string iSessionID) : base(null)
        {
            mType = DeviceType.STORE;
            mName = iName;
            mSessionID = iSessionID;
        }

        public override void InitializeParams()
        {
            base.InitializeParams();
            GameObject lButtonClose = InstanciateParam(ParamType.BUTTON);
            Button lButtonCloseComponent = lButtonClose.GetComponent<Button>();
            GameObject lButtonOpen = InstanciateParam(ParamType.BUTTON);
            Button lButtonOpenComponent = lButtonOpen.GetComponent<Button>();

            lButtonCloseComponent.Label.text = "Close";
            lButtonCloseComponent.Label.resizeTextForBestFit = true;
            IOTStoreCmd lClose = new IOTStoreCmd(this, 2);
            lButtonCloseComponent.ClickCommands.Add(lClose);

            lButtonOpenComponent.Label.text = "Open";
            lButtonOpenComponent.Label.resizeTextForBestFit = true;
            IOTStoreCmd lOpen = new IOTStoreCmd(this, 3);
            lButtonOpenComponent.ClickCommands.Add(lOpen);
        }

        public override void Command(int iCommand)
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