using System;

using UnityEngine;
using Buddy;
using System.IO;

namespace BuddyApp.PlayMath {
    public class CertificateSummary {

        public int Difficulty { get; private set; }
        public Operand Operands { get; private set; }
        public int Table { get; private set; }
        public DateTime TimeStamp { get; private set; }
        private string mPicturePath;

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

                mPicturePath = String.Format("{0}_{1}.jpg", User.Instance.Name, levelName);
                string fullPath = BYOS.Instance.Resources.GetPathToRaw(mPicturePath, LoadContext.APP);
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
    }
}

