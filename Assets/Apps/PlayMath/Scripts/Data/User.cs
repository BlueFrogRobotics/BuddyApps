using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using Buddy;

namespace BuddyApp.PlayMath{
    public class User {

		private static User sInstance;

        public string Name { get; private set; }
        private int id;
        public GameParameters GameParameters { get; private set;}
        public CertificateSummaryList Certificates{ get; private set; }
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
            string filename = BYOS.Instance.Resources.GetPathToRaw("userdata.xml");
            FileStream stream = new FileStream(filename, FileMode.Create,FileAccess.Write);
            stream.Close();
        }

        public static User LoadDefaultUser()
        {
            string filename = BYOS.Instance.Resources.GetPathToRaw("userdata.xml");
            User newObject;
            if (File.Exists(filename))
            {
                FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                // Deserialize the data and read it from the instance.
                stream.Close();
				newObject = new User();
            }
            else
            {
                newObject = new User();
            }
            return newObject;
        }
	}
}
