using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

using BlueQuark;
using BlueQuark.Remote;
using System;
using System.Collections;

namespace BuddyApp.RemoteControl
{
    [Serializable]
    class RemoteUser
    {
        public string Email;
        public bool AlwaysAccept;
    }

    [Serializable]
    class RemoteUsers
    {
        public RemoteUser[] Users;
    }

    [RequireComponent(typeof(Animator))]

    /* A basic monobehaviour as "AI" behaviour for your app */
    public class RemoteControlBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private RemoteControlData mAppData;

        [SerializeField]
        private Animator mReceiveCallAnim;

        [SerializeField]
        private Animator callAnimator;

        [SerializeField]
        private Webrtc webRTC;

        [SerializeField]
        private Text userCalling = null;

        [SerializeField]
        private AudioClip musicCall;

        private bool mIncomingCallHandled;

        public void backToLobby()
        {
            if (webRTC.ConnectionState == Webrtc.CONNECTION.CONNECTING)
                webRTC.HangUp();
            AAppActivity.QuitApp();
        }

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            //callAnimator.SetTrigger("Open_WCall");
            //StartCoroutine(Call());
            //RemoteUsers lUserList = new RemoteUsers();

            //StreamReader lstreamReader = new StreamReader(BuddyTools.Utils.GetStreamingAssetFilePath("callRights.txt"));
            //string lTemp = lstreamReader.ReadToEnd();
            //lstreamReader.Close();

            //lUserList = JsonUtility.FromJson<RemoteUsers>(lTemp);
            //string lRemoteCaller = BuddyOS.Net.WebRTCListener.RemoteID;

            ////TODO : faire la gestion des droits pour un appel entrant
            //// il faut lire la liste des users, l'enregistrer après acceptation de l'appel, si changement des autorisations
            //// Faire une fonction qui confirme et check tout au lieu des trigger toussa toussa

            //mReceiveCallAnim.SetTrigger("Select");
            ////callAnimator.SetTrigger("Close_WCall");
            mIncomingCallHandled = true;
        }

        public void LaunchCall()
        {
            callAnimator.SetTrigger("Open_WCall");
            webRTC.gameObject.SetActive(true);
            // Launch the hide animation of the prefab toast inside the custom toast
            mReceiveCallAnim.SetTrigger("Unselect");
            // Hide the custom toast
            Buddy.GUI.Toaster.Hide();
        }

        public void StopCall()
        {
            if (!mIncomingCallHandled)
                return;

            mIncomingCallHandled = false;
            StartCoroutine(CloseApp());
        }

        // Hide all call object, (Feedback window, buddy face window, ...)
        // Wait until the hide animation is finished, (When the animator state is on Window_Call_Off, using a tag)
        public IEnumerator CloseApp()
        {
            callAnimator.SetTrigger("Close_WCall");
            yield return new WaitUntil(() => { return callAnimator.GetCurrentAnimatorStateInfo(0).IsTag("windowCallOff"); });
            AAppActivity.QuitApp();
        }

        public void PressedYes()
        {
            Debug.Log("AcceptCallWithButton");
            Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            LaunchCall();
        }

        public void PressedNo()
        {
            Debug.Log("RejectCallWithButton");
            Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            StartCoroutine(CloseApp());
        }

        public void GetIncomingCall()
        {
            if (mIncomingCallHandled)
                return;

            mIncomingCallHandled = true;
            mReceiveCallAnim.SetTrigger("Select");
        }

        public void LaunchCallWithoutWindow()
        {
            webRTC.gameObject.SetActive(true);
        }


        public IEnumerator Call()
        {
            if (!RemoteControlData.Instance.DiscreteMode)
            {
                Buddy.Actuators.Speakers.Media.Play(musicCall);
                yield return new WaitForSeconds(1.5F);
                string lReceiver = "";
                UserAccount[] lUsers = Buddy.Platform.Users.GetUsers();
                foreach (UserAccount lUser in lUsers)
                {
                    if (WebRTCListener.RemoteID.Trim() == lUser.Email)
                    {
                        lReceiver = lUser.FirstName;
                    }
                }
                string lTextToSay = "[user]";
                if (lReceiver == "")
                {
                    lTextToSay = lTextToSay.Replace("[user]", WebRTCListener.RemoteID);
                    if (userCalling)
                        userCalling.text = WebRTCListener.RemoteID;
                }
                else
                {
                    lTextToSay = lTextToSay.Replace("[user]", lReceiver);
                    if (userCalling)
                        userCalling.text = lReceiver;
                }
                Buddy.Vocal.Say(lTextToSay);

            }
            yield return null;
        }

        /// <summary>
        /// Will send a notification to the mobile app
        /// </summary>
        public void SendMessageFirebase()
        {
            Debug.Log("Send message");
            JSONNode lNode = new JSONObject();

            ///It's the key of the firebase server
            string lServerKey = "dZ416EcYA0s:APA91bFeYlrC6h5ykx6HN7cvYaDllWvaB_ZF5Iu7eHnZ48Vv4008x0293SQEnPbc8Eu54xYPPr3ynhcYce1XcZCFQSrIUJxZefukCTCXxMsmGKgE0-EG4t7f-0k8pePgsNXLMGHL2Fdw";
            lNode.Add("to", lServerKey);
            JSONNode lNotification = new JSONObject();
            lNotification.Add("title", "le titre");
            lNotification.Add("body", "le contenu");
            lNode.Add("notification", lNotification);

            Debug.Log("json a envoyer: " + lNode.ToString());

            ///TODO: get the token from a database
            string lDeviceToken = "AAAA6tk1qr0:APA91bEUIFMXzivaYQnPIzfFevwbeqtLgz_MUpzHNd4l3xIQiD6MTZJrTZBPnD7pEEhYYSjbla03pU41wDpiMcpfTg1klA5OQYEq6JWuXdK6ZeGLo6wRDcYKa03XNy3MeojOdPB1ioDN";
            StartCoroutine(PostRequest("https://fcm.googleapis.com/fcm/send", lNode.ToString(), lDeviceToken));
        }


        /// <summary>
        /// Sends a post request to the firebase server
        /// </summary>
        /// <param name="url">url of the google api</param>
        /// <param name="bodyJsonString">the json text to send</param>
        /// <param name="iDeviceToken">the device token. This should be saved in a database and associated with an account user</param>
        /// <returns></returns>
        IEnumerator PostRequest(string url, string bodyJsonString, string iDeviceToken)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "key=" + iDeviceToken);

            yield return request.Send();

            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
}