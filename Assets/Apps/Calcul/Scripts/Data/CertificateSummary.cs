using System;

using UnityEngine;
using BlueQuark;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace BuddyApp.Calcul {
    public class CertificateSummary {

        public int Difficulty { get; set; }
        public Operand Operands { get; set; }
        public int Table { get; set; }
        public string PicturePath { get; set; }

        [XmlIgnore]
        public DateTime TimeStamp { get; set; }

        [XmlElement("TimeStamp")]
        public string CertificateDateTime
        {
            get { return this.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { this.TimeStamp = DateTime.Parse(value); }
        }

        [XmlIgnore]
        public Texture2D Picture { get; private set; }

        public CertificateSummary(Certificate certif, bool savePic=true)
        {
            this.Table = certif.GameParams.Table;
            this.Difficulty = this.Table>0 ? 0 : certif.GameParams.Difficulty;
            this.Operands = this.Table > 0 ? Operand.MULTI : certif.GameParams.Operands;
            this.TimeStamp = certif.TimeStamp;
            this.Picture = certif.UserPic;

            // Export picture to JPG
            if (savePic)
            {
                string levelName;
                if (this.Table > 0)
                    levelName = String.Format("table{0}", this.Table);
                else
                    levelName = String.Format("{0}lvl{1}", this.Operands.ToString(), this.Difficulty);

                PicturePath = String.Format("{0}_{1}.jpg", User.Instance.Name, levelName);
                string fullPath = Buddy.Resources.AppRawDataPath + PicturePath;
                byte[] bytes = Picture.EncodeToJPG();
                File.WriteAllBytes(fullPath, bytes);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CertificateSummary))
                return false;

            CertificateSummary oSummary = (CertificateSummary)obj;

            if (this.Table > 0)
                return oSummary.Table == this.Table;
            else
            {
                bool first = (oSummary.Difficulty == this.Difficulty);
                bool second = (this.Operands & ~oSummary.Operands)==0 ; 
                return ( first && second );
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 1311073211;
            hashCode = hashCode * -1521134295 + Difficulty.GetHashCode();
            hashCode = hashCode * -1521134295 + Operands.GetHashCode();
            hashCode = hashCode * -1521134295 + Table.GetHashCode();
            hashCode = hashCode * -1521134295 + TimeStamp.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PicturePath);
            hashCode = hashCode * -1521134295 + EqualityComparer<Texture2D>.Default.GetHashCode(Picture);
            return hashCode;
        }
    }
}

