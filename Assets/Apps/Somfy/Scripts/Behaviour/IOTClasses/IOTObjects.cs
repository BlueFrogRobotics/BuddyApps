using UnityEngine;
using Buddy;
using System.Collections.Generic;

namespace BuddyApp.Somfy
{
    public class IOTObjects
    {
        protected List<string> mCredentials = new List<string>();
        protected bool mAvailable = false;
        
        protected string mName;
        protected string mSpriteName;

        protected List<GameObject> mListIntantParams = new List<GameObject>();

        public List<string> Credentials { get { return mCredentials; } set { mCredentials = value; } }
        public bool Available { get { return mAvailable; } }
        
        public string Name { get { return mName; } set { mName = value; } }
        public string SpriteName { get { return mSpriteName; }}

        public void PlaceParams(Transform iContent)
        {
            for (int i = 0; i < mListIntantParams.Count; i++)
                mListIntantParams[i].transform.SetParent(iContent, false);
        }

        public virtual void Connect() { }

        public virtual void OnOff(bool iOnOff)
        {

        }

        public IOTObjects()
        {
            mCredentials.Add("");
            mCredentials.Add("");
            mCredentials.Add("");
        }
    }
}
