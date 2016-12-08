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

        bool start = false;
        Dropdown lDropDownComponent;
        // Use this for initialization
        void OnEnable()
        {
            GameObject lDropDown = Instantiate(paramGameObjects[5]);
            lDropDownComponent = lDropDown.GetComponent<Dropdown>();

            lDropDown.transform.SetParent(parametersGroup, false);
        }

        void Update()
        {
            if (!start)
            {

                IOTDropdownCmd lCmd = new IOTDropdownCmd("IOTPhilipsHue");
                //lDropDownComponent.UpdatesCommands.Add(lCmd);
                lDropDownComponent.UpdateCommands.Add(lCmd);
                lDropDownComponent.AddOption("Philips Hue", new object[] { this, "BuddyApp.IOT.IOTPhilipsHue" });
                lDropDownComponent.AddOption("Philips Hue", new object[] { this, "BuddyApp.IOT.IOTPhilipsHue" });
                start = true;
            }
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
