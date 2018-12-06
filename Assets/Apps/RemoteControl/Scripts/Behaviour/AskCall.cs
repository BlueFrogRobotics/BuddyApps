using BlueQuark;

using BlueQuark.Remote;

using UnityEngine;

using UnityEngine.UI;

using System;

using System.Collections;

using System.Collections.Generic;

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
            () => {
                // On Display
                // Launch the display animation of the custom toast
                mCustomCapAnim.SetTrigger("Select");
                RemoteControlData.Instance.CustomToastIsBusy = true;
            }, () => {
                // On Hide
                Debug.Log("----- ON HIDE ----");
                //StartCoroutine(CloseReceivCall());
            });
        }

        // This coroutine purpose is to launch the hide animation of the custom toast
        // and wait until the animation is not finished, then a boolean is set, to unblock the OnQuit coroutine. (RemoteControlActivity.cs)
        // I thought this were enought to block the code in the OnQuit coroutine (RemoteControlActivity.cs), but it's not.
        public IEnumerator CloseReceivCall()
        {
            Debug.Log("----- UNSELECT ... ----");
            if (mCustomCapAnim)
                mCustomCapAnim.SetTrigger("Unselect");
            yield return new WaitUntil(() => {
                Debug.Log("----- CUSTOM IS NOT HIDE ----");
                if (!mCustomCapAnim)
                    return false;
                return mCustomCapAnim.GetCurrentAnimatorStateInfo(0).IsTag("customToastOff");
            });
            RemoteControlData.Instance.CustomToastIsBusy = false;
            Debug.Log("----- CUSTOM IS HIDE ----");

            // This code was a try to avoid black square on quit
            // I don't delete it, to get your feedback, and to show what i have already test
            //
            //{
            //    if (!mCustomCapAnim.GetCurrentAnimatorStateInfo(0).IsName("minder_roundblock_off") ||
            //        mCustomCapAnim.GetCurrentAnimatorStateInfo(0).IsName("toaster_roundblock_unselect -> minder_roundblock_off")) //IsTag("customToastOff"))
            //    {
            //        Debug.Log("----- CUSTOM IS NOT HIDE ----");
            //        return false;
            //    }
            //    else
            //    {
            //        RemoteControlData.Instance.CustomToastIsBusy = false;
            //        Debug.Log("----- CUSTOM IS HIDE ----");
            //        return true;
            //    }
            //});
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Buddy.Vocal.IsSpeaking || mListening || !mHasInitializedRemote)
                return;
            else if (mQuit) {
                Debug.LogError("------------- CLOSE APP ----------------");
                StartCoroutine(mRemoteControlBehaviour.CloseApp());

                Debug.LogError("------------- AFTER CLOSE APP ----------------");
            }

            if (string.IsNullOrEmpty(mSpeechReco)) {
                //Buddy.Vocal.Listen();
                mListening = true;
                Buddy.Behaviour.SetMood(Mood.LISTENING);
                return;
            }
            // On accept call reco
            if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings("yes"))) {
                Debug.LogError("------------- RECO: no ----------------");
                AcceptCall();
            }
            // On reject call reco
            else if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings("no"))) {
                Debug.LogError("------------- RECO: no ----------------");
                RejectCall();
            }
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
            Debug.LogError("------------- REJECT CALL ----------------");
            Debug.Log("RejectCall");
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            mQuit = true;
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