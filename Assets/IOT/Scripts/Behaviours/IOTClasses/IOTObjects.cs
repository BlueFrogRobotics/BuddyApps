using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTObjects
    {
        public enum ParamType : int { BUTTON, GAUGE, ONOFF, PASSWORD, TEXTFIELD};
        private List<GameObject> mListParams = new List<GameObject>();
        public List<GameObject> ListParam { get { return mListParams; } }

        private List<GameObject> mListIntantParams = new List<GameObject>();

        protected GameObject instanciateParam(ParamType iType) { GameObject lTmp = GameObject.Instantiate(mListParams[(int)iType]); mListIntantParams.Add(lTmp); return lTmp; }
        public virtual void initializeParams() { }
        public void placeParams(Transform iContent) { }
    }
}
