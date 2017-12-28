using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using Buddy;
using System.Xml.Serialization;

namespace BuddyApp.PlayMath{
    public class User {
		private static User sInstance;

        [XmlAttribute("name")]
        public string Name { get; private set; }
        [XmlAttribute("id")]
        private int id;
        [XmlElement("game_parameters")]
        public GameParameters GameParameters { get; private set;}
        [XmlElement("certificates")]
        public CertificateSummaryList Certificates{ get; private set; }
        [XmlElement("scores")]
        public ScoreSummaryList Scores { get; private set;}

        public const string USERFILE = "userdata.xml";

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
            this.Certificates = new CertificateSummaryList();
		}

        public void ResetScores(){
            Scores = new ScoreSummaryList();
            Certificates = new CertificateSummaryList();
        }

        public bool HasCurrentCertificate()
        {
            Certificate certif = new Certificate();
            certif.GameParams = this.GameParameters;
            CertificateSummary summary = new CertificateSummary(certif, false);

            return Certificates.Summaries.Contains(summary);
        }

        public bool HasTableCertificate(int table)
        {
            Certificate certif = new Certificate();
            certif.GameParams = this.GameParameters;
            certif.GameParams.Table = table;
            CertificateSummary summary = new CertificateSummary(certif, false);

            return Certificates.Summaries.Contains(summary);
        }

        public static void SaveUser()
        {
            string filename = BYOS.Instance.Resources.GetPathToRaw(USERFILE);
            Utils.SerializeXML(User.Instance, filename);
        }

        public static User LoadDefaultUser()
        {
            string filename = BYOS.Instance.Resources.GetPathToRaw(USERFILE);
            User newObject;
            if (File.Exists(filename))
            {
				newObject = Utils.UnserializeXML<User>(filename);
            }
            else
            {
                newObject = new User();
            }
            return newObject;
        }
	}
}
