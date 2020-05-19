using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BlueQuark;
using System;

namespace BuddyApp.CoursTelepresence
{

    //TODO : 
    // - make function to start routine from another script
    // - Return an object User with all informations
    // - update with funtion for studentInfo from ZohoTestBehaviour

    
    public sealed class DBManager : MonoBehaviour
    {
        private static DBManager instance = null;
        private static readonly object padLock = new object();

        DBManager()
        {

        }

        public static DBManager Instance
        {
            get
            {
                lock(padLock)
                {
                    if(instance == null)
                    {
                        instance = new DBManager();
                    }
                    return instance;
                }
            }
        }

        private const string TOKEN = "98bb0865eb455a6e61a993a43f63d601";
        private const string GET_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_DEVICE_USERS_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_user_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_TYPE_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Type_device_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string UPDATE_INFO = "https://creator.zoho.eu/api/bluefrogrobotics/json/flotte/form/Device/record/update?authtoken=" + TOKEN + "&scope=creatorapi&criteria=Uid=={";
        private const string GET_USER = "https://creator.zoho.eu/api/json/flotte/view/User_Report?authtoken="+ TOKEN +"&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_INFO_STUDENTS = "https://creator.zoho.eu/api/json/flotte/view/User_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_PLANNING = "https://creator.zoho.eu/api/json/flotte/view/Device_user_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";

        private const string TABLET_TYPE_DEVICE = "Tablette";

        public User UserStudent{ get; private set; }

        private string mURIBattery;
        private string mURIPing;
        private string mURIPosition;
        private WWWForm mEmptyForm;
        public bool DBConnected { get; private set; }
        public List<string> ListUIDTablet { get; private set; }
        public bool Peering { get; private set; }
        public Planning Planning { get; private set; }

        private int mNbIteration;

        // Use this for initialization
        void Start()
        {
            Peering = false;
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
                mURIBattery = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Batterie=" + (Buddy.Sensors.Battery.Level*100).ToString();
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
                yield return new WaitForSeconds(5F);
            }

        }

        private IEnumerator UpdatePingAndPosition()
        {
            while (true)
            {
                mNbIteration++;

                //if(mNbIteration % 2 == 0)
                //{
                    //Update GPS Position TODO : search for a method to have the GPS position, waiting for thierry's answer
                    mURIPosition = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Position_GPS=" + "";
                    Debug.LogWarning("mURIPing : " + mURIPing);
                    using (UnityWebRequest lUpdatePosition = UnityWebRequest.Post(mURIPosition, mEmptyForm))
                    {
                        lUpdatePosition.chunkedTransfer = false;
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
                //}
                //Update DB with ping
                mURIPing = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Qualite_signal=" + CoursTelepresenceData.Instance.Ping;
                Debug.LogWarning("mURIPing : " + mURIPing);
                using (UnityWebRequest lUpdatePing = UnityWebRequest.Post(mURIPing, mEmptyForm))
                {
                    lUpdatePing.chunkedTransfer = false;
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
            while(true && !Peering)
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
                                    foreach (DeviceUser entry in lDeviceUsers.Device_user)
                                    {
                                        if (entry.DeviceId == lIdDeviceRobot)
                                            lIdUserRobotList.Add(entry.UserId);
                                    }
                                    if (lIdUserRobotList.Count > 0)
                                    {
                                        foreach (DeviceUser entry in lDeviceUsers.Device_user)
                                        {
                                            if (lIdUserRobotList.Contains(entry.UserId)
                                                && entry.DeviceId != lIdDeviceRobot)
                                                lIdOtherDevices.Add(entry.DeviceId);
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

                    List<int> IdDevice = new List<int>();
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
                                            IdDevice.Add(device.idDevice);
                                        }
                                        if (ListUIDTablet.Count > 0)
                                            Peering = true;
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
                yield return new WaitForSeconds(300F);

            }
            StartCoroutine(GetPlanning());
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
                            foreach (DeviceUser device in devicesUser.Device_user)
                            {
                                for (int i = 0; i < iListIdTablet.Count; ++i)
                                {
                                    if (device.DeviceId == iListIdTablet[i])
                                        idStudents.Add(device.UserId);
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
            UserStudent = new User();
            foreach (int lIdUser in idStudents)
            {
                Debug.Log("IDUSER : " + lIdUser);
                string requestInfo = GET_INFO_STUDENTS + "&idUser=" + lIdUser;
                using (UnityWebRequest www = UnityWebRequest.Get(requestInfo))
                {
                    yield return www.SendWebRequest();
                    if (www.isHttpError || www.isNetworkError)
                    {
                        Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
                    }
                    else
                    {
                        string res = www.downloadHandler.text;
                        Debug.Log("Result get user info : " + res);
                        try
                        {
                            UserList userList = Utils.UnserializeJSON<UserList>(res);
                            foreach (User user in userList.User)
                            {
                                UserStudent.Nom = user.Nom;
                                UserStudent.Prenom = user.Prenom;
                                UserStudent.Organisme = user.Organisme;
                                
                                Debug.LogWarning("USER NAME : " + UserStudent.Nom + " Class : " + UserStudent.Organisme + " USER PRENOM : " + UserStudent.Prenom);
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

        private IEnumerator GetPlanning()
        {
            string request = GET_PLANNING + "&idUser=" + ListUIDTablet[0];
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
                    Debug.Log("Result get user info : " + res);
                    try
                    {
                        PlanningList planningList = Utils.UnserializeJSON<PlanningList>(res);
                        foreach (Planning planning in planningList.Planning)
                        {
                            Planning.Date_Debut = planning.Date_Debut;
                            Planning.Date_Fin = planning.Date_Fin;
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
