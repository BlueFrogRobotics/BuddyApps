using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BlueQuark;
using System;

namespace BuddyApp.CoursTelepresence
{
    public class DBManager : MonoBehaviour
    {
        private const string TOKEN = "98bb0865eb455a6e61a993a43f63d601";
        private const string GET_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_DEVICE_USERS_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_user_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_TYPE_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Type_device_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string UPDATE_INFO = "https://creator.zoho.eu/api/bluefrogrobotics/json/flotte/form/Device/record/update?authtoken=" + TOKEN + "&scope=creatorapi&criteria=Uid=={";
        private const string TABLET_TYPE_DEVICE = "Tablette";

        private string mURIBattery;
        private string mURIPing;
        private string mURIPosition;
        private WWWForm mEmptyForm;
        public bool DBConnected { get; private set; }
        public List<string> ListUIDTablet { get; private set; }

        private int mNbIteration;

        // Use this for initialization
        void Start()
        {
            mNbIteration = 0;
            DBConnected = false;
            StartCoroutine(GetTabletUID(Buddy.Platform.RobotUID));
            StartCoroutine(UpdatePingAndPosition());
            StartCoroutine(UpdateBattery());
        }

        private IEnumerator UpdateBattery()
        {
            while (true)
            {
                //Update DB with battery Level
                mURIBattery = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Batterie={" + Buddy.Sensors.Battery.Level.ToString() + "}";
                Debug.LogWarning("lURI Battery : " + mURIBattery);
                using (UnityWebRequest lUpdateBattery = UnityWebRequest.Post(mURIBattery, mEmptyForm))
                {
                    lUpdateBattery.chunkedTransfer = false;
                    yield return lUpdateBattery.SendWebRequest();
                    if (lUpdateBattery.isHttpError || lUpdateBattery.isNetworkError)
                    {
                        Debug.LogError("Request error battery : " + lUpdateBattery.error + " " + lUpdateBattery.downloadHandler.text);
                    }
                    else
                    {
                        Debug.LogWarning("Post done battery : " + lUpdateBattery.downloadHandler.text + " " + lUpdateBattery.downloadHandler.isDone);
                    }
                }
                yield return new WaitForSeconds(300F);
            }
        }

        private IEnumerator UpdatePingAndPosition()
        {
            while (true)
            {
                mNbIteration++;

                if(mNbIteration % 2 == 0)
                {
                    //Update GPS Position
                    mURIPosition = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Position_GPS={}";
                    Debug.LogWarning("mURIPing : " + mURIPing);
                    using (UnityWebRequest lUpdatePosition = UnityWebRequest.Post(mURIPosition, mEmptyForm))
                    {
                        yield return lUpdatePosition.SendWebRequest();
                        if (lUpdatePosition.isHttpError || lUpdatePosition.isNetworkError)
                        {
                            Debug.LogError("Request error position : " + lUpdatePosition.error + " " + lUpdatePosition.downloadHandler.text);
                        }
                        else
                        {
                            Debug.LogWarning("Post done position : " + lUpdatePosition.downloadHandler.text + " " + lUpdatePosition.downloadHandler.isDone);
                        }
                    }
                }
                //Update DB with ping
                mURIPing = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Qualite_signal={" + CoursTelepresenceData.Instance.Ping + "}";
                Debug.LogWarning("mURIPing : " + mURIPing);
                using (UnityWebRequest lUpdatePing = UnityWebRequest.Post(mURIPing, mEmptyForm))
                {
                    yield return lUpdatePing.SendWebRequest();
                    if (lUpdatePing.isHttpError || lUpdatePing.isNetworkError)
                    {
                        Debug.LogError("Request error ping : " + lUpdatePing.error + " " + lUpdatePing.downloadHandler.text);
                    }
                    else
                    {
                        Debug.LogWarning("Post done ping : "  + lUpdatePing.downloadHandler.text + " " + lUpdatePing.downloadHandler.isDone);
                    }
                }
                yield return new WaitForSeconds(5F);
            }
        }

        private IEnumerator GetTabletUID(string iRobotUID)
        {
            int lIdDeviceRobot = -1;
            string lRequest = GET_DEVICE_URL + "&Uid=" + iRobotUID;
            using (UnityWebRequest lRequestDevice = UnityWebRequest.Get(lRequest))
            {
                yield return lRequestDevice.SendWebRequest();

                if (lRequestDevice.isHttpError || lRequestDevice.isNetworkError)
                {
                    Debug.LogError("Request error " + lRequestDevice.error + " " + lRequestDevice.downloadHandler.text);
                }
                else
                {
                    string lRes = lRequestDevice.downloadHandler.text;
                    DBConnected = true;
                    Debug.Log("Result get robot device : " + lRes);

                    try
                    {
                        DevicesCollection devices = Utils.UnserializeJSON<DevicesCollection>(lRes);

                        if (devices != null
                            && devices.Device != null
                            && devices.Device.Length > 0)
                        {
                            
                            lIdDeviceRobot = devices.Device[0].idDevice;
                        }
                        else
                        {
                            Debug.LogError("No device found with Uid " + iRobotUID);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing robot device request answer : " + e.Message);
                    }
                }
            }

            if (lIdDeviceRobot != -1)
            {
                List<int> lIdOtherDevices = new List<int>();
                lRequest = GET_DEVICE_USERS_URL;
                using (UnityWebRequest lRequestDeviceUser = UnityWebRequest.Get(lRequest))
                {
                    yield return lRequestDeviceUser.SendWebRequest();
                    if (lRequestDeviceUser.isHttpError || lRequestDeviceUser.isNetworkError)
                    {
                        Debug.Log("Request error " + lRequestDeviceUser.error + " " + lRequestDeviceUser.downloadHandler.text);
                    }
                    else
                    {
                        string lRes = lRequestDeviceUser.downloadHandler.text;
                        Debug.Log("Result get robot users : " + lRes);
                        try
                        {
                            DeviceUserCollection lDeviceUsers = Utils.UnserializeJSON<DeviceUserCollection>(lRes);//JsonUtility.FromJson<DeviceUserCollection>(res);

                            if (lDeviceUsers != null
                                && lDeviceUsers.Device_user != null)
                            {
                                List<int> lIdUserRobotList = new List<int>();
                                foreach (DeviceUserData entry in lDeviceUsers.Device_user)
                                {
                                    if (entry.Device == lIdDeviceRobot)
                                        lIdUserRobotList.Add(entry.User);
                                }
                                if (lIdUserRobotList.Count > 0)
                                {
                                    foreach (DeviceUserData entry in lDeviceUsers.Device_user)
                                    {
                                        if (lIdUserRobotList.Contains(entry.User)
                                            && entry.Device != lIdDeviceRobot)
                                            lIdOtherDevices.Add(entry.Device);
                                    }
                                }
                                else
                                {
                                    Debug.Log("No device user found for Device " + lIdDeviceRobot);
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
                if (lIdOtherDevices.Count == 0)
                    Debug.Log("There is no device connected to the robot");

                
                foreach (int idDevice in lIdOtherDevices)
                {
                    lRequest = GET_DEVICE_URL + "&idDevice=" + idDevice + "&Type_device=" + TABLET_TYPE_DEVICE;
                    Debug.Log("Sending device request : " + lRequest);
                    using (UnityWebRequest lRequestTabletUID = UnityWebRequest.Get(lRequest))
                    {
                        yield return lRequestTabletUID.SendWebRequest();
                        if (lRequestTabletUID.isHttpError || lRequestTabletUID.isNetworkError)
                        {
                            Debug.Log("Request error " + lRequestTabletUID.error + " " + lRequestTabletUID.downloadHandler.text);
                        }
                        else
                        {
                            string lRes = lRequestTabletUID.downloadHandler.text;
                            Debug.Log("Result get devices : " + lRes);
                            try
                            {
                                DevicesCollection devices = Utils.UnserializeJSON<DevicesCollection>(lRes);
                                if (devices != null
                                    && devices.Device != null
                                    && devices.Device.Length > 0)
                                {
                                    foreach (DeviceData device in devices.Device)
                                    {
                                        Debug.Log("Tablet connected to the robot " + device.Nom);
                                        ListUIDTablet.Add(device.Uid);
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
