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

		public void AddDeviceURL(string name, string deviceURL)
		{
            Debug.Log("[JSON] add device " + name + " with url " + deviceURL);
			mURLDevices.Add(name,deviceURL);
		}

        public string GetDeviceURL(string key)
        {
            Debug.Log("[JSON] get device url: " + key);
            return mURLDevices[key];
        }

		public JSONObject CreateAction(string deviceName, string commandName, List<string> parameters)
		{
			JSONObject json = new JSONObject();
            Debug.Log("[JSON] create action with device name " + deviceName + " and command " + commandName);
			// Fill first fields with empty data
			json.Add("label", deviceName + "_" + commandName);
			json.Add("metadata", "");
			json.Add("shortcut", false);
			json.Add("notificationTypeMask", 1);
			json.Add("notificationCondition", "ALWAYS");
			json.Add("notificationText", commandName + parameters.ToString());
			json.Add("notificationTitle", deviceName);
			json.Add("targetEmailAddresses", new JSONArray());
			json.Add("targetPhoneNumbers", new JSONArray());
			json.Add("targetPushSubscriptions", new JSONArray());

			JSONArray actions = new JSONArray();
			JSONObject action = new JSONObject();
			JSONArray commands = new JSONArray();
			JSONObject command = new JSONObject();

			JSONArray jParameters = new JSONArray();
			foreach (string param in parameters)
				jParameters.Add(param);

			action.Add("deviceURL", mURLDevices[deviceName]);
			command.Add("type", 1);
			command.Add("name", commandName);
			command.Add("parameters", jParameters);
			commands.Add(command);
			action.Add("commands", commands);
			actions.Add(action);

			json.Add("actions", actions);

			return json;
		}
	}
}

