using BlueQuark;

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
        private IOTSomfyDevice mStore;
        private IOTSomfyDevice mPlug;
        private IOTSomfyDevice mPlug2;
        private IOTSomfyDevice mThermostat;
        private IOTSomfyDevice mThermometer;
        private IOTSomfyDevice mSonos;


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

        public void ConnectDevices()
        {
            Box.GetDevices();
        }

        public IEnumerator ConnectTheDevices()
        {
            return Box.GetTheDevices();
        }

        public void GetDevices()
        {
            //box.GetDevices();
            Debug.Log("debut de get device");
            Debug.Log("nb devices: " + Box.Devices.Count);
            foreach (IOTDevices lDevice in Box.Devices)
            {
                //Debug.Log("le machin");
                Debug.Log("category : " + lDevice.Type.ToString());

                if (lDevice.Type == IOTDevices.DeviceType.STORE && lDevice.Name == "Living room blind")
                {
                    Debug.Log("store");
                    mStore = (IOTSomfyDevice)lDevice;
                    //if (store.states != null)
                    //{
                    //    foreach (IOTSomfyStateJSON state in store.states)
                    //    {
                    //        Debug.Log("state store, " + state.name + ": " + state.value);
                    //    }
                    //}
                }
                else if (lDevice.Type == IOTDevices.DeviceType.SWITCH && lDevice.Name == "Living room plug")
                {
                    Debug.Log("plug living room");
                    mPlug = (IOTSomfyDevice)lDevice;
                    //if (plug.states != null)
                    //{
                    //    foreach (IOTSomfyStateJSON state in plug.states)
                    //    {
                    //        Debug.Log("state, " + state.name + ": " + state.value);
                    //    }
                    //}
                }
                else if (lDevice.Type == IOTDevices.DeviceType.SWITCH && lDevice.Name == "Office plug")
                {
                    Debug.Log("plug Office");
                    mPlug2 = (IOTSomfyDevice)lDevice;
                    //if (plug.states != null)
                    //{
                    //    foreach (IOTSomfyStateJSON state in plug.states)
                    //    {
                    //        Debug.Log("state, " + state.name + ": " + state.value);
                    //    }
                    //}
                }
                else if (lDevice.Type == IOTDevices.DeviceType.THERMOSTAT && lDevice.Name == "thermostat")
                {
                    Debug.Log("le thermos: " + lDevice.Name);
                    mThermostat = (IOTSomfyDevice)lDevice;
                }
                else if (lDevice.Type == IOTDevices.DeviceType.THERMOMETER && lDevice.Name == "thermostat-1")
                {
                    Debug.Log("le thermos");
                    mThermometer = (IOTSomfyDevice)lDevice;
                    if (mThermometer.states != null)
                    {
                        foreach (IOTSomfyStateJSON state in mThermometer.states)
                        {
                            Debug.Log("state temperature, " + state.name + ": " + state.value);
                        }
                    }
                }
                else if (lDevice.Type == IOTDevices.DeviceType.SPEAKER)
                {
                    Debug.Log("le sonos: " + lDevice.Name);
                    mSonos = (IOTSomfyDevice)lDevice;
                }
            }
        }

        public void OpenStore()
        {
            if (mStore != null && mStore.HasFinishedCommand())
            {
                Buddy.Vocal.Say("OK");
                Buddy.Vocal.SayKey("openstore");
                mStore.Command(3);
            }
        }

        public void CloseStore()
        {
            if (mStore != null && mStore.HasFinishedCommand())
            {
                Buddy.Vocal.Say("OK");
                Buddy.Vocal.SayKey("closestore");
                mStore.Command(2);
            }
        }

        public void SwitchOnLivingRoomPlug()
        {
            if (mPlug != null && mPlug.HasFinishedCommand())
            {
                Buddy.Vocal.Say("OK");
                Buddy.Vocal.SayKey("on");
                mPlug.OnOff(true);
            }
        }

        public void SwitchOffLivingRoomPlug()
        {
            if (mPlug != null && mPlug.HasFinishedCommand())
            {
                Buddy.Vocal.Say("OK");
                Buddy.Vocal.SayKey("off");
                mPlug.OnOff(false);
            }
        }

        public void SwitchOnOfficeRoomPlug()
        {
            if (mPlug2 != null && mPlug2.HasFinishedCommand())
            {
                Buddy.Vocal.Say("OK");
                Buddy.Vocal.SayKey("on");
                mPlug2.OnOff(true);
            }
        }

        public void SwitchOffOfficeRoomPlug()
        {
            if (mPlug2 != null && mPlug2.HasFinishedCommand())
            {
                Buddy.Vocal.Say("OK");
                Buddy.Vocal.SayKey("off");
                mPlug2.OnOff(false);
            }
        }

        public void SwitchOnAllPlugs()
        {
            if (mPlug2 != null && mPlug2.HasFinishedCommand())
                StartCoroutine(SwitchOnAllPlugsCoroutine());    
        }

        IEnumerator SwitchOnAllPlugsCoroutine()
        {
            Buddy.Vocal.Say("OK");
            Buddy.Vocal.SayKey("onalllights");
            if (mPlug2 != null && mPlug2.HasFinishedCommand())
            {
                mPlug2.OnOff(true);
            }
            while (!mPlug2.HasFinishedCommand())
                yield return null;
            if (mPlug != null && mPlug.HasFinishedCommand())
            {
                mPlug.OnOff(true);
            }
        }

        public void SwitchOffAllPlugs()
        {
            if (mPlug2 != null && mPlug2.HasFinishedCommand())
            {
                StartCoroutine(SwitchOffAllPlugsCoroutine());
            }
        }

        IEnumerator SwitchOffAllPlugsCoroutine()
        {
            Buddy.Vocal.Say("OK");
            Buddy.Vocal.SayKey("offalllights");
            if (mPlug2 != null && mPlug2.HasFinishedCommand())
            {
                mPlug2.OnOff(false);
            }
            while (!mPlug2.HasFinishedCommand())
                yield return null;
            if (mPlug != null && mPlug.HasFinishedCommand())
            {
                mPlug.OnOff(false);
            }
        }
        public void SwitchPlug(bool iVal)
        {
            Debug.Log("switch plug");
            if (mPlug2 != null && mPlug2.HasFinishedCommand())
            {
                Buddy.Vocal.Say("OK");
                if (iVal)
                    Buddy.Vocal.SayKey("on");
                else
                    Buddy.Vocal.SayKey("off");
                mPlug2.OnOff(iVal);
            }
        }

        public void GetVolume()
        {
            if (mSonos != null && mSonos.HasFinishedCommand())
            {
                //Buddy.Vocal.SayKey("playmusic");
                mSonos.Command(8);
            }
        }

        public void SetSonosVolume(float iVolume)
        {
            if (mSonos != null && mSonos.HasFinishedCommand())
            {
                //Buddy.Vocal.Say("Ok, " + string.Format(Buddy.Resources.GetString("settemperature"), iTemp));
                //Debug.Log(string.Format(Buddy.Resources.GetString("settemperature"), iTemp));
                mSonos.Command(4, iVolume);
            }
        }

        public void SetTemperature(float iTemp)
        {
            if (mThermostat != null && mThermostat.HasFinishedCommand())
            {
                Buddy.Vocal.Say("OK, " + string.Format(Buddy.Resources.GetString("settemperature"), iTemp));
                //Debug.Log(string.Format(Buddy.Resources.GetString("settemperature"), iTemp));
                mThermostat.Command(4, iTemp);
            }
        }

        public string GetTemperature()
        {
            string lTemperature = "";
            if (mThermometer != null && mThermometer.states != null)
            {
                foreach (IOTSomfyStateJSON state in mThermometer.states)
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
            if (mSonos != null && mSonos.HasFinishedCommand())
            {
                Buddy.Vocal.SayKey("playmusic");
                mSonos.Command(2);
            }
        }

        public void StopMusic()
        {
            if (mSonos != null && mSonos.HasFinishedCommand())
            {
                Buddy.Vocal.SayKey("stopmusic");
                mSonos.Command(3);
            }
        }

        public void NextMusic()
        {
            if (mSonos != null && mSonos.HasFinishedCommand())
            {
                //Buddy.Vocal.SayKey("stopmusic");
                mSonos.Command(5);
            }
        }

        public void PreviousMusic()
        {
            if (mSonos != null && mSonos.HasFinishedCommand())
            {
                //Buddy.Vocal.SayKey("stopmusic");
                mSonos.Command(6);
            }
        }

        public void RewindMusic()
        {
            if (mSonos != null && mSonos.HasFinishedCommand())
            {
                //Buddy.Vocal.SayKey("stopmusic");
                mSonos.Command(7);
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