using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.UI;
using BuddyFeature.Web;

namespace BuddyApp.IOT
{
    public class IOTPhilipsHue : IOTSystems
    {

        public IOTPhilipsHue()
        {
            mName = "Philips HUE";
            mSpriteName = "IOT_System_Hue";
        }
        public override void InitializeParams()
        {
            base.InitializeParams();
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
            lSearchComponent.Label.resizeTextForBestFit = true;
            lSearchComponent.UpdateCommands.Add(lCmd);

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
            base.Connect();

            PlayerPrefs.SetString("philips_ip", mCredentials[0]);
            PlayerPrefs.SetString("philips_user", mCredentials[1]);
            PlayerPrefs.Save();
        }

        public override void GetDevices()
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
                    lDevice.Credentials[0] = mCredentials[0];
                    lDevice.Credentials[1] = mCredentials[1];
                    lDevice.Credentials[2] = mCredentials[2];
                    lDevice.Indice = i;
                    lDevice.Name = "DEVICE " + i.ToString("D2");
                }

                GetAllValues();
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
