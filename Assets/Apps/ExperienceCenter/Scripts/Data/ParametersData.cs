using System;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Collections.Generic;

using UnityEngine;

using Buddy;

namespace BuddyApp.ExperienceCenter
{
	[DataContract]
	public class ParametersData
	{
		private static ParametersData sInstance;

		[DataMember(Name="apiUrl")]
		public string API_URL { get; set; }
		[DataMember(Name="userID")]
		public string UserID { get; set; }
		[DataMember(Name="password")]
		public string Password { get; set; }
		[DataMember(Name="deviceState")]
		public Dictionary<string, bool> DeviceState;
		[DataMember(Name="language")]
		public string Language { get; set; }

		public string Command { get; set; }
		public bool ShouldTestIOT { get; set; }

		/*
		 * Singleton access
		 */
		public static ParametersData Instance
		{
			get
			{
				if (sInstance == null)
					sInstance = LoadDefault();
				return sInstance;
			}
		}

		public ParametersData()
		{
			API_URL = "";
			UserID = "";
			Password = "";
			Command = "";

			DeviceState = new Dictionary<string,bool>();
		}

		public static void SaveDefault()
		{
			Debug.Log("saving ParametersData default...");
			string filename = BYOS.Instance.Resources.GetPathToRaw("parametersdata.xml");
			DataContractSerializer serializer = new DataContractSerializer(typeof(ParametersData));
			FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
			serializer.WriteObject(stream, sInstance);
			stream.Close();
		}

		public static ParametersData LoadDefault()
		{
			Debug.Log("loading ParametersData default...");
			string filename = BYOS.Instance.Resources.GetPathToRaw("parametersdata.xml");
			ParametersData newObject;

			if (File.Exists(filename))
			{
				FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
				XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
				DataContractSerializer ser = new DataContractSerializer(typeof(ParametersData));

				//Deserialize the dta and read it from the instance
				newObject = (ParametersData)ser.ReadObject(reader, true);
				reader.Close();
				stream.Close();
			}
			else
			{
				newObject = new ParametersData();
			}

			newObject.ShouldTestIOT = false;
			newObject.Command = "";

			return newObject;
		}
	}
}

