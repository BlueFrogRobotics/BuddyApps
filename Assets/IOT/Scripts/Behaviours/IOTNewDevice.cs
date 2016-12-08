﻿using UnityEngine;
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
        private ParametersGameObjectContainer paramContainer;

        [SerializeField]
        private List<string> systemName = new List<string>();
        [SerializeField]
        private List<string> systemList = new List<string>();

        [SerializeField]
        private Transform parametersGroup;

        bool start = false;
        Dropdown lDropDownComponent;
        // Use this for initialization
        void OnEnable()
        {
            GameObject lDropDown = Instantiate(paramContainer.ParametersList[5]);
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
                for(int i = 0; i < systemList.Count; ++i)
                    lDropDownComponent.AddOption(systemName[i], new object[] { this, systemList[i] });
                start = true;
            }
        }

        public void FillParamClasses()
        {
            for (int i = 0; i < paramContainer.ParametersList.Count; ++i)
                mIOTObject.ListParam.Add(paramContainer.ParametersList[i]);
        }

        public void InitiliazeParameters()
        {
            mIOTObject.InitializeParams();
            mIOTObject.PlaceParams(parametersGroup);
        }
    }
}
