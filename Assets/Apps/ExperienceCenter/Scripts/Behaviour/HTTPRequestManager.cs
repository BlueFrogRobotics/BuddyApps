using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using BlueQuark;
using Newtonsoft.Json.Linq;

namespace BuddyApp.ExperienceCenter
{
    public class HTTPRequestManager : MonoBehaviour
    {

        public bool Connected { get; private set; }
        public bool RetrieveDevices { get; private set; }

        private string mCookie;

        private JSONBuilder jBuilder;

        private Dictionary<string, string> IOTLabels;

        public HTTPRequestManager()
        {

        }

        void Awake()
        {
            Debug.Log("awake http request");
            jBuilder = new JSONBuilder();
            Connected = false;
            RetrieveDevices = false;
            mCookie = "";
            IOTLabels = new Dictionary<string, string>() {
                { "light", "Led Outdoor" },
                { "sound", "Living room" },
                { "store", "Awning" }
            };

            ExperienceCenterData.Instance.ShouldTestIOT = false;
            StartCoroutine(ShouldTestIOT());
            StartCoroutine(RetrieveIOTStates());
        }

        /**********************************************************
         * Individually enable/disable each device
         * ********************************************************/

        // Fire device state control only if set in parameters layout
        private IEnumerator ShouldTestIOT()
        {
            while (true) {
                if (ExperienceCenterData.Instance.ShouldTestIOT) {
                    ExperienceCenterData.Instance.ShouldTestIOT = false;
                    StartCoroutine(EnableDevices());
                }
                yield return new WaitForSeconds(1.0f);
            }
        }

        // Enable/Disable each device
        private IEnumerator EnableDevices()
        {
            // Connect if not already connected
            if (!Connected) {
                Login();
                yield return new WaitUntil(() => RetrieveDevices);
            }

            ExperienceCenterData data = ExperienceCenterData.Instance;

            if (data.LightState != data.IsLightOn)
                LightOn(data.LightState);

            if (data.StoreState != data.IsStoreDeployed)
                StoreDeploy(data.StoreState);

            if (data.SonosState != data.IsMusicOn)
                SonosPlay(data.SonosState);
        }

        // Command light
        public void LightOn(bool enable)
        {
            ExecuteAction(IOTLabels["light"], enable ? "on" : "off");
        }

        // Command store
        public void StoreDeploy(bool enable)
        {
            ExecuteAction(IOTLabels["store"], enable ? "deploy" : "undeploy");
        }

        // Command sonos
        public void SonosPlay(bool enable)
        {
            ExecuteAction(IOTLabels["sound"], enable ? "play" : "stop");

            // As there are no way to check Sonos state through the Rest API
            ExperienceCenterData.Instance.IsMusicOn = enable;
        }

        /**********************************************************
         * Individually retrieve each device state
         * ********************************************************/

        // Retrieve each device state
        private IEnumerator RetrieveIOTStates()
        {
            while (true) {
                if (!Connected) {
                    Login();
                    yield return new WaitUntil(() => RetrieveDevices);
                }

                LightStatus();
                StoreStatus();
                //SonosStatus(); - NOT IMPLEMENTED
                yield return new WaitForSeconds(5.0f);
            }
        }

        private void LightStatus()
        {
            System.Action<JArray, long> onLightStatus = delegate (JArray response, long responseCode) {
                Debug.LogFormat("[EXCENTER] LightStatus received response code '{0}' with response '{1}'", responseCode, response.ToString());

                if (responseCode == (long)HttpStatusCode.OK) {
                    for (int i = 0; i < response.Count; i++) {
                        if ((string)response[i]["name"] == "core:NameState") {
                            //Unexpected value, abort
                            if ((string)response[i]["value"] != "Led Outdoor") {
                                Debug.LogErrorFormat("[EXCENTER] Unexpected core:NameState value : {0}", (string)response[i]["value"]);
                                break;
                            }
                        } else if ((string)response[i]["name"] == "core:OnOffState") {
                            ExperienceCenterData.Instance.IsLightOn = (string)response[i]["value"] == "on";
                        }
                    }
                }
            };


            //if (jBuilder.GetDeviceURL(IOTLabels["light"]) != null) {
            string apiEntry = String.Format("setup/devices/{0}/states", WWW.EscapeURL(jBuilder.GetDeviceURL(IOTLabels["light"])));
            Debug.LogFormat("[EXCENTER] Sending GET request on apiEntry : {0}", apiEntry);
            StartCoroutine(Get(apiEntry, onLightStatus));
            //} else
            //    Debug.Log("[EXCENTER] light url is null");
        }

        private void StoreStatus()
        {
            System.Action<JArray, long> onStoreStatus = delegate (JArray response, long responseCode) {
                Debug.LogFormat("[EXCENTER] StoreStatus received response code '{0}' with response '{1}'", responseCode, response.ToString());
                if (responseCode == (long)HttpStatusCode.OK) {
                    for (int i = 0; i < response.Count; i++) {
                        if ((string)response[i]["name"] == "core:NameState") {
                            //Unexpected value, abort
                            if ((string)response[i]["value"] != "Awning")
                                break;
                        } else if ((string)response[i]["name"] == "core:OpenClosedState")
                            ExperienceCenterData.Instance.IsStoreDeployed = ((string) response[i]["value"] == "open");
                    }
                }
            };

            //if (jBuilder.GetDeviceURL(IOTLabels["store"]) != null) {
            string apiEntry = String.Format("setup/devices/{0}/states", WWW.EscapeURL(jBuilder.GetDeviceURL(IOTLabels["store"])));
            Debug.LogFormat("[EXCENTER] Sending GET request on apiEntry : {0}", apiEntry);
            StartCoroutine(Get(apiEntry, onStoreStatus));
            //} else
            //    Debug.Log("[EXCENTER] store url is null");
        }

        private void SonosStatus()
        {
            Debug.LogError("[EXCENTER] Status request not implemented !");

            //System.Action<JArray, long> onSonosStatus = delegate (JArray response, long responseCode)
            //{
            //    // No response from device... ?
            //};

            //string apiEntry = String.Format("setup/devices/{0}/states", jBuilder.GetDeviceURL(IOTLabels["sonos"]));
            //StartCoroutine(Get(apiEntry, onSonosStatus));
        }

        /*****************************************************
		 *  API Base Requests
		 * ***************************************************/
        public void Login()
        {
            Debug.Log("Login method");
            WWWForm form = new WWWForm();
            form.AddField("userId", ExperienceCenterData.Instance.UserID);
            form.AddField("userPassword", ExperienceCenterData.Instance.Password);
            Debug.Log("userid: " + ExperienceCenterData.Instance.UserID + " psswd: " + ExperienceCenterData.Instance.Password);
            System.Action<JObject, long> onLogin = delegate (JObject response, long responseCode) {
                Debug.LogFormat("[EXCENTER] Login response code : {0}", responseCode);
                if (response["success"] != null) {
                    Debug.Log("[EXCENTER] Authentication success");
                    Connected = true;
                    Debug.Log("[EXCENTER] Retrieve list of devices");
                    Devices();
                } else if (response["errorCode"] != null) {
                    Debug.LogFormat("[EXCENTER] Authentication failure : ({0}) {1}",
                        response["errorCode"], response["error"]);
                    Connected = false;
                }
            };

            StartCoroutine(Post("login", form, onLogin));
        }

        public void Logout()
        {
            System.Action<JObject, long> onLogout = delegate (JObject response, long responseCode) {
                Debug.LogFormat("[EXCENTER] Logout response code : {0}", responseCode);
                if (response["logout"] != null && (bool)response["logout"]) {
                    Debug.Log("[EXCENTER] Logout success");
                    Connected = false;
                    mCookie = "";
                } else
                    Debug.Log("[EXCENTER] Unexpected logout error");
            };

            WWWForm form = new WWWForm();

            StartCoroutine(Post("logout", form, onLogout));
        }

        // Retrieve the list of configured devices on the targeted TaHoma box
        // Store associated device urls, needed to send commands to a specific device
        private void Devices()
        {
            System.Action<JArray, long> onDevices = delegate (JArray response, long responseCode) {
                Debug.LogFormat("[EXCENTER] Get Devices response code : {0}", responseCode);
                for (int i = 0; i < response.Count; i++) {
                    Debug.Log("[DEVICES] label: " + response[i]["label"]);
                    jBuilder.AddDeviceURL((string)response[i]["label"], (string)response[i]["deviceURL"]);
                    Debug.Log("ajout de l url: " + response[i]["deviceURL"] + " fait");
                }

                RetrieveDevices = true;
            };

            StartCoroutine(Get("setup/devices", onDevices));
        }

        // Execute a specific action, described in json format
        private void ExecuteAction(string deviceName, string commandName)
        {
            System.Action<JObject, long> onExecute = delegate (JObject response, long responseCode) {
                Debug.LogFormat("[EXCENTER] Execute action response code : {0}", responseCode);
                //Expected answer on success
                if (response["execId"] != null && (bool)response["execId"]) {
                    Debug.LogFormat("[EXCENTER] Send command '{0}' to device '{1}'", deviceName, commandName);
                } else if ((string)response["error"] == "Not authenticated") {
                    Debug.LogFormat("[EXCENTER] Last command '{0}' on device '{1}' failed : reconnecting due to connection loss...",
                        deviceName, commandName);
                    Connected = false;
                }
            };

            JObject json = jBuilder.CreateAction(deviceName, commandName, new List<string>());

            Debug.Log("[EXCENTER] Executing action group : " + json.ToString());
            StartCoroutine(Post("exec/apply", json, onExecute));
        }

        /********************************************************************************
		 * Base HTTP requests (POST, GET, PUT, DELETE)
		 * ******************************************************************************/

        // POST request using json data
        private IEnumerator Post(string apiEntry, JObject json, Action<JObject, long> onResponse)
        {
            Debug.Log("Post1 method with: " + apiEntry);
            UnityWebRequest request = new UnityWebRequest(ExperienceCenterData.Instance.API_URL + apiEntry, UnityWebRequest.kHttpVerbPOST);

            //Explicitly parse to UTF8 encoding to make it work !
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json.ToString());
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            // if logged in, put given cookie in header
            if (Connected) {
                request.SetRequestHeader("Cookie", mCookie);
                request.SetRequestHeader("Content-Type", "application/json");
            }

            yield return request.SendWebRequest();

            if (request.isNetworkError)
                Debug.LogErrorFormat("[EXCENTER] Failed {0} request : {1}", apiEntry, request.error);
            else {
                JObject response = Utils.UnserializeJSONtoObject(request.downloadHandler.text);
                if (onResponse != null)
                    onResponse(response, request.responseCode);
            }
        }

        // POST request using form data
        private IEnumerator Post(string apiEntry, WWWForm form, Action<JObject, long> onResponse)
        {
            Debug.Log("Post2 method with: " + apiEntry);
            UnityWebRequest request = UnityWebRequest.Post(ExperienceCenterData.Instance.API_URL + apiEntry, form);

            // if logged in, put given cookie in header
            if (Connected)
                request.SetRequestHeader("Cookie", mCookie);

            yield return request.SendWebRequest();

            if (request.isNetworkError)
                Debug.LogErrorFormat("Failed {0} request : {1}", apiEntry, request.error);
            else {
                // On Login, retrieve authentication cookie
                if (!Connected) {
                    Dictionary<string, string> responseHeaders = request.GetResponseHeaders();
                    if (responseHeaders.ContainsKey("Set-Cookie"))
                        mCookie = responseHeaders["Set-Cookie"];
                }

                JObject response = Utils.UnserializeJSONtoObject(request.downloadHandler.text);
                Debug.Log(response.ToString());
                if (onResponse != null)
                    onResponse(response, request.responseCode);
            }
        }

        // GET request retrieving JArray
        private IEnumerator Get(string apiEntry, Action<JArray, long> onResponse)
        {
            Debug.Log("Get method with: " + apiEntry);
            UnityWebRequest request = UnityWebRequest.Get(ExperienceCenterData.Instance.API_URL + apiEntry);

            // if logged in, put given cookie in header
            if (Connected)
                request.SetRequestHeader("Cookie", mCookie);

            yield return request.SendWebRequest();

            if (request.isNetworkError)
                Debug.LogErrorFormat("[EXCENTER] Failed {0} request : {1}", apiEntry, request.error);
            else {
                Debug.Log("avant cast");
                JArray response = Utils.UnserializeJSONtoArray(request.downloadHandler.text);
                //response.Add(JSON.Parse(request.downloadHandler.text));
                Debug.Log("apres cast");
                if (onResponse != null)
                    onResponse(response, request.responseCode);
            }
        }

        /**********************************************************************************
		 * TODO
		 * ********************************************************************************/
        private IEnumerator Put(string apiEntry, WWWForm form, Action<JObject> onResponse)
        {
            Debug.LogError("[EXCENTER] Put request not implemented !");
            yield return null;
        }

        private IEnumerator Put(string apiEntry, JObject json, Action<JObject> onResponse)
        {
            Debug.LogError("[EXCENTER] Put request not implemented !");
            yield return null;
        }

        private IEnumerator Delete(string apiEntry, WWWForm form, Action<JObject> onResponse)
        {
            Debug.LogError("[EXCENTER] Delete request not implemented !");
            yield return null;
        }

        private IEnumerator Delete(string apiEntry, JObject json, Action<JObject> onResponse)
        {
            Debug.LogError("[EXCENTER] Delete request not implemented !");
            yield return null;
        }
    }
}
