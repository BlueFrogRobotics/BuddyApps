using UnityEngine;
using System.Collections;
using BuddyOS.UI;
using BuddyFeature.Web;

namespace BuddyApp.IOT
{
    public class IOTSomfy : IOTSystems
    {

        public IOTSomfy()
        {
            mName = "Philips HUE";
        }

        public override void InitializeParams()
        {
            GameObject lSearch1 = InstanciateParam(ParamType.TEXTFIELD);
            SearchField lSearch1Component = lSearch1.GetComponent<SearchField>();
            GameObject lSearch2 = InstanciateParam(ParamType.TEXTFIELD);
            SearchField lPasswordComponent = lSearch2.GetComponent<SearchField>();
            GameObject lConnect = InstanciateParam(ParamType.BUTTON);
            Button lConnectComponent = lConnect.GetComponent<Button>();

            IOTCredentialTextFieldCmd lCmd1 = new IOTCredentialTextFieldCmd(this, 1, "");
            lSearch1Component.Label.text = "USERNAME";
            lSearch1Component.Label.resizeTextForBestFit = true;
            lSearch1Component.UpdateCommands.Add(lCmd1);

            IOTCredentialTextFieldCmd lCmd2 = new IOTCredentialTextFieldCmd(this, 2, "");
            lPasswordComponent.Label.text = "PASSWORD";
            lPasswordComponent.Field.contentType = UnityEngine.UI.InputField.ContentType.Password;
            lPasswordComponent.Label.resizeTextForBestFit = true;
            lPasswordComponent.UpdateCommands.Add(lCmd2);

            IOTConnectCmd lCmd3 = new IOTConnectCmd(this);
            lConnectComponent.Label.text = "CONNECT";
            lConnectComponent.Label.resizeTextForBestFit = true;
            lConnectComponent.ClickCommands.Add(lCmd3);
        }

        public override void Connect()
        {

        }
    }
}
