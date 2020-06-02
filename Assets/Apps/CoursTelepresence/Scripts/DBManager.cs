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

    
    public sealed class DBManager : MonoBehaviour
    {
        private static DBManager instance = null;
        //private static readonly object padLock = new object();

        //DBManager()
        //{

        //}

        public static DBManager Instance
        {
            get
            {
                //lock(padLock)
                //{
                    //if(instance == null)
                    //{
                    //    instance = new DBManager();
                    //}
                    return instance;
                //}
            }
        }

        private const string TOKEN = "98bb0865eb455a6e61a993a43f63d601";
        private const string GET_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_DEVICE_USERS_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_user_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_TYPE_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Type_device_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string UPDATE_INFO = "https://creator.zoho.eu/api/bluefrogrobotics/json/flotte/form/Device/record/update?authtoken=" + TOKEN + "&scope=creatorapi&criteria=Uid=={";
        private const string GET_USER = "https://creator.zoho.eu/api/json/flotte/view/User_Report?authtoken="+ TOKEN +"&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_INFO_STUDENTS = "https://creator.zoho.eu/api/json/flotte/view/User_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_PLANNING = "https://creator.zoho.eu/api/json/flotte/view/Planning_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";

        private const string TABLET_TYPE_DEVICE = "Tablette";

        public List<User> ListUserStudent { get; private set; }
        public User UserStudent{ get; private set; }

        private string mURIBattery;
        private string mURIPing;
        private string mURIPosition;
        private WWWForm mEmptyForm;
        public bool DBConnected { get; private set; }
        public List<string> ListUIDTablet { get; private set; }
        public bool Peering { get; private set; }
        public Planning Planning { get; private set; }
        public bool InfoRequestedDone { get; private set; }
        private List<Planning> ListPlanning;
        private Planning mPlanningNextCourse;
        private Planning mPlanningCheck;
        

        private float mTimer;
        private bool mPlanning;
        public bool CanStartCourse { get; private set; }
        public bool CanEndCourse { get; private set; }

        private int mNbIteration;
        private DateTime mDateNow;
        private DateTime mPlanningEnd;
        private TimeSpan mSpan;
        private bool mCheckDBEndCall;

        private void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {
            StartDBManager();
        }

        public void StartDBManager()
        {
            mPlanningNextCourse = new Planning();
            mPlanningCheck = new Planning();
            mTimer = 0F;
            mPlanning = false;
            mCheckDBEndCall = false;
            Peering = false;
            InfoRequestedDone = false;
            mNbIteration = 0;
            DBConnected = false;
            CanStartCourse = false;
            CanEndCourse = false;
            //StartCoroutine(GetTabletUID(Buddy.Platform.RobotUID));
            StartCoroutine(GetTabletUID("33060759D0898EDB5EBC"));

            //StartCoroutine(UpdatePingAndPosition());
            //StartCoroutine(UpdateBattery());
        }

        private void Update()
        {
            
            if(CanStartCourse && !CanEndCourse)
            {
                mDateNow = DateTime.Now;
                
                mSpan = mPlanningEnd.Subtract(mDateNow);
                if (mSpan.TotalMinutes < 5F)
                {
                    Debug.LogError("<color=blue> RESTE 5 MIN </color>");
                    //if(!mCheckDBEndCall)
                    //{
                    //    StartCoroutine(GetPlanning(mPlanningCheck));
                    //    mPlanningEnd = DateTime.Parse(mPlanningCheck.Date_Fin);

                    //    if(mPlanningCheck.Date_Fin == mPlanningNextCourse.Date_Fin)
                    //        mCheckDBEndCall = true;
                    //}
                    if (mSpan.TotalMinutes < 0F)
                    {
                        Debug.LogError("<color=blue> END CALL </color>");
                        CanEndCourse = true;
                        mPlanning = false;
                    }
                }
            }
            if(!CanStartCourse)
            {
                CheckStartPlanning();
            }
        }

        public void AddFiveMinutes()
        {
            mPlanningEnd.AddMinutes(5F);
        }

        public void StartCoroutinePlanningInfo()
        {
            StartCoroutine(GetPlanning());
        }

        #region Use this update with another db than Zoho
        private IEnumerator UpdateBattery()
        {
            while (true)
            {
                //Update DB with battery Level
                mURIBattery = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Batterie=" + (Buddy.Sensors.Battery.Level*100).ToString();
                //Debug.LogWarning("lURI Battery : " + mURIBattery);
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
                        //Debug.LogWarning("Post done battery : " + lUpdateBattery.downloadHandler.text + " " + lUpdateBattery.downloadHandler.isDone);
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
                    //Debug.LogWarning("mURIPing : " + mURIPing);
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
                            //Debug.LogWarning("Post done position : " + lUpdatePosition.downloadHandler.text + " " + lUpdatePosition.downloadHandler.isDone);
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
        #endregion

        /// <summary>
        /// Get all the tablet ID peered with the Robot UID
        /// </summary>
        /// <param name="iRobotUID"></param>
        /// <returns>Fill a list of all UID from all the tablet peered to the robot, also fill a list of ID from all those tablet</returns>
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
                                Debug.LogError("iddevicerobot " + lIdDeviceRobot);
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
                                            Debug.LogError("entrydeviceid: " + entry.DeviceId);
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
                    ListUIDTablet = new List<string>();
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
                                            Debug.LogError("Tablet connected to the robot " + device.Nom);
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
                                    Debug.LogError("get tabletuid Error parsing device request answer : " + e.Message);
                                }
                            }
                        }
                    }
                    StartCoroutine(GetStudentInfo(IdDevice));
                    
                }
                yield return new WaitForSeconds(300F);

            }
            //StartCoroutine(GetPlanning());
        }

        /// <summary>
        /// Retrieve all the informations of students who are peered with the robot
        /// </summary>
        /// <param name="iListIdTablet"></param>
        /// <returns>return a list of </returns>
        private IEnumerator GetStudentInfo(List<int> iListIdTablet)
        {
            List<int> idStudents = new List<int>();
            ListUserStudent = new List<User>();
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
                        DeviceUserCollection devicesUser = Utils.UnserializeJSON<DeviceUserCollection>(res); 
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
                        Debug.LogError("getstudentinfo Error parsing device request answer1 : " + e.Message);
                    }
                }



            }
            if (idStudents.Count == 0)
                Debug.Log("There is no user for this device id");
            
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
                                UserStudent = new User();
                                UserStudent.Nom = user.Nom;
                                UserStudent.Prenom = user.Prenom;
                                //TODO : CHANGE THAT TO HAVE ONLY THE CLASS | FORMAT IN THE DB FOR NOW : 15 - CM1
                                UserStudent.Organisme = user.Organisme;
                                if (string.IsNullOrEmpty(user.Prenom) || user.Prenom =="")
                                    UserStudent.Prenom = " ";
                                ListUserStudent.Add(UserStudent);
                                
                                //Debug.LogError("USER NAME : " + UserStudent.Nom + " Class : " + UserStudent.Organisme + " USER PRENOM : " + UserStudent.Prenom);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("getstudentinfo Error parsing user info request answer2 : " + e.Message);
                        }
                    }
                }
            }
            StartCoroutine(GetPlanning());
            if (!string.IsNullOrEmpty(ListUserStudent[0].Nom) 
                && !string.IsNullOrEmpty(ListUserStudent[0].Prenom) 
                && !string.IsNullOrEmpty(ListUserStudent[0].Organisme) 
                && !string.IsNullOrEmpty(ListUIDTablet[0]))
            {
                InfoRequestedDone = true;
            }
        }

        private IEnumerator GetPlanning()
        {
            ListPlanning = new List<Planning>();
            
            string request = GET_PLANNING;
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
                            Planning = new Planning();
                            Planning.Date_Fin = planning.Date_Fin;
                            Planning.DeviceId = planning.DeviceId;
                            Planning.Eleve = planning.Eleve;
                            Planning.Device = planning.Device;
                            Planning.Date_Debut = planning.Date_Debut;
                            Planning.idPlanning = planning.idPlanning;
                            Planning.ID = planning.ID;
                            Planning.UserId = planning.UserId;
                            Planning.Prof = planning.Prof;
                            ListPlanning.Add(Planning);
                        }
                        mPlanningNextCourse = GetPlanningFromDB(ListPlanning);
                        Debug.LogError("<color=blue> Date Debut : " + mPlanningNextCourse.Date_Debut + " Date fin : " + mPlanningNextCourse.Date_Fin + "</color>");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing user info request answer : " + e.Message);
                    }
                }
            }

        }

        private Planning GetPlanningFromDB(List<Planning> iList)
        {
            //Debug.LogError("<color=red> GET PLANNING FROM DB " + ListUserStudent[0].Nom + "  </color>");
            foreach (Planning lPlanning in iList)
            {
                Debug.LogError("<color=red> GET PLANNING FROM DB : " + ListUserStudent.Count + " USER :  " + lPlanning.Eleve + " LISTUSER STUDENT :  " + ListUserStudent[0].Nom + "</color>");
                if (lPlanning.Eleve.Contains(ListUserStudent[0].Nom))
                {
                    string[] lSplitDate = lPlanning.Date_Debut.Split('-');
                    string lDateNow = DateTime.Now.ToString();
                    string[] lSpliteDateNow = lDateNow.Split('/');
                    Debug.LogError("<color=red> lsplitDate : " + lSplitDate[0] + " lSpliteDateNow[0] : " + lSpliteDateNow[0] + " lSpliteDateNow[1] : " + lSplitDate[1] + " lSpliteDateNow[1] : " + lSpliteDateNow[1] + "</color>");

                    if ((lSplitDate[0] == lSpliteDateNow[0]) && (lSplitDate[1] == lSpliteDateNow[1]))
                    {
                        Debug.LogError("<color=red> Date Debut : " + lPlanning.Date_Debut + " Date fin : " + lPlanning.Date_Fin + "</color>");
                        mPlanning = true;
                        return lPlanning;
                    }
                }
            }
            return new Planning();
        }

        private void CheckStartPlanning()
        {
            if (mPlanning && !CanStartCourse)
            {
                Debug.LogError("<color=red> ARRAY  : " + mPlanningNextCourse.Date_Debut + "</color>");
                if (!string.IsNullOrEmpty(mPlanningNextCourse.Date_Debut) && !string.IsNullOrEmpty(mPlanningNextCourse.Date_Fin))
                {
                    DateTime LDateNow = DateTime.Now;
                    DateTime lPlanningStart = DateTime.Parse(mPlanningNextCourse.Date_Debut.Replace("-", "/"));
                    TimeSpan lSpan = lPlanningStart.Subtract(LDateNow);
                    Debug.LogError("<color=red> START COURSE IN : " + lSpan.TotalMinutes + "</color>");

                    if (lSpan.TotalMinutes < 0F)
                    {
                        Debug.LogError("<color=blue> START COURSE </color>");
                        mPlanningEnd = DateTime.Parse(mPlanningNextCourse.Date_Fin.Replace("-", "/"));
                        CanStartCourse = true;
                    }
                }
                else
                {
                    Debug.LogError("Date_Debut or Date_Fin are null or empty");
                }

            }
        }
    }

}
