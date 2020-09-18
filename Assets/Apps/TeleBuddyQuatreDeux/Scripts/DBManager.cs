using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BlueQuark;
using System;
using System.Globalization;
using UnityEngine.UI;

namespace BuddyApp.TeleBuddyQuatreDeux
{
    /// <summary>
    /// This class retrieves all informations from the DB.
    /// </summary>
    public sealed class DBManager : MonoBehaviour
    {
        private static DBManager instance = null;
        public static DBManager Instance
        {
            get
            {
                return instance;
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
        
        public List<User> ListUserStudent { get; private set; }
        public User UserStudent { get; private set; }

        public List<string> ListUIDTablet { get; private set; }
        public bool Peering { get; private set; }
        public bool InfoRequestedDone { get; private set; }
        public string NameProf { get; private set; }
        public bool IsRefreshButtonPushed { get; set; }
        public bool IsCheckPlanning { get; set; }
        public int IndexPlanning { get; set; }

        [SerializeField]
        private Animator Animator;

        private string mRobotName;

        public bool LaunchDb { get; set; }

        private bool mPlanning;
        public bool CanStartCourse { get; private set; }
        public bool CanEndCourse { get; set; }

        private DateTime mDateNow;
        private DateTime mPlanningEnd;
        private TimeSpan mSpan;

        private CultureInfo mProviderFR = new CultureInfo("fr-FR");

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
            TeleBuddyQuatreDeuxData.Instance.AllPlanning = new List<string>();
            IsRefreshButtonPushed = false;
            StartDBManager();
        }

        /// <summary>
        /// Start the connexion to the DB and fill all the variable.
        /// This function can be launched from the parameter to do a new research on the DB.
        /// </summary>
        public void StartDBManager()
        {
            IsCheckPlanning = false;
            LaunchDb = true;
            TeleBuddyQuatreDeuxData.Instance.ConnectivityProblem = ConnectivityProblem.LaunchDatabase;
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fr-FR");
            mPlanning = false;
            Peering = false;
            InfoRequestedDone = false;
            CanStartCourse = false;
            CanEndCourse = false;
            mDeviceUserLiaisonList.Clear();
            ListUIDTablet.Clear();
            ListUserStudent.Clear();
            TeleBuddyQuatreDeuxData.Instance.AllPlanning.Clear();
            mListTabletUser.Clear();

            if (!IsRefreshButtonPushed)
                StartCoroutine(GetUserIdFromUID(Buddy.Platform.RobotUID));  
            //StartCoroutine(GetUserIdFromUID("EED7BF3ABE076D2D7A40"));
        }

        private void Update()
        {
            if (CanStartCourse && !CanEndCourse)
            {
                mDateNow = DateTime.Now;

                mSpan = mPlanningEnd.Subtract(mDateNow);
                if (mSpan.TotalMinutes < 5F)
                {
                    if (mSpan.TotalMinutes < 0F)
                    {
                        if(TeleBuddyQuatreDeuxData.Instance.AllPlanning.Count > 0)
                        {
                            TeleBuddyQuatreDeuxData.Instance.AllPlanning.RemoveAt(0);
                        }
                        CanEndCourse = true;
                        CanStartCourse = false;
                    }
                }
            }
            if (!CanStartCourse && IsCheckPlanning && TeleBuddyQuatreDeuxData.Instance.AllPlanning.Count > 0)
            {
                CheckStartPlanning();
            }
        }

        /// <summary>
        /// Connection to the DB. Search for the robot name and all the users of this robot then launch an other coroutine.
        /// </summary>
        /// <param name="iRobotUID">UID of the robot</param>
        /// <returns></returns>
        private IEnumerator GetUserIdFromUID(string iRobotUID)
        {
            yield return new WaitForSeconds(5F);
            mDeviceUserLiaisonList.Clear();
            string lRequest = GET_ALL_LIAISON + '"' + iRobotUID + '"';
            using (UnityWebRequest lRequestDevice = UnityWebRequest.Get(lRequest))
            {
                yield return lRequestDevice.SendWebRequest();
                if (lRequestDevice.isHttpError || lRequestDevice.isNetworkError)
                {
                    Debug.LogError("Request from GetUserIdFromUID error " + lRequestDevice.error + " " + lRequestDevice.downloadHandler.text);
                    TeleBuddyQuatreDeuxData.Instance.ConnectivityProblem = ConnectivityProblem.DatabaseProblem;
                }
                else
                {
                    string lRes = lRequestDevice.downloadHandler.text;
                    Debug.LogError("Result from GetUserIdFromUID with Robot UID : " + lRes);
                    try
                    {
                        DeviceUserLiaisonList devices = Utils.UnserializeJSON<DeviceUserLiaisonList>(lRes);

                        mRobotName = devices.Device_user[0].DeviceNom;
                        if (devices != null)
                        {
                            for (int i = 0; i < devices.Device_user.Length; ++i)
                            {
                                mDeviceUserLiaison = devices.Device_user[i];
                                mDeviceUserLiaisonList.Add(mDeviceUserLiaison);
                            }
                            if (mDeviceUserLiaisonList.Count > 0)
                            {
                                StartCoroutine(GetInfoForUsers(mDeviceUserLiaisonList));
                            }
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

        /// <summary>
        /// Search for all th informations linked to all the users of this robot.
        /// </summary>
        /// <param name="iListDeviceUserLiaison">List of user for the robot</param>
        /// <returns></returns>
        private IEnumerator GetInfoForUsers(List<DeviceUserLiaison> iListDeviceUserLiaison)
        {
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
                                if (lLiaison.DeviceType_device.Contains(TABLET_TYPE_DEVICE))
                                {
                                    mListTabletUser.Add(lLiaison);
                                }
                                if(lLiaison.DeviceType_device.Contains("Robot") && !string.IsNullOrEmpty(mRobotName) && mRobotName.Equals(lLiaison.DeviceNom))
                                {
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
                                UserStudent.NeedPlanning = lDeviceUserLiaison.DeviceNeedPlanning;
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
                                    LaunchDb = false;
                                }
                            }
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

        /// <summary>
        /// Function called by ConnectingState when one button from the list of Robot's user is clicked.
        /// Check if there is multiple planning for a student or it doesn't need any planning and fill a list with the planning.
        /// If it doesn't need any planning, create one with the datetime.now and then add 100 days.
        /// Planning in the DB are not on a good format so it does all the change in this function, then the function sort the date.
        /// </summary>
        /// <param name="iName">Name of the student chosen</param>
        /// <param name="iFirstName">First name of the student chosen</param>
        public void FillPlanningStart(string iName, string iFirstName)
        {
            CanStartCourse = false;
            TeleBuddyQuatreDeuxData.Instance.AllPlanning.Clear();
            lAllPlaningList.Clear();
            lPlanningBuffer.Clear();
            foreach (DeviceUserLiaison lLiaison in mListRobotUser)
            {
                if (!string.IsNullOrEmpty(lLiaison.UserNom) && !string.IsNullOrEmpty(lLiaison.UserPrenom) && lLiaison.UserNom == iName && lLiaison.UserPrenom == iFirstName)
                {

                    if(string.IsNullOrEmpty(lLiaison.PlanningidPlanning) || lLiaison.PlanningidPlanning == " " || lLiaison.PlanningidPlanning == "")
                    {
                        TeleBuddyQuatreDeuxData.Instance.AllPlanning.Add(" ");
                    }

                    if(lLiaison.PlanningidPlanning.Contains(",") || !lLiaison.DeviceNeedPlanning)
                    {
                        string[] lAllPlanning;
                        if (!lLiaison.DeviceNeedPlanning)
                        {
                            List<string> lBuffer = new List<string>();
                            DateTime lDateTimeNow = DateTime.Now;
                            lBuffer.Add("000&" + lDateTimeNow + "&" + lDateTimeNow.AddDays(100)+ "&");
                            lAllPlanning = lBuffer.ToArray();
                        }
                        else
                            lAllPlanning = lLiaison.PlanningidPlanning.Split(',');

                        int index = 0;
                        lAllPlaningList.AddRange(lAllPlanning);
                        foreach(string lPlanning in lAllPlaningList)
                        {
                            string[] lAllPlanningListSplit = lPlanning.Split('&');
                            DateTime lDateNowTest = DateTime.Now;
                            DateTime lPlanningEnd = DateTime.ParseExact(lAllPlanningListSplit[2].Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);
                             
                            TimeSpan lSpan = lPlanningEnd.Subtract(lDateNowTest);
                            if (lSpan.TotalMinutes < 0F)
                            {
                                lPlanningBuffer.Add(lAllPlaningList[index]);
                            }
                            ++index;
                        }
                        lAllPlaningList.RemoveAll(item => lPlanningBuffer.Contains(item));

                        if (lAllPlaningList.Count == 0)
                            return;

                        lAllPlanning = lAllPlaningList.ToArray();

                        List<DateTime> lSortDateTime = new List<DateTime>();
                        for (int i = 0; i < lAllPlanning.Length; ++i)
                        {
                            string[] lSplitDateAllPlanning = lAllPlanning[i].Split('&');
                            DateTime lPlanning = new DateTime();
                            string lRightFormatDate = lSplitDateAllPlanning[1].Replace("-", "/").TrimEnd().TrimStart();

                            lPlanning = DateTime.ParseExact(lRightFormatDate, "dd/MM/yyyy HH:mm:ss", mProviderFR);
                            lSortDateTime.Add(lPlanning);

                        }
                        if(lSortDateTime.Count > 1)
                            lSortDateTime.Sort(delegate (DateTime d1, DateTime d2) { return DateTime.Compare(d1, d2); });

                        for (int j = 0; j < lSortDateTime.Count; ++j)
                        {
                            for  (int i = 0; i < lAllPlanning.Length; ++i)
                            {
                                if (lAllPlanning[i].Replace("-", "/").Contains(lSortDateTime[j].ToString()))
                                {
                                    TeleBuddyQuatreDeuxData.Instance.AllPlanning.Add(lAllPlanning[i]);
                                }
                            }
                                
                        }
                        if(TeleBuddyQuatreDeuxData.Instance.AllPlanning.Count > 0)
                        {
                            for (int k = 0; k < TeleBuddyQuatreDeuxData.Instance.AllPlanning.Count; ++k)
                            {
                                if (TeleBuddyQuatreDeuxData.Instance.AllPlanning[k] == " ")
                                    TeleBuddyQuatreDeuxData.Instance.AllPlanning.RemoveAt(k);
                            }
                        }
                        mPlanning = true;
                        break;
                    }
                    else
                    {
                        if (!lLiaison.DeviceNeedPlanning)
                        {
                            string lPlanning;
                            DateTime lDateTimeNow = DateTime.Now;
                            lPlanning = "000&" + lDateTimeNow + "&" + lDateTimeNow.AddDays(100) + "&";
                            TeleBuddyQuatreDeuxData.Instance.AllPlanning.Add(lPlanning);
                        }
                        else
                            TeleBuddyQuatreDeuxData.Instance.AllPlanning.Add(lLiaison.PlanningidPlanning);

                        mPlanning = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Check if the planning on the right format for starting date, the end date and the name of the professor. If the name of the professor is empty it gives the name of the robot instead.
        /// Check if the course can start and change the global variable CanStartCourse to true.
        /// </summary>
        private void CheckStartPlanning()
        {
            if (mPlanning && !CanStartCourse && !string.IsNullOrEmpty(TeleBuddyQuatreDeuxData.Instance.AllPlanning[0]) && TeleBuddyQuatreDeuxData.Instance.AllPlanning.Count > 0 && TeleBuddyQuatreDeuxData.Instance.AllPlanning[0] != " ") 
            {
                string[] lSplitDate = TeleBuddyQuatreDeuxData.Instance.AllPlanning[0].Split('&');
                Planning mCurrentPlanning = new Planning();
                mCurrentPlanning.Date_Debut = lSplitDate[1];
                mCurrentPlanning.Date_Fin = lSplitDate[2]; 
                mCurrentPlanning.Prof = mRobotName;
                if (lSplitDate.Length > 3)
                {
                    if (!string.IsNullOrEmpty(lSplitDate[3]))
                        mCurrentPlanning.Prof = lSplitDate[3];
                    else
                        mCurrentPlanning.Prof = mRobotName;
                } 
                else if(lSplitDate.Length <= 3)
                {
                    mCurrentPlanning.Prof = mRobotName;
                }
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

                    if (lSpan.TotalMinutes < 0F)
                    {
                        mPlanningEnd = DateTime.ParseExact(mCurrentPlanning.Date_Fin.Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", mProviderFR);
                        CanStartCourse = true;
                        CanEndCourse = false;
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
