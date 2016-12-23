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
            mName = iObject.label;
        }

        public IOTSomfyStore(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mName = iObject.label;
            mSessionID = iSessionID;
        }

        public IOTSomfyStore(string iName, string iURL, string iSessionID) : base(null)
        {
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
            IOTStoreCmd lClose = new IOTStoreCmd(this, 0);
            lButtonCloseComponent.ClickCommands.Add(lClose);

            lButtonOpenComponent.Label.text = "Open";
            lButtonOpenComponent.Label.resizeTextForBestFit = true;
            IOTStoreCmd lOpen = new IOTStoreCmd(this, 1);
            lButtonOpenComponent.ClickCommands.Add(lOpen);
        }

        public void Command(int iCommand)
        {
            switch (iCommand)
            {
                case 0:
                    PostAction("close");
                    break;
                case 1:
                    PostAction("open");
                    break;
            }
        }
    }
}