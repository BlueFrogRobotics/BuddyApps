using BlueQuark;

using BlueQuark.Remote;

using UnityEngine;

using UnityEngine.UI;

using UnityEngine.Networking;

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
        private Webrtc mWebRtc;

        [SerializeField]
        private Text mUserCalling = null;

        [SerializeField]
        private AudioClip mMusicCall;

        public bool mCallIsInProgress;

        private bool mIncomingCallHandled;
        private bool mCallStoped;

        // Custom Toast - UI to manage the call, and view all video feedback
        [SerializeField]
        private GameObject mCallView;

        public void backToLobby()
        {
            if (mWebRtc.ConnectionState == Webrtc.CONNECTION.CONNECTING)
                mWebRtc.HangUp();
            AAppActivity.QuitApp();
        }

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            ////TODO : faire la gestion des droits pour un appel entrant
            //// il faut lire la liste des users, l'enregistrer après acceptation de l'appel, si changement des autorisations
            //// Faire une fonction qui confirme et check tout au lieu des trigger toussa toussa

            mCallIsInProgress = false;
            mIncomingCallHandled = false;
            mCallStoped = false;

            // CallView in custom toast
            if (!mCallView) {
                Debug.LogError("Please add reference to CallView_customToast");
                return;
            }
        }

        // RemoteControl mode, Display the UI that manage the call & active the WebRTC object.
        public void LaunchCall()
        {
            Buddy.Vocal.StopAndClear();
            mCallIsInProgress = true;
            mWebRtc.gameObject.SetActive(true);
            Buddy.GUI.Toaster.Hide();

            // Display the custom prefab to manage the call
            Buddy.GUI.Toaster.Display<CustomToast>().With(mCallView,
            () => {
                // On Display, Launch the display animation of the custom toast
                if (RemoteControlData.Instance.RemoteMode == RemoteControlData.AvailableRemoteMode.REMOTE_CONTROL)
                    mCallView.GetComponent<Animator>().SetTrigger("Open_WCall");
                else if (RemoteControlData.Instance.RemoteMode == RemoteControlData.AvailableRemoteMode.TAKE_CONTROL)
                    mCallView.GetComponent<Animator>().SetTrigger("Open_WCall");
            }, null);
        }

        // Wizard Of Oz mode, Just active the WebRTC object.
        public void LaunchCallWithoutWindow()
        {
            mWebRtc.gameObject.SetActive(true);
        }

        // Button stop the call, during the call.
        public void StopCall()
        {
            if (mCallStoped)
                return;

            mCallStoped = true;
            StopAllCoroutines();
            AAppActivity.QuitApp();
        }

        // Button Answer the call of the custom toast
        public void PressedYes()
        {
            if (mIncomingCallHandled)
                return;

            mIncomingCallHandled = true;
            Debug.Log("AcceptCallWithButton");
            Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            LaunchCall();
        }

        // Button Reject the call of the custom toast
        public void PressedNo()
        {
            if (mIncomingCallHandled)
                return;

            mIncomingCallHandled = true;
            Debug.Log("RejectCallWithButton");
            Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            // A Toaster hide request is present in the OnQuit function.
            AAppActivity.QuitApp();
        }

        public IEnumerator Call()
        {
            if (!RemoteControlData.Instance.DiscreteMode) {
                yield return new WaitForSeconds(1.5F);
                string lReceiver = "";
                UserAccount[] lUsers = Buddy.Platform.Users.GetUsers();
                foreach (UserAccount lUser in lUsers) {
                    if (WebRTCListener.RemoteID.Trim() == lUser.Email) {
                        lReceiver = lUser.FirstName;
                    }
                }
                string lTextToSay = "[user]";
                if (string.IsNullOrEmpty(lReceiver)) {
                    lTextToSay = lTextToSay.Replace("[user]", WebRTCListener.RemoteID);
                    if (mUserCalling)
                        mUserCalling.text = WebRTCListener.RemoteID;
                } else {
                    lTextToSay = lTextToSay.Replace("[user]", lReceiver);
                    if (mUserCalling)
                        mUserCalling.text = lReceiver;
                }
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