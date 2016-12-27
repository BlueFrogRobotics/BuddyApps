using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.UI;
using BuddyFeature.Web;
using System.Linq;
using System.Text;

namespace BuddyApp.IOT
{
    public class IOTSomfy : IOTSystems
    {
        private string mSessionID;
        private Dictionary<string,string> mHeaders = new Dictionary<string, string>();

        public IOTSomfy()
        {
            mName = "Somfy";
            mSpriteName = "IOT_System_Somfy";
        }

        public override void InitializeParams()
        {
            base.InitializeParams();
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
            Login();

            PlayerPrefs.SetString("somfy_user", mCredentials[1]);
            PlayerPrefs.SetString("somfy_password", mCredentials[2]);
            PlayerPrefs.Save();
        }

        public override void Login()
        {
            string lUrl = "https://ha102-1.overkiz.com/enduser-mobile-web/enduserAPI/" + "login" + "?userId=" + Credentials[1] + "&userPassword=" + Credentials[2];
            Request lRequest = new Request("POST",lUrl);
            lRequest.Send((lResult) =>
            {
                if (lResult == null)
                {
                    Debug.LogError("Somfy not connected");
                    return;
                }
                mHeaders.Clear();
                mHeaders["SET-COOKIE"] = lResult.response.GetHeader("SET-COOKIE");
                getSessionId();
                GetDevices();
            });
        }

        public override void GetDevices()
        {
            string url = "https://ha102-1.overkiz.com/enduser-mobile-web/enduserAPI/" + "/setup/devices";

            Request lRequest = new Request("GET", url);
            lRequest.cookieJar = null;
            lRequest.SetHeader("cookie", mSessionID);

            lRequest.Send((lResult) =>
            {
                if (lResult == null)
                {
                    Debug.LogError("Somfy not connected");
                    return;
                }
                IOTSomfyDeviceCollection lDevices = JsonUtility.FromJson<IOTSomfyDeviceCollection>("{\"devices\" :" +lResult.response.Text + "}");
                Debug.Log("{\"devices\" :" + lResult.response.Text + "}");
                if(lDevices != null)
                {
                    mDevices = lDevices.devices.ToList<IOTDevices>();
                    int j = 0;
                    for (int i = 0; i < mDevices.Count; ++i)
                    {
                        string iUiClass = ((IOTSomfyDevice)mDevices[i]).uiClass;
                        if (iUiClass == "Pod")
                        {
                            mDevices.RemoveAt(i);
                            i--;
                            j++;
                        }
                        else
                        {
                            mDevices[i].Credentials[1] = mCredentials[1];
                            mDevices[i].Credentials[2] = mCredentials[2];
                            if (iUiClass == "OnOff")
                                mDevices[i] = new IOTSomfySwitch(lDevices.devices[i + j], mSessionID);
                            else if (iUiClass == "Screen")
                                mDevices[i] = new IOTSomfyStore(lDevices.devices[i + j], mSessionID);
                        }
                    }
                }
            });
        }

        private string getSessionId()
        {
            string res = null;
            string[] data = null;
            if (mHeaders != null)
            {
                foreach (KeyValuePair<string,string> post_arg in mHeaders)
                {
                    if (post_arg.Key.Equals("SET-COOKIE"))
                    {
                        data = ((string)post_arg.Value).Split(";"[0]);
                        if (data.Length > 0)
                        {
                            res = data[0];
                            mSessionID = res;
                            Debug.Log("sessionId: " + res);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("headers is null");
            }
            return res;
        }
    }
}
