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
        DatabaseProblem,
        None
    }

    public class CheckConnectivity : MonoBehaviour
    {
        private const float REFRESH_TIME = 3F;

        private Action<bool/*, bool*/> OnRequestConnectionWifiOrMobileNetwork;
        private Action<bool> OnNetworkWorking;
        private Action<bool> OnRequestDatabase;
        private float mRefreshTime;

        private bool mRequestDone;
        private bool UIDatabaseDisplayed;
        private float mTimerUIDatabase;

        // Use this for initialization
        void Start()
        {
            CoursTelepresenceData.Instance.InitializeDone = false;
            UIDatabaseDisplayed = false;
            mTimerUIDatabase = 0F;
            mRequestDone = false;
            if (!Buddy.IO.WiFi.CurrentWiFiNetwork.Connected)
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.WifiProblem;
            OnRequestConnectionWifiOrMobileNetwork += CheckWifiAndMobileNetwork;
            OnNetworkWorking += CheckNetwork;
            if (OnRequestConnectionWifiOrMobileNetwork != null)
                OnRequestConnectionWifiOrMobileNetwork(Buddy.IO.WiFi.CurrentWiFiNetwork.Connected/*, true*/);
            OnRequestDatabase += CheckDatabase;
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
            if(UIDatabaseDisplayed && mTimerUIDatabase > 10F)
            {
                UIDatabaseDisplayed = false;
                Buddy.GUI.Toaster.Hide();
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.NetworkProblem;
            }

            if ((mRefreshTime > REFRESH_TIME || mRequestDone) && CoursTelepresenceData.Instance.ConnectivityProblem != ConnectivityProblem.None)
            {
                switch (CoursTelepresenceData.Instance.ConnectivityProblem)
                {
                    case ConnectivityProblem.WifiProblem:
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
                    case ConnectivityProblem.None:
                        break;
                    case ConnectivityProblem.DatabaseProblem:
                        if(OnRequestDatabase != null)
                            OnRequestDatabase(DBManager.Instance.InfoRequestedDone);
                        break;
                    default:
                        break;

                }
            }
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
        private void CheckDatabase(bool iDatabaseconnected)
        {
            UIDatabaseDisplayed = true;
            mTimerUIDatabase = 0F;
            if (!Buddy.GUI.Toaster.IsBusy && !iDatabaseconnected)
            {
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetLabel(Buddy.Resources.GetString("educonnectingdb"));
                });
            }
            else if (iDatabaseconnected)
            {
                CoursTelepresenceData.Instance.InitializeDone = true;
                Buddy.GUI.Toaster.Hide();
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.None;
            }
        }
    }

}
