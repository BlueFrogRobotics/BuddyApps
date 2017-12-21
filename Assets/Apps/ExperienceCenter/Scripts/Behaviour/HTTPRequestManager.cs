﻿using System;
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

		public HTTPRequestManager()
		{
			jBuilder = new JSONBuilder();
			Connected = false;
			RetrieveDevices = false;
			mCookie = "";
		}

		void Awake()
		{
			InvokeRepeating("ShouldTestIOT", 1.0f, 1.0f);
		}

		private void ShouldTestIOT()
		{
			if (ExperienceCenterData.Instance.ShouldTestIOT)
			{
				ExperienceCenterData.Instance.ShouldTestIOT = false;
				StartCoroutine(TestDevices());
			}
		}

		private IEnumerator TestDevices()
		{
			if (!Connected)
			{
				Login();

				yield return new WaitUntil(() => RetrieveDevices);
			}

			LightOn(ExperienceCenterData.Instance.LightState);
			StoreDeploy(ExperienceCenterData.Instance.StoreState);
			SonosPlay(ExperienceCenterData.Instance.SonosState);
		}

		public void LightOn(bool enable)
		{
			ExecuteAction("Active button", enable ? "setPodLedOn" : "setPodLedOff");
		}

		public void StoreDeploy(bool enable)
		{
			ExecuteAction("SUNEA io", enable ? "deploy" : "my");
		}

		public void SonosPlay(bool enable)
		{
			ExecuteAction("Awabureau", enable ? "play" : "stop");
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
