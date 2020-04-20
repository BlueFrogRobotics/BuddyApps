using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using BlueQuark;

namespace BuddyApp.CoursTelepresence
{
    public class UIManager : MonoBehaviour
    {
        // TODO : Check hour to know which display of the header we need to display
        // Check which message we need to send to the tablet to activate the call

        [SerializeField]
        private Animator ConnectingScreen;
        [SerializeField]
        private Animator DisplayCallFromStudent;
        [SerializeField]
        private Animator DisplayHeader;

        //Need to know if it will be an animator or just a text for the test 94
        [SerializeField]
        private Animator DisplayMessage;

        private RTMCom mRTMCom;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ButtonCall(string iMessage)
        {
            //Code to call the student, link this to the call button from the header

            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            Debug.Log("message: \n" + iMessage);
            Debug.Log("Sent at " + timestamp);
            Buddy.WebServices.Agoraio.SendPeerMessage(mRTMCom.mIdTablet, iMessage);
        }
    }

}
