using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BlueQuark;
using System;
using System.Globalization;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{

    //TODO : 
    // - Care about missing Date_Debut or Date_Fin from planning, do the verif on the string -> CheckStartPlanning might be enough


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
        private List<DeviceUserLiaison> mListRobotUser;
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
        public string NameProf { get; private set; }
        public bool IsRefreshButtonPushed { get; set; }
        public bool IsCheckPlanning { get; set; }
        public int IndexPlanning { get; set; }

        private string mNameRobot;

        private string mRobotName;
        //private List<Planning> ListPlanning;
        private Planning mPlanningNextCourse;
        //private Planning mPlanningCheck;

        public bool LaunchDb { get; set; }

        //private float mTimer;
        private bool mPlanning;
        public bool CanStartCourse { get; private set; }
        public bool CanEndCourse { get; set; }

        //private int mNbIteration;
        private DateTime mDateNow;
        private DateTime mPlanningEnd;
        private TimeSpan mSpan;
        //private bool mCheckDBEndCall;

        private CultureInfo mProviderFR = new CultureInfo("fr-FR");

        private RTMManager mRTMManager;

        List<string> lAllPlaningList = new List<string>();
        List<string> lPlanningBuffer = new List<string>();

        private void Awake()
        {
            instance = this;
            LaunchDb = false;
        }

        // Use this for initialization
        void Start()
        {
            mDeviceUserLiaisonList = new List<DeviceUserLiaison>();
            ListUIDTablet = new List<string>();
            ListUserStudent = new List<User>();
            mListTabletUser = new List<DeviceUserLiaison>();
            mListRobotUser = new List<DeviceUserLiaison>();
            mDeviceUserLiaison = new DeviceUserLiaison();
            CoursTelepresenceData.Instance.AllPlanning = new List<string>();
            IsRefreshButtonPushed = false;
            StartDBManager();


        }

        public void StartDBManager()
        {
            mNameRobot = "";
            LaunchDb = true;
            CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.LaunchDatabase;
            mRTMManager = GetComponent<RTMManager>();
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fr-FR");
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
            mDeviceUserLiaisonList.Clear();
            ListUIDTablet.Clear();
            ListUserStudent.Clear();
            CoursTelepresenceData.Instance.AllPlanning.Clear();
            mListTabletUser.Clear();
            //mListRobotUser.Clear();
            

            //CoursTelepresenceData.Instance.AllPlanning.Clear();
            //mDeviceUserLiaisonList.Clear();
            //mListTabletUser.Clear();
            //mListRobotUser.Clear();
            //ListUIDTablet.Clear();
            //ListUserStudent.Clear();


            //POUR LES TESTS 
            CoursTelepresenceData.Instance.AllPlanning.Clear();
            if(!IsRefreshButtonPushed)
                StartCoroutine(GetUserIdFromUID(Buddy.Platform.RobotUID)); 
            //StartCoroutine(GetUserIdFromUID("EED7BF3ABE076D2D7A40"));

            //StartCoroutine(UpdatePingAndPosition());
            //StartCoroutine(UpdateBattery());
        }

        private void Update()
        {
            //if(mStartUpdate)
            //{
            //Debug.LogError("<color=blue> CanstartCourse " + CanStartCourse + " CanEndCourse " + CanEndCourse  + "</color>");
            if (CanStartCourse && !CanEndCourse)
            {
                ////A TEST
                //if (!mButtonCallEnabled)
                //{
                //    mButtonCallEnabled = true;
                //    ButtonCall.interactable = true;
                //    mImageFromButtonCall = ButtonCall.GetComponentInChildren<Image>();
                //    Color lTempColor = mImageFromButtonCall.color;
                //    lTempColor.a = 1F;
                //    mImageFromButtonCall.color = lTempColor;
                //}
                mDateNow = DateTime.Now;

                mSpan = mPlanningEnd.Subtract(mDateNow);
                //Debug.LogError("<color=blue> mSPAN " + mSpan + "</color>");
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
                            if(CoursTelepresenceData.Instance.AllPlanning.Count > 0)
                            {
                                CoursTelepresenceData.Instance.AllPlanning.RemoveAt(0);
                            }
                            CanEndCourse = true;
                            CanStartCourse = false;
                            //mPlanning = false;
                            //mStartUpdate = false;
                        }
                }
            }
            if (!CanStartCourse && IsCheckPlanning)
            {
                CheckStartPlanning();
            }
            //}

        }

        private IEnumerator GetUserIdFromUID(string iRobotUID)
        {
            yield return new WaitForSeconds(5F);
            mDeviceUserLiaisonList.Clear();
            string lRequest = GET_ALL_LIAISON + '"' + iRobotUID + '"';
            Debug.LogError("Request : " + lRequest);
            using (UnityWebRequest lRequestDevice = UnityWebRequest.Get(lRequest))
            {
                yield return lRequestDevice.SendWebRequest();
                if (lRequestDevice.isHttpError || lRequestDevice.isNetworkError)
                {
                    Debug.LogError("Request from GetUserIdFromUID error " + lRequestDevice.error + " " + lRequestDevice.downloadHandler.text);
                    CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.DatabaseProblem;
                }
                else
                {
                    string lRes = lRequestDevice.downloadHandler.text;
                    Debug.LogError("Result from GetUserIdFromUID with Robot UID : " + lRes);

                    try
                    {
                        DeviceUserLiaisonList devices = Utils.UnserializeJSON<DeviceUserLiaisonList>(lRes);

                        mRobotName = devices.Device_user[0].DeviceNom;
                        Debug.LogError("Result from GetUserIdFromUID with Robot UID FOR ROBOT NAME : " + mRobotName);
                        if (devices != null)
                        {
                            DBConnected = true;
                            //mNameRobot = devices.Device_user[0].DeviceNom;
                            for (int i = 0; i < devices.Device_user.Length; ++i)
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

        public IEnumerator RefreshPlanning()
        {
            //StartCoroutine(GetInfoForUsers(mDeviceUserLiaisonList));
            StartCoroutine(GetUserIdFromUID(Buddy.Platform.RobotUID)); 
            while (!InfoRequestedDone)
                yield return null;
            //FillPlanningStart(ListUserStudent[0].Nom);
            //mRTMManager.SetTabletId(ListUIDTablet[0]);
        }

        private IEnumerator GetInfoForUsers(List<DeviceUserLiaison> iListDeviceUserLiaison)
        {
            //ListUIDTablet = new List<string>();
            //ListUserStudent = new List<User>();
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
                mListTabletUser.Clear();
                mListRobotUser.Clear();
                ListUserStudent.Clear();
                ListUIDTablet.Clear();
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
                                Debug.LogError("<color=red> foreach lliaison " + lLiaison.DeviceType_device + " robot name : " + mRobotName + " lliaison.devicenom : " + lLiaison.DeviceNom +  " </color>");
                                if (lLiaison.DeviceType_device.Contains(TABLET_TYPE_DEVICE))
                                {
                                    Debug.LogError("<color=red> lliaison typedevice  : " + lLiaison.DeviceType_device + " lliaison " + lLiaison.UserNom + " " + lLiaison.UserPrenom + " " + lLiaison.PlanningidPlanning +  "</color>");
                                    mListTabletUser.Add(lLiaison);
                                }
                                if(lLiaison.DeviceType_device.Contains("Robot") && !string.IsNullOrEmpty(mRobotName) && mRobotName.Equals(lLiaison.DeviceNom))
                                {
                                    Debug.LogError("<color=red> foreach lliaison ET DEVICE TYPE = ROBOT : " + lLiaison.DeviceType_device + " info complémentaire robot : " + lLiaison.UserPrenom + " " + lLiaison.PlanningidPlanning + " nom robot : " + mNameRobot + " lliaison nom robot : " + lLiaison.DeviceNom + " </color>");

                                    mListRobotUser.Add(lLiaison);
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
                            Debug.LogError("<color=red> ****************DBMANAGER  : list UID tablet  " + ListUIDTablet.Count + " list user student :" + ListUserStudent.Count +  " ******************</color>");


                            if (mListTabletUser.Count > 0 && ListUserStudent.Count > 0)
                            {
                                Peering = true;
                                if (!string.IsNullOrEmpty(ListUserStudent[0].Nom)
                                    && !string.IsNullOrEmpty(ListUserStudent[0].Prenom)
                                    && !string.IsNullOrEmpty(ListUserStudent[0].Organisme)
                                    && !string.IsNullOrEmpty(ListUIDTablet[0]))
                                {
                                    InfoRequestedDone = true;
                                    LaunchDb = false;
                                    //Buddy.GUI.Toaster.Hide();
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

        public void FillPlanningStart(/*int iIndexChosen, */string iName)
        {
            CanStartCourse = false;
            CoursTelepresenceData.Instance.AllPlanning.Clear();
            lAllPlaningList.Clear();
            lPlanningBuffer.Clear();
            Debug.LogError("<color=blue>mlistrobotuser  : " + mListRobotUser.Count + "</color>");
            foreach (DeviceUserLiaison lLiaison in mListRobotUser)
            {
                Debug.LogError("<color=blue>lLiaison : " + lLiaison.UserNom + " " + lLiaison.PlanningidPlanning + "</color>");
                if (!string.IsNullOrEmpty(lLiaison.UserNom) && !string.IsNullOrEmpty(lLiaison.PlanningidPlanning) && lLiaison.UserNom == iName)
                {
                    if(string.IsNullOrEmpty(lLiaison.PlanningidPlanning))
                    {
                        CoursTelepresenceData.Instance.AllPlanning.Add(" ");
                        Debug.LogError("<color=red> add empty all planning</color>");
                    }
                    if(lLiaison.PlanningidPlanning.Contains(","))
                    {
                        string[] lAllPlanning = lLiaison.PlanningidPlanning.Split(',');

                        int index = 0;
                        lAllPlaningList.AddRange(lAllPlanning);
                        foreach(string lPlanning in lAllPlaningList)
                        {
                            string[] lAllPlanningListSplit = lPlanning.Split('&');
                            DateTime lDateNowTest = DateTime.Now;
                            DateTime lPlanningEnd = DateTime.ParseExact(lAllPlanningListSplit[2].Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture) /*Convert.ToDateTime(lAllPlanningListSplit[2].Replace("-", "/").TrimEnd().TrimStart())*/;
                            //DateTime lPlanningEnd = DateTime.ParseExact(lAllPlanningListSplit[3])
                            Debug.LogError("<color=blue>0 LPLANNING START : " + lPlanningEnd.ToString()+ " " + lDateNowTest.ToString() + " Culture info name " + CultureInfo.CurrentCulture.Name + "</color>");
                             
                            TimeSpan lSpan = lPlanningEnd.Subtract(lDateNowTest);
                            if (lSpan.TotalMinutes < 0F)
                            {
                                lPlanningBuffer.Add(lAllPlaningList[index]);
                            }
                            ++index;
                        }
                        lAllPlaningList.RemoveAll(item => lPlanningBuffer.Contains(item));
                        Debug.LogError("<color=green>0 LALLPLANNINGLIST COUNT : " + lAllPlaningList.Count + "</color>");

                        if (lAllPlaningList.Count == 0)
                            return;
                        //FINIR ICI PARCE QUAUCUN PLANNING NE CORRESPOND A LA BONNE JOURNEE si count = 0
                        //SINON FAIRE LE RESTE et clear les list
                        lAllPlanning = lAllPlaningList.ToArray();

                        //Debug.LogError("<color=green>1 LLIAISON PLANNING ET COUNT CoursTelepresenceData.Instance.AllPlanning.Count: " + lLiaison.PlanningidPlanning +  " " + CoursTelepresenceData.Instance.AllPlanning.Count +  "</color>");
                        //if (CoursTelepresenceData.Instance.AllPlanning.Count == 0)
                        //{
                            List<DateTime> lSortDateTime = new List<DateTime>();
                            
                            //for(int j = 0; j < lAllPlanning.Length; ++j)
                            //{
                            //    Debug.LogError("<color=green>2 ALL PLANNING : " + lAllPlanning[j] + "</color>");

                            //}
                            for (int i = 0; i < lAllPlanning.Length; ++i)
                            {
                                string[] lSplitDateAllPlanning = lAllPlanning[i].Split('&');
                                Debug.LogError("<color=blue>3  ET RECUPERER DATE PREMIER : " + lSplitDateAllPlanning[1] + " DATETIME NOW : " + DateTime.Now.ToString(mProviderFR) + "</color>");
                                DateTime lPlanning = new DateTime();
                                //if (Application.systemLanguage == SystemLanguage.French)
                                //{
                                    
                                    Debug.LogError("<color=blue>3.5 FRENCH</color>");
                                    string lRightFormatDate = lSplitDateAllPlanning[1].Replace("-", "/").TrimEnd().TrimStart();

                                    Debug.LogError("<color=blue>3.55 FRENCH : " + lRightFormatDate + " DATE TIME NOW " +  "</color>");
                                    lPlanning = DateTime.ParseExact(lRightFormatDate, "dd/MM/yyyy HH:mm:ss", mProviderFR);
                                    Debug.LogError("<color=blue>3.65 FRENCH : " + lPlanning.ToString() + "</color>");  
                                //}
                                //else if(Application.systemLanguage == SystemLanguage.English)
                                //{
                                //    Debug.LogError("<color=blue>3.5 ENGLISH</color>");
                                //    string[] lSwapDayMonth = lSplitDateAllPlanning[1].Replace("-", "/").Split('/');
                                //    string lSwap = lSwapDayMonth[0];
                                //    lSwapDayMonth[0] = lSwapDayMonth[1];
                                //    lSwapDayMonth[1] = lSwap;
                                //    string lFinalDate = lSwapDayMonth[0] + "/" + lSwapDayMonth[1] + "/" + lSwapDayMonth[2];
                                //    Debug.LogError("<color=blue>3.5  ET RECUPERER DATE PREMIER : " + lFinalDate + " DATETIME NOW : " + DateTime.Now.ToString(mProviderFR) + "</color>");
                                //    lPlanning = DateTime.Parse(lFinalDate);
                                //}
                                Debug.LogError("<color=blue>4  ET RECUPERER DATE PREMIER : " + lPlanning.ToString() + " DATETIME NOW : " + DateTime.Now.ToString() + "</color>");
                                lSortDateTime.Add(lPlanning);

                            }
                            if(lSortDateTime.Count > 1)
                                lSortDateTime.Sort(delegate (DateTime d1, DateTime d2) { return DateTime.Compare(d1, d2); });
                            foreach(DateTime lDateTime in lSortDateTime)
                            {
                                Debug.LogError("<color=blue>**********SORTDATETIME**********  : " + lDateTime.ToString() + "</color>");
                            }
                            for (int j = 0; j < lSortDateTime.Count; ++j)
                            {
                                for  (int i = 0; i < lAllPlanning.Length; ++i)
                                {
                                    if (lAllPlanning[i].Replace("-", "/").Contains(lSortDateTime[j].ToString()))
                                    {
                                        
                                        CoursTelepresenceData.Instance.AllPlanning.Add(lAllPlanning[i]);
                                        Debug.LogError("<color=blue>VERIFICATION ALL PLANING  : " + lAllPlanning[i] + "</color>");
                                    }
                                }
                                
                            }
                            //test debug allplaning list : 
                            if(CoursTelepresenceData.Instance.AllPlanning.Count > 0)
                            {

                                for (int k = 0; k < CoursTelepresenceData.Instance.AllPlanning.Count; ++k)
                                {
                                    Debug.LogError("<color=red> COURS TELEPRESENCE DATA ALL PLANING  : " + CoursTelepresenceData.Instance.AllPlanning[k] + "</color>");
                                }
                            }
                            //CoursTelepresenceData.Instance.AllPlanning.AddRange(lAllPlanning);
                            //CoursTelepresenceData.Instance.AllPlanning.Sort(,);
                        //}
                        //string[] lSplitDate = CoursTelepresenceData.Instance.AllPlanning[0].Split('&');
                        //string[] lSplitDateDayMonth = lSplitDate[1].Split('-');
                        ////lSplitDate[1] start planning lSplitDate[2] end planning
                        //string lDateNow = DateTime.Now.ToString();
                        //string[] lSpliteDateNow = lDateNow.Split('/');
                        ////Debug.LogError("<color=red> lSplitDateDayMonth day : " + lSplitDateDayMonth[0] + " lSplitDateDayMonth month : " + lSplitDateDayMonth[1] + " DATE NOW  : " + lSpliteDateNow[0] + " && " + lSpliteDateNow[1] +"</color>");
                        //Debug.LogError("<color=red> lSplitDate start : " + lSplitDate[1] + " lSplitDate end : " + lSplitDate[2] + " DATE NOW  : " + lSpliteDateNow[0] + " && " + lSpliteDateNow[1] + "</color>");
                        ////if (Application.systemLanguage == SystemLanguage.French)
                        ////{
                        //    if ((lSplitDateDayMonth[0] == lSpliteDateNow[0]) && (lSplitDateDayMonth[1] == lSpliteDateNow[1]))
                        //    {
                        //        Planning mPlanningUser = new Planning();
                        //        mPlanningUser.Date_Debut = lSplitDate[1];
                        //        mPlanningUser.Date_Fin = lSplitDate[2];
                        //        mPlanningUser.Prof = lSplitDate[3];
                        //        Debug.LogError("<color=red> Date Debut : " + mPlanningUser.Date_Debut + " Date fin : " + mPlanningUser.Date_Fin + "</color>");
                        //        mPlanningNextCourse = mPlanningUser;
                                mPlanning = true;
                            //}
                        //}
                        //else if (Application.systemLanguage == SystemLanguage.English)
                        //{
                        //    if ((lSplitDateDayMonth[0] == lSpliteDateNow[1]) && (lSplitDateDayMonth[1] == lSpliteDateNow[0]))
                        //    {
                        //        Planning mPlanningUser = new Planning();
                        //        string[] lSplitDateEnd = lSplitDate[2].Split('-');
                        //        string[] lSplitDateStart = lSplitDate[1].Split('-');
                        //        //string lTemp = lSplitDateEnd[0];
                        //        //lSplitDateEnd[0] = lSplitDateEnd[1];
                        //        //lSplitDateEnd[1] = lTemp;
                        //        string lDateEndReformed = lSplitDateEnd[1] + "-" + lSplitDateEnd[0] + "-" + lSplitDateEnd[2];
                        //        string lDateStartReformed = lSplitDateStart[1] + "-" + lSplitDateStart[0] + "-" + lSplitDateStart[2];
                        //        mPlanningUser.Date_Fin = lDateEndReformed;
                        //        mPlanningUser.Date_Debut = lDateStartReformed;
                        //        mPlanningUser.Prof = lSplitDate[3];
                        //        Debug.LogError("<color=red> Date Debut : " + mPlanningUser.Date_Debut + " Date fin : " + mPlanningUser.Date_Fin + "</color>");
                        //        mPlanningNextCourse = mPlanningUser;
                        //        mPlanning = true;
                        //    }
                        //}
                        break;
                    }
                    else
                    {
                       // string[] lSplitDate = lLiaison.PlanningidPlanning.Split('&');
                        CoursTelepresenceData.Instance.AllPlanning.Add(lLiaison.PlanningidPlanning);
                        //string[] lSplitDateDayMonth = lSplitDate[1].Split('-');
                        ////lSplitDate[1] start planning lSplitDate[2] end planning
                        //string lDateNow = DateTime.Now.ToString();
                        //string[] lSpliteDateNow = lDateNow.Split('/');
                        ////Debug.LogError("<color=red> lSplitDateDayMonth day : " + lSplitDateDayMonth[0] + " lSplitDateDayMonth month : " + lSplitDateDayMonth[1] + " DATE NOW  : " + lSpliteDateNow[0] + " && " + lSpliteDateNow[1] +"</color>");
                        //Debug.LogError("<color=red> lSplitDate start : " + lSplitDate[1] + " lSplitDate end : " + lSplitDate[2] + " DATE NOW  : " + lSpliteDateNow[0] + " && " + lSpliteDateNow[1] + "</color>");
                        ////if (Application.systemLanguage == SystemLanguage.French)
                        ////{
                        //    if ((lSplitDateDayMonth[0] == lSpliteDateNow[0]) && (lSplitDateDayMonth[1] == lSpliteDateNow[1]))
                        //    {
                        //        Planning mPlanningUser = new Planning();
                        //        mPlanningUser.Date_Debut = lSplitDate[1];
                        //        mPlanningUser.Date_Fin = lSplitDate[2];
                        //        mPlanningUser.Prof = lSplitDate[3];
                        //        Debug.LogError("<color=red> Date Debut : " + mPlanningUser.Date_Debut + " Date fin : " + mPlanningUser.Date_Fin + "</color>");
                        //        mPlanningNextCourse = mPlanningUser;
                                mPlanning = true;
                           // }
                        //}
                        //else if (Application.systemLanguage == SystemLanguage.English)
                        //{
                        //    if ((lSplitDateDayMonth[0] == lSpliteDateNow[1]) && (lSplitDateDayMonth[1] == lSpliteDateNow[0]))
                        //    {
                        //        Planning mPlanningUser = new Planning();
                        //        string[] lSplitDateEnd = lSplitDate[2].Split('-');
                        //        string[] lSplitDateStart = lSplitDate[1].Split('-');
                        //        //string lTemp = lSplitDateEnd[0];
                        //        //lSplitDateEnd[0] = lSplitDateEnd[1];
                        //        //lSplitDateEnd[1] = lTemp;
                        //        string lDateEndReformed = lSplitDateEnd[1] + "-" + lSplitDateEnd[0] + "-" + lSplitDateEnd[2];
                        //        string lDateStartReformed = lSplitDateStart[1] + "-" + lSplitDateStart[0] + "-" + lSplitDateStart[2];
                        //        mPlanningUser.Date_Fin = lDateEndReformed;
                        //        mPlanningUser.Date_Debut = lDateStartReformed;
                        //        mPlanningUser.Prof = lSplitDate[3];
                        //        Debug.LogError("<color=red> Date Debut : " + mPlanningUser.Date_Debut + " Date fin : " + mPlanningUser.Date_Fin + "</color>");
                        //        mPlanningNextCourse = mPlanningUser;
                        //        mPlanning = true;
                        //    }
                        //}
                        break;
                    }



                }
            }
            

            //mStartUpdate = true;

        }

        private void CheckStartPlanning()
        {
            Debug.LogError("<color=blue> DBMANAGER : CHECK START PLANNING : " + CoursTelepresenceData.Instance.AllPlanning.Count + "  all planing 0 : " + CoursTelepresenceData.Instance.AllPlanning[0] + "</color>");
            if (mPlanning && !CanStartCourse && !string.IsNullOrEmpty(CoursTelepresenceData.Instance.AllPlanning[0])  && CoursTelepresenceData.Instance.AllPlanning.Count > 0)
            {
                string[] lSplitDate = CoursTelepresenceData.Instance.AllPlanning[0].Split('&');
                Planning mCurrentPlanning = new Planning();
                mCurrentPlanning.Date_Debut = lSplitDate[1];
                mCurrentPlanning.Date_Fin = lSplitDate[2];
                if (lSplitDate.Length > 3 && !string.IsNullOrEmpty(lSplitDate[3]))
                {
                    mCurrentPlanning.Prof = lSplitDate[3];
                } 
                else
                {
                    mCurrentPlanning.Prof = mRobotName;
                }
                //Debug.LogError("<color=red> ARRAY  : " + mPlanningNextCourse.Date_Debut + "</color>");
                if (!string.IsNullOrEmpty(mCurrentPlanning.Date_Debut) && !string.IsNullOrEmpty(mCurrentPlanning.Date_Fin))
                {
                    DateTime lDateNow = DateTime.Now;
                    DateTime lPlanningStart = DateTime.ParseExact(mCurrentPlanning.Date_Debut.Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", mProviderFR);
                    TimeSpan lSpan = lPlanningStart.Subtract(lDateNow);
                    if (!string.IsNullOrEmpty(mCurrentPlanning.Prof))
                    {
                        NameProf = mCurrentPlanning.Prof;
                    }
                    else
                        NameProf = "";
                        

                    //Debug.LogError("<color=red> START COURSE IN : " + lSpan.TotalMinutes + "</color>");
                    //Debug.LogError("planning start: " + lPlanningStart + "date now" + lDateNow);

                    if (lSpan.TotalMinutes < 0F)
                    {
                        //Buddy.GUI.Toaster.Hide();
                        Debug.LogError("<color=blue> START COURSE </color>");
                        mPlanningEnd = DateTime.ParseExact(mCurrentPlanning.Date_Fin.Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", mProviderFR);
                        CanStartCourse = true;
                        CanEndCourse = false;
                        Debug.LogError("planning end: " + mPlanningEnd + " prof : " + NameProf);
                    }
                }
                else
                {
                    Debug.LogError("Date_Debut or Date_Fin are null or empty");
                }

            }
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

        //private IEnumerator GetDeviceInfos(int iIdUser)
        //{
        //    ListUIDTablet = new List<string>();
        //    ListUserStudent = new List<User>();
        //    string lRequest = GET_USER_TABLET + iIdUser.ToString();
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
        //            Debug.LogError("Result get user device : " + lRes);

        //            try
        //            {
        //                DeviceUserLiaisonList devices = Utils.UnserializeJSON<DeviceUserLiaisonList>(lRes);

        //                if (devices != null)
        //                {
        //                    foreach (DeviceUserLiaison lLiaison in devices.Device_user)
        //                    {
        //                        Debug.LogError("<color=red> foreach lliaison " + lLiaison.DeviceType_device + " </color>");
        //                        if (lLiaison.DeviceType_device.Contains(TABLET_TYPE_DEVICE))
        //                        {
        //                            //Debug.LogError("<color=red> lliaison typedevice  : " + lLiaison.DeviceType_device + " lliaison " + lLiaison.UserNom + " " + lLiaison.UserPrenom +  "</color>");
        //                            mListTabletUser.Add(lLiaison);
        //                        }
        //                    }

        //                    foreach (DeviceUserLiaison lDeviceUserLiaison in mListTabletUser)
        //                    {
        //                        UserStudent = new User();
        //                        UserStudent.Nom = lDeviceUserLiaison.UserNom;
        //                        UserStudent.Prenom = lDeviceUserLiaison.UserPrenom;
        //                        UserStudent.Organisme = lDeviceUserLiaison.UserOrganisme;
        //                        ListUserStudent.Add(UserStudent);

        //                        ListUIDTablet.Add(lDeviceUserLiaison.DeviceUid);
        //                    }

        //                    if (mListTabletUser.Count > 0 && ListUserStudent.Count > 0)
        //                    {
        //                        Peering = true;
        //                        if (!string.IsNullOrEmpty(ListUserStudent[0].Nom)
        //                            && !string.IsNullOrEmpty(ListUserStudent[0].Prenom)
        //                            && !string.IsNullOrEmpty(ListUserStudent[0].Organisme)
        //                            && !string.IsNullOrEmpty(ListUIDTablet[0]))
        //                        {
        //                            InfoRequestedDone = true;
        //                        }
        //                        StartCoroutine(GetPlanning());
        //                    }

        //                    Debug.LogError("<color=red> lliaison typedevice  : " + mListTabletUser[0].DeviceType_device + " lliaison " + mListTabletUser[0].UserNom + " " + mListTabletUser[0].UserPrenom + "</color>");

        //                    //Debug.LogError("<color=red>mdeviceuserliaison :  " + mDeviceUserLiaisonList[0].UserIdUser + " nom : " + mDeviceUserLiaisonList[0].UserNom + " prenom : " + mDeviceUserLiaisonList[0].UserPrenom + "</color>");
        //                }
        //                else
        //                {
        //                    Debug.LogError("No tablet found with id user : " + iIdUser);
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.LogError("Error parsing tablet device request answer : " + e.Message);
        //            }
        //        }
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
        //                    //Planning = planning;
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
        //                    Debug.LogError("<color=green>FOREACH PLANNING Date Debut : " + Planning.Date_Debut + " Date fin : " + Planning.Date_Fin + "</color>");
        //                    if (planning.DeviceId.HasValue && planning.idPlanning.HasValue && planning.EleveIdUser.HasValue)
        //                        ListPlanning.Add(Planning);
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

        //            if (Application.systemLanguage == SystemLanguage.French)
        //            {
        //                if ((lSplitDate[0] == lSpliteDateNow[0]) && (lSplitDate[1] == lSpliteDateNow[1]))
        //                {
        //                    Debug.LogError("<color=red> Date Debut : " + lPlanning.Date_Debut + " Date fin : " + lPlanning.Date_Fin + "</color>");
        //                    mPlanning = true;
        //                    return lPlanning;
        //                }
        //            }
        //            else if (Application.systemLanguage == SystemLanguage.English)
        //            {
        //                if ((lSplitDate[0] == lSpliteDateNow[1]) && (lSplitDate[1] == lSpliteDateNow[0]))
        //                {
        //                    string[] lSplitDateEnd = lPlanning.Date_Fin.Split('-');
        //                    //string lTemp = lSplitDateEnd[0];
        //                    //lSplitDateEnd[0] = lSplitDateEnd[1];
        //                    //lSplitDateEnd[1] = lTemp;
        //                    string lDateEndReformed = lSplitDateEnd[1] + "-" + lSplitDateEnd[0] + "-" + lSplitDateEnd[2];
        //                    string lDateStartReformed = lSplitDate[1] + "-" + lSplitDate[0] + "-" + lSplitDate[2];
        //                    lPlanning.Date_Fin = lDateEndReformed;
        //                    lPlanning.Date_Debut = lDateStartReformed;
        //                    Debug.LogError("<color=red> Date Debut : " + lPlanning.Date_Debut + " Date fin : " + lPlanning.Date_Fin + "</color>");
        //                    mPlanning = true;
        //                    return lPlanning;
        //                }
        //            }

        //        }
        //        else
        //            Debug.LogError("<color=red> There is no student for any planning </color>");
        //    }
        //    return new Planning();
        //}






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
