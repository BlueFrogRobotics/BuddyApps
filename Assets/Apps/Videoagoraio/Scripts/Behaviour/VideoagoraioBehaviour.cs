using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Videoagoraio
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class VideoagoraioBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private VideoagoraioData mAppData;

        [SerializeField]
        private GameObject ObjVideoSurface;

        [SerializeField]
        private InputField channel;

        [SerializeField]
        private InputField id;

        [SerializeField]
        private InputField message;

        [SerializeField]
        private Text text;

        [SerializeField]
        private GameObject[] objects;

        private bool mIsShown;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			VideoagoraioActivity.Init(null);

            mIsShown = true;

            /*
			* Init your app data
			*/
            mAppData = VideoagoraioData.Instance;
            Buddy.WebServices.Agoraio.LoadEngine("dc949460a57e4fb0990a219b799ccf13");

        }

        public void Join()
        {
            Buddy.WebServices.Agoraio.OnUserConnected = onUserJoined;
            Buddy.WebServices.Agoraio.OnUserOffline = onUserOffline;
            Buddy.WebServices.Agoraio.Join(channel.text);
        }

        public void Leave()
        {
            Buddy.WebServices.Agoraio.Leave();
        }


        public void InitRTM()
        {
            Buddy.WebServices.Agoraio.InitRTM();
            Buddy.WebServices.Agoraio.OnMessage = OnMessage;
            text.text = "initialized";
        }

        public void Login()
        {
            Buddy.WebServices.Agoraio.Login(id.text);
            text.text = "logged";
        }

        public void SendMessage()
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            text.text = "message sent at " + timestamp;
            Buddy.WebServices.Agoraio.SendPeerMessage(id.text, message.text);
        }

        public void Send10Message()
        {
            for (int i = 0; i < 10; i++) {
                SendMessage();
            }
        }

        public void CreateChannel()
        {
            Buddy.WebServices.Agoraio.CreateChannel();
        }

        public void ClearText()
        {
            text.text = "";
        }

        public void SendChannelMessage()
        {
            Buddy.WebServices.Agoraio.SendChannelMessage(message.text);
        }

        public void Logout()
        {
            Buddy.WebServices.Agoraio.Logout();
            text.text = "logout";
        }

        public void ShowUI()
        {
            mIsShown = !mIsShown;
            foreach (GameObject obj in objects) {
                obj.SetActive(mIsShown);
            }
        }

        private void OnMessage(string iMessage)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            text.text += timestamp + " : " + iMessage + "\n";
            if (iMessage.Contains("forward"))
                Buddy.Actuators.Wheels.SetVelocities(0.4F, 0F);
            else if (iMessage.Contains("backward"))
                Buddy.Actuators.Wheels.SetVelocities(-0.4F, 0F);
            else if (iMessage.Contains("stop"))
                Buddy.Actuators.Wheels.ImmediateStop();
            else if (iMessage.Contains("left"))
                Buddy.Actuators.Wheels.SetVelocities(0F, 40F, AccDecMode.NORMAL);
            else if (iMessage.Contains("right"))
                Buddy.Actuators.Wheels.SetVelocities(0F, -40F, AccDecMode.NORMAL);

            //text.text += "\n";
        }

        private void onUserJoined(uint uid, int elapsed)
        {
            Debug.Log("[AGORAIO] onUserJoined: uid = " + uid + " elapsed = " + elapsed);

            VideoSurface lVideoSurface = ObjVideoSurface.AddComponent<VideoSurface>();
            lVideoSurface.SetForUser(uid);
            lVideoSurface.SetEnable(true);
            lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            lVideoSurface.SetGameFps(30);
        }

        private void onUserOffline(uint uid)
        {
            // remove video stream
            Debug.Log("[AGORAIO] onUserOffline: uid = " + uid);
            Destroy(ObjVideoSurface.GetComponent<VideoSurface>());
            ObjVideoSurface.GetComponent<RawImage>().texture = null;
 
        }
    }
}