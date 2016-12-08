using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.UI;

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
            GameObject lDropDown = GameObject.Instantiate(paramGameObjects[5]);
            Dropdown lDropDownComponent = lDropDown.GetComponent<Dropdown>();

            IOTDropdownCmd lCmd = new IOTDropdownCmd(this);

            //lDropDownComponent.UpdatesCommands.Add(lCmd);
            //lDropDownComponent.AddOption("Philips Hue", lCmd);
        }

        public void FillParamClasses()
        {
            for (int i = 0; i < paramGameObjects.Count; ++i)
                mIOTObject.ListParam.Add(paramGameObjects[i]);
        }

        public void InitiliazeParameters()
        {
            mIOTObject.InitializeParams();
            mIOTObject.PlaceParams(parametersGroup);
        }
    }
}
