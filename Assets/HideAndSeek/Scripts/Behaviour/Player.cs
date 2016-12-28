using UnityEngine;
using System.Collections;
using OpenCVUnity;

namespace BuddyApp.HideAndSeek
{
    public class Player
    {
        public string Name { get { return mName; } set{ mName = value; } }
        public Mat FaceMat { get { return mFaceMat; } set { mFaceMat = value; } }

        private string mName;
        private Mat mFaceMat;

        public Player(string iName, Mat iFaceMat)
        {
            mName = iName;
            mFaceMat = iFaceMat;
        }
    }
}