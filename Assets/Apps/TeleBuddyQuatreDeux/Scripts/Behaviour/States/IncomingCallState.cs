using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using System.Collections;
using UnityEngine.UI;

namespace BuddyApp.TeleBuddyQuatreDeux
{

    public sealed class IncomingCallState : AStateMachineBehaviour
    {
        private const float REPEAT_TIME = 60F;

        private AudioClip mMusic;
        private Text mCallingUserName;
        private Text mCallingUserClass;
        private Button mRefuseButton;
        private Button mAcceptButton;

        private RTMManager mRTMManager;
        private RTCManager mRTCManager;
        private float mTimeRepeated;

        private bool mHasExited;

        override public void Start()
        {
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
            TeleBuddyQuatreDeuxData.Instance.CurrentState = TeleBuddyQuatreDeuxData.States.INCOMMING_CALL_STATE;
            mHasExited = false;

            mTimeRepeated = Time.time;
            if (mRTMManager.mCallRequest == null || string.IsNullOrWhiteSpace(mRTMManager.mCallRequest.userName))
                mCallingUserName.text = "Inconnu";
            else
                mCallingUserName.text = mRTMManager.mCallRequest.userName;
            //VOCON
            Buddy.Vocal.StopAndClear();
            Buddy.Actuators.Speakers.Media.Play(mMusic);
            //VOCON
            Buddy.Vocal.SayAndListen("tu as un appel de " + mCallingUserName.text + " veux-tu décrocher?", null,OnSpeechReco, null);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (!mHasExited && Time.time - mTimeRepeated > REPEAT_TIME)
            {
                RejectCall();
                mHasExited = true;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Toaster.Hide();
            //VOCON
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            Buddy.Actuators.Speakers.Media.Stop();
        }

        private void OnSpeechReco(SpeechInput iSpeechInput)
        {
            if (mHasExited || iSpeechInput.IsInterrupted)
                return;

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "yes")
                AcceptCall();
            else if (Utils.GetRealStartRule(iSpeechInput.Rule) == "no")
            {
                RejectCall();
            }
            else
            {
                RestartListenning();
            }
        }

        private void RestartListenning()
        {
            if (Time.time - mTimeRepeated <= REPEAT_TIME) {
                if (!Buddy.Actuators.Speakers.Media.IsBusy)
                    Buddy.Actuators.Speakers.Media.Play(mMusic);
                //VOCON
                Buddy.Vocal.SayAndListen("tu as un appel de " + mCallingUserName.text + " veux-tu décrocher?", null, OnSpeechReco, null);

            } else {
                Buddy.GUI.Notifier.Display<SimpleNotification>().With("Appel manqué en provenance de " + mCallingUserName.text);
                RejectCall();
            }
        }

        private void OnSpeechError(SpeechInputStatus iSpeechInputStatus)
        {
            if (iSpeechInputStatus.IsError) {
                RestartListenning();
            }
        }

        private void RejectCall()
        {
            Trigger("IDLE");
            Buddy.GUI.Toaster.Hide();
            //VOCON
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            Buddy.Actuators.Speakers.Media.Stop();
            mRTMManager.AnswerCallRequest(false);
        }

        private void AcceptCall()
        {
            Trigger("CALL");

            Buddy.GUI.Toaster.Hide();
            //VOCON
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            Buddy.Actuators.Speakers.Media.Stop();
            mRTMManager.AnswerCallRequest(true);
            mRTCManager.Join(mRTMManager.mCallRequest.channelId);
        }
    }
}
