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
        private IOTSomfyDevice plug2;
        private IOTSomfyDevice thermostat;
        private IOTSomfyDevice thermometer;
        private IOTSomfyDevice sonos;


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
            foreach (IOTDevices device in Box.Devices)
            {
                //Debug.Log("le machin");
                Debug.Log("category : " + device.Type.ToString());

                if (device.Type == IOTDevices.DeviceType.STORE && device.Name == "screen 1")
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
                else if (device.Type == IOTDevices.DeviceType.SWITCH && device.Name == "kitchen")
                {
                    Debug.Log("plug2");
                    plug2 = (IOTSomfyDevice)device;
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
                else if (device.Type == IOTDevices.DeviceType.SPEAKER)
                {
                    Debug.Log("le sonos: " + device.Name);
                    sonos = (IOTSomfyDevice)device;
                }
            }
        }

        public void OpenStore()
        {
            if (store != null && store.HasFinishedCommand())
            {
                BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
                BYOS.Instance.Interaction.TextToSpeech.SayKey("openstore");
                store.Command(3);
            }
        }

        public void CloseStore()
        {
            if (store != null && store.HasFinishedCommand())
            {
                BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
                BYOS.Instance.Interaction.TextToSpeech.SayKey("closestore");
                store.Command(2);
            }
        }

        public void SwitchOnLivingRoomPlug()
        {
            if (plug != null && plug.HasFinishedCommand())
            {
                BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
                BYOS.Instance.Interaction.TextToSpeech.SayKey("on");
                plug.OnOff(true);
            }
        }

        public void SwitchOffLivingRoomPlug()
        {
            if (plug != null && plug.HasFinishedCommand())
            {
                BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
                BYOS.Instance.Interaction.TextToSpeech.SayKey("off");
                plug.OnOff(false);
            }
        }

        public void SwitchOnOfficeRoomPlug()
        {
            if (plug2 != null && plug2.HasFinishedCommand())
            {
                BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
                BYOS.Instance.Interaction.TextToSpeech.SayKey("on");
                plug2.OnOff(true);
            }
        }

        public void SwitchOffOfficeRoomPlug()
        {
            if (plug2 != null && plug2.HasFinishedCommand())
            {
                BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
                BYOS.Instance.Interaction.TextToSpeech.SayKey("off");
                plug2.OnOff(false);
            }
        }

        public void SwitchOnAllPlugs()
        {
            if (plug2 != null && plug2.HasFinishedCommand())
                StartCoroutine(SwitchOnAllPlugsCoroutine());
        }

        IEnumerator SwitchOnAllPlugsCoroutine()
        {
            BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
            BYOS.Instance.Interaction.TextToSpeech.SayKey("onalllights");
            if (plug2 != null && plug2.HasFinishedCommand())
            {
                plug2.OnOff(true);
            }
            while (!plug2.HasFinishedCommand())
                yield return null;
            if (plug != null && plug.HasFinishedCommand())
            {
                plug.OnOff(true);
            }
        }

        public void SwitchOffAllPlugs()
        {
            if (plug2 != null && plug2.HasFinishedCommand())
            {
                StartCoroutine(SwitchOffAllPlugsCoroutine());
            }
        }

        IEnumerator SwitchOffAllPlugsCoroutine()
        {
            BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
            BYOS.Instance.Interaction.TextToSpeech.SayKey("offalllights");
            if (plug2 != null && plug2.HasFinishedCommand())
            {
                plug2.OnOff(false);
            }
            while (!plug2.HasFinishedCommand())
                yield return null;
            if (plug != null && plug.HasFinishedCommand())
            {
                plug.OnOff(false);
            }
        }
        public void SwitchPlug(bool iVal)
        {
            if (plug != null && plug.HasFinishedCommand())
            {
                BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
                if (iVal)
                    BYOS.Instance.Interaction.TextToSpeech.SayKey("on");
                else
                    BYOS.Instance.Interaction.TextToSpeech.SayKey("off");
                plug.OnOff(iVal);
            }
        }

        public void SetSonosVolume(float iVolume)
        {
            if (sonos != null && sonos.HasFinishedCommand())
            {
                //BYOS.Instance.Interaction.TextToSpeech.Say("Ok, " + string.Format(BYOS.Instance.Dictionary.GetString("settemperature"), iTemp));
                //Debug.Log(string.Format(BYOS.Instance.Dictionary.GetString("settemperature"), iTemp));
                sonos.Command(4, iVolume);
            }
        }

        public void SetTemperature(float iTemp)
        {
            if (thermostat != null && thermostat.HasFinishedCommand())
            {
                BYOS.Instance.Interaction.TextToSpeech.Say("Ok, " + string.Format(BYOS.Instance.Dictionary.GetString("settemperature"), iTemp));
                //Debug.Log(string.Format(BYOS.Instance.Dictionary.GetString("settemperature"), iTemp));
                thermostat.Command(4, iTemp);
            }
        }

        public string GetTemperature()
        {
            string lTemperature = "";
            if (thermometer != null && thermometer.states != null)
            {
                foreach (IOTSomfyStateJSON state in thermometer.states)
                {
                    Debug.Log("le temp state name : " + state.name + " value: " + state.value);
                    if (state.name.Contains("core:TemperatureState"))
                    {
                        Debug.Log("la temperature: " + state.value);
                        lTemperature = state.value;
                        //float.TryParse(state.value, out lTemperature);
                    }
                }
            }
            return lTemperature;
        }

        public void PlayMusic()
        {
            if (sonos != null && sonos.HasFinishedCommand())
            {
                BYOS.Instance.Interaction.TextToSpeech.SayKey("playmusic");
                sonos.Command(2);
            }
        }

        public void StopMusic()
        {
            if (sonos != null && sonos.HasFinishedCommand())
            {
                BYOS.Instance.Interaction.TextToSpeech.SayKey("stopmusic");
                sonos.Command(3);
            }
        }

        public void NextMusic()
        {
            if (sonos != null && sonos.HasFinishedCommand())
            {
                //BYOS.Instance.Interaction.TextToSpeech.SayKey("stopmusic");
                sonos.Command(5);
            }
        }

        public void PreviousMusic()
        {
            if (sonos != null && sonos.HasFinishedCommand())
            {
                //BYOS.Instance.Interaction.TextToSpeech.SayKey("stopmusic");
                sonos.Command(6);
            }
        }

        public void RewindMusic()
        {
            if (sonos != null && sonos.HasFinishedCommand())
            {
                //BYOS.Instance.Interaction.TextToSpeech.SayKey("stopmusic");
                sonos.Command(7);
            }
        }

        //public void OpenThermostat()
        //{
        //    if(thermostat==null)
        //    {
        //        Debug.Log("c est nul!");
        //    }
        //    thermostat.Command(3, 15f);
        //}

        //public void CloseThermostat()
        //{
        //    thermostat.Command(3, 25f);
        //    //thermometer.Command(7);
        //}
    }
}