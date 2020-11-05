using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Agoraio
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class AgoraioBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private AgoraioData mAppData;

        [SerializeField]
        private InputField id;

        [SerializeField]
        private InputField message;

        [SerializeField]
        private Text text;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			AgoraioActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = AgoraioData.Instance;
        }

        public void InitRTM()
        {
            //Buddy.WebServices.Agoraio.InitRTM();
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

        private void OnMessage(string iMessage)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
           // text.text +=  timestamp+" : "+iMessage+"\n";
            //if (iMessage.Contains("forward"))
            //    Buddy.Actuators.Wheels.SetVelocities(0.4F, 0F);
            //else if (iMessage.Contains("backward"))
            //    Buddy.Actuators.Wheels.SetVelocities(-0.4F, 0F);
            //else if (iMessage.Contains("stop"))
            //    Buddy.Actuators.Wheels.ImmediateStop();
            //else if (iMessage.Contains("left"))
            //    Buddy.Actuators.Wheels.SetVelocities(0F, 40F, AccDecMode.NORMAL, 0F, 45F);
            //else if (iMessage.Contains("right"))
            //    Buddy.Actuators.Wheels.SetVelocities(0F, 40F, AccDecMode.NORMAL, 0F, -45F);

            

            if (iMessage.Contains("stop"))
                Buddy.Actuators.Wheels.ImmediateStop();
            else
            {
                string[] mSplit = iMessage.Split('|');
                string[] mSecondSplit = mSplit[1].Split(',');
                text.text += timestamp + " : " + mSplit[0] + " " + (float.Parse(mSecondSplit[0]) / 10F).ToString() + "\n";
                Buddy.Actuators.Wheels.SetVelocities(float.Parse(mSecondSplit[0]) / 10F, float.Parse(mSplit[0]));
            }

            //text.text += "\n";
        }

    }
}