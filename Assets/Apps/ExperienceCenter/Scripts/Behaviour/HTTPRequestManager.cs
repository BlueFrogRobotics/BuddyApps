using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using Buddy;

namespace BuddyApp.ExperienceCenter {
	public class HTTPRequestManager : MonoBehaviour	{

		public bool Connected { get; private set; }
		public bool RetrieveDevices { get; private set; }

		private string mCookie;

		private JSONBuilder jBuilder;

        private Dictionary<string, string> IOTLabels;

        public HTTPRequestManager()
        {
            jBuilder = new JSONBuilder();
            Connected = false;
            RetrieveDevices = false;
            mCookie = "";
            IOTLabels = new Dictionary<string, string>()
            {
                { "light", "Active button" },
                { "sound", "Awabureau" },
                { "store", "SUNEA io"}
            };
        }

		void Awake()
		{
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
            while(true)
            {
                if (ExperienceCenterData.Instance.ShouldTestIOT)
                {
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
            if (!Connected)
			{
				Login();
				yield return new WaitUntil(() => RetrieveDevices);
			}

            ExperienceCenterData data = ExperienceCenterData.Instance;

            if(data.LightState != data.IsLightOn)
			    LightOn(data.LightState);

            if (data.StoreState != data.IsStoreDeployed)
			    StoreDeploy(data.StoreState);

            if (data.SonosState != data.IsMusicOn)
			    SonosPlay(data.SonosState);
		}

        // Command light
		public void LightOn(bool enable)
		{
			ExecuteAction(IOTLabels["light"], enable ? "setPodLedOn" : "setPodLedOff");
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
            while(true)
            {
                if (!Connected)
                {
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
            // Active button
            // core:NameState -> Box
            // internal:LightingLedPodModeState -> 0/1

            System.Action<JSONArray, long> onLightStatus = delegate (JSONArray response, long responseCode)
            {
                Debug.LogFormat("LightStatus received response code '{0}' with response '{1}'", responseCode, response.ToString());

                if (responseCode == (long)HttpStatusCode.OK)
                {
                    for (int i = 0; i < response.Count; i++)
                    {
                        if (response[i]["name"].Value == "core:NameState")
                        {
                            //Unexpected value, abort
                            if (response[i]["value"].Value != "Box")
                            {
                                Debug.LogErrorFormat("Unexpected core:NameState value : {0}", response[i]["value"].Value);
                                break;
                            }
                        }
                        else if (response[i]["name"].Value == "internal:LightingLedPodModeState")
                            ExperienceCenterData.Instance.IsLightOn = (response[i]["value"].Value == "1.0");
                    }
                }
            };

            string apiEntry = String.Format("setup/devices/{0}/states",WWW.EscapeURL(jBuilder.GetDeviceURL(IOTLabels["light"])));
            Debug.LogFormat("Sending GET request on apiEntry : {0}", apiEntry);
            StartCoroutine(Get(apiEntry,onLightStatus));
        }

        private void StoreStatus()
        {
            // SUNEA io
            // core:NameState -> SUNEA io
            // core:OpenClosedState -> closed/open
            // core:DeploymentState -> 0-100
            System.Action<JSONArray, long> onStoreStatus = delegate (JSONArray response, long responseCode)
            {
                Debug.LogFormat("StoreStatus received response code '{0}' with response '{1}'", responseCode, response.ToString());
                if (responseCode == (long)HttpStatusCode.OK)
                {
                    for(int i=0; i< response.Count; i++)
                    {
                        if (response[i]["name"].Value == "core:NameState")
                        {
                            //Unexpected value, abort
                            if (response[i]["value"].Value != "SUNEA io")
                                break;
                        }
                        else if (response[i]["name"].Value == "core:OpenClosedState")
                            ExperienceCenterData.Instance.IsStoreDeployed = (response[i]["value"].Value == "open");
                    }
                }
            };

            string apiEntry = String.Format("setup/devices/{0}/states", WWW.EscapeURL(jBuilder.GetDeviceURL(IOTLabels["store"])));
            Debug.LogFormat("Sending GET request on apiEntry : {0}", apiEntry);
            StartCoroutine(Get(apiEntry, onStoreStatus));
        }

        private void SonosStatus()
        {
            Debug.LogError("Status request not implemented !");

            //System.Action<JSONArray, long> onSonosStatus = delegate (JSONArray response, long responseCode)
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
			WWWForm form = new WWWForm();
			form.AddField("userId", ExperienceCenterData.Instance.UserID);
			form.AddField("userPassword", ExperienceCenterData.Instance.Password);

			System.Action<JSONObject,long> onLogin = delegate (JSONObject response, long responseCode)
			{
					Debug.LogFormat("Login response code : {0}", responseCode);
					if (response["success"])
					{
						Debug.Log("Authentication success");
						Connected = true;
						Debug.Log("Retrieve list of devices");
						Devices();
					}
					else if (response["errorCode"])
					{
						Debug.LogFormat("Authentication failure : ({0}) {1}",
							response["errorCode"],response["error"]);
						Connected = false;
					}
			};

			StartCoroutine(Post("login",form,onLogin));			
		}

		public void Logout()
		{
			System.Action<JSONObject,long> onLogout = delegate (JSONObject response, long responseCode)
			{
					Debug.LogFormat("Logout response code : {0}", responseCode);
					if(response["logout"])
					{
						Debug.Log("Logout success");
						Connected = false;
						mCookie = "";
					}
					else
						Debug.Log("Unexpected logout error");
			};

			WWWForm form = new WWWForm();

			StartCoroutine(Post("logout", form, onLogout));
		}

		// Retrieve the list of configured devices on the targeted TaHoma box
		// Store associated device urls, needed to send commands to a specific device
		private void Devices()
		{
			System.Action<JSONArray,long> onDevices = delegate (JSONArray response, long responseCode)
			{
					Debug.LogFormat("Get Devices response code : {0}", responseCode);
					for(int i=0; i<response.Count; i++)
						jBuilder.AddDeviceURL(response[i]["label"],response[i]["deviceURL"]);

					RetrieveDevices = true;
			};

			StartCoroutine(Get("setup/devices", onDevices));
		}

		// Execute a specific action, described in json format
		private void ExecuteAction(string deviceName, string commandName)
		{
			System.Action<JSONObject,long> onExecute = delegate (JSONObject response, long responseCode)
			{
					Debug.LogFormat("Execute action response code : {0}", responseCode);
					//Expected answer on success
					if(response["execId"])
					{
						Debug.LogFormat("Send command '{0}' to device '{1}'", deviceName, commandName);
					}
					else if(response["error"]=="Not authenticated")
					{
						Debug.LogFormat("Last command '{0}' on device '{1}' failed : reconnecting due to connection loss...",
							deviceName,commandName);
						Connected = false;
					}
			};
			
			JSONObject json = jBuilder.CreateAction(deviceName, commandName, new List<string>());

			Debug.Log("Executing action group : " + json.ToString());
			StartCoroutine(Post("exec/apply",json,onExecute));
		}

		/********************************************************************************
		 * Base HTTP requests (POST, GET, PUT, DELETE)
		 * ******************************************************************************/

		// POST request using json data
		private IEnumerator Post(string apiEntry, JSONObject json, Action<JSONObject,long>onResponse)
		{
			UnityWebRequest request = new UnityWebRequest(ExperienceCenterData.Instance.API_URL + apiEntry, UnityWebRequest.kHttpVerbPOST);

			//Explicitly parse to UTF8 encoding to make it work !
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json.ToString());
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();

			// if logged in, put given cookie in header
			if (Connected)
			{
				request.SetRequestHeader("Cookie", mCookie);
				request.SetRequestHeader("Content-Type", "application/json");
			}

			yield return request.Send();

			if (request.isError)
				Debug.LogErrorFormat("Failed {0} request : {1}", apiEntry, request.error);
			else
			{
				JSONObject response = (JSONObject)JSON.Parse(request.downloadHandler.text);
				if(onResponse != null)
					onResponse(response,request.responseCode);
			}
		}

		// POST request using form data
		private IEnumerator Post(string apiEntry, WWWForm form, Action<JSONObject,long> onResponse)
		{
			UnityWebRequest request = UnityWebRequest.Post(ExperienceCenterData.Instance.API_URL + apiEntry, form);

            // if logged in, put given cookie in header
			if (Connected)
				request.SetRequestHeader("Cookie", mCookie);

			yield return request.Send();

			if (request.isError)
				Debug.LogErrorFormat("Failed {0} request : {1}", apiEntry, request.error);
			else
			{
				// On Login, retrieve authentication cookie
				if (!Connected)
				{
					Dictionary<string, string> responseHeaders = request.GetResponseHeaders();
					if (responseHeaders.ContainsKey("Set-Cookie"))
						mCookie = responseHeaders["Set-Cookie"];
				}

				JSONObject response = (JSONObject)JSON.Parse(request.downloadHandler.text);
				Debug.Log(response.ToString());
				if(onResponse != null)
					onResponse(response,request.responseCode);
			}
		}

		// GET request retrieving JSONArray
		private IEnumerator Get(string apiEntry, Action<JSONArray,long> onResponse)
		{
			UnityWebRequest request = UnityWebRequest.Get(ExperienceCenterData.Instance.API_URL + apiEntry);

            // if logged in, put given cookie in header
            if (Connected)
				request.SetRequestHeader("Cookie", mCookie);

			yield return request.Send();

			if (request.isError)
				Debug.LogErrorFormat("Failed {0} request : {1}", apiEntry, request.error);
			else
			{
				JSONArray response = (JSONArray)JSON.Parse(request.downloadHandler.text);
				if(onResponse != null)
					onResponse(response,request.responseCode);
			}
		}

		/**********************************************************************************
		 * TODO
		 * ********************************************************************************/
		private IEnumerator Put(string apiEntry, WWWForm form, Action<JSONObject> onResponse)
		{
			Debug.LogError("Put request not implemented !");
			yield return null;
		}

		private IEnumerator Put(string apiEntry, JSONObject json, Action<JSONObject> onResponse)
		{
			Debug.LogError("Put request not implemented !");
			yield return null;
		}

		private IEnumerator Delete(string apiEntry, WWWForm form, Action<JSONObject> onResponse)
		{
			Debug.LogError("Delete request not implemented !");
			yield return null;
		}

		private IEnumerator Delete(string apiEntry, JSONObject json, Action<JSONObject> onResponse)
		{
			Debug.LogError("Delete request not implemented !");
			yield return null;
		}
	}
}
