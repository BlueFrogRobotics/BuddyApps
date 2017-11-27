using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

using Buddy;

namespace BuddyApp.PlayMath{
    [DataContract]
    public class User : SerializableData {

		private static User sInstance;

        [DataMember(Name="name")]
        public string Name { get; private set; }
        [DataMember(Name="id")]
        private int id;
        [DataMember(Name="gameparameters")]
        public GameParameters GameParameters { get; private set;}

		// private DegreeList mDegrees TODO
        [DataMember(Name="scoresummarylist")]
        public ScoreSummaryList Scores { get; private set;}
			
		/*
         * Singleton access
         */
		public static User Instance
		{
			get
			{
				if (sInstance == null)
					sInstance = LoadDefaultUser();
				return sInstance;
			}
		}

		public User() {
			this.Name = "buddy";
			this.id = 0;
			this.GameParameters = new GameParameters();
			this.Scores = new ScoreSummaryList();
		}

        public void ResetScores(){
            Scores = new ScoreSummaryList();
        }

        public static void SaveUser()
        {
            string filename = BYOS.Instance.Resources.GetPathToRaw("userdata.xml");
            Debug.Log("Serializing user data to xml file...");
            DataContractSerializer serializer = new DataContractSerializer(typeof(User));
            FileStream stream = new FileStream(filename, FileMode.Create,FileAccess.Write);
            serializer.WriteObject(stream, sInstance);
            stream.Close();
        }

        public static User LoadDefaultUser()
        {
            Debug.Log("Unserializing user data from xml file...");
            string filename = BYOS.Instance.Resources.GetPathToRaw("userdata.xml");
            User newObject;
            if (File.Exists(filename))
            {
                FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
                DataContractSerializer ser = new DataContractSerializer(typeof(User));

                // Deserialize the data and read it from the instance.
                newObject = (User)ser.ReadObject(reader, true);
                reader.Close();
                stream.Close();
            }
            else
            {
                newObject = new User();
            }
            return newObject;
        }
	}
}
