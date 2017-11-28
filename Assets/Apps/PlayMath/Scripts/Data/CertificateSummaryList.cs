using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections.Generic;

namespace BuddyApp.PlayMath{
    [DataContract]
    public class CertificateSummaryList : SerializableData {

        [DataMember(Name="certificates")]
        public List<CertificateSummary> Summaries { get; set; }

        public CertificateSummaryList()
        {
            Summaries = new List<CertificateSummary>();
        }

        public void NewCertificate(Certificate certif)
        {
            CertificateSummary lCertifSummary = new CertificateSummary(certif);
            if (!Summaries.Contains(lCertifSummary))
                Summaries.Add(lCertifSummary);
        }
    }
}

