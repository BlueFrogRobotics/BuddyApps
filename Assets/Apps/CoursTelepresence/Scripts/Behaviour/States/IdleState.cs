using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{

    public sealed class IdleState : AStateMachineBehaviour
    {

        private RTMManager mRTMManager;
        private RTCManager mRTCManager;
        private Button mCallButton;
        private string mChannelId = "channeltest";
        private bool mAddListenerButtonCall;

        override public void Start()
        {
            // This returns the GameObject named RTMCom.
            mRTMManager = GetComponent<RTMManager>();
            mRTCManager = GetComponent<RTCManager>();
           
            mCallButton = GetGameObject(10).GetComponent<Button>();

            Buddy.Vocal.DefaultInputParameters.Grammars = new string[1] { "grammar" };
            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
            Buddy.Vocal.DefaultInputParameters.RecognitionThreshold = 5000;
            Buddy.Vocal.OnTrigger.Add((lHotWord) => Buddy.Vocal.Listen("grammar", OnEndListen, SpeechRecognitionMode.GRAMMAR_ONLY));

        }


        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mAddListenerButtonCall = false;
            mCallButton.gameObject.SetActive(true);
            Color lColor;
            lColor = mCallButton.GetComponent<Image>().color;
            lColor.a = 0.75F;
            mCallButton.GetComponent<Image>().color = lColor;
            GameObject NameStudent = GetGameObject(14).transform.GetChild(0).GetChild(0).gameObject;
            GameObject FirstNameStudent = GetGameObject(14).transform.GetChild(0).GetChild(1).gameObject;
            GameObject ClassStudent = GetGameObject(14).transform.GetChild(1).GetChild(0).gameObject;
            //TODO : DECOM TEST
            NameStudent.GetComponent<Text>().text = DBManager.Instance.UserStudent.Nom;
            FirstNameStudent.GetComponent<Text>().text = DBManager.Instance.UserStudent.Prenom;
            ClassStudent.GetComponent<Text>().text = " - " + DBManager.Instance.UserStudent.Organisme;
            mRTMManager.OncallRequest = (CallRequest lCall) => { Trigger("INCOMING CALL"); };

            mRTMManager.OncallRequest = (CallRequest lCall) => { Trigger("INCOMING CALL"); };

            // Manage trigger and vocal
           Buddy.Vocal.EnableTrigger = true;

            
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (DBManager.Instance.CanStartCourse && !mAddListenerButtonCall)
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
            Debug.LogWarning("Join channel " + mChannelId + " waiting for tablet answer");
            Trigger("CALLING");
            mRTCManager.Join(mChannelId);
            mRTMManager.RequestConnexion(mChannelId, Buddy.Platform.RobotUID);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mCallButton.gameObject.SetActive(false);
            Buddy.Vocal.EnableTrigger = false;
            mRTMManager.OncallRequest = null;
            mCallButton.onClick.RemoveAllListeners();
            Debug.LogError("Idle state exit");
        }
    }

}