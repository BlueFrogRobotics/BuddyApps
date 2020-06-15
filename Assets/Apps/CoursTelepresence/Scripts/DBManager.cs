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
        private const string GET_USER_TABLET = "https://creator.zoho.eu/api/json/flotte/view/all_liaison_device_user?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true&criteria=User.idUser==";
        private const string GET_ALL_LIAISON = "https://creator.zoho.eu/api/json/flotte/view/all_liaison_device_user?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true&criteria=Device.Uid==";
        private const string TABLET_TYPE_DEVICE = "Tablette";


        private DeviceUserLiaison mDeviceUserLiaison;
        private List<DeviceUserLiaison> mDeviceUserLiaisonList;
        private List<DeviceUserLiaison> mListTabletUser;
        private bool mStartUpdate;

        //private const string GET_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        //private const string GET_DEVICE_USERS_URL = "https://creator.zoho.eu/api/json/flotte/view/Device_user_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        //private const string GET_TYPE_DEVICE_URL = "https://creator.zoho.eu/api/json/flotte/view/Type_device_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        //private const string UPDATE_INFO = "https://creator.zoho.eu/api/bluefrogrobotics/json/flotte/form/Device/record/update?authtoken=" + TOKEN + "&scope=creatorapi&criteria=Uid=={";
        //private const string GET_USER = "https://creator.zoho.eu/api/json/flotte/view/User_Report?authtoken="+ TOKEN +"&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        //private const string GET_INFO_STUDENTS = "https://creator.zoho.eu/api/json/flotte/view/User_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";
        private const string GET_PLANNING = "https://creator.zoho.eu/api/json/flotte/view/Planning_Report?authtoken=" + TOKEN + "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true";

        //private const string TABLET_TYPE_DEVICE = "Tablette";

        public List<User> ListUserStudent { get; private set; }
        public User UserStudent { get; private set; }

        //private string mURIBattery;
        //private string mURIPing;
        //private string mURIPosition;
        //private WWWForm mEmptyForm;
        public bool DBConnected { get; private set; }
        public List<string> ListUIDTablet { get; private set; }
        public bool Peering { get; private set; }
        public Planning Planning { get; private set; }
        public bool InfoRequestedDone { get; private set; }
        private List<Planning> ListPlanning;
        private Planning mPlanningNextCourse;
        //private Planning mPlanningCheck;


        //private float mTimer;
        private bool mPlanning;
        public bool CanStartCourse { get; private set; }
        public bool CanEndCourse { get; private set; }

        //private int mNbIteration;
        private DateTime mDateNow;
        private DateTime mPlanningEnd;
        private TimeSpan mSpan;
        //private bool mCheckDBEndCall;

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
            mStartUpdate = false;
            //mPlanningCheck = new Planning();
            //mTimer = 0F;
            mPlanning = false;
            //mCheckDBEndCall = false;
            Peering = false;
            InfoRequestedDone = false;
            //mNbIteration = 0;
            DBConnected = false;
            CanStartCourse = false;
            CanEndCourse = false;
            mDeviceUserLiaison = new DeviceUserLiaison();
            mDeviceUserLiaisonList = new List<DeviceUserLiaison>();
            mListTabletUser = new List<DeviceUserLiaison>();
            StartCoroutine(GetUserIdFromUID(Buddy.Platform.RobotUID));
            //StartCoroutine(GetUserIdFromUID("AFD3183637443D5E5A95"));

            //StartCoroutine(UpdatePingAndPosition());
            //StartCoroutine(UpdateBattery());
        }

        private void Update()
        {
            if(mStartUpdate)
            {
                if (CanStartCourse && !CanEndCourse)
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
                            CanStartCourse = false;
                            mPlanning = false;
                            mStartUpdate = false;
                        }
                    }
                }
                if (!CanStartCourse)
                {
                    CheckStartPlanning();
                }
            }

        }

        private IEnumerator GetUserIdFromUID(string iRobotUID)
        {
            string lRequest = GET_ALL_LIAISON + '"' + iRobotUID + '"';
            Debug.LogError("Request : " + lRequest);
            using (UnityWebRequest lRequestDevice = UnityWebRequest.Get(lRequest))
            {
                yield return lRequestDevice.SendWebRequest();
                if (lRequestDevice.isHttpError || lRequestDevice.isNetworkError)
                {
                    Debug.LogError("Request from GetUserIdFromUID error " + lRequestDevice.error + " " + lRequestDevice.downloadHandler.text);
                }
                else
                {
                    string lRes = lRequestDevice.downloadHandler.text;
                    Debug.LogError("Result from GetUserIdFromUID with Robot UID : " + lRes);

                    try
                    {
                        DeviceUserLiaisonList devices = Utils.UnserializeJSON<DeviceUserLiaisonList>(lRes);

                        if (devices != null)
                        {
                            DBConnected = true;
                            for(int i = 0; i < devices.Device_user.Length; ++i)
                            {
                                mDeviceUserLiaison = devices.Device_user[i];
                                mDeviceUserLiaisonList.Add(mDeviceUserLiaison);
                            }
                            

                            if (mDeviceUserLiaisonList.Count > 0)
                            {
                                Debug.LogError("<color=red>mdeviceuserliaison Count sup a 0 : " + mDeviceUserLiaisonList.Count + "</color>");
                                StartCoroutine(GetInfoForUsers(mDeviceUserLiaisonList));

                            }
                            //Debug.LogError("<color=red>mdeviceuserliaison :  " + mDeviceUserLiaisonList[0].UserIdUser + " nom : " + mDeviceUserLiaisonList[0].UserNom + " prenom : " + mDeviceUserLiaisonList[0].UserPrenom + "</color>");
                        }
                        else
                        {
                            Debug.LogError("List of user is null, check the class DeviceData. The robot UID used is : " + iRobotUID);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing robot device request answer : " + e.Message);
                    }

                }

            }
        }

        private IEnumerator GetInfoForUsers(List<DeviceUserLiaison> iListDeviceUserLiaison)
        {
            ListUIDTablet = new List<string>();
            ListUserStudent = new List<User>();
            string lRequest = GET_USER_TABLET;

            if (iListDeviceUserLiaison.Count == 1)
            {
                lRequest += iListDeviceUserLiaison[0].UserIdUser.ToString();

                Debug.LogError("<color=green>REQUEST DEUXIEME : " + lRequest + "</color>");
            }
            else
            {
                lRequest += iListDeviceUserLiaison[0].UserIdUser.ToString();
                for (int i = 1; i < iListDeviceUserLiaison.Count; ++i)
                {
                    if (iListDeviceUserLiaison[i].UserIdUser.HasValue)
                    {
                        lRequest += "||User.idUser==" + iListDeviceUserLiaison[i].UserIdUser.ToString();
                    }
                }
                //Debug.LogError("<color=blue>REQUEST DEUXIEME : " + lRequest + "</color>");

            }

            using (UnityWebRequest lRequestDevice = UnityWebRequest.Get(lRequest))
            {
                yield return lRequestDevice.SendWebRequest();

                if (lRequestDevice.isHttpError || lRequestDevice.isNetworkError)
                {
                    Debug.LogError("Request from GetInfoForUsers error " + lRequestDevice.error + " " + lRequestDevice.downloadHandler.text);
                }
                else
                {
                    string lRes = lRequestDevice.downloadHandler.text;
                    Debug.LogError("Result from GetInfoForUsers : " + lRes);

                    try
                    {
                        DeviceUserLiaisonList devices = Utils.UnserializeJSON<DeviceUserLiaisonList>(lRes);

                        if (devices != null)
                        {
                            foreach (DeviceUserLiaison lLiaison in devices.Device_user)
                            {
                                Debug.LogError("<color=red> foreach lliaison " + lLiaison.DeviceType_device + " </color>");
                                if (lLiaison.DeviceType_device.Contains(TABLET_TYPE_DEVICE))
                                {
                                    Debug.LogError("<color=red> lliaison typedevice  : " + lLiaison.DeviceType_device + " lliaison " + lLiaison.UserNom + " " + lLiaison.UserPrenom +  "</color>");
                                    mListTabletUser.Add(lLiaison);
                                }
                            }

                            foreach (DeviceUserLiaison lDeviceUserLiaison in mListTabletUser)
                            {
                                UserStudent = new User();
                                UserStudent.Nom = lDeviceUserLiaison.UserNom;
                                UserStudent.Prenom = lDeviceUserLiaison.UserPrenom;
                                string[] lOrgaSplit = lDeviceUserLiaison.UserOrganisme.Split('-');
                                UserStudent.Organisme = lOrgaSplit[1].Trim();
                                UserStudent.Planning = lDeviceUserLiaison.PlanningidPlanning;
                                ListUserStudent.Add(UserStudent);
                                ListUIDTablet.Add(lDeviceUserLiaison.DeviceUid);
                            }

                            if (mListTabletUser.Count > 0 && ListUserStudent.Count > 0)
                            {
                                Peering = true;
                                if (!string.IsNullOrEmpty(ListUserStudent[0].Nom)
                                    && !string.IsNullOrEmpty(ListUserStudent[0].Prenom)
                                    && !string.IsNullOrEmpty(ListUserStudent[0].Organisme)
                                    && !string.IsNullOrEmpty(ListUIDTablet[0]))
                                {
                                    InfoRequestedDone = true;
                                }
                            }

                            Debug.LogError("<color=red> lliaison typedevice  : " + mListTabletUser[0].DeviceType_device + " lliaison " + mListTabletUser[0].UserNom + " " + mListTabletUser[0].UserPrenom + "</color>");

                            //Debug.LogError("<color=red>mdeviceuserliaison :  " + mDeviceUserLiaisonList[0].UserIdUser + " nom : " + mDeviceUserLiaisonList[0].UserNom + " prenom : " + mDeviceUserLiaisonList[0].UserPrenom + "</color>");
                        }
                        else
                        {
                            Debug.LogError("No devices found in the list iListDeviceUserLiaison");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing tablet device request answer : " + e.Message);
                    }
                }
            }
        }

        public void FillPlanningStart(int iIndexChosen)
        {

            string[] lSplitDate = mListTabletUser[iIndexChosen].PlanningidPlanning.Split('&');
            string[] lSplitDateDayMonth = lSplitDate[1].Split('-');
            //lSplitDate[1] start planning lSplitDate[2] end planning
            string lDateNow = DateTime.Now.ToString();
            string[] lSpliteDateNow = lDateNow.Split('/');
            //Debug.LogError("<color=red> lSplitDateDayMonth day : " + lSplitDateDayMonth[0] + " lSplitDateDayMonth month : " + lSplitDateDayMonth[1] + " DATE NOW  : " + lSpliteDateNow[0] + " && " + lSpliteDateNow[1] +"</color>");
            Debug.LogError("<color=red> lSplitDate start : " + lSplitDate[1] + " lSplitDate end : " + lSplitDate[2] + " DATE NOW  : " + lSpliteDateNow[0] + " && " + lSpliteDateNow[1] + "</color>");
            if (Application.systemLanguage == SystemLanguage.French)
            {
                if ((lSplitDateDayMonth[0] == lSpliteDateNow[0]) && (lSplitDateDayMonth[1] == lSpliteDateNow[1]))
                {
                    Planning mPlanningUser = new Planning();
                    mPlanningUser.Date_Debut = lSplitDate[1];
                    mPlanningUser.Date_Fin = lSplitDate[2];
                    Debug.LogError("<color=red> Date Debut : " + mPlanningUser.Date_Debut + " Date fin : " + mPlanningUser.Date_Fin + "</color>");
                    mPlanningNextCourse = mPlanningUser;
                    mPlanning = true;
                }
            }
            else if (Application.systemLanguage == SystemLanguage.English)
            {
                if ((lSplitDateDayMonth[0] == lSpliteDateNow[1]) && (lSplitDateDayMonth[1] == lSpliteDateNow[0]))
                {
                    Planning mPlanningUser = new Planning();
                    string[] lSplitDateEnd = lSplitDate[2].Split('-');
                    //string lTemp = lSplitDateEnd[0];
                    //lSplitDateEnd[0] = lSplitDateEnd[1];
                    //lSplitDateEnd[1] = lTemp;
                    string lDateEndReformed = lSplitDateEnd[1] + "-" + lSplitDateEnd[0] + "-" + lSplitDateEnd[2];
                    string lDateStartReformed = lSplitDate[1] + "-" + lSplitDate[0] + "-" + lSplitDate[2];
                    mPlanningUser.Date_Fin = lDateEndReformed;
                    mPlanningUser.Date_Debut = lDateStartReformed;
                    Debug.LogError("<color=red> Date Debut : " + mPlanningUser.Date_Debut + " Date fin : " + mPlanningUser.Date_Fin + "</color>");
                    mPlanningNextCourse = mPlanningUser;
                    mPlanning = true;
                }
            }

            //mStartUpdate = true;

        }



            //private IEnumerator GetUserId(string iRobotUID)
            //{
            //    string lRequest = GET_ALL_LIAISON + iRobotUID;
            //    using (UnityWebRequest lRequestDevice = UnityWebRequest.Get(lRequest))
            //    {
            //        yield return lRequestDevice.SendWebRequest();

            //        if (lRequestDevice.isHttpError || lRequestDevice.isNetworkError)
            //        {
            //            Debug.LogError("Request error " + lRequestDevice.error + " " + lRequestDevice.downloadHandler.text);
            //        }
            //        else
            //        {
            //            string lRes = lRequestDevice.downloadHandler.text;

            //            Debug.LogError("Result get robot device : " + lRes);

            //            try
            //            {
            //                DeviceUserLiaisonList devices = Utils.UnserializeJSON<DeviceUserLiaisonList>(lRes);

            //                if (devices != null)
            //                {
            //                    DBConnected = true;
            //                    mDeviceUserLiaison = devices.Device_user[0];
            //                    mDeviceUserLiaisonList.Add(mDeviceUserLiaison);
            //                    if (mDeviceUserLiaisonList.Count > 0)
            //                    {
            //                        Debug.LogError("<color=red>mdeviceuserliaison user id user :  " + mDeviceUserLiaisonList[0].UserIdUser + "</color>");
            //                        StartCoroutine(GetDeviceInfos(mDeviceUserLiaisonList[0].UserIdUser));

            //                    }
            //                    //Debug.LogError("<color=red>mdeviceuserliaison :  " + mDeviceUserLiaisonList[0].UserIdUser + " nom : " + mDeviceUserLiaisonList[0].UserNom + " prenom : " + mDeviceUserLiaisonList[0].UserPrenom + "</color>");
            //                }
            //                else
            //                {
            //                    Debug.LogError("No device found with Uid " + iRobotUID);
            //                }
            //            }
            //            catch (Exception e)
            //            {
            //                Debug.LogError("Error parsing robot device request answer : " + e.Message);
            //            }
            //        }
            //    }


            //}

            private IEnumerator GetDeviceInfos(int iIdUser)
        {
            ListUIDTablet = new List<string>();
            ListUserStudent = new List<User>();
            string lRequest = GET_USER_TABLET + iIdUser.ToString();
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
                    Debug.LogError("Result get user device : " + lRes);

                    try
                    {
                        DeviceUserLiaisonList devices = Utils.UnserializeJSON<DeviceUserLiaisonList>(lRes);

                        if (devices != null)
                        {
                            foreach (DeviceUserLiaison lLiaison in devices.Device_user)
                            {
                                Debug.LogError("<color=red> foreach lliaison " + lLiaison.DeviceType_device + " </color>");
                                if (lLiaison.DeviceType_device.Contains(TABLET_TYPE_DEVICE))
                                {
                                    //Debug.LogError("<color=red> lliaison typedevice  : " + lLiaison.DeviceType_device + " lliaison " + lLiaison.UserNom + " " + lLiaison.UserPrenom +  "</color>");
                                    mListTabletUser.Add(lLiaison);
                                }
                            }

                            foreach (DeviceUserLiaison lDeviceUserLiaison in mListTabletUser)
                            {
                                UserStudent = new User();
                                UserStudent.Nom = lDeviceUserLiaison.UserNom;
                                UserStudent.Prenom = lDeviceUserLiaison.UserPrenom;
                                UserStudent.Organisme = lDeviceUserLiaison.UserOrganisme;
                                ListUserStudent.Add(UserStudent);

                                ListUIDTablet.Add(lDeviceUserLiaison.DeviceUid);
                            }

                            if (mListTabletUser.Count > 0 && ListUserStudent.Count > 0)
                            {
                                Peering = true;
                                if (!string.IsNullOrEmpty(ListUserStudent[0].Nom)
                                    && !string.IsNullOrEmpty(ListUserStudent[0].Prenom)
                                    && !string.IsNullOrEmpty(ListUserStudent[0].Organisme)
                                    && !string.IsNullOrEmpty(ListUIDTablet[0]))
                                {
                                    InfoRequestedDone = true;
                                }
                                StartCoroutine(GetPlanning());
                            }

                            Debug.LogError("<color=red> lliaison typedevice  : " + mListTabletUser[0].DeviceType_device + " lliaison " + mListTabletUser[0].UserNom + " " + mListTabletUser[0].UserPrenom + "</color>");

                            //Debug.LogError("<color=red>mdeviceuserliaison :  " + mDeviceUserLiaisonList[0].UserIdUser + " nom : " + mDeviceUserLiaisonList[0].UserNom + " prenom : " + mDeviceUserLiaisonList[0].UserPrenom + "</color>");
                        }
                        else
                        {
                            Debug.LogError("No tablet found with id user : " + iIdUser);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing tablet device request answer : " + e.Message);
                    }
                }
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
                            //Planning = planning;
                            Planning.Date_Fin = planning.Date_Fin;
                            Planning.DeviceId = planning.DeviceId;
                            Planning.Eleve = planning.Eleve;
                            Planning.Device = planning.Device;
                            Planning.Date_Debut = planning.Date_Debut;
                            Planning.idPlanning = planning.idPlanning;
                            Planning.ID = planning.ID;
                            Planning.EleveIdUser = planning.EleveIdUser;
                            Planning.DeviceUID = planning.DeviceUID;
                            Planning.Prof = planning.Prof;
                            Debug.LogError("<color=green>FOREACH PLANNING Date Debut : " + Planning.Date_Debut + " Date fin : " + Planning.Date_Fin + "</color>");
                            if (planning.DeviceId.HasValue && planning.idPlanning.HasValue && planning.EleveIdUser.HasValue)
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

                    if (Application.systemLanguage == SystemLanguage.French)
                    {
                        if ((lSplitDate[0] == lSpliteDateNow[0]) && (lSplitDate[1] == lSpliteDateNow[1]))
                        {
                            Debug.LogError("<color=red> Date Debut : " + lPlanning.Date_Debut + " Date fin : " + lPlanning.Date_Fin + "</color>");
                            mPlanning = true;
                            return lPlanning;
                        }
                    }
                    else if (Application.systemLanguage == SystemLanguage.English)
                    {
                        if ((lSplitDate[0] == lSpliteDateNow[1]) && (lSplitDate[1] == lSpliteDateNow[0]))
                        {
                            string[] lSplitDateEnd = lPlanning.Date_Fin.Split('-');
                            //string lTemp = lSplitDateEnd[0];
                            //lSplitDateEnd[0] = lSplitDateEnd[1];
                            //lSplitDateEnd[1] = lTemp;
                            string lDateEndReformed = lSplitDateEnd[1] + "-" + lSplitDateEnd[0] + "-" + lSplitDateEnd[2];
                            string lDateStartReformed = lSplitDate[1] + "-" + lSplitDate[0] + "-" + lSplitDate[2];
                            lPlanning.Date_Fin = lDateEndReformed;
                            lPlanning.Date_Debut = lDateStartReformed;
                            Debug.LogError("<color=red> Date Debut : " + lPlanning.Date_Debut + " Date fin : " + lPlanning.Date_Fin + "</color>");
                            mPlanning = true;
                            return lPlanning;
                        }
                    }

                }
                else
                    Debug.LogError("<color=red> There is no student for any planning </color>");
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
                    DateTime lDateNow = DateTime.Now;
                    DateTime lPlanningStart = DateTime.Parse(mPlanningNextCourse.Date_Debut.Replace("-", "/"));
                    TimeSpan lSpan = lPlanningStart.Subtract(lDateNow);
                    Debug.LogError("<color=red> START COURSE IN : " + lSpan.TotalMinutes + "</color>");
                    Debug.LogError("planning start: " + lPlanningStart + "date now" + lDateNow);

                    if (lSpan.TotalMinutes < 0F)
                    {
                        Debug.LogError("<color=blue> START COURSE </color>");
                        mPlanningEnd = DateTime.Parse(mPlanningNextCourse.Date_Fin.Replace("-", "/"));
                        CanStartCourse = true;
                        Debug.LogError("planning end: " + mPlanningEnd);
                    }
                }
                else
                {
                    Debug.LogError("Date_Debut or Date_Fin are null or empty");
                }

            }
        }




        //public void AddFiveMinutes()
        //{
        //    mPlanningEnd.AddMinutes(5F);
        //}

        //public void StartCoroutinePlanningInfo()
        //{
        //    StartCoroutine(GetPlanning());
        //}

        //#region Use this update with another db than Zoho
        //private IEnumerator UpdateBattery()
        //{
        //    while (true)
        //    {
        //        //Update DB with battery Level
        //        mURIBattery = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Batterie=" + (Buddy.Sensors.Battery.Level*100).ToString();
        //        //Debug.LogWarning("lURI Battery : " + mURIBattery);
        //        using (UnityWebRequest lUpdateBattery = UnityWebRequest.Post(mURIBattery, mEmptyForm))
        //        {
        //            lUpdateBattery.chunkedTransfer = false;
        //            yield return lUpdateBattery.SendWebRequest();
        //            if (lUpdateBattery.isHttpError || lUpdateBattery.isNetworkError)
        //            {
        //                Debug.LogError("Request error battery : " + lUpdateBattery.error + " " + lUpdateBattery.downloadHandler.text);
        //            }
        //            else
        //            {
        //                //Debug.LogWarning("Post done battery : " + lUpdateBattery.downloadHandler.text + " " + lUpdateBattery.downloadHandler.isDone);
        //            }
        //        }
        //        yield return new WaitForSeconds(5F);
        //    }

        //}

        //private IEnumerator UpdatePingAndPosition()
        //{
        //    while (true)
        //    {
        //        mNbIteration++;

        //        //if(mNbIteration % 2 == 0)
        //        //{
        //            //Update GPS Position TODO : search for a method to have the GPS position, waiting for thierry's answer
        //            mURIPosition = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Position_GPS=" + "";
        //            //Debug.LogWarning("mURIPing : " + mURIPing);
        //            using (UnityWebRequest lUpdatePosition = UnityWebRequest.Post(mURIPosition, mEmptyForm))
        //            {
        //                lUpdatePosition.chunkedTransfer = false;
        //                yield return lUpdatePosition.SendWebRequest();
        //                if (lUpdatePosition.isHttpError || lUpdatePosition.isNetworkError)
        //                {
        //                    Debug.LogError("Request error position : " + lUpdatePosition.error + " " + lUpdatePosition.downloadHandler.text);
        //                }
        //                else
        //                {
        //                    //Debug.LogWarning("Post done position : " + lUpdatePosition.downloadHandler.text + " " + lUpdatePosition.downloadHandler.isDone);
        //                }
        //            }
        //        //}
        //        //Update DB with ping
        //        mURIPing = UPDATE_INFO + Buddy.Platform.RobotUID + "}" + "&Qualite_signal=" + CoursTelepresenceData.Instance.Ping;
        //        Debug.LogWarning("mURIPing : " + mURIPing);
        //        using (UnityWebRequest lUpdatePing = UnityWebRequest.Post(mURIPing, mEmptyForm))
        //        {
        //            lUpdatePing.chunkedTransfer = false;
        //            yield return lUpdatePing.SendWebRequest();
        //            if (lUpdatePing.isHttpError || lUpdatePing.isNetworkError)
        //            {
        //                Debug.LogError("Request error ping : " + lUpdatePing.error + " " + lUpdatePing.downloadHandler.text);
        //            }
        //            else
        //            {
        //                Debug.LogWarning("Post done ping : "  + lUpdatePing.downloadHandler.text + " " + lUpdatePing.downloadHandler.isDone);
        //            }
        //        }
        //        yield return new WaitForSeconds(5F);
        //    }
        //}
        //#endregion

        ///// <summary>
        ///// Get all the tablet ID peered with the Robot UID
        ///// </summary>
        ///// <param name="iRobotUID"></param>
        ///// <returns>Fill a list of all UID from all the tablet peered to the robot, also fill a list of ID from all those tablet</returns>
        //private IEnumerator GetTabletUID(string iRobotUID)
        //{
        //    while(true && !Peering)
        //    {
        //        int lIdDeviceRobot = -1;
        //        string lRequest = GET_DEVICE_URL + "&Uid=" + iRobotUID;
        //        using (UnityWebRequest lRequestDevice = UnityWebRequest.Get(lRequest))
        //        {
        //            yield return lRequestDevice.SendWebRequest();

        //            if (lRequestDevice.isHttpError || lRequestDevice.isNetworkError)
        //            {
        //                Debug.LogError("Request error " + lRequestDevice.error + " " + lRequestDevice.downloadHandler.text);
        //            }
        //            else
        //            {
        //                string lRes = lRequestDevice.downloadHandler.text;
        //                DBConnected = true;
        //                Debug.Log("Result get robot device : " + lRes);

        //                try
        //                {
        //                    DevicesCollection devices = Utils.UnserializeJSON<DevicesCollection>(lRes);

        //                    if (devices != null
        //                        && devices.Device != null
        //                        && devices.Device.Length > 0)
        //                    {

        //                        lIdDeviceRobot = devices.Device[0].idDevice;
        //                        Debug.LogError("iddevicerobot " + lIdDeviceRobot);
        //                    }
        //                    else
        //                    {
        //                        Debug.LogError("No device found with Uid " + iRobotUID);
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    Debug.LogError("Error parsing robot device request answer : " + e.Message);
        //                }
        //            }
        //        }

        //        if (lIdDeviceRobot != -1)
        //        {
        //            List<int> lIdOtherDevices = new List<int>();
        //            lRequest = GET_DEVICE_USERS_URL;
        //            using (UnityWebRequest lRequestDeviceUser = UnityWebRequest.Get(lRequest))
        //            {
        //                yield return lRequestDeviceUser.SendWebRequest();
        //                if (lRequestDeviceUser.isHttpError || lRequestDeviceUser.isNetworkError)
        //                {
        //                    Debug.Log("Request error " + lRequestDeviceUser.error + " " + lRequestDeviceUser.downloadHandler.text);
        //                }
        //                else
        //                {
        //                    string lRes = lRequestDeviceUser.downloadHandler.text;
        //                    Debug.Log("Result get robot users : " + lRes);
        //                    try
        //                    {
        //                        DeviceUserCollection lDeviceUsers = Utils.UnserializeJSON<DeviceUserCollection>(lRes);//JsonUtility.FromJson<DeviceUserCollection>(res);

        //                        if (lDeviceUsers != null
        //                            && lDeviceUsers.Device_user != null)
        //                        {
        //                            List<int> lIdUserRobotList = new List<int>();
        //                            foreach (DeviceUser entry in lDeviceUsers.Device_user)
        //                            {
        //                                if (entry.DeviceId == lIdDeviceRobot)
        //                                    lIdUserRobotList.Add(entry.UserId);
        //                            }
        //                            if (lIdUserRobotList.Count > 0)
        //                            {
        //                                foreach (DeviceUser entry in lDeviceUsers.Device_user)
        //                                {
        //                                    Debug.LogError("entrydeviceid: " + entry.DeviceId);
        //                                    if (lIdUserRobotList.Contains(entry.UserId)
        //                                        && entry.DeviceId != lIdDeviceRobot)
        //                                        lIdOtherDevices.Add(entry.DeviceId);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                Debug.Log("No device user found for Device " + lIdDeviceRobot);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Debug.Log("No device user found");
        //                        }
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        Debug.LogError("Error parsing device users request answer : " + e.Message);
        //                    }
        //                }
        //            }
        //            if (lIdOtherDevices.Count == 0)
        //                Debug.Log("There is no device connected to the robot");

        //            List<int> IdDevice = new List<int>();
        //            ListUIDTablet = new List<string>();
        //            foreach (int idDevice in lIdOtherDevices)
        //            {
        //                lRequest = GET_DEVICE_URL + "&idDevice=" + idDevice + "&Type_device=" + TABLET_TYPE_DEVICE;
        //                Debug.Log("Sending device request : " + lRequest);
        //                using (UnityWebRequest lRequestTabletUID = UnityWebRequest.Get(lRequest))
        //                {
        //                    yield return lRequestTabletUID.SendWebRequest();
        //                    if (lRequestTabletUID.isHttpError || lRequestTabletUID.isNetworkError)
        //                    {
        //                        Debug.Log("Request error " + lRequestTabletUID.error + " " + lRequestTabletUID.downloadHandler.text);
        //                    }
        //                    else 
        //                    {
        //                        string lRes = lRequestTabletUID.downloadHandler.text;
        //                        Debug.Log("Result get devices : " + lRes);
        //                        try
        //                        {
        //                            DevicesCollection devices = Utils.UnserializeJSON<DevicesCollection>(lRes);
        //                            if (devices != null
        //                                && devices.Device != null
        //                                && devices.Device.Length > 0)
        //                            {
        //                                foreach (DeviceData device in devices.Device)
        //                                {
        //                                    Debug.LogError("Tablet connected to the robot " + device.Nom);
        //                                    ListUIDTablet.Add(device.Uid);
        //                                    IdDevice.Add(device.idDevice);
        //                                }
        //                                if (ListUIDTablet.Count > 0)
        //                                    Peering = true;
        //                            }
        //                            else
        //                            {
        //                                Debug.Log("No device found with idDevice " + idDevice + " & Type_device " + TABLET_TYPE_DEVICE);
        //                            }
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            Debug.LogError("get tabletuid Error parsing device request answer : " + e.Message);
        //                        }
        //                    }
        //                }
        //            }
        //            StartCoroutine(GetStudentInfo(IdDevice));

        //        }
        //        yield return new WaitForSeconds(300F);

        //    }
        //    //StartCoroutine(GetPlanning());
        //}

        ///// <summary>
        ///// Retrieve all the informations of students who are peered with the robot
        ///// </summary>
        ///// <param name="iListIdTablet"></param>
        ///// <returns>return a list of user</returns>
        //private IEnumerator GetStudentInfo(List<int> iListIdTablet)
        //{
        //    List<int> idStudents = new List<int>();
        //    ListUserStudent = new List<User>();
        //    string request = GET_DEVICE_USERS_URL;
        //    using (UnityWebRequest www = UnityWebRequest.Get(request))
        //    {

        //        yield return www.SendWebRequest();
        //        if (www.isHttpError || www.isNetworkError)
        //        {
        //            Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
        //        }
        //        else
        //        {
        //            string res = www.downloadHandler.text;
        //            Debug.Log("Result get devices : " + res);
        //            try
        //            {
        //                DeviceUserCollection devicesUser = Utils.UnserializeJSON<DeviceUserCollection>(res); 
        //                if (devicesUser != null
        //                       && devicesUser.Device_user != null)
        //                {
        //                    foreach (DeviceUser device in devicesUser.Device_user)
        //                    {
        //                        for (int i = 0; i < iListIdTablet.Count; ++i)
        //                        {
        //                            if (device.DeviceId == iListIdTablet[i])
        //                                idStudents.Add(device.UserId);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.LogError("getstudentinfo Error parsing device request answer1 : " + e.Message);
        //            }
        //        }



        //    }
        //    if (idStudents.Count == 0)
        //        Debug.Log("There is no user for this device id");

        //    foreach (int lIdUser in idStudents)
        //    {
        //        Debug.Log("IDUSER : " + lIdUser);
        //        string requestInfo = GET_INFO_STUDENTS + "&idUser=" + lIdUser;
        //        using (UnityWebRequest www = UnityWebRequest.Get(requestInfo))
        //        {
        //            yield return www.SendWebRequest();
        //            if (www.isHttpError || www.isNetworkError)
        //            {
        //                Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
        //            }
        //            else
        //            {
        //                string res = www.downloadHandler.text;
        //                Debug.Log("Result get user info : " + res);
        //                try
        //                {
        //                    UserList userList = Utils.UnserializeJSON<UserList>(res);
        //                    foreach (User user in userList.User)
        //                    {
        //                        UserStudent = new User();
        //                        UserStudent.Nom = user.Nom;
        //                        UserStudent.Prenom = user.Prenom;
        //                        //TODO : CHANGE THAT TO HAVE ONLY THE CLASS | FORMAT IN THE DB FOR NOW : 15 - CM1
        //                        UserStudent.Organisme = user.Organisme;
        //                        if (string.IsNullOrEmpty(user.Prenom) || user.Prenom =="")
        //                            UserStudent.Prenom = " ";
        //                        ListUserStudent.Add(UserStudent);

        //                        //Debug.LogError("USER NAME : " + UserStudent.Nom + " Class : " + UserStudent.Organisme + " USER PRENOM : " + UserStudent.Prenom);
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    Debug.LogError("getstudentinfo Error parsing user info request answer2 : " + e.Message);
        //                }
        //            }
        //        }
        //    }
        //    StartCoroutine(GetPlanning());
        //    if (!string.IsNullOrEmpty(ListUserStudent[0].Nom) 
        //        && !string.IsNullOrEmpty(ListUserStudent[0].Prenom) 
        //        && !string.IsNullOrEmpty(ListUserStudent[0].Organisme) 
        //        && !string.IsNullOrEmpty(ListUIDTablet[0]))
        //    {
        //        InfoRequestedDone = true;
        //    }
        //}

        //private IEnumerator GetPlanning()
        //{
        //    ListPlanning = new List<Planning>();

        //    string request = GET_PLANNING;
        //    using (UnityWebRequest www = UnityWebRequest.Get(request))
        //    {
        //        yield return www.SendWebRequest();
        //        if (www.isHttpError || www.isNetworkError)
        //        {
        //            Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
        //        }
        //        else
        //        {
        //            string res = www.downloadHandler.text;
        //            Debug.Log("Result get user info : " + res);
        //            try
        //            {
        //                PlanningList planningList = Utils.UnserializeJSON<PlanningList>(res);
        //                foreach (Planning planning in planningList.Planning)
        //                {
        //                    Planning = new Planning();
        //                    Planning.Date_Fin = planning.Date_Fin;
        //                    Planning.DeviceId = planning.DeviceId;
        //                    Planning.Eleve = planning.Eleve;
        //                    Planning.Device = planning.Device;
        //                    Planning.Date_Debut = planning.Date_Debut;
        //                    Planning.idPlanning = planning.idPlanning;
        //                    Planning.ID = planning.ID;
        //                    Planning.EleveIdUser = planning.EleveIdUser;
        //                    Planning.DeviceUID = planning.DeviceUID;
        //                    Planning.Prof = planning.Prof;
        //                    ListPlanning.Add(Planning);
        //                }
        //                mPlanningNextCourse = GetPlanningFromDB(ListPlanning);
        //                Debug.LogError("<color=blue> Date Debut : " + mPlanningNextCourse.Date_Debut + " Date fin : " + mPlanningNextCourse.Date_Fin + "</color>");
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.LogError("Error parsing user info request answer : " + e.Message);
        //            }
        //        }
        //    }

        //}

        //private Planning GetPlanningFromDB(List<Planning> iList)
        //{
        //    //Debug.LogError("<color=red> GET PLANNING FROM DB " + ListUserStudent[0].Nom + "  </color>");
        //    foreach (Planning lPlanning in iList)
        //    {
        //        Debug.LogError("<color=red> GET PLANNING FROM DB : " + ListUserStudent.Count + " USER :  " + lPlanning.Eleve + " LISTUSER STUDENT :  " + ListUserStudent[0].Nom + "</color>");
        //        if (lPlanning.Eleve.Contains(ListUserStudent[0].Nom))
        //        {
        //            string[] lSplitDate = lPlanning.Date_Debut.Split('-');
        //            string lDateNow = DateTime.Now.ToString();
        //            string[] lSpliteDateNow = lDateNow.Split('/');
        //            Debug.LogError("<color=red> lsplitDate : " + lSplitDate[0] + " lSpliteDateNow[0] : " + lSpliteDateNow[0] + " lSpliteDateNow[1] : " + lSplitDate[1] + " lSpliteDateNow[1] : " + lSpliteDateNow[1] + "</color>");

        //            if(Application.systemLanguage == SystemLanguage.French)
        //            {
        //                if ((lSplitDate[0] == lSpliteDateNow[0]) && (lSplitDate[1] == lSpliteDateNow[1]))
        //                {
        //                    Debug.LogError("<color=red> Date Debut : " + lPlanning.Date_Debut + " Date fin : " + lPlanning.Date_Fin + "</color>");
        //                    mPlanning = true;
        //                    return lPlanning;
        //                }
        //            }
        //            else if(Application.systemLanguage == SystemLanguage.English)
        //            {
        //                if ((lSplitDate[0] == lSpliteDateNow[1]) && (lSplitDate[1] == lSpliteDateNow[0]))
        //                {
        //                    Debug.LogError("<color=red> Date Debut : " + lPlanning.Date_Debut + " Date fin : " + lPlanning.Date_Fin + "</color>");
        //                    mPlanning = true;
        //                    return lPlanning;
        //                }

        //            }

        //        }
        //    }
        //    return new Planning();
        //}

        //private void CheckStartPlanning()
        //{
        //    if (mPlanning && !CanStartCourse)
        //    {
        //        Debug.LogError("<color=red> ARRAY  : " + mPlanningNextCourse.Date_Debut + "</color>");
        //        if (!string.IsNullOrEmpty(mPlanningNextCourse.Date_Debut) && !string.IsNullOrEmpty(mPlanningNextCourse.Date_Fin))
        //        {
        //            DateTime LDateNow = DateTime.Now;
        //            DateTime lPlanningStart = DateTime.Parse(mPlanningNextCourse.Date_Debut.Replace("-", "/"));
        //            TimeSpan lSpan = lPlanningStart.Subtract(LDateNow);
        //            Debug.LogError("<color=red> START COURSE IN : " + lSpan.TotalMinutes + "</color>");

        //            if (lSpan.TotalMinutes < 0F)
        //            {
        //                Debug.LogError("<color=blue> START COURSE </color>");
        //                mPlanningEnd = DateTime.Parse(mPlanningNextCourse.Date_Fin.Replace("-", "/"));
        //                CanStartCourse = true;
        //            }
        //        }
        //        else
        //        {
        //            Debug.LogError("Date_Debut or Date_Fin are null or empty");
        //        }

        //    }
        //}
    }

}
