using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTNewDevice : MonoBehaviour
    {
        private IOTObjects mIOTObject;
        public IOTObjects IOTObject { get { return mIOTObject; } set { mIOTObject = value; } }
        
        [SerializeField]
        private List<GameObject> paramGameObjects = new List<GameObject>();

        [SerializeField]
        private Transform parametersGroup;
        // Use this for initialization
        void OnEnable()
        {
            for(int i = 0; i < paramGameObjects.Count; ++i)
                mIOTObject.ListParam.Add(paramGameObjects[i]);

            mIOTObject.initializeParams();
            mIOTObject.placeParams(parametersGroup);
        }
    }
}
