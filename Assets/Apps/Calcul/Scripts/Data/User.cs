using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using BlueQuark;
using System.Xml.Serialization;

namespace BuddyApp.Calcul{
    public class User{
        [XmlIgnore]
		private static User sInstance;

        public string Name { get; set; }
        public int Id { get; set; }

        public GameParameters GameParameters { get; set;}
        [XmlIgnore]
        public CertificateSummaryList Certificates{ get; set; }
        public ScoreSummaryList Scores { get; set;}

        [XmlIgnore]
        private const string USERFILE = "userdata.xml";

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
			this.Id = 0;
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
            string filename = Buddy.Resources.AppRawDataPath + USERFILE;
            Utils.SerializeXML<User>(User.Instance, filename);
            
        }

        public static User LoadDefaultUser()
        {
            string filename = Buddy.Resources.AppRawDataPath + USERFILE;
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
