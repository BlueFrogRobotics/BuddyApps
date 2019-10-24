using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BuddyApp.Calcul{
    public class CertificateSummaryList {

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

