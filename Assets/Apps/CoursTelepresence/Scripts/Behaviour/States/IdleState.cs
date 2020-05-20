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

            mCallButton = GetGameObject(10).GetComponentInChildren<Button>();

            Buddy.Vocal.DefaultInputParameters.Grammars = new string[1] { "grammar" };
            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
            Buddy.Vocal.DefaultInputParameters.RecognitionThreshold = 5000;
            Buddy.Vocal.OnTrigger.Add((lHotWord) => Buddy.Vocal.Listen("grammar", OnEndListen, SpeechRecognitionMode.GRAMMAR_ONLY));

        }


        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.LogError("Idle state");

            mRTMManager.OncallRequest = (CallRequest lCall) => { Trigger("INCOMING CALL"); };

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
            Buddy.Vocal.EnableTrigger = false;
            mRTMManager.OncallRequest = null;
            mCallButton.onClick.RemoveAllListeners();
            Debug.LogError("Idle state exit");
        }
    }

}