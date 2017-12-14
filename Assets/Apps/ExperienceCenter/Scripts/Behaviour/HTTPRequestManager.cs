﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using Buddy;

namespace BuddyApp.ExperienceCenter {
	public class HTTPRequestManager : MonoBehaviour	{

		[SerializeField]
		private string mBaseURL;
		[SerializeField]
		private string mUserID;
		[SerializeField]
		private string mUserPassword;

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

		public void TestAPI()
		{
			StartCoroutine(Test());
		}

		private IEnumerator Test()
		{
			StoreDeploy();
			yield return new WaitForSeconds(5.0f);
			StoreUndeploy();
			yield return new WaitForSeconds(5.0f);

			SonosPlay();
			yield return new WaitForSeconds(20.0f);
			SonosStop();
			yield return new WaitForSeconds(2.0f);

			while (true)
			{
				LightOff();
				yield return new WaitForSeconds(5.0f);
				LightOn();
				yield return new WaitForSeconds(5.0f);
			}
		}

		public void LightOn()
		{
			ExecuteAction("Active button", "setPodLedOn");
		}

		public void LightOff()
		{
			ExecuteAction("Active button", "setPodLedOff");
		}

		public void StoreDeploy()
		{
			ExecuteAction("SUNEA io", "deploy");
		}

		public void StoreUndeploy()
		{
			ExecuteAction("SUNEA io", "undeploy");
		}

		public void SonosPlay()
		{
			ExecuteAction("Sonos PLAY:1", "play");
		}

		public void SonosStop()
		{
			ExecuteAction("Sonos PLAY:1", "stop");
		}

		//TODO Implement SONOS Commands

		/*****************************************************
		 *  API Base Requests
		 * ***************************************************/
		public void Login()
		{
			WWWForm form = new WWWForm();
			form.AddField("userId", mUserID);
			form.AddField("userPassword", mUserPassword);

			System.Action<JSONObject> onLogin = delegate (JSONObject response)
			{
					if (response["success"])
					{
						Debug.Log("Authentication success");
						Connected = true;
						Debug.Log("Retrieve list of devices");
						Devices();
					}
					else if (response["errorCode"])
					{
						string message = "Authentication failure : ({0}) {1}";
						Debug.Log(String.Format(message,response["errorCode"],response["error"]));
						Connected = false;
					}
			};

			StartCoroutine(Post("login",form,onLogin));			
		}

		public void Logout()
		{
			System.Action<JSONObject> onLogout = delegate (JSONObject response)
			{
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
			System.Action<JSONArray> onDevices = delegate (JSONArray response)
			{
					for(int i=0; i<response.Count; i++)
						jBuilder.AddDeviceURL(response[i]["label"],response[i]["deviceURL"]);

					RetrieveDevices = true;
			};

			StartCoroutine(Get("setup/devices", onDevices));
		}

		// Execute a specific action, described in json format
		private void ExecuteAction(string deviceName, string commandName)
		{
			System.Action<JSONObject> onExecute = delegate (JSONObject response)
			{
					string msg = "Send command '{0}' to device '{1}'";
					Debug.Log(String.Format(msg, deviceName, commandName));
			};
			
			JSONObject json = jBuilder.CreateAction(deviceName, commandName, new List<string>());

			Debug.Log("Executing action group : " + json.ToString());
			StartCoroutine(Post("exec/apply",json,onExecute));
		}

		/********************************************************************************
		 * Base HTTP requests (POST, GET, PUT, DELETE)
		 * ******************************************************************************/

		// POST request using json data
		private IEnumerator Post(string apiEntry, JSONObject json, Action<JSONObject>onResponse)
		{
			UnityWebRequest request = new UnityWebRequest(mBaseURL + apiEntry, UnityWebRequest.kHttpVerbPOST);

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
			{
				string msg = "Failed {0} request : {1}";
				Debug.LogError(String.Format(msg, apiEntry, request.error));
			}
			else
			{
				Debug.Log(request.downloadHandler.text);
				JSONObject response = (JSONObject)JSON.Parse(request.downloadHandler.text);
				if(onResponse != null)
					onResponse(response);
			}
		}

		// POST request using form data
		private IEnumerator Post(string apiEntry, WWWForm form, Action<JSONObject> onResponse)
		{
			UnityWebRequest request = UnityWebRequest.Post(mBaseURL + apiEntry, form);

			// if logged in, put given cookie in header
			if (Connected)
				request.SetRequestHeader("Cookie", mCookie);

			yield return request.Send();

			if (request.isError)
			{
				string msg = "Failed {0} request : {1}";
				Debug.LogError(String.Format(msg, apiEntry, request.error));
			}
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
					onResponse(response);
			}
		}

		// GET request retrieving JSONArray
		private IEnumerator Get(string apiEntry, Action<JSONArray> onResponse)
		{
			UnityWebRequest request = UnityWebRequest.Get(mBaseURL + apiEntry);

			// if logged in, put given cookie in header
			if (Connected)
				request.SetRequestHeader("Cookie", mCookie);

			yield return request.Send();

			if (request.isError)
			{
				string msg = "Failed {0} request : {1}";
				Debug.LogError(String.Format(msg, apiEntry, request.error));
			}
			else
			{
				JSONArray response = (JSONArray)JSON.Parse(request.downloadHandler.text);
				Debug.Log(response.ToString());
				if(onResponse != null)
					onResponse(response);
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