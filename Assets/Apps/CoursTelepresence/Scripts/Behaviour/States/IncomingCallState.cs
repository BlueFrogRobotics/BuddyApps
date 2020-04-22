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
        private const int REPEAT_TIME = 60;


        // Custom Toast - UI to choose between accept or reject the call
        private GameObject mCustomCapsuleToast;
        private AudioClip mMusic;
        private Text mCallingUserName;
        private Button mRefuseButton;
        private Button mAcceptButton;

        private RTMCom mRTMCom;
        private CallRequest mCallRequest;
        private bool mAcceptCallVocally = false;
        private float mTimeRepeated;

        override public void Start()
        {
            // This returns the GameObject named RTMCom.
            mRTMCom = GetComponent<RTMCom>();
            mRTMCom.OncallRequest = (CallRequest lCall) => { mCallRequest = lCall; };
            // Get the custom capsule toast
            if (!(mCustomCapsuleToast = GetGameObject(0)))
                Debug.LogError("Please add reference to CallManager_customCapsuleToast, in GameObjects list in AIBehaviour.");
            
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
            Debug.Log("Incoming call state");

            mTimeRepeated = Time.time;

            // Setting of Vocon param
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters() {
                RecognitionThreshold = RECOGNITION_SENSIBILITY
            };
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add(OnSpeechReco);

            // Start Call coroutine & Notification repeater

            if (mCallRequest == null || string.IsNullOrWhiteSpace(mCallRequest.userName))
                mCallingUserName.text = "Inconnu";
            else
                mCallingUserName.text = mCallRequest.userName;

            
            StartCoroutine(RepeatNotification());

            //Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("receivingcall"));

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

        

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Incoming call state exit");
            Buddy.GUI.Toaster.Hide();
             Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            Buddy.Actuators.Speakers.Media.Stop();
            StopCoroutine(RepeatNotification());
            mAcceptCallVocally = false;
        }


        private void OnSpeechReco(SpeechInput iSpeechInput)
        {
            if (iSpeechInput.IsInterrupted)
                return;

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "yes")
                AcceptCall();
            else if (Utils.GetRealStartRule(iSpeechInput.Rule) == "no")
                RejectCall();
        }

        private void RejectCall()
        {
            Debug.Log("RejectCall");
            mRTMCom.AnswerCallRequest(false);
            Trigger("IDLE");
        }

        private void AcceptCall()
        {
            Debug.Log("AcceptCall");
            mRTMCom.AnswerCallRequest(true);
            mAcceptCallVocally = true;
            Trigger("CALL");
        }



        /*
         *  This function launches a notification (Music + Vocal request) and repeats this several times.
         *  When a notification is launched:
         *  The music call is launched
         *  We warn user, with speech "incomingcall" and then we listen the user's answer. (yes/no)
         *  When the music ends, the process restarts
         */
        private IEnumerator RepeatNotification()
        {
            // Coroutine is suspended until the music is playing.
            yield return new WaitUntil(() => {
                // If the call was launched or discrete mode selected, stop the coroutine
                if (mAcceptCallVocally) {
                    Buddy.Vocal.StopAndClear();
                    Buddy.Actuators.Speakers.Media.Stop();
                    StopCoroutine(RepeatNotification());
                    return true;
                }
                return !Buddy.Actuators.Speakers.Media.IsBusy;
            });

            // Wait 1.5 second before restart the notification
            yield return new WaitForSeconds(1.5F);

            // If actual number of repeat is less than REPEAT_CALL, relaunch notification and the actual coroutine ends.
            if (Time.time - mTimeRepeated <= REPEAT_TIME) {
                Buddy.Vocal.StopAndClear();
                Buddy.Actuators.Speakers.Media.Play(mMusic);
                //Buddy.Vocal.SayKeyAndListen("incomingcall");

                Buddy.Vocal.SayAndListen("tu as un appel de " + mCallingUserName.text + " veux-tu décrocher?");
                
                // new coroutine to launch
                StartCoroutine(RepeatNotification());
            }
            // If the number max of repeats is reached, consider call as rejected and go back to IDLE.
            else {
                Buddy.GUI.Notifier.Display<SimpleNotification>().With("Appel manqué en provenance de " + mCallingUserName.text);
                RejectCall();

            }
        }
    }
}
