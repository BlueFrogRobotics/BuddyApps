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
        private IOTDevices store;
        private IOTDevices plug;
        private IOTDevices thermostat;
        private IOTDevices thermometer;


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
                    store = device;
                }
                else if (device.Type == IOTDevices.DeviceType.SWITCH && device.Name=="plug 2")
                    plug = device;
                else if (device.Type == IOTDevices.DeviceType.THERMOSTAT && device.Name=="thermos")
                {
                    Debug.Log("le thermos: " + device.Name);
                    thermostat = device;
                }
                else if (device.Type == IOTDevices.DeviceType.THERMOMETER)
                {
                    Debug.Log("le thermos");
                    thermometer = device;
                }
            }
        }

        public void OpenStore()
        {
            store.Command(2);
        }

        public void CloseStore()
        {
            store.Command(3);
        }

        public void SwitchOnPlug()
        {
            plug.OnOff(true);
        }

        public void SwitchOffPlug()
        {
            plug.OnOff(false);
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