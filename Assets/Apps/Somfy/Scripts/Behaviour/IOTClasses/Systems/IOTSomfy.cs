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

using Newtonsoft.Json.Linq;
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
            mSessionID = "";
        }


        //public override void Connect()
        //{
        //    Login();

        //    //PlayerPrefs.SetString("somfy_user", mCredentials[1]);
        //    //PlayerPrefs.SetString("somfy_password", mCredentials[2]);
        //    //PlayerPrefs.Save();
        //}

        /// <summary>
        /// Send login request
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Login()
        {
            Debug.Log("Somfy login: "+ SomfyData.Instance.Login);
            Credentials[1] = SomfyData.Instance.Login;
            Credentials[2] = SomfyData.Instance.Password;

            WWWForm form = new WWWForm();
            form.AddField("userId", Credentials[1]);
            form.AddField("userPassword", Credentials[2]);

            using (UnityWebRequest www = UnityWebRequest.Post(SomfyData.Instance.URL_API + "/login", form))
            { 
                yield return www.SendWebRequest();

                if (www.isHttpError || www.isNetworkError)
                {
                    mAvailable = false;
                    Debug.Log("Login error " + www.error + " " + www.downloadHandler.text);
                }
                else
                {
                    mAvailable = true;
                    Debug.Log("Dowload handler " + www.downloadHandler.text);
                    mHeaders.Clear();
                    mHeaders["SET-COOKIE"] = www.GetResponseHeader("SET-COOKIE");
                    Debug.Log("header received: " + mHeaders["SET-COOKIE"]);
                    if (!UpdateSessionId())
                    {
                        Debug.Log("Error setting session ID");
                    }
                }
            }

            //Upload(Credentials[1], Credentials[2]);
            

        }

        //private IEnumerator GetDelayedDevices()
        //{
        //    yield return new WaitForSeconds(2);
        //    GetDevices();
        //    yield return new WaitForSeconds(2);
        //    yield return Upload(Credentials[1], Credentials[2]);
        //}




        //IEnumerator Upload(string id, string password)
        //{
        //    //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //    WWWForm form = new WWWForm();
        //    form.AddField("userId", id);
        //    form.AddField("userPassword", password);
        //    //formData.Add(new MultipartFormDataSection("userId="+id+"&userPassword="+password));

        //    UnityWebRequest www = UnityWebRequest.Post(SomfyData.Instance.URL_API + "/login", form);
        //    //yield return www.Send();
        //    yield return www.SendWebRequest();
        //    if (www.isNetworkError)
        //    {
        //        Debug.Log(www.error);
        //    }
        //    else
        //    {
        //        Debug.Log("Form upload complete!" + www.downloadHandler.text);
        //    }
        //}

        /// <summary>
        /// Gets all connected devices.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetTheDevices()
        {
            string url = SomfyData.Instance.URL_API + "/setup/devices";

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            { 
                // Cookie header already set on runtime
                if (Application.platform == RuntimePlatform.WindowsEditor)
                    www.SetRequestHeader("Cookie", "JSESSIONID=" + System.Uri.EscapeDataString(mSessionID));

                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Debug.Log("Get devices error " + www.error + " " + www.downloadHandler.text);
                }
                else
                {
                    Debug.Log("Get devices upload complete!" + www.downloadHandler.text);
                    string response = "{\"devices\" :" + www.downloadHandler.text + "}";
                    IOTSomfyDeviceCollection lDevices = new IOTSomfyDeviceCollection(response);
                    //Debug.Log("avant ecriture reponse");
                    //File.WriteAllText(Buddy.Resources.GetRawFullPath("response.txt"), response);
                    //Debug.Log("apres ecriture reponse");
                    //Debug.Log("{\"devices\" :" + lResult.response.Text + "}");
                    if (lDevices != null)
                    {
                        Debug.Log("Number somfy devices: " + lDevices.devices.Length);
                        mDevices = lDevices.devices.ToList<IOTDevices>();
                        //Debug.Log("apres init mdevice");
                        int j = 0;
                        for (int i = 0; i < mDevices.Count; ++i)
                        {
                            string iUiClass = ((IOTSomfyDevice)mDevices[i]).uiClass;
                            Debug.Log("Device " + i + " UI class: " + iUiClass);
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
                    //Upload(Credentials[1], Credentials[2]);
                }
            }
        }

        /// <summary>
        /// Gets all scenarios.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetScenarios()
        {
            string url = SomfyData.Instance.URL_API + "/actionGroups";

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                // Cookie header already set on runtime
                if (Application.platform == RuntimePlatform.WindowsEditor)
                    www.SetRequestHeader("Cookie", "JSESSIONID=" + System.Uri.EscapeDataString(mSessionID));

                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Debug.Log("Get scenarios error " + www.error + " " + www.downloadHandler.text);
                }
                else
                {
                    Debug.Log("Get scenarios upload complete!" + www.downloadHandler.text);
                    string response = "{\"scenarios\" :" + www.downloadHandler.text + "}";
                    JObject lJsonNode = Utils.UnserializeJSONtoObject(response);
                    JArray lJArray = (JArray)lJsonNode["scenarios"];
                    //Debug.Log("device count " + lJArray.Count);
                    for (int i = 0; i < lJArray.Count; i++)
                    {
                        string label = lJArray[i].Value<string>("label");
                        string oid = lJArray[i].Value<string>("oid");
                        if (!string.IsNullOrEmpty(label)
                            && !string.IsNullOrEmpty(oid))
                        {
                            mScenarios.Add(label.ToLower(), oid);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Launch a scenario.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator LaunchScenario(string oid)
        {
            string url = SomfyData.Instance.URL_API + "/exec/" + oid;
            WWWForm form = new WWWForm();
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                // Cookie header already set on runtime
                if (Application.platform == RuntimePlatform.WindowsEditor)
                    www.SetRequestHeader("Cookie", "JSESSIONID=" + System.Uri.EscapeDataString(mSessionID));

                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Debug.Log("Launch Scenario error " + www.error + " " + www.downloadHandler.text);
                }
                else
                {
                    Debug.Log("Launch Scenario complete!" + www.downloadHandler.text);
                }
            }
        }


        //public override void GetDevices()
        //{
        //    string url = SomfyData.Instance.URL_API + "/setup/devices";
        //    Debug.Log("url api: " + url);
        //    Request lRequest = new Request("GET", url);
        //    lRequest.cookieJar = null;
        //    lRequest.SetHeader("Cookie", "JSESSIONID="+ System.Uri.EscapeDataString(mSessionID));

        //    //UnityWebRequest www = UnityWebRequest.Get(url);
        //    //www.SetRequestHeader("Cookie", "JSESSIONID=" + System.Uri.EscapeDataString(mSessionID));
        //    //www.SendWebRequest();
        //    //Debug.Log("get device upload complete!" + www.downloadHandler.text);
        //    lRequest.Send((lResult) =>
        //    {
        //        if (lResult == null || lResult.response == null)
        //        {
        //            Debug.LogError("Somfy not connected");
        //            return;
        //        }
        //        Debug.Log("avant response");
        //        Debug.Log("identifiants magique: " + SomfyData.Instance.Login + " " + SomfyData.Instance.Password);

        //        string response = "{\"devices\" :" + lResult.response.Text + "}";
        //        Debug.Log("reponse: " + response);
        //        //JSONNode lJsonNode = Buddy.JSON.Parse(response);
        //        //Debug.Log("response: " + response);
        //        //IOTSomfyDeviceCollection lDevices = JsonUtility.FromJson<IOTSomfyDeviceCollection>(response.Trim());
        //        IOTSomfyDeviceCollection lDevices = new IOTSomfyDeviceCollection(response);
        //        //lDevices.devices = new IOTSomfyDevice[lJsonNode["devices"].Count];
        //        //Debug.Log("device count "+lJsonNode["devices"].Count);
        //        //for(int i=0; i< lJsonNode["devices"].Count; i++)
        //        //{
        //        //    lDevices.devices[i] = new IOTSomfyDevice();
        //        //    long.TryParse(lJsonNode["devices"][i]["creationTime"].Value, out lDevices.devices[i].creationTime);
        //        //    long.TryParse(lJsonNode["devices"][i]["lastUpdateTime"].Value, out lDevices.devices[i].lastUpdateTime);
        //        //    Debug.Log("1");
        //        //    lDevices.devices[i].label = lJsonNode["devices"][i]["label"].Value;
        //        //    Debug.Log("device: " + lJsonNode["devices"][i]["label"].Value);
        //        //    lDevices.devices[i].deviceURL= lJsonNode["devices"][i]["deviceURL"].Value;
        //        //    bool.TryParse(lJsonNode["devices"][i]["shortcut"].Value, out lDevices.devices[i].shortcut);
        //        //    lDevices.devices[i].uiClass = lJsonNode["devices"][i]["uiClass"].Value;
        //        //}



        //        //string lResponseText = "{\"devices\" :" + lResult.response.Text + "}";
        //        Debug.Log("avant ecriture reponse");
        //        File.WriteAllText(Buddy.Resources.GetRawFullPath("response.txt"), response);
        //        Debug.Log("apres ecriture reponse");
        //        //Debug.Log("{\"devices\" :" + lResult.response.Text + "}");
        //        if (lDevices != null)
        //        {
        //            Debug.Log("nombre somfy devices: " + lDevices.devices.Length);
        //            mDevices = lDevices.devices.ToList<IOTDevices>();
        //            Debug.Log("apres init mdevice");
        //            int j = 0;
        //            for (int i = 0; i < mDevices.Count; ++i)
        //            {
        //                string iUiClass = ((IOTSomfyDevice)mDevices[i]).uiClass;
        //                Debug.Log("ui class: " + iUiClass);
        //                if (iUiClass == "Pod")
        //                {
        //                    mDevices.RemoveAt(i);
        //                    i--;
        //                    j++;
        //                }
        //                else
        //                {
        //                    mDevices[i].Credentials[1] = mCredentials[1];
        //                    mDevices[i].Credentials[2] = mCredentials[2];
        //                    if (iUiClass == "OnOff")
        //                        mDevices[i] = new IOTSomfySwitch(lDevices.devices[i + j], mSessionID);
        //                    else if (iUiClass == "Screen")
        //                        mDevices[i] = new IOTSomfyStore(lDevices.devices[i + j], mSessionID);
        //                    else if (iUiClass == "HeatingSystem")
        //                        mDevices[i] = new IOTSomfyThermostat(lDevices.devices[i + j], mSessionID);
        //                    else if (iUiClass == "TemperatureSensor")
        //                        mDevices[i] = new IOTSomfyThermometer(lDevices.devices[i + j], mSessionID);
        //                    else if (iUiClass == "MusicPlayer")
        //                        mDevices[i] = new IOTSomfySonos(lDevices.devices[i + j], mSessionID);
        //                }
        //            }
        //        }
        //    });

        //    Upload(Credentials[1], Credentials[2]);
        //}

        /// <summary>
        /// Updates the session identifier from header.
        /// </summary>
        /// <returns>True if session Id has been found</returns>
        private bool UpdateSessionId()
        {
            string res = null;
            string[] data = null;
            if (mHeaders != null)
            {
                foreach (KeyValuePair<string, string> post_arg in mHeaders)
                {
                    if (post_arg.Key.Equals("SET-COOKIE"))
                    {
                        data = ((string)post_arg.Value).Split(';');
                        if (data.Length > 0)
                        {
                            res = data[0].Replace("JSESSIONID=", "");
                            mSessionID = res;
                            Debug.Log("SessionId: " + res);
                            return true;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Headers is null");
            }
            return false;
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
