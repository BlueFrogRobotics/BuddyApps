using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using BlueQuark;
using BlueQuark.Remote;

namespace BuddyApp.RemoteControl
{
    public class AskCall : AStateMachineBehaviour
    {
        private bool mListening;
        private string mSpeechReco;

        private int mError;
        private bool mQuit;
        private bool mHasInitializedRemote;

        private RemoteControlBehaviour mRemoteControlBehaviour;

        private GameObject mCustomCapsuleToast;
        private Animator mCustomCapAnim;

        public override void Start()
        {
            mRemoteControlBehaviour = GetComponent<RemoteControlBehaviour>();
            // Get the custom capsule toast
            mCustomCapsuleToast = GetGameObject(0);
            // Get the animator of the capsule toast
            mCustomCapAnim = mCustomCapsuleToast.GetComponent<Animator>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Ask new request");
            mError = 0;
            mListening = false;
            mSpeechReco = "";

            mQuit = false;
            mHasInitializedRemote = false;

            StartCoroutine(ActivateDisplay());

            Buddy.GUI.Toaster.Display<CustomToast>().With(mCustomCapsuleToast,
            () =>
            {
                // On Display
                // Launch the display animation of the custom toast
                mCustomCapAnim.SetTrigger("Select");
            }, () =>
            {
                // On Hide
            });
        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Buddy.Vocal.IsSpeaking || mListening || !mHasInitializedRemote)
                return;
            else if (mQuit) {
               StartCoroutine(mRemoteControlBehaviour.CloseApp());
            }

            if (string.IsNullOrEmpty(mSpeechReco)) {
                //Buddy.Vocal.Listen();
                mListening = true;
                Buddy.Behaviour.SetMood(Mood.LISTENING);
                return;
            }
            // On accept call reco
            if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings("yes")))
                AcceptCall();
            // On reject call reco
            else if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings("no")))
                RejectCall();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            mSpeechReco = "";
            mListening = false;
        }

        private void OnSpeechReco(SpeechInput iVoiceInput)
        {
            Debug.Log("reco vocale: " + iVoiceInput.Utterance);
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);

            mSpeechReco = iVoiceInput.Utterance;
            mListening = false;
        }

        private void RejectCall()
        {
            Debug.Log("RejectCall");
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            StartCoroutine(mRemoteControlBehaviour.CloseApp());
        }

        private void AcceptCall()
        {
            Debug.Log("AcceptCall");
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            mRemoteControlBehaviour.LaunchCall();
        }

        private IEnumerator ActivateDisplay()
        {
            yield return mRemoteControlBehaviour.Call();
            mHasInitializedRemote = true;
            //Buddy.Vocal.OnEndListening.Clear();
            //Buddy.Vocal.OnEndListening.Add(OnSpeechReco);
        }

        private bool ContainsOneOf(string iSpeech, string[] iListSpeech)
        {
            iSpeech = iSpeech.ToLower();
            for (int i = 0; i < iListSpeech.Length; ++i) {
                string[] words = iListSpeech[i].Split(' ');
                if (words.Length < 2) {
                    words = iSpeech.Split(' ');
                    foreach (string word in words) {
                        if (word == iListSpeech[i].ToLower()) {
                            return true;
                        }
                    }
                } else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
                    return true;
            }
            return false;
        }

    }
}