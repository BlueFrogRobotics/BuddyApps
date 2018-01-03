using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Somfy
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class SomfyBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private SomfyData mAppData;

        public IOTSomfy Box { get; private set; }
        private IOTSomfyDevice store;
        private IOTSomfyDevice plug;
        private IOTSomfyDevice thermostat;
        private IOTSomfyDevice thermometer;


        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			SomfyActivity.Init(null);

			
			/*
			* Init your app data
			*/
            mAppData = SomfyData.Instance;

            SomfyData.Instance.URL_API = "https://ha102-1.overkiz.com/enduser-mobile-web/enduserAPI";

            
            
        }

        public void Login()
        {
            Box = new IOTSomfy();
            Box.Login();
        }

        public void GetDevices()
        {
            //box.GetDevices();
            Debug.Log("nb devices: " + Box.Devices.Count);
            foreach(IOTDevices  device in Box.Devices)
            {
                //Debug.Log("le machin");
                Debug.Log("category : " + device.Type.ToString());

                if (device.Type == IOTDevices.DeviceType.STORE)
                {
                    Debug.Log("store");
                    store = (IOTSomfyDevice)device;
                    //if (store.states != null)
                    //{
                    //    foreach (IOTSomfyStateJSON state in store.states)
                    //    {
                    //        Debug.Log("state store, " + state.name + ": " + state.value);
                    //    }
                    //}
                }
                else if (device.Type == IOTDevices.DeviceType.SWITCH && device.Name == "plug 2")
                {
                    Debug.Log("plug");
                    plug = (IOTSomfyDevice)device;
                    //if (plug.states != null)
                    //{
                    //    foreach (IOTSomfyStateJSON state in plug.states)
                    //    {
                    //        Debug.Log("state, " + state.name + ": " + state.value);
                    //    }
                    //}
                }
                else if (device.Type == IOTDevices.DeviceType.THERMOSTAT && device.Name == "thermostat")
                {
                    Debug.Log("le thermos: " + device.Name);
                    thermostat = (IOTSomfyDevice)device;
                }
                else if (device.Type == IOTDevices.DeviceType.THERMOMETER)
                {
                    Debug.Log("le thermos");
                    thermometer = (IOTSomfyDevice)device;
                    if (thermometer.states != null)
                    {
                        foreach (IOTSomfyStateJSON state in thermometer.states)
                        {
                            Debug.Log("state temperature, " + state.name + ": " + state.value);
                        }
                    }
                }
            }
        }

        public void OpenStore()
        {
            if(store!=null && store.HasFinishedCommand())
                store.Command(3);
        }

        public void CloseStore()
        {
            if (store != null && store.HasFinishedCommand())
                store.Command(2);
        }

        public void SwitchOnPlug()
        {
            if (plug != null && plug.HasFinishedCommand())
                plug.OnOff(true);
        }

        public void SwitchOffPlug()
        {
            if (plug != null && plug.HasFinishedCommand())
                plug.OnOff(false);
        }

        public void SwitchPlug(bool iVal)
        {
            if (plug != null && plug.HasFinishedCommand())
                plug.OnOff(iVal);
        }

        public void SetTemperature(float iTemp)
        {
            if (thermostat != null && thermostat.HasFinishedCommand())
                thermostat.Command(4, iTemp);
        }

        public string GetTemperature()
        {
            string lTemperature = "";
            if(thermometer!=null && thermometer.states!=null)
            {
                foreach(IOTSomfyStateJSON state in thermometer.states)
                {
                    Debug.Log("le temp state name : " + state.name + " value: " + state.value);
                    if(state.name.Contains("core:TemperatureState"))
                    {
                        Debug.Log("la temperature: " + state.value);
                        lTemperature = state.value;
                        //float.TryParse(state.value, out lTemperature);
                    }
                }
            }
            return lTemperature;
        }

        public void OpenThermostat()
        {
            if(thermostat==null)
            {
                Debug.Log("c est nul!");
            }
            thermostat.Command(3, 15f);
        }

        public void CloseThermostat()
        {
            thermostat.Command(3, 25f);
            //thermometer.Command(7);
        }
    }
}