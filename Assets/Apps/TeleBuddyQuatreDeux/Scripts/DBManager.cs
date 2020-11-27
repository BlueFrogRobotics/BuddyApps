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

        /// <summary>
        /// ZOHO V2
        /// </summary>
        /// 
        private TokenData mTokenData;
        private static string mRefresh_Token;
        private bool mRefreshTokenDownloaded;

        private const string TOKEN_REFRESH = "1000.40941a3170f32aabd46f944f413b29f0.2de9752287fec953a4759ace033704df";
        private const string CLIENT_ID = "1000.H33KJH9WVF674U3XC87QZXU825YETR";
        private const string CLIENT_SECRET = "5899144bd40ce6fd7b8be909137050b6f9f93021d7";
        private string REQUEST_ACCESSTOKEN_WITH_REFRESHTOKEN = "https://accounts.zoho.eu/oauth/v2/token?refresh_token=" + TOKEN_REFRESH + "&client_id=" + CLIENT_ID + "&client_secret=" + CLIENT_SECRET + "&grant_type=refresh_token";
        //private const string REQUEST_ZOHO_REFRESH_TOKEN = "https://accounts.zoho.eu/oauth/v2/token?grant_type=authorization_code&code=" + "CODE SECRET "+ "&client_id=" + CLIENT_ID + "&client_secret=" + CLIENT_SECRET;


        public string mRobotTokenRTM { get; private set; }
        public string mRobotTokenRTC { get; private set; }

        private static string mAccessToken;

        private const string TOKEN = "98bb0865eb455a6e61a993a43f63d601";
        private string ROOT_LINK_DB = "https://creator.zoho.eu/api/v2/bluefrogrobotics/flotte/report/all_liaison_device_user?authtoken=";
        private string GET_USER_TABLET =  "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true&criteria=User.idUser==";
        private string GET_ALL_LIAISON = "&scope=creatorapi&zc_ownername=bluefrogrobotics&raw=true&criteria=Device.Uid==";
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

        private bool mDisplayBeforeEnd;

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
            mRefreshTokenDownloaded = false;
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
            Debug.LogError("DB MANAGER : START DB MANAGER");
            mDisplayBeforeEnd = false;
            IsCheckPlanning = false;
            LaunchDb = true;
            Debug.LogError("<color=green>DB MANAGER : LAUNCH DB ENUM 1 </color>");
            TeleBuddyQuatreDeuxData.Instance.ConnectivityProblem = ConnectivityProblem.LaunchDatabase;
            Debug.LogError("<color=green>DB MANAGER : LAUNCH DB ENUM 2 </color>");
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

            //ZOHO V2 
            if (!mRefreshTokenDownloaded)
                StartCoroutine(GetAccessTokenWithRefreshToken());
            //if (!IsRefreshButtonPushed)
            //    StartCoroutine(GetUserIdFromUID(Buddy.Platform.RobotUID));
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

                    DisplayUIBeforeEnd();

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

        private void DisplayUIBeforeEnd()
        {
            if(!mDisplayBeforeEnd)
            {
                mDisplayBeforeEnd = true;
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetLabel(Buddy.Resources.GetString("eduendplanning"));
                });
                StartCoroutine(HideUIAfterTimer(5F));

            }

        }

        private IEnumerator HideUIAfterTimer(float iTimeHideUI)
        {
            yield return new WaitForSeconds(iTimeHideUI);
            if (Buddy.GUI.Toaster.IsBusy)
                Buddy.GUI.Toaster.Hide();
        }

        /// <summary>
        /// Retrieve Tokens from request for ZOHO V2
        /// </summary>
        /// <returns></returns>
        //private IEnumerator GetToken()
        //{
        //    Debug.LogError("<color=green>******DB MANAGER : GET TOKEN with request : " + REQUEST_ZOHO_REFRESH_TOKEN + " *******</color>");
        //    yield return new WaitForSeconds(2F);
        //    using (UnityWebRequest lRequestToken = UnityWebRequest.Get(REQUEST_ZOHO_REFRESH_TOKEN))
        //    {
        //        yield return lRequestToken.SendWebRequest();
        //        if (lRequestToken.isHttpError || lRequestToken.isNetworkError)
        //        {
        //            Debug.LogError("Request from GetToken error " + lRequestToken.error + " " + lRequestToken.downloadHandler.text);
        //            TeleBuddyQuatreDeuxData.Instance.ConnectivityProblem = ConnectivityProblem.DatabaseProblem;
        //        }
        //        else
        //        {
        //            string lResultRequestToken = lRequestToken.downloadHandler.text;
        //            Debug.LogError("<color=green>******DB MANAGER : RESULT TOKEN request : " + lResultRequestToken + " *******</color>");

        //            try
        //            {
        //                mTokenData = Utils.UnserializeJSON<TokenData>(lResultRequestToken);
        //                Debug.LogError("<color=green>******DB MANAGER : RESULT token data class : Access data : " + mTokenData.Access_Token + " API DOMAIN : " + mTokenData.Api_Domain + " EXPIRE IN : " + mTokenData.Expires_In + " REFRESH TOKEN : " + mTokenData.Refresh_Token + " TOKEN TYPE : " + mTokenData.Token_Type + " *******</color>");
        //                mRefresh_Token = mTokenData.Refresh_Token;
        //                if(!string.IsNullOrEmpty(mTokenData.Refresh_Token))
        //                {
        //                    //ZOHO V2 voir si on doit envoyer en param le refresh token
        //                    StartCoroutine(GetUserIdFromUID(Buddy.Platform.RobotUID));
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.LogError("Error parsing robot device request answer : " + e.Message);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Retrieves Access token with Refresh token for ZOHO V2 
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetAccessTokenWithRefreshToken()
        {
            //https://accounts.zoho.eu/oauth/v2/token?refresh_token=1000.40941a3170f32aabd46f944f413b29f0.2de9752287fec953a4759ace033704df&client_id=1000.H33KJH9WVF674U3XC87QZXU825YETR&client_secret=5899144bd40ce6fd7b8be909137050b6f9f93021d7&grant_type=refresh_token
            Debug.LogError("<color=green>******DB MANAGER : GET ACCESS TOKEN with request : " + REQUEST_ACCESSTOKEN_WITH_REFRESHTOKEN + " *******</color>"); 
            yield return new WaitForSeconds(0F);
            using (UnityWebRequest lRequestToken = UnityWebRequest.Post(REQUEST_ACCESSTOKEN_WITH_REFRESHTOKEN, ""))
            {
                yield return lRequestToken.SendWebRequest();
                if (lRequestToken.isHttpError || lRequestToken.isNetworkError)
                {
                    Debug.LogError("Request from GetAccessTokenWithRefreshToken error " + lRequestToken.error + " " + lRequestToken.downloadHandler.text);
                    TeleBuddyQuatreDeuxData.Instance.ConnectivityProblem = ConnectivityProblem.DatabaseProblem;
                }
                else
                {
                    string lResultRequestToken = lRequestToken.downloadHandler.text;
                    Debug.LogError("<color=green>******DB MANAGER : RESULT ACCESS TOKEN request : " + lResultRequestToken + " *******</color>");

                    try
                    {
                        mTokenData = Utils.UnserializeJSON<TokenData>(lResultRequestToken);
                        Debug.LogError("<color=green>******DB MANAGER : RESULT token data class : Access data : " + mTokenData.Access_Token + " API DOMAIN : " + mTokenData.Api_Domain + " EXPIRE IN : " + mTokenData.Expires_In + " REFRESH TOKEN : " + mTokenData.Refresh_Token + " TOKEN TYPE : " + mTokenData.Token_Type + " *******</color>");
                        
                        if (!string.IsNullOrEmpty(mTokenData.Access_Token))
                        {
                            //ZOHO V2 voir si on doit envoyer en param le refresh token
                            mAccessToken = mTokenData.Access_Token;
                            Debug.LogError(" ACCESS TOKEN " + mAccessToken);
                            StartCoroutine(GetUserIdFromUID(Buddy.Platform.RobotUID, mAccessToken));
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
        /// Connection to the DB. Search for the robot name and all the users of this robot then launch an other coroutine.
        /// </summary>
        /// <param name="iRobotUID">UID of the robot</param>
        /// <returns></returns>
        private IEnumerator GetUserIdFromUID(string iRobotUID, string iToken)
        {
            Debug.LogError("DB MANAGER : FIRST REQUEST");
            yield return new WaitForSeconds(2F);
            mDeviceUserLiaisonList.Clear();
           // string lRequest = ROOT_LINK_DB + iToken + GET_ALL_LIAISON + '"' + iRobotUID + '"';
            //string uidSC = "0743B88DAFFC511AE3BA";
            string lRequest2 = "https://creator.zoho.eu/api/v2/bluefrogrobotics/flotte/report/all_liaison_device_user?criteria=Device.Uid==" + '"' + iRobotUID/*uidSC*/ + '"';

            Debug.LogError("Request from GetUserIdFromUID LREREQUEST :" + lRequest2);
            using (UnityWebRequest lRequestDevice = UnityWebRequest.Get(lRequest2))
            {
                lRequestDevice.SetRequestHeader("Authorization", "Zoho-oauthtoken " + iToken);
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
                        Debug.LogError("<color=red>GetInfoFromUID 1</color>");
                        DeviceUserLiaisonList devices = Utils.UnserializeJSON<DeviceUserLiaisonList>(lRes);
                        Debug.LogError("<color=red>GetInfoFromUID 2 : " + devices.data.Length +  "</color>");
                        mRobotName = devices.data[0].DeviceNom;

                        if (devices != null)
                        {
                            Debug.LogError("<color=red>GetInfoFromUID 3</color>");
                            for (int i = 0; i < devices.data.Length; ++i)
                            {
                                mDeviceUserLiaison = devices.data[i];
                                mDeviceUserLiaisonList.Add(mDeviceUserLiaison);
                            }
                            Debug.LogError("<color=red>GetInfoFromUID 4</color>");
                            if (mDeviceUserLiaisonList.Count > 0)
                            {
                                Debug.LogError("<color=red>GetInfoFromUID 5</color>");
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
        /// Search for all the informations linked to all the users of this robot.
        /// </summary>
        /// <param name="iListDeviceUserLiaison">List of user for the robot</param>
        /// <returns></returns>
        private IEnumerator GetInfoForUsers(List<DeviceUserLiaison> iListDeviceUserLiaison)
        {
            //string lRequest = GET_USER_TABLET;
            string lRequest2 = "https://creator.zoho.eu/api/v2/bluefrogrobotics/flotte/report/all_liaison_device_user?criteria=User.idUser==";
            Debug.LogError(" ACCESS TOKEN GET INFO FOR USER" + mAccessToken);
            if (iListDeviceUserLiaison.Count == 1)
            {
                lRequest2 += iListDeviceUserLiaison[0].UserIdUser.ToString();

                Debug.LogError("<color=green>REQUEST DEUXIEME : " + lRequest2 + "</color>");
            }
            else
            {
                lRequest2 += iListDeviceUserLiaison[0].UserIdUser.ToString();
                for (int i = 1; i < iListDeviceUserLiaison.Count; ++i)
                {
                    if (!string.IsNullOrEmpty(iListDeviceUserLiaison[i].UserIdUser))
                    {
                        lRequest2 += "||User.idUser==" + iListDeviceUserLiaison[i].UserIdUser;
                    }
                }
            }
            Debug.LogError("<color=green>REQUEST DEUXIEME fin : " + lRequest2 + "</color>");
            using (UnityWebRequest lRequestDevice = UnityWebRequest.Get(lRequest2))
            {
                lRequestDevice.SetRequestHeader("Authorization", "Zoho-oauthtoken " + mAccessToken);
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
                        Debug.LogError("<color=red>GetInfoForUsers 1</color>");
                        DeviceUserLiaisonList devices = Utils.UnserializeJSON<DeviceUserLiaisonList>(lRes);
                        Debug.LogError("<color=red>GetInfoForUsers 2</color>");
                        if (devices != null) 
                        {
                            Debug.LogError("<color=red>GetInfoForUsers 3</color>");
                            foreach (DeviceUserLiaison lLiaison in devices.data)
                            {
                                if (lLiaison.DeviceType_device.Contains(TABLET_TYPE_DEVICE))
                                {
                                    mListTabletUser.Add(lLiaison);
                                }
                                if(lLiaison.DeviceType_device.Contains("Robot") && !string.IsNullOrEmpty(mRobotName) && mRobotName.Equals(lLiaison.DeviceNom))
                                {
                                    mRobotTokenRTC = lLiaison.DeviceRTC;
                                    mRobotTokenRTM = lLiaison.DeviceRTM;
                                    mListRobotUser.Add(lLiaison);
                                }
                            }
                            Debug.LogError("<color=red>GetInfoForUsers 4</color>");
                            foreach (DeviceUserLiaison lDeviceUserLiaison in mListRobotUser)
                            {
                                UserStudent = new User();
                                UserStudent.Nom = lDeviceUserLiaison.UserNom;
                                UserStudent.Prenom = lDeviceUserLiaison.UserPrenom;
                                string[] lOrgaSplit = lDeviceUserLiaison.UserOrganisme.Split('-');
                                UserStudent.Organisme = lOrgaSplit[1].Trim();
                                for (int i = 0; i < lDeviceUserLiaison.PlanningInfos.Count; ++i)
                                {
                                    if (i == 0)
                                    {
                                        UserStudent.Planning = lDeviceUserLiaison.PlanningInfos[i].display_value;
                                    }
                                    else
                                    {
                                        UserStudent.Planning += "," + lDeviceUserLiaison.PlanningInfos[i].display_value;
                                    }
                                    Debug.LogError("PLANNING TEST : " + UserStudent.Planning);

                                }


                                //UserStudent.Planning = lDeviceUserLiaison.PlanningidPlanning;
                                UserStudent.NeedPlanning = lDeviceUserLiaison.DeviceNeedPlanning;
                                UserStudent.RTCToken = lDeviceUserLiaison.DeviceRTC;
                                UserStudent.RTMToken = lDeviceUserLiaison.DeviceRTM;
                                UserStudent.AppID = lDeviceUserLiaison.DeviceAppID;

                                Debug.LogError(" ###################################### RTC TOKEN : " + UserStudent.RTCToken + " RTM TOKEN : " + UserStudent.RTMToken + " app id : " + UserStudent.AppID + " planning user : " + UserStudent.Planning);
                                ListUserStudent.Add(UserStudent);
                            }
                            foreach(DeviceUserLiaison lDeviceUserLiaison in mListTabletUser)
                            {
                                ListUIDTablet.Add(lDeviceUserLiaison.DeviceUid);

                            }
                            Debug.LogError("<color=red>GetInfoForUsers 5</color>");
                            if (mListTabletUser.Count > 0 && ListUserStudent.Count > 0)
                            {
                                Debug.LogError("<color=red>GetInfoForUsers 5.1 mListTabletUser.Count : " + mListTabletUser.Count + " ListUserStudent.Count :  " + ListUserStudent.Count + "</color>");
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
            string lPlanningString = "";
            foreach (DeviceUserLiaison lLiaison in mListRobotUser)
            {
                if (!string.IsNullOrEmpty(lLiaison.UserNom) && !string.IsNullOrEmpty(lLiaison.UserPrenom) && lLiaison.UserNom == iName && lLiaison.UserPrenom == iFirstName)
                {
                    for (int i = 0; i < lLiaison.PlanningInfos.Count; ++i)
                    {
                        if (i == 0)
                        {
                            lPlanningString = lLiaison.PlanningInfos[i].display_value;
                        }

                        lPlanningString += "," + lLiaison.PlanningInfos[i].display_value;
                    }

                    if (string.IsNullOrEmpty(lPlanningString) || lPlanningString == " " || lPlanningString == "")
                    {
                        TeleBuddyQuatreDeuxData.Instance.AllPlanning.Add(" ");
                    }
                    Debug.LogError("FILL PLANNING TEST PLANNING : " + lPlanningString);
                    if (lPlanningString.Contains(",") || !lLiaison.DeviceNeedPlanning)
                    {
                        string[] lAllPlanning;
                        if (!lLiaison.DeviceNeedPlanning)
                        {
                            List<string> lBuffer = new List<string>();
                            DateTime lDateTimeNow = DateTime.Now;
                            lBuffer.Add("000&" + lDateTimeNow + "&" + lDateTimeNow.AddDays(100) + "&");
                            lAllPlanning = lBuffer.ToArray();
                        }
                        else
                            lAllPlanning = lPlanningString.Split(',');

                        int index = 0;
                        lAllPlaningList.AddRange(lAllPlanning);
                        foreach (string lPlanning in lAllPlaningList)
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
                        if (lSortDateTime.Count > 1)
                            lSortDateTime.Sort(delegate (DateTime d1, DateTime d2) { return DateTime.Compare(d1, d2); });

                        for (int j = 0; j < lSortDateTime.Count; ++j)
                        {
                            for (int i = 0; i < lAllPlanning.Length; ++i)
                            {
                                if (lAllPlanning[i].Replace("-", "/").Contains(lSortDateTime[j].ToString()))
                                {
                                    TeleBuddyQuatreDeuxData.Instance.AllPlanning.Add(lAllPlanning[i]);
                                }
                            }

                        }
                        if (TeleBuddyQuatreDeuxData.Instance.AllPlanning.Count > 0)
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
                            TeleBuddyQuatreDeuxData.Instance.AllPlanning.Add(lPlanningString);

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
                PlanningInfo mCurrentPlanning = new PlanningInfo();
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
                        mDisplayBeforeEnd = false;
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
