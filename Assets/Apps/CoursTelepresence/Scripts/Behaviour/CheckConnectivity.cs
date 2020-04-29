using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace BuddyApp.CoursTelepresence
{
    public class CheckConnectivity : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            StartCoroutine(CheckInternetConnection((isConnected) => { if (isConnected) CoursTelepresenceData.Instance.ConnectedToInternet = true; else CoursTelepresenceData.Instance.ConnectedToInternet = false; }));
        }

        public static IEnumerator CheckInternetConnection(Action<bool> syncResult)
        {
            const string echoServer = "http://google.fr";

            bool result;
            while(true)
            {
                yield return new WaitForSeconds(2F);
                using (var request = UnityWebRequest.Head(echoServer))
                {
                    request.timeout = 5;
                    yield return request.SendWebRequest();
                    result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
                }
                syncResult(result);
            }
        }
    }

}
