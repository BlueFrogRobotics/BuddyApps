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

        private bool mQuit;
        private bool mHasInitializedRemote;

        private RemoteControlBehaviour mRemoteControlBehaviour;

        // Custom Toast - UI to choose between accept or reject the call
        private GameObject mCustomCapsuleToast;

        public override void Start()
        {
            mRemoteControlBehaviour = GetComponent<RemoteControlBehaviour>();
            // Get the custom capsule toast
            if (!(mCustomCapsuleToast = GetGameObject(0)))
                Debug.LogError("Please add reference to CallManager_customCapsuleToast, in GameObjects list in AIBehaviour.");
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Ask new request");
            mListening = false;
            mSpeechReco = "";

            mQuit = false;
            mHasInitializedRemote = false;

            StartCoroutine(ActivateDisplay());

            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("receivingcall"));

            //RemoteControlData.Instance.CallViewAnim = mCustomCapAnim;

            Buddy.GUI.Toaster.Display<CustomToast>().With(mCustomCapsuleToast,
            () => {
                // On Display, Launch the display animation of the custom toast
                mCustomCapsuleToast.GetComponent<Animator>().SetTrigger("Select");
            }, () => {
                // On Hide
                Debug.Log("----- ON HIDE ----");
                Buddy.GUI.Header.HideTitle();
            });
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Buddy.Vocal.IsSpeaking || mListening || !mHasInitializedRemote)
                return;
            else if (mQuit) {
                Debug.Log("------------- STOP CALL ----------------");
                mRemoteControlBehaviour.StopCall();
                Debug.Log("------------- AFTER STOP CALL ----------------");
            }

            if (string.IsNullOrEmpty(mSpeechReco)) {
                Debug.Log("------------- LAUNCH RECO ----------------");
                //Buddy.Vocal.Listen("common", OnSpeechReco);
                mListening = true;
                Buddy.Behaviour.SetMood(Mood.LISTENING);
                return;
            }
            // On accept call reco
            if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings("yes"))) {
                Debug.Log("------------- RECO: yes ----------------");
                AcceptCall();
            }
            // On reject call reco
            else if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings("no"))) {
                Debug.Log("------------- RECO: no ----------------");
                RejectCall();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            mSpeechReco = "";
            mListening = false;
        }

        private void OnSpeechReco(SpeechInput iSpeechInput)
        {
            Debug.Log("----- iSpeechInput: " + iSpeechInput.Utterance);
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);

            mSpeechReco = iSpeechInput.Utterance;

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "yes")
                Debug.Log("--------- YES RULE ---------");
            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "no")
                Debug.Log("--------- NO RULE ---------");
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