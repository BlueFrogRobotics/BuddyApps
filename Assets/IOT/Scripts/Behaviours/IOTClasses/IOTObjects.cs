using UnityEngine;
using BuddyOS.UI;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTObjects
    {
        protected List<string> mCredentials = new List<string>();
        protected bool mAvailable = false;

        protected ParametersGameObjectContainer mParamsGO;
        protected string mName;
        protected string mSpriteName;

        private List<GameObject> mListIntantParams = new List<GameObject>();

        public List<string> Credentials { get { return mCredentials; } set { mCredentials = value; } }
        public bool Available { get { return mAvailable; } }

        public ParametersGameObjectContainer ParamGO { get { return mParamsGO; } set { mParamsGO = value; } }
        public string Name { get { return mName; } set { mName = value; } }
        public string SpriteName { get { return mSpriteName; }}

        public enum ParamType : int { BUTTON, GAUGE, ONOFF, PASSWORD, TEXTFIELD, DROPDOWN, COLORS, LABEL };

        protected GameObject InstanciateParam(ParamType iType)
        {
            GameObject lTmp = GameObject.Instantiate(mParamsGO.ParametersList[(int)iType]);
            mListIntantParams.Add(lTmp);
            return lTmp;
        }

        public virtual void InitializeParams() { mListIntantParams.Clear();

            GameObject lAvailable = InstanciateParam(ParamType.LABEL);
            Label lAvailableComponent = lAvailable.GetComponent<Label>();

            lAvailableComponent.Text = mAvailable?"AVAILABLE":"NOT AVAILABLE";
        }

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
