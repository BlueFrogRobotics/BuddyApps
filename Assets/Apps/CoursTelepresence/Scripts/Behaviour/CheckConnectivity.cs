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
        private const float REFRESH_TIME = 5F;

        private Action<bool/*, bool*/> OnRequestConnectionWifiOrMobileNetwork;
        private Action<bool> OnNetworkWorking;
        private Action<bool> OnRequestDatabase;
        private float mRefreshTime;

        private bool mRequestDone;

        // Use this for initialization
        void Start()
        {
            CoursTelepresenceData.Instance.InitializeDone = false;
            mRequestDone = false;
            if (!Buddy.IO.WiFi.CurrentWiFiNetwork.Connected)
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.WifiProblem;
            OnRequestConnectionWifiOrMobileNetwork += CheckWifiAndMobileNetwork;
            if(OnRequestConnectionWifiOrMobileNetwork != null)
                OnRequestConnectionWifiOrMobileNetwork(Buddy.IO.WiFi.CurrentWiFiNetwork.Connected/*, true*/);
            OnRequestDatabase += CheckDatabase;
            //StartCoroutine(CheckInternetConnection(/*(isConnected) => { if (isConnected) CoursTelepresenceData.Instance.ConnectedToInternet = true; else CoursTelepresenceData.Instance.ConnectedToInternet = false; }*/));
        }

        private void Update()
        {
            mRefreshTime += Time.deltaTime;
            if (!Buddy.IO.WiFi.CurrentWiFiNetwork.Connected)
            {
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.WifiProblem;
            }

            if (mRefreshTime > REFRESH_TIME || mRequestDone)
            {
                switch (CoursTelepresenceData.Instance.ConnectivityProblem)
                {
                    case ConnectivityProblem.WifiProblem:
                        OnRequestConnectionWifiOrMobileNetwork(Buddy.IO.WiFi.CurrentWiFiNetwork.Connected/*, true*/);
                        break;
                    case ConnectivityProblem.NetworkProblem:
                        mRequestDone = false;
                        StartCoroutine(CheckInternetConnection(/*(isConnected) => { if (isConnected) CoursTelepresenceData.Instance.ConnectedToInternet = true; else CoursTelepresenceData.Instance.ConnectedToInternet = false; }*/));
                        break;
                    case ConnectivityProblem.TelepresenceServiceProblem:
                        break;
                    case ConnectivityProblem.None:
                        break;
                    case ConnectivityProblem.DatabaseProblem:
                        OnRequestDatabase(DBManager.Instance.InfoRequestedDone);
                        break;
                    default:
                        break;

                }
            }
        }

        public IEnumerator CheckInternetConnection(/*Action<bool> syncResult*/)
        {
            OnNetworkWorking += CheckNetwork;
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
                CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.NetworkProblem;
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

        private void CheckDatabase(bool iDatabaseconnected)
        {
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
