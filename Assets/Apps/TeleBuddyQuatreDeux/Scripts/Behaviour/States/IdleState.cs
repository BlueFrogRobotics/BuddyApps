using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;
using System.Text;
using System.Security.Cryptography;

namespace BuddyApp.TeleBuddyQuatreDeux
{

    public sealed class IdleState : AStateMachineBehaviour
    {

        private RTMManager mRTMManager;
        private RTCManager mRTCManager;
        private Button mCallButton;
        private string mChannelId = "channeltest";
        private bool mAddListenerButtonCall;

        private GameObject NameStudent;
        private GameObject FirstNameStudent;
        private GameObject ClassStudent;

        override public void Start()
        {
            mRTMManager = GetComponent<RTMManager>();
            mRTCManager = GetComponent<RTCManager>();
           
            mCallButton = GetGameObject(10).GetComponent<Button>(); 
            mChannelId = /*Buddy.Platform.RobotUID*/Buddy.IO.MobileData.IMEI()+RandomString(10);

            //VOCON
            //Buddy.Vocal.DefaultInputParameters.Grammars = new string[1] { "grammar" };
            //Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
            //Buddy.Vocal.DefaultInputParameters.RecognitionThreshold = 5000;
            //Buddy.Vocal.OnTrigger.Add((lHotWord) => Buddy.Vocal.Listen("grammar", OnEndListen, SpeechRecognitionMode.GRAMMAR_ONLY));
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            TeleBuddyQuatreDeuxData.Instance.CurrentState = TeleBuddyQuatreDeuxData.States.IDLE_STATE;
            Buddy.Behaviour.Mood = Mood.NEUTRAL;
            if (DBManager.Instance.ListUIDTablet.Count > 1)
            {
                GetGameObject(21).SetActive(true);
            }
            Buddy.GUI.Header.DisplayParametersButton(true);
            GetGameObject(17).SetActive(false);
            GetGameObject(20).SetActive(true);
            mAddListenerButtonCall = false;
            Debug.LogError("IDLE STATE : AVANT SETACTIVE BUTTON CALL");
            mCallButton.gameObject.SetActive(true);
            Color lColor;
            lColor = mCallButton.GetComponent<Image>().color;
            lColor.a = 0.1F;
            mCallButton.GetComponent<Image>().color = lColor;
            NameStudent = GetGameObject(14).transform.GetChild(0).GetChild(0).gameObject;
            FirstNameStudent = GetGameObject(14).transform.GetChild(0).GetChild(1).gameObject;
            ClassStudent = GetGameObject(14).transform.GetChild(1).GetChild(0).gameObject;
            int lIndexTab = /*mRTMManager.IndexTablet*/ TeleBuddyQuatreDeuxData.Instance.IndexTablet;
            NameStudent.GetComponent<Text>().text = DBManager.Instance.ListUserStudent[lIndexTab].Nom;
            FirstNameStudent.GetComponent<Text>().text = DBManager.Instance.ListUserStudent[lIndexTab].Prenom;
            ClassStudent.GetComponent<Text>().text = " - " + DBManager.Instance.ListUserStudent[lIndexTab].Organisme;
            mRTMManager.OncallRequest = (CallRequest lCall) => { Trigger("INCOMING CALL"); };

            Buddy.Vocal.EnableTrigger = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (DBManager.Instance.CanStartCourse && !mAddListenerButtonCall && mRTMManager.PingReceived)
            {
                mAddListenerButtonCall = true;
                Color lColor;
                lColor = mCallButton.GetComponent<Image>().color;
                lColor.a = 1F;
                mCallButton.GetComponent<Image>().color = lColor;
                mCallButton.onClick.AddListener(LaunchCall);
            }
        }

        private void OnEndListen(SpeechInput iSpeechInput)
        {
            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "callfriend")
               LaunchCall();
        }

        private void LaunchCall()
        {
            Trigger("CALLING");
            mRTCManager.Join(mChannelId);
            mRTMManager.RequestConnexion(mChannelId, DBManager.Instance.NameProf);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetGameObject(21).SetActive(false);
            Buddy.GUI.Header.DisplayParametersButton(false);
            mCallButton.gameObject.SetActive(false);
            //VOCON
            //Buddy.Vocal.EnableTrigger = false;
            mRTMManager.OncallRequest = null;
            mCallButton.onClick.RemoveAllListeners();
            //VOCON
            //Buddy.Vocal.OnTrigger.Clear();
            //Buddy.Vocal.ListenOnTrigger = false;
            ResetTrigger("IDLE");
        }

        private string RandomString(int iLength)
        {
            const string VALID = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder lRandom = new StringBuilder();
            using (RNGCryptoServiceProvider lRng = new RNGCryptoServiceProvider())
            {
                byte[] lUintBuffer = new byte[sizeof(uint)];

                while (iLength-- > 0)
                {
                    lRng.GetBytes(lUintBuffer);
                    uint lNum = BitConverter.ToUInt32(lUintBuffer, 0);
                    lRandom.Append(VALID[(int)(lNum % (uint)VALID.Length)]);
                }
            }
            return lRandom.ToString();
        }
    }


}