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
        private const int RECOGNITION_SENSIBILITY = 5000;
        private const int REPEAT_CALL = 5;

        [SerializeField]
        private AudioClip mMusicCall;

        private IEnumerator mRepeatNotification;
        private bool mAcceptCallVocally = false;

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
            // Setting of Vocon param
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters()
            {
                RecognitionThreshold = RECOGNITION_SENSIBILITY
            };
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add(OnSpeechReco);

            // Start Call coroutine & Notification repeater
            StartCoroutine(ActivateDisplay());
            mRepeatNotification = RepeatNotification(0);
            StartCoroutine(mRepeatNotification);

            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("receivingcall"));

            Buddy.GUI.Toaster.Display<CustomToast>().With(mCustomCapsuleToast,
            () =>
            {
                // On Display, Launch the display animation of the custom toast
                mCustomCapsuleToast.GetComponent<Animator>().SetTrigger("Select");
            }, () =>
            {
                // On Hide
                Debug.Log("----- ON HIDE ----");
                Buddy.GUI.Header.HideTitle();
            });
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            StopCoroutine(mRepeatNotification);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mAcceptCallVocally)
            {
                Buddy.Vocal.StopAndClear();
                Buddy.Actuators.Speakers.Media.Stop();
                StopCoroutine(mRepeatNotification);
                mRemoteControlBehaviour.LaunchCall();
                mAcceptCallVocally = false;
            }
        }

        private void OnSpeechReco(SpeechInput iSpeechInput)
        {
            if (iSpeechInput.Confidence == -1)
                return;

            Buddy.Behaviour.SetMood(Mood.NEUTRAL);

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "yes")
                AcceptCall();
            else if (Utils.GetRealStartRule(iSpeechInput.Rule) == "no")
                RejectCall();
        }

        private void RejectCall()
        {
            Debug.Log("RejectCall");
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            mRemoteControlBehaviour.StopCall();
        }

        private void AcceptCall()
        {
            Debug.Log("AcceptCall");
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            mAcceptCallVocally = true;
        }

        /*
         *  This function launches a notification (Music + Vocal request) and repeats this several times.
         *  When a notification is launched:
         *  The music call is launched
         *  We warn user, with speech "incomingcall" and then we listen the user's answer. (yes/no)
         *  When the music ends, the process restarts
         */

        private IEnumerator RepeatNotification(int iRepeated)
        {
            // Coroutine is suspended until the music is playing.
            yield return new WaitUntil(() =>
            {
                // If the call was launched or discrete mode selected, stop the coroutine
                if (mRemoteControlBehaviour.mCallIsInProgress || RemoteControlData.Instance.DiscreteMode)
                {
                    Buddy.Vocal.StopAndClear();
                    Buddy.Actuators.Speakers.Media.Stop();
                    StopCoroutine(mRepeatNotification);
                    return true;
                }
                return !Buddy.Actuators.Speakers.Media.IsBusy;
            });

            // Wait 1.5 second before restart the notification
            yield return new WaitForSeconds(1.5F);

            // If actual number of repeat is less than REPEAT_CALL, relaunch notification and the actual coroutine ends.
            if (iRepeated <= REPEAT_CALL)
            {
                Buddy.Vocal.StopAndClear();
                Buddy.Actuators.Speakers.Media.Play(mMusicCall);
                Buddy.Vocal.SayKeyAndListen("incomingcall");
                // Store the new coroutine that will be launch
                mRepeatNotification = RepeatNotification(iRepeated + 1);
                StartCoroutine(mRepeatNotification);
            }
            // If the number max of repeats is reached, the app is closed.
            else
                mRemoteControlBehaviour.StopCall();
        }

        private IEnumerator ActivateDisplay()
        {
            yield return mRemoteControlBehaviour.Call();
        }
    }
}