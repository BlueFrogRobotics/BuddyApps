using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;
using BlueQuark;

namespace BuddyApp.CoursTelepresence
{
    //edunetworknotworking
    //edutelepresencenetworknotworking

    public enum ConnectivityProblem
    {
        WifiProblem,
        NetworkProblem,
        TelepresenceServiceProblem,
        LaunchDatabase,
        DatabaseProblem,
        None
    }

    public class CheckConnectivity : MonoBehaviour
    {
        private const float REFRESH_TIME = 3F;

        private Action<bool/*, bool*/> OnRequestConnectionWifiOrMobileNetwork;
        private Action<bool> OnNetworkWorking;
        private Action OnRequestDatabase;
        private Action<bool> OnLaunchDatabase;
        public Action<int> OnErrorAgoraio { get; private set; }
        private float mRefreshTime;

        [SerializeField]
        private Animator Animator;

        private bool mRequestDone;
        private bool UIDatabaseDisplayed;
        private float mTimerUIDatabase;

        private static CheckConnectivity mInstance = null;
        public static CheckConnectivity Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new CheckConnectivity();
                return mInstance as CheckConnectivity;
            }
        }


        // Use this for initialization
        void Start()
        {
            CoursTelepresenceData.Instance.InitializeDone = false;
            UIDatabaseDisplayed = false;
            mTimerUIDatabase = 0F;
            mRequestDone = false;
            if (!Buddy.IO.WiFi.CurrentWiFiNetwork.Connected)
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.WifiProblem;
            OnRequestConnectionWifiOrMobileNetwork = CheckWifiAndMobileNetwork;
            OnNetworkWorking = CheckNetwork;
            if (OnRequestConnectionWifiOrMobileNetwork != null)
                OnRequestConnectionWifiOrMobileNetwork(Buddy.IO.WiFi.CurrentWiFiNetwork.Connected/*, true*/);
            OnRequestDatabase = CheckDatabase;
            OnLaunchDatabase = LaunchDatabase;
            OnErrorAgoraio = ErrorAgoraio;
            //StartCoroutine(CheckInternetConnection(/*(isConnected) => { if (isConnected) CoursTelepresenceData.Instance.ConnectedToInternet = true; else CoursTelepresenceData.Instance.ConnectedToInternet = false; }*/));
        }

        private void Update()
        {
            Debug.LogError("<color=blue> CONNECTIVITY PROBLEM : " + CoursTelepresenceData.Instance.ConnectivityProblem + "</color>");
            mRefreshTime += Time.deltaTime;
            mTimerUIDatabase += Time.deltaTime;
            if (!Buddy.IO.WiFi.CurrentWiFiNetwork.Connected)
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.WifiProblem;
            if(Buddy.IO.WiFi.CurrentWiFiNetwork.Connected && !Buddy.WebServices.HasInternetAccess)
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.NetworkProblem;
            //if(UIDatabaseDisplayed && mTimerUIDatabase > 10F)
            //{
            //    UIDatabaseDisplayed = false;
            //    Buddy.GUI.Toaster.Hide();
            //    CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.NetworkProblem;
            //}

            if ((mRefreshTime > REFRESH_TIME || mRequestDone) && CoursTelepresenceData.Instance.ConnectivityProblem != ConnectivityProblem.None)
            {
                switch (CoursTelepresenceData.Instance.ConnectivityProblem)
                {
                    case ConnectivityProblem.WifiProblem:
                        if (OnRequestConnectionWifiOrMobileNetwork != null)
                            OnRequestConnectionWifiOrMobileNetwork(Buddy.IO.WiFi.CurrentWiFiNetwork.Connected/*, true*/);
                        break;
                    case ConnectivityProblem.NetworkProblem:
                        if(OnNetworkWorking != null)
                            OnNetworkWorking(Buddy.WebServices.HasInternetAccess);
                        //mRequestDone = false;
                        //StartCoroutine(CheckInternetConnection(/*(isConnected) => { if (isConnected) CoursTelepresenceData.Instance.ConnectedToInternet = true; else CoursTelepresenceData.Instance.ConnectedToInternet = false; }*/));
                        break;
                    case ConnectivityProblem.TelepresenceServiceProblem:

                        break;
                    case ConnectivityProblem.LaunchDatabase:
                        if (OnLaunchDatabase != null)
                            OnLaunchDatabase(DBManager.Instance.LaunchDb);
                        break;
                    case ConnectivityProblem.DatabaseProblem:
                        if(OnRequestDatabase != null)
                            OnRequestDatabase();
                        break;
                    default:
                        break;

                }
            }
        }

        private void HideUi()
        {
            Buddy.GUI.Toaster.Hide();
            CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.LaunchDatabase;
        }

        public IEnumerator CheckInternetConnection(/*Action<bool> syncResult*/)
        {
            
            const string echoServer = "http://google.fr";

            bool result;
            while(true)
            {
                yield return new WaitForSeconds(0.2F);
                using (var request = UnityWebRequest.Head(echoServer))
                {
                    request.timeout = 5;
                    yield return request.SendWebRequest();
                    result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
                    mRequestDone = true;
                }
                if(OnNetworkWorking != null)
                    OnNetworkWorking(result);
            }
        }

        private void CheckWifiAndMobileNetwork(bool iConnected/*, bool iMobileNetwork*/)
        {
            if(!Buddy.GUI.Toaster.IsBusy && !iConnected/* && !iMobileNetwork*/)
            {
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetLabel(Buddy.Resources.GetString("eduwifi"));
                });
            }
            else if(iConnected)
            {
                Buddy.GUI.Toaster.Hide();
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.None;
            }
        }


        private void CheckNetwork(bool iNetworkWorking)
        {
            if(!Buddy.GUI.Toaster.IsBusy && !iNetworkWorking)
            {
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetLabel(Buddy.Resources.GetString("edunetworknotworking"));
                });
            }
            else if(iNetworkWorking)
            {
                Buddy.GUI.Toaster.Hide();
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.None;
            }
        }


        //Si c'est trop long -> repasser en vérification de la co
        private void CheckDatabase()
        {
            //UIDatabaseDisplayed = true;
            //mTimerUIDatabase = 0F;
            if (!Buddy.GUI.Toaster.IsBusy)
            {
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetLabel(Buddy.Resources.GetString("edudbproblem"));
                });
                this.Invoke("HideUI", 5F);
            }
            else
            {
                DBManager.Instance.IsRefreshButtonPushed = true;
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.LaunchDatabase;
                CoursTelepresenceData.Instance.InitializeDone = false;
                DBManager.Instance.StartDBManager();
                StartCoroutine(DBManager.Instance.RefreshPlanning());
                Animator.Play("CONNECTING");
            }
        }

        private void LaunchDatabase(bool iLaunchDatabase)
        {
            if (!Buddy.GUI.Toaster.IsBusy && iLaunchDatabase)
            {
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetLabel(Buddy.Resources.GetString("educonnectingdb"));
                });
            }
            else if (!iLaunchDatabase)
            {
                CoursTelepresenceData.Instance.InitializeDone = true;
                Buddy.GUI.Toaster.Hide();
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.None;
            }
        }

        private void ErrorAgoraio(int iIdError)
        {
            if((iIdError == 104 || iIdError == 106 || iIdError == 107) && !Buddy.GUI.Toaster.IsBusy)
            {
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetLabel(Buddy.Resources.GetString("edutelepresencenotworking"));
                });

                this.Invoke("HideUI", 3F);
                
            }
        }
    }

}
