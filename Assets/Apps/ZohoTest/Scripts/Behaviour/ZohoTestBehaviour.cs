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

        private const string TOKEN = "98bb0865eb455a6e61a993a43f63d601";
        //private const string GET_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        //private const string GET_DEVICE_USERS_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_user_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        //private const string GET_TYPE_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Type_device_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string UPDATE_INFO = "https://creator.zoho.eu/api/bluefrogrobotics/json/flotte/form/Device/record/update?authtoken=" + TOKEN + "&scope=creatorapi&criteria=Uid=={";
        private const string GET_INFO_STUDENTS = "https://creator.zoho.eu/api/json/flotte/view/User_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";


        private int mNbIteration;
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
            Debug.Log("START APP DEMO"); 
            StartCoroutine(GetTabletsUID("buddy1"));
            //StartCoroutine(UpdatePingAndPosition());
        }


        //private IEnumerator UpdateBattery()
        //{
        //    while (true)
        //    {
        //        //Update DB with battery Level

        //        string lURI = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Batterie={" + Buddy.Sensors.Battery.Level.ToString() + "}";
        //        using (UnityWebRequest lUpdateBattery = UnityWebRequest.Post(lURI, ""))
        //        {

        //        }
        //        yield return new WaitForSeconds(300F);
        //    }
        //}

        //private IEnumerator UpdatePingAndPosition()
        //{
        //    while (true)
        //    {
        //        mNbIteration++;

        //        if (mNbIteration % 2 == 0)
        //        {
        //            //Update GPS Position
        //        }
        //        //Update DB with ping
        //        WWWForm formVide = new WWWForm();
        //        //form.AddField("myField", "myData");
        //        string lURI = UPDATE_INFO + "965441" + "}" + "&Qualite_signal=" + 1337;
        //        string urlTest = "https://creator.zoho.eu/api/bluefrogrobotics/json/flotte/form/Device/record/update?";
        //        Debug.LogWarning("lURI : " + lURI);
        //        using (UnityWebRequest lUpdatePingAndPosition = UnityWebRequest.Post(lURI, formVide))
        //        {
        //            lUpdatePingAndPosition.chunkedTransfer = false;
        //            yield return lUpdatePingAndPosition.SendWebRequest();
        //            if (lUpdatePingAndPosition.isHttpError || lUpdatePingAndPosition.isNetworkError)
        //            {
        //                Debug.LogError("Request error " + lUpdatePingAndPosition.error + " " + lUpdatePingAndPosition.downloadHandler.text);
        //            }
        //            else
        //            {
        //                Debug.LogWarning("POST DONE---------------------------" + lUpdatePingAndPosition.downloadHandler.text + " " + lUpdatePingAndPosition.downloadHandler.isDone);
        //            }
        //        }
        //        yield return new WaitForSeconds(5F);
        //    }
        //}

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
                                    {
                                        idUserRobotList.Add(entry.User);
                                        Debug.Log("ID USER ROBOT LIST : " + entry.User.ToString());
                                    }
                                        
                                }
                                if (idUserRobotList.Count > 0)
                                {
                                    foreach (DeviceUserData entry in deviceUsers.Device_user)
                                    {
                                        if (idUserRobotList.Contains(entry.User)
                                            && entry.Device != idDeviceRobot)
                                        {
                                            idOtherDevices.Add(entry.Device);
                                            Debug.Log("OTHERDEVICE : " + entry.Device.ToString());
                                        }
                                            
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
                List<int> IdDevice = new List<int>();
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
                                        Debug.Log("Tablet connected to the robot " + device.Nom + " UID TABLET : " + device.Uid + " DEVICE ID : " + device.idDevice);
                                        uidTablets.Add(device.Uid);
                                        IdDevice.Add(device.idDevice);
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
                StartCoroutine(GetStudentInfo(IdDevice));
            }
        }

        private IEnumerator GetStudentInfo(List<int> iListIdTablet)
        {
            List<int> idStudents = new List<int>();
            string request = GET_DEVICE_USERS_URL;
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
                        DeviceUserCollection devicesUser = JsonUtility.FromJson<DeviceUserCollection>(res);
                        if (devicesUser != null
                               && devicesUser.Device_user != null)
                        {
                            foreach (DeviceUserData device in devicesUser.Device_user)
                            {
                                for(int i = 0; i < iListIdTablet.Count; ++i)
                                {
                                    if (device.Device == iListIdTablet[i])
                                        idStudents.Add(device.User);
                                }
                                
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing device request answer : " + e.Message);
                    }
                }
                


            }
            if (idStudents.Count == 0)
                Debug.Log("There is no user for this device id");
            foreach (int lIdUser in idStudents)
            {
                Debug.Log("IDUSER : " + lIdUser);
                string requestInfo = GET_INFO_STUDENTS + "&idUser=" + lIdUser;
                Debug.Log("1 : " + requestInfo);
                using (UnityWebRequest www = UnityWebRequest.Get(requestInfo))
                {
                    Debug.Log("2");
                    yield return www.SendWebRequest();
                    Debug.Log("3");
                    if (www.isHttpError || www.isNetworkError)
                    {
                        Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
                    }
                    else
                    {
                        Debug.Log("4");
                        string res = www.downloadHandler.text;
                        Debug.Log("5");
                        Debug.Log("Result get user info : " + res);
                        try
                        {
                            UserList userList = Utils.UnserializeJSON<UserList>(res);
                            foreach(User user in userList.User)
                            {
                                Debug.Log(user.Nom + " Class : " + user.Organisme);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Error parsing user info request answer : " + e.Message);
                        }
                    }

                }
            }


        }
    }
}