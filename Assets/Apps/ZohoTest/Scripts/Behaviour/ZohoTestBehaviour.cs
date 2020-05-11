using BlueQuark;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.ZohoTest
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class ZohoTestBehaviour : MonoBehaviour
    {
        private const string GET_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_Report?authtoken=98bb0865eb455a6e61a993a43f63d601&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_DEVICE_USERS_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_user_Report?authtoken=98bb0865eb455a6e61a993a43f63d601&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_TYPE_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Type_device_Report?authtoken=98bb0865eb455a6e61a993a43f63d601&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string TABLET_TYPE_DEVICE = "Tablette";

        /*
         * Data of the application. Save on disc when app is quitted
         */
        private ZohoTestData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			ZohoTestActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = ZohoTestData.Instance;

            StartCoroutine(GetTabletsUID("buddy1"));
        }

        private IEnumerator GetTabletsUID(string robotUID)
        {
            int idDeviceRobot = -1;

            string request = GET_DEVICE_URL + "&Uid=" + robotUID;
            Debug.Log("Sending robot device request : " + request);
            using (UnityWebRequest www = UnityWebRequest.Get(request))
            {
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Debug.LogError("Request error " + www.error + " " + www.downloadHandler.text);
                }
                else
                {
                    string res = www.downloadHandler.text;
                    Debug.Log("Result get robot device : " + res);

                    try
                    {
                        DevicesCollection devices = JsonUtility.FromJson<DevicesCollection>(res);

                        if (devices != null
                            && devices.Device != null
                            && devices.Device.Length > 0)
                        {
                            idDeviceRobot = devices.Device[0].idDevice;
                        }
                        else
                        {
                            Debug.Log("No device found with Uid " + robotUID);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing robot device request answer : " + e.Message);
                    }
                }
            }

            if (idDeviceRobot != -1)
            {
                List<int> idOtherDevices = new List<int>();
                request = GET_DEVICE_USERS_URL;
                Debug.Log("Sending device users request : " + request);
                using (UnityWebRequest www = UnityWebRequest.Get(request))
                {
                    yield return www.SendWebRequest();
                    if (www.isHttpError || www.isNetworkError)
                    {
                        Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
                    }
                    else
                    {
                        string res = www.downloadHandler.text;
                        Debug.Log("Result get robot users : " + res);
                        try
                        {
                            DeviceUserCollection deviceUsers = JsonUtility.FromJson<DeviceUserCollection>(res);
                            if (deviceUsers != null
                                && deviceUsers.Device_user != null)
                            {
                                List<int> idUserRobotList = new List<int>();
                                foreach (DeviceUserData entry in deviceUsers.Device_user)
                                {
                                    if (entry.Device == idDeviceRobot)
                                        idUserRobotList.Add(entry.User);
                                }
                                if (idUserRobotList.Count > 0)
                                {
                                    foreach (DeviceUserData entry in deviceUsers.Device_user)
                                    {
                                        if (idUserRobotList.Contains(entry.User)
                                            && entry.Device != idDeviceRobot)
                                            idOtherDevices.Add(entry.Device);
                                    }
                                }
                                else
                                {
                                    Debug.Log("No device user found for Device " + idDeviceRobot);
                                }
                            }
                            else
                            {
                                Debug.Log("No device user found");
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Error parsing device users request answer : " + e.Message);
                        }
                    }
                }
                if (idOtherDevices.Count == 0)
                    Debug.Log("There is no device connected to the robot");

                List<string> uidTablets = new List<string>();
                foreach (int idDevice in idOtherDevices)
                {
                    request = GET_DEVICE_URL + "&idDevice=" + idDevice + "&Type_device=" + TABLET_TYPE_DEVICE;
                    Debug.Log("Sending device request : " + request);
                    using (UnityWebRequest www = UnityWebRequest.Get(request))
                    {
                        yield return www.SendWebRequest();
                        if (www.isHttpError || www.isNetworkError)
                        {
                            Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
                        }
                        else
                        {
                            string res = www.downloadHandler.text;
                            Debug.Log("Result get devices : " + res);
                            try
                            {
                                DevicesCollection devices = JsonUtility.FromJson<DevicesCollection>(res);
                                if (devices != null
                                    && devices.Device != null
                                    && devices.Device.Length > 0)
                                {
                                    foreach (DeviceData device in devices.Device)
                                    {
                                        Debug.Log("Tablet connected to the robot " + device.Nom);
                                        uidTablets.Add(device.Uid);
                                    }
                                }
                                else
                                {
                                    Debug.Log("No device found with idDevice " + idDevice + " & Type_device " + TABLET_TYPE_DEVICE);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("Error parsing device request answer : " + e.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}