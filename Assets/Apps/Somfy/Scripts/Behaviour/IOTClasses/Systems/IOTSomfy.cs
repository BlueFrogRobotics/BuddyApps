using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BlueQuark;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
//using System.Web; 

namespace BuddyApp.Somfy
{
    public class IOTSomfy : IOTSystems
    {
        private string mSessionID;
        private Dictionary<string, string> mHeaders = new Dictionary<string, string>();

        public IOTSomfy()
        {
            mName = "Somfy";
            mSpriteName = "IOT_System_Somfy";
        }


        public override void Connect()
        {
            Login();

            //PlayerPrefs.SetString("somfy_user", mCredentials[1]);
            //PlayerPrefs.SetString("somfy_password", mCredentials[2]);
            //PlayerPrefs.Save();
        }

        public override void Login()
        {
            string lUrl = SomfyData.Instance.URL_API + "/" + "login"/* + "?userId=" + Credentials[1] + "&userPassword=" + Credentials[2]*/;
            Hashtable user = new Hashtable();
            Credentials[1] = SomfyData.Instance.Login;//"innofair2";
            Credentials[2] = SomfyData.Instance.Password;//"2016fair2";
            user["userId"] = Credentials[1];
            user["userPassword"] = Credentials[2];
            /*Request lRequest = new Request("POST",lUrl, user);
            lRequest.Send((lResult) =>
            {
                if (lResult == null)
                {
                    Debug.LogError("Somfy not connected");
                    mAvailable = false;
                    return;
                }
                Debug.Log(lResult.response.Text);
                mHeaders.Clear();
                mHeaders["SET-COOKIE"] = lResult.response.GetHeader("SET-COOKIE");
                getSessionId();
                GetDevices();
            });*/
            /*WWWForm form = new WWWForm();
            form.AddField("userId", Credentials[1]);
            form.AddField("userPassword", Credentials[2]);
            UnityWebRequest www = UnityWebRequest.Post("https://ha102-1.overkiz.com/enduser-mobile-web/enduserAPI/" + "login",form);
            www.Send();
            while (!www.isDone)
            {

            }
            Debug.Log(www.error);*/
            WWWForm form = new WWWForm();
            form.AddField("userId", Credentials[1]);
            form.AddField("userPassword", Credentials[2]);
            //formData.Add(new MultipartFormDataSection("userId="+id+"&userPassword="+password));

            UnityWebRequest www = UnityWebRequest.Post(SomfyData.Instance.URL_API + "/login", form);
            //yield return www.Send();
            www.Send();
            while (!www.isDone)
            {

            }
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                mHeaders.Clear();
                mHeaders["SET-COOKIE"] = www.GetResponseHeader("SET-COOKIE");
                getSessionId();
                GetDevices();
            }

            Upload(Credentials[1], Credentials[2]);

        }


        void Upload(string id, string password)
        {
            //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            WWWForm form = new WWWForm();
            form.AddField("userId", id);
            form.AddField("userPassword", password);
            //formData.Add(new MultipartFormDataSection("userId="+id+"&userPassword="+password));

            UnityWebRequest www = UnityWebRequest.Post(SomfyData.Instance.URL_API + "/login", form);
            //yield return www.Send();
            www.Send();
            while (!www.isDone)
            {

            }
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!" + www.downloadHandler.text);
            }
        }

        public override void GetDevices()
        {
            string url = SomfyData.Instance.URL_API + "/setup/devices";

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
                string response = "{\"devices\" :" + lResult.response.Text + "}";
                //JSONNode lJsonNode = Buddy.JSON.Parse(response);

                //IOTSomfyDeviceCollection lDevices = JsonUtility.FromJson<IOTSomfyDeviceCollection>(response.Trim());
                IOTSomfyDeviceCollection lDevices = new IOTSomfyDeviceCollection(response);
                //lDevices.devices = new IOTSomfyDevice[lJsonNode["devices"].Count];
                //Debug.Log("device count "+lJsonNode["devices"].Count);
                //for(int i=0; i< lJsonNode["devices"].Count; i++)
                //{
                //    lDevices.devices[i] = new IOTSomfyDevice();
                //    long.TryParse(lJsonNode["devices"][i]["creationTime"].Value, out lDevices.devices[i].creationTime);
                //    long.TryParse(lJsonNode["devices"][i]["lastUpdateTime"].Value, out lDevices.devices[i].lastUpdateTime);
                //    Debug.Log("1");
                //    lDevices.devices[i].label = lJsonNode["devices"][i]["label"].Value;
                //    Debug.Log("device: " + lJsonNode["devices"][i]["label"].Value);
                //    lDevices.devices[i].deviceURL= lJsonNode["devices"][i]["deviceURL"].Value;
                //    bool.TryParse(lJsonNode["devices"][i]["shortcut"].Value, out lDevices.devices[i].shortcut);
                //    lDevices.devices[i].uiClass = lJsonNode["devices"][i]["uiClass"].Value;
                //}



                //string lResponseText = "{\"devices\" :" + lResult.response.Text + "}";
                File.WriteAllText(Buddy.Resources.GetRawFullPath("response.txt"), response);
                //Debug.Log("{\"devices\" :" + lResult.response.Text + "}");
                if (lDevices != null)
                {
                    Debug.Log("nombre somfy devices: " + lDevices.devices.Length);
                    mDevices = lDevices.devices.ToList<IOTDevices>();
                    int j = 0;
                    for (int i = 0; i < mDevices.Count; ++i)
                    {
                        string iUiClass = ((IOTSomfyDevice)mDevices[i]).uiClass;
                        Debug.Log("ui class: " + iUiClass);
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
                            else if (iUiClass == "HeatingSystem")
                                mDevices[i] = new IOTSomfyThermostat(lDevices.devices[i + j], mSessionID);
                            else if (iUiClass == "TemperatureSensor")
                                mDevices[i] = new IOTSomfyThermometer(lDevices.devices[i + j], mSessionID);
                            else if (iUiClass == "MusicPlayer")
                                mDevices[i] = new IOTSomfySonos(lDevices.devices[i + j], mSessionID);
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
                foreach (KeyValuePair<string, string> post_arg in mHeaders)
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

        public static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }
    }
}
