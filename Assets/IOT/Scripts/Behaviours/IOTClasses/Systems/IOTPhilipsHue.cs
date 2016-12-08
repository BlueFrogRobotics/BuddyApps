using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.UI;
using BuddyFeature.Web;

namespace BuddyApp.IOT
{
    public class IOTPhilipsHue : IOTSystems
    {
        public override void InitializeParams()
        {
            GameObject lSearch = InstanciateParam(ParamType.TEXTFIELD);
            SearchField lSearchComponent = lSearch.GetComponent<SearchField>();
            GameObject lSearch1 = InstanciateParam(ParamType.TEXTFIELD);
            SearchField lSearch1Component = lSearch1.GetComponent<SearchField>();
            GameObject lSearch2 = InstanciateParam(ParamType.TEXTFIELD);
            SearchField lPasswordComponent = lSearch2.GetComponent<SearchField>();
            GameObject lConnect = InstanciateParam(ParamType.BUTTON);
            Button lConnectComponent = lConnect.GetComponent<Button>();

            IOTCredentialTextFieldCmd lCmd = new IOTCredentialTextFieldCmd(this, 0, "");
            lSearchComponent.Label.text = "IP";
            lSearchComponent.UpdateCommands.Add(lCmd);

            IOTCredentialTextFieldCmd lCmd1 = new IOTCredentialTextFieldCmd(this, 1, "");
            lSearch1Component.Label.text = "USERNAME";
            lSearch1Component.UpdateCommands.Add(lCmd1);

            IOTCredentialTextFieldCmd lCmd2 = new IOTCredentialTextFieldCmd(this, 2, "");
            lPasswordComponent.Label.text = "PASSWORD";
            lPasswordComponent.UpdateCommands.Add(lCmd2);

            IOTConnectCmd lCmd3 = new IOTConnectCmd(this);
            lConnectComponent.Label.text = "CONNECT";
            lConnectComponent.ClickCommands.Add(lCmd3);
        }

        public override void Connect()
        {
            AskLightsCount();
            GetAllValues();
        }


        private void AskLightsCount()
        {
            Request lRequest = new Request("GET", "http://" + Credentials[0] + "/api/" + Credentials[1] + "/lights");
            lRequest.Send((request) =>
            {
                Hashtable lResult = request.response.Object;
                mDevices.Clear();
                for(int i = 0; i < lResult.Count; ++i)
                    mDevices.Add(new IOTPhilipsLightHUE());
                for (int i = 0; i < mDevices.Count; ++i)
                {
                    IOTPhilipsLightHUE lDevice = (IOTPhilipsLightHUE)mDevices[i];
                    lDevice.Credentials[0] = Credentials[0];
                    lDevice.Credentials[1] = Credentials[1];
                    lDevice.Credentials[2] = Credentials[2];
                    lDevice.Indice = i;
                }

                if (lResult == null)
                {
                    return;
                }
            });
        }

        private void GetAllValues()
        {
            for (int i = 0; i < mDevices.Count; ++i)
                ((IOTPhilipsLightHUE)mDevices[i]).GetValue();
        }
        
        public void OnOffForAll(bool iState)
        {
            for(int i = 0; i < mDevices.Count; ++i)
                ((IOTPhilipsLightHUE)mDevices[i]).OnOff(iState);
        }
    }
}
