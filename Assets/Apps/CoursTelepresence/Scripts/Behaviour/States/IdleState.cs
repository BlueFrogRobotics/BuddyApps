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
            Debug.LogError("Idle state");
            mCallButton.gameObject.SetActive(true);
            GameObject NameStudent = GetGameObject(14).transform.GetChild(0).GetChild(0).gameObject;
            GameObject FirstNameStudent = GetGameObject(14).transform.GetChild(0).GetChild(1).gameObject;
            Debug.LogError("Idle state 1");
            GameObject ClassStudent = GetGameObject(14).transform.GetChild(1).GetChild(0).gameObject;
            Debug.LogError("Idle state 2");
            NameStudent.GetComponent<Text>().text = DBManager.Instance.UserStudent.Nom;// + " " + DBManager.Instance.UserStudent.Prenom;
            FirstNameStudent.GetComponent<Text>().text = DBManager.Instance.UserStudent.Prenom;
            Debug.LogError("Idle state 3");
            ClassStudent.GetComponent<Text>().text = " - " + DBManager.Instance.UserStudent.Organisme;
            Debug.LogError("Idle state 4");
            mRTMManager.OncallRequest = (CallRequest lCall) => { Trigger("INCOMING CALL"); };
            Debug.LogError("Idle state 5");

            mRTMManager.OncallRequest = (CallRequest lCall) => { Trigger("INCOMING CALL"); };
            Debug.LogError("Idle state 6");

            // Manage trigger and vocal
           Buddy.Vocal.EnableTrigger = true;

            mCallButton.onClick.AddListener(LaunchCall);
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