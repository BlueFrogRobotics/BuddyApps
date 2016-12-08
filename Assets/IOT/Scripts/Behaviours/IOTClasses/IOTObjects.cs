using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTObjects
    {
        protected List<string> mCredentials = new List<string>(3);
        protected List<GameObject> mListParams = new List<GameObject>();
        protected string mName;

        private List<GameObject> mListIntantParams = new List<GameObject>();

        public List<string> Credentials { get { return mCredentials; } }
        public List<GameObject> ListParam { get { return mListParams; } }
        public string Name { get { return mName; } set { mName = value; } }

        public enum ParamType : int { BUTTON, GAUGE, ONOFF, PASSWORD, TEXTFIELD, DROPDOWN};

        protected GameObject InstanciateParam(ParamType iType) {
            GameObject lTmp = GameObject.Instantiate(mListParams[(int)iType]);
            mListIntantParams.Add(lTmp);
            return lTmp;
        }

        public virtual void InitializeParams() { }

        public void PlaceParams(Transform iContent) {
            for(int i = 0; i < mListIntantParams.Count; i++)
                mListIntantParams[i].transform.SetParent(iContent, false);
        }

        public virtual void Connect() { }
    }
}
