using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTObjects
    {
        protected List<string> mCredentials = new List<string>();
        protected ParametersGameObjectContainer mParamsGO;
        protected string mName;
        protected string mSpriteName;

        private List<GameObject> mListIntantParams = new List<GameObject>();

        public List<string> Credentials { get { return mCredentials; } set { mCredentials = value; } }
        public ParametersGameObjectContainer ParamGO { get { return mParamsGO; } set { mParamsGO = value; } }
        public string Name { get { return mName; } set { mName = value; } }
        public string SpriteName { get { return mSpriteName; }}

        public enum ParamType : int { BUTTON, GAUGE, ONOFF, PASSWORD, TEXTFIELD, DROPDOWN, COLORS };

        protected GameObject InstanciateParam(ParamType iType)
        {
            GameObject lTmp = GameObject.Instantiate(mParamsGO.ParametersList[(int)iType]);
            mListIntantParams.Add(lTmp);
            return lTmp;
        }

        public virtual void InitializeParams() { mListIntantParams.Clear(); }

        public void PlaceParams(Transform iContent)
        {
            for (int i = 0; i < mListIntantParams.Count; i++)
                mListIntantParams[i].transform.SetParent(iContent, false);
        }

        public virtual void Connect() { }

        public IOTObjects()
        {
            mCredentials.Add("");
            mCredentials.Add("");
            mCredentials.Add("");
        }
    }
}
