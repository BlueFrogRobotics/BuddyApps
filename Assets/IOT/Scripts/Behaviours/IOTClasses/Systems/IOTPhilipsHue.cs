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

        public override void Creation()
        {
            base.Creation();
            GameObject lSearch = InstanciateParam(ParamType.TEXTFIELD);
            TextField lSearchComponent = lSearch.GetComponent<TextField>();
            GameObject lSearch1 = InstanciateParam(ParamType.TEXTFIELD);
            TextField lSearch1Component = lSearch1.GetComponent<TextField>();
            GameObject lConnect = InstanciateParam(ParamType.BUTTON);
            Button lConnectComponent = lConnect.GetComponent<Button>();

            IOTCredentialTextFieldCmd lCmd = new IOTCredentialTextFieldCmd(this, 0, "");
            lSearchComponent.Label.text = "IP";
            if (PlayerPrefs.GetString("philips_ip") != "")
                lSearchComponent.Field.text = PlayerPrefs.GetString("philips_ip");
            lSearchComponent.UpdateCommands.Add(lCmd);

            IOTCredentialTextFieldCmd lCmd1 = new IOTCredentialTextFieldCmd(this, 1, "");
            lSearch1Component.Label.text = "USERNAME";
            if (PlayerPrefs.GetString("philips_user") != "")
                lSearch1Component.Field.text = PlayerPrefs.GetString("philips_user");
            lSearch1Component.UpdateCommands.Add(lCmd1);

            IOTConnectCmd lCmd3 = new IOTConnectCmd(this);
            lConnectComponent.Label.text = "CONNECT";
            lConnectComponent.ClickCommands.Add(lCmd3);
        }

        public override void InitializeParams()
        {
            base.InitializeParams();
            GameObject lSearch = InstanciateParam(ParamType.TEXTFIELD);
            TextField lSearchComponent = lSearch.GetComponent<TextField>();
            GameObject lSearch1 = InstanciateParam(ParamType.TEXTFIELD);
            TextField lSearch1Component = lSearch1.GetComponent<TextField>();
            GameObject lSearch2 = InstanciateParam(ParamType.TEXTFIELD);
            TextField lPasswordComponent = lSearch2.GetComponent<TextField>();
            GameObject lConnect = InstanciateParam(ParamType.BUTTON);
            Button lConnectComponent = lConnect.GetComponent<Button>();

            IOTCredentialTextFieldCmd lCmd = new IOTCredentialTextFieldCmd(this, 0, "");
            lSearchComponent.Label.text = "IP";
            if (PlayerPrefs.GetString("philips_ip") != "")
                lSearchComponent.Field.text = PlayerPrefs.GetString("philips_ip");
            lSearchComponent.UpdateCommands.Add(lCmd);

            IOTCredentialTextFieldCmd lCmd1 = new IOTCredentialTextFieldCmd(this, 1, "");
            lSearch1Component.Label.text = "USERNAME";
            if (PlayerPrefs.GetString("philips_user") != "")
                lSearch1Component.Field.text = PlayerPrefs.GetString("philips_user");
            lSearch1Component.UpdateCommands.Add(lCmd1);

            IOTCredentialTextFieldCmd lCmd2 = new IOTCredentialTextFieldCmd(this, 2, "");
            lPasswordComponent.Label.text = "PASSWORD";
            lPasswordComponent.Field.contentType = UnityEngine.UI.InputField.ContentType.Password;
            lPasswordComponent.UpdateCommands.Add(lCmd2);

            IOTConnectCmd lCmd3 = new IOTConnectCmd(this);
            lConnectComponent.Label.text = "CONNECT";
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
            mDevices.Clear();
            lRequest.Send((lResult) =>
            {
                if (lResult == null || lResult.response == null)
                {
                    Debug.LogError("Couldn't connect to Philips HUE");
                    mAvailable = false;
                    return;
                }
                Hashtable lObjects = lResult.response.Object;
                for(int i = 0; i < lObjects.Count; ++i)
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
