using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using BlueQuark;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{
    public class UIManager : MonoBehaviour
    {
        // TODO : Check hour to know which display of the header we need to display
        // Check which message we need to send to the tablet to activate the call

        
        [SerializeField]
        private GameObject ConnectingScreen;

        [SerializeField]
        private GameObject DisplayCallFromStudent;


        /// <summary>
        /// Header when you call the student
        /// </summary>
        [SerializeField]
        private GameObject DisplayHeader;
        [SerializeField]
        private Text PingHeader;
        [SerializeField]
        private Button CallStudentButton;
        [SerializeField]
        private Text StudentName;
        [SerializeField]
        private Text StudentClass;


        [SerializeField]
        private Text Ping;

        [SerializeField]
        private Button EndCallButton;

        private RTMCom mRTMCom;

        [SerializeField]
        private Text BatteryLevel;

        private static UIManager mInstance = null;

        public static UIManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new UIManager();
                return mInstance as UIManager;
            }
        }

        // Use this for initialization
        void Start()
        {
            if(CallStudentButton != null)
                CallStudentButton.onClick.AddListener(delegate { ButtonCall("IMESSAGE TEST");  });
            if(EndCallButton != null)
                EndCallButton.onClick.AddListener(delegate { ButtonEndCall(); });
            //put student name from the db
            //mName = 
        }

        // Update is called once per frame
        void Update()
        {
            Ping.text = CoursTelepresenceData.Instance.Ping;
            PingHeader.text = CoursTelepresenceData.Instance.Ping;
            DisplayBatteryLevel();
        }

        private void ButtonCall(string iMessage)
        {
            //Code to call the student, link this to the call button from the header
            //Disable this gameobject with a trigger + enable loading screen with a trigger

            //string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            //Debug.Log("message: \n" + iMessage);
            //Debug.Log("Sent at " + timestamp);
            //Buddy.WebServices.Agoraio.SendPeerMessage(mRTMCom.mIdTablet, iMessage);
            Debug.Log("Test Button : " + iMessage);
        }

        private void ButtonEndCall()
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                TText lText = iBuilder.CreateWidget<TText>(); 
                lText.SetLabel("TEXTE A METTRE DANS LE DICO POUR CONFIRMATION DE LA DECO");
            }, () => {
                Debug.Log("Cancel");
                Buddy.GUI.Toaster.Hide();
            }, "Cancel",
            () => {
                Debug.Log("OK");
                Buddy.GUI.Toaster.Hide();
                mRTMCom.Logout();
                //Réafficher l'écran de démarrage
             }, "OK");
        }

        private void DisplayBatteryLevel()
        {
            //Waiting for visual to display image of a battery
            BatteryLevel.text = (SystemInfo.batteryLevel * 100F).ToString();
        }

    }

}
