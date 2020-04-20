using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using UnityEngine.UI;
using System;
using System.Globalization;

namespace BuddyApp.CoursTelepresence
{
    public class CoursTelepresenceLanding : AStateMachineBehaviour
    {
        private string mIdRobot;
        private List<string> mListTablet;
        private string mTimeStampSendMessage;
        private Text mTextStudentNotConnected;
        private GameObject mButtonConnect;
        private float mRefreshTimer;
        private bool mTabletConnected;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mRefreshTimer = 0F;
            mTabletConnected = false;
            //Add this text in the dictionnary
            mTextStudentNotConnected.text = "Student not connected";
            //Connect to the db to retrieve the id of the robot
            mIdRobot = "PUT HERE THE ID OF THE ROBOT";
            //Connect to the db to retrieve the list of tablet
            mListTablet = new List<string>();
            //mListTablet = list from the tablet
            InitAgora(mIdRobot);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mRefreshTimer += Time.deltaTime;
            if(mRefreshTimer > 3F && !mTabletConnected)
            {
                mRefreshTimer = 0F;
                SendMessage(" ");
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            
        }

        private void InitAgora(string iId)
        {
            Buddy.WebServices.Agoraio.InitRTM();
            Buddy.WebServices.Agoraio.Login(iId);
            Buddy.WebServices.Agoraio.CreateChannel();
            mTimeStampSendMessage = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            SendMessage(" ");
        }

        private void SendMessage(string iMessage)
        {
            Buddy.WebServices.Agoraio.SendPeerMessage(mListTablet[0], iMessage);
        }
    }

}




//If timeout connexion, display “élève non connecté“
//Display disabled connexion button
//If no connexion, check tablet conection every 3 seconds