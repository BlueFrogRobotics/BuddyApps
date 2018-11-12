﻿using UnityEngine;
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
                RemoteControlData.Instance.CustomToastIsBusy = true;
            }, () =>
            {
                // On Hide
                Debug.Log("----- ON HIDE ----");
                StartCoroutine(CloseReceivCall());
            });
        }

        public IEnumerator CloseReceivCall()
        {
            Debug.Log("----- UNSELECT ... ----");
            mCustomCapAnim.SetTrigger("Unselect");
            yield return new WaitUntil(() => { Debug.Log("----- CUSTOM IS NOT HIDE ----"); return mCustomCapAnim.GetCurrentAnimatorStateInfo(0).IsTag("customToastOff"); });
            RemoteControlData.Instance.CustomToastIsBusy = false;
            Debug.Log("----- CUSTOM IS HIDE ----");
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
            else if (mQuit)
            {
                StartCoroutine(mRemoteControlBehaviour.CloseApp());
            }

            if (string.IsNullOrEmpty(mSpeechReco))
            {
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
            //Buddy.Vocal.OnEndListening.Clear();
            //Buddy.Vocal.OnEndListening.Add(OnSpeechReco);
        }

        private bool ContainsOneOf(string iSpeech, string[] iListSpeech)
        {
            iSpeech = iSpeech.ToLower();
            for (int i = 0; i < iListSpeech.Length; ++i)
            {
                string[] words = iListSpeech[i].Split(' ');
                if (words.Length < 2)
                {
                    words = iSpeech.Split(' ');
                    foreach (string word in words)
                    {
                        if (word == iListSpeech[i].ToLower())
                        {
                            return true;
                        }
                    }
                }
                else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
                    return true;
            }
            return false;
        }

    }
}