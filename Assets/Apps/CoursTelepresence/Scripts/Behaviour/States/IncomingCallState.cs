using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using System.Collections;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{

    public sealed class IncomingCallState : AStateMachineBehaviour
    {


        private const int RECOGNITION_SENSIBILITY = 5000;
        private const float REPEAT_TIME = 60F;


        // Custom Toast - UI to choose between accept or reject the call
        private AudioClip mMusic;
        private Text mCallingUserName;
        private Text mCallingUserClass;
        private Button mRefuseButton;
        private Button mAcceptButton;

        private RTMManager mRTMManager;
        private RTCManager mRTCManager;
        private float mTimeRepeated;

        override public void Start()
        {
            // This returns the GameObject named RTMCom.
            mRTCManager = GetComponent<RTCManager>();
            mRTMManager = GetComponent<RTMManager>();

            mCallingUserClass = GetGameObject(0).GetComponentInChildren<Text>();
            mCallingUserName = GetGameObject(1).GetComponentInChildren<Text>();

            mRefuseButton = GetGameObject(2).GetComponentInChildren<Button>();
            mRefuseButton.onClick.AddListener(RejectCall);

            mAcceptButton = GetGameObject(3).GetComponentInChildren<Button>();
            mAcceptButton.onClick.AddListener(AcceptCall);

            mMusic = Buddy.Resources.Get<AudioClip>("call_ring.wav");
        }


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.LogError("Incoming call state");

            mTimeRepeated = Time.time;

            // Setting of Vocon param
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters() {
                RecognitionThreshold = RECOGNITION_SENSIBILITY
            };

            // Start Call coroutine & Notification repeater

            if (mRTMManager.mCallRequest == null || string.IsNullOrWhiteSpace(mRTMManager.mCallRequest.userName))
                mCallingUserName.text = "Inconnu";
            else
                mCallingUserName.text = mRTMManager.mCallRequest.userName;



            Buddy.Vocal.StopAndClear();
            Buddy.Actuators.Speakers.Media.Play(mMusic);
            //Buddy.Vocal.SayKeyAndListen("incomingcall");

            Buddy.Vocal.SayAndListen("tu as un appel de " + mCallingUserName.text + " veux-tu décrocher?", null, OnSpeechReco, OnSpeechError);
            
           
        }



        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.LogError("Incoming call state exit");
            Buddy.GUI.Toaster.Hide();
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            Buddy.Actuators.Speakers.Media.Stop();
        }


        private void OnSpeechReco(SpeechInput iSpeechInput)
        {
            if (iSpeechInput.IsInterrupted)
                return;

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "yes")
                AcceptCall();
            else if (Utils.GetRealStartRule(iSpeechInput.Rule) == "no")
            {
                Debug.LogError("Incoming no");
                RejectCall();
            }
            else
            {
                Debug.LogWarning("Wrong rule, restart listenning");
                RestartListenning();
            }
        }

        private void RestartListenning()
        {
            if (Time.time - mTimeRepeated <= REPEAT_TIME) {
                if (!Buddy.Actuators.Speakers.Media.IsBusy)
                    Buddy.Actuators.Speakers.Media.Play(mMusic);
                Buddy.Vocal.SayAndListen("tu as un appel de " + mCallingUserName.text + " veux-tu décrocher?");

            } else {
                Buddy.GUI.Notifier.Display<SimpleNotification>().With("Appel manqué en provenance de " + mCallingUserName.text);
                Debug.LogError("Incoming restart");
                RejectCall();
            }
        }

        private void OnSpeechError(SpeechInputStatus iSpeechInputStatus)
        {
            if (iSpeechInputStatus.IsError) {
                Debug.LogWarning("Error, restart listenning");
                RestartListenning();
            }
        }

        private void RejectCall()
        {
            Debug.LogError("RejectCall");
            Trigger("IDLE");

            Buddy.GUI.Toaster.Hide();
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            Buddy.Actuators.Speakers.Media.Stop();
            mRTMManager.AnswerCallRequest(false);
        }

        private void AcceptCall()
        {
            Debug.Log("AcceptCall");
            Trigger("CALL");

            Buddy.GUI.Toaster.Hide();
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            Buddy.Actuators.Speakers.Media.Stop();
            mRTMManager.AnswerCallRequest(true);
            mRTCManager.Join(mRTMManager.mCallRequest.channelId);
            Debug.LogWarning("joining channel from incoming call " + mRTMManager.mCallRequest.channelId);
        }
    }
}
