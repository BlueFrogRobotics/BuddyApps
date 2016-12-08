using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTObjects
    {
        private List<string> mCredentials = new List<string>(3);
        public List<string> Credentials { get { return mCredentials; } }

        public enum ParamType : int { BUTTON, GAUGE, ONOFF, PASSWORD, TEXTFIELD, DROPDOWN};
        private List<GameObject> mListParams = new List<GameObject>();
        public List<GameObject> ListParam { get { return mListParams; } }

        private List<GameObject> mListIntantParams = new List<GameObject>();

        protected GameObject InstanciateParam(ParamType iType) { GameObject lTmp = GameObject.Instantiate(mListParams[(int)iType]); mListIntantParams.Add(lTmp); return lTmp; }
        public virtual void InitializeParams() { }
        public void PlaceParams(Transform iContent) {
            for(int i = 0; i < mListIntantParams.Count; i++)
                mListIntantParams[i].transform.SetParent(iContent, false);
        }

        public virtual void Connect()
        {

        }
    }
}
