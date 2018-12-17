using System;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.ExperienceCenter {
	public class JSONBuilder
	{
		private Dictionary<string,string> mURLDevices;

		public JSONBuilder()
		{
            Debug.Log("jsonbuilder");
			mURLDevices = new Dictionary<string,string>();
		}

		public void AddDeviceURL(string iName, string iDeviceURL)
		{
            Debug.Log("[JSON] add device " + iName + " with url " + iDeviceURL);
            //if(!string.IsNullOrEmpty(iName) && !string.IsNullOrEmpty(iDeviceURL))
			    mURLDevices.Add(iName,iDeviceURL);
		}

        public string GetDeviceURL(string iKey)
        {
            Debug.Log("[JSON] get device url: " + iKey);
            //if (mURLDevices.ContainsKey(iKey))
                return mURLDevices[iKey];
            //else
            //    return null;
        }

		public JSONObject CreateAction(string iDeviceName, string iCommandName, List<string> iParameters)
		{
			JSONObject lJson = new JSONObject();
            Debug.Log("[JSON] create action with device name " + iDeviceName + " and command " + iCommandName);
			// Fill first fields with empty data
			lJson.Add("label", iDeviceName + "_" + iCommandName);
			lJson.Add("metadata", "");
			lJson.Add("shortcut", false);
			lJson.Add("notificationTypeMask", 1);
			lJson.Add("notificationCondition", "ALWAYS");
			lJson.Add("notificationText", iCommandName + iParameters.ToString());
			lJson.Add("notificationTitle", iDeviceName);
			lJson.Add("targetEmailAddresses", new JSONArray());
			lJson.Add("targetPhoneNumbers", new JSONArray());
			lJson.Add("targetPushSubscriptions", new JSONArray());

			JSONArray lActions = new JSONArray();
			JSONObject lAction = new JSONObject();
			JSONArray lCommands = new JSONArray();
			JSONObject lCommand = new JSONObject();

			JSONArray lJsonParameters = new JSONArray();
			foreach (string param in iParameters)
				lJsonParameters.Add(param);

			lAction.Add("deviceURL", mURLDevices[iDeviceName]);
			lCommand.Add("type", 1);
			lCommand.Add("name", iCommandName);
			lCommand.Add("parameters", lJsonParameters);
			lCommands.Add(lCommand);
			lAction.Add("commands", lCommands);
			lActions.Add(lAction);

			lJson.Add("actions", lActions);

			return lJson;
		}
	}
}

