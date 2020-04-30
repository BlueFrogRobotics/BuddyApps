using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{

    public sealed class CallState : AStateMachineBehaviour
    {

        private Scrollbar VolumeScrollbar;
        private Button Volume;
        private Button Video;
        private Button Micro;
        private Button VideoFeedback;
        private Button Hangup;

        private RTCManager mRTCManager;
        private RTMManager mRTMManager;
        private float mTimeVolume;

        private float mTimer;
        private bool mConnected;

        // Use this for initialization
        override public void Start()
        {
            mTimer = 0F;
            mConnected = false;

            mRTCManager = GetComponent<RTCManager>();
            mRTMManager = GetComponent<RTMManager>();
            VolumeScrollbar = GetGameObject(4).GetComponentInChildren<Scrollbar>();
            Volume = GetGameObject(5).GetComponentInChildren<Button>();
            Video = GetGameObject(6).GetComponentInChildren<Button>();
            Micro = GetGameObject(7).GetComponentInChildren<Button>();
            VideoFeedback = GetGameObject(8).GetComponentInChildren<Button>();
            Hangup = GetGameObject(9).GetComponentInChildren<Button>();

            VolumeScrollbar.onValueChanged.AddListener(
                (lValue) => {
                    Debug.Log("Volume set to " + lValue);
                    Buddy.Actuators.Speakers.Volume = lValue;
                    mTimeVolume = Time.time;
                });

            Volume.onClick.AddListener(
                () => {
                    VolumeScrollbar.gameObject.SetActive(!VolumeScrollbar.gameObject.activeInHierarchy);
                    mTimeVolume = Time.time;
                    // TODO update button image
                }
                );


            Video.onClick.AddListener(
                () => {
                    // TODO update button image
                    //TODO update video broadcast or not
                }
                );


            Micro.onClick.AddListener(
                () => {
                    // TODO update button image
                    //TODO update micro broadcast or not
                }
                );

            VideoFeedback.onClick.AddListener(
                () => {
                    // TODO update button image
                    //TODO update video feedback or not
                }
                );

            Hangup.onClick.AddListener(OnHangup);
        }

        private void OnHangup()
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                TText lText = iBuilder.CreateWidget<TText>();
                lText.SetLabel("Veux-tu vraiment mettre fin à l'appel?");
            },
            () => {
                Debug.Log("Cancel");
                Buddy.GUI.Toaster.Hide();
            }, "Cancel",
            () => {
                Debug.Log("OK");
                Buddy.GUI.Toaster.Hide();
                //Réafficher l'écran de démarrage
                Trigger("IDLE");
            }, "OK"
            );
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("call state");


            mRTCManager.OnEndUserOffline = () => Buddy.GUI.Dialoger.Display<IconToast>("Communication coupée").
                    With(Buddy.Resources.Get<Sprite>("os_icon_phoneoff_big"),
                        () => {
                            Trigger("IDLE");
                            Buddy.GUI.Dialoger.Hide();
                        },
                        null,
                        () => {
                            Trigger("IDLE");
                            Buddy.GUI.Dialoger.Hide();
                        }
                        );

            mRTMManager.OnDisplayMessage = (lMessage) => {
                Buddy.GUI.Dialoger.Display<IconToast>(lMessage).With(
                    Buddy.Resources.Get<Sprite>("os_icon_bubble"),
                    Buddy.GUI.Dialoger.Hide
                );
            };


            mRTMManager.OnRaiseHand = (lHandUp) => {
                if (lHandUp) {
                    //TODO Display icon
                    Buddy.Actuators.LEDs.FlashIntensity = 0.03F;
                    Buddy.Actuators.LEDs.SetBodyLights(60, 100, 93);
                    Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.BASIC_BLINK);
                } else {
                    //TODO hide icon
                    Buddy.Actuators.LEDs.Flash = false;
                    Buddy.Behaviour.SetMood(Mood.NEUTRAL);
                }
            };

            mRTMManager.OnSpeechMessage = (lMessage) => {
                Buddy.Vocal.Say(lMessage);
            };

            Volume.gameObject.SetActive(true);
            Video.gameObject.SetActive(true);
            Micro.gameObject.SetActive(true);
            VideoFeedback.gameObject.SetActive(true);
            Hangup.gameObject.SetActive(true);

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!CoursTelepresenceData.Instance.ConnectedToInternet)
            {
                mConnected = true;
                if(mTimer <= 6F && mConnected)
                {
                    Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
                    {
                        TText lText = iBuilder.CreateWidget<TText>(); 
                        lText.SetLabel(Buddy.Resources.GetString("edunotconnected"));
                    }, null, () => Trigger("IDLE"));
                }
                else if(mTimer > 6F && mConnected)
                {
                    Buddy.GUI.Toaster.Hide();
                }
            }
                
            if (VolumeScrollbar.gameObject.activeInHierarchy && Time.time - mTimeVolume > 5.0F)
                VolumeScrollbar.gameObject.SetActive(false);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("call state exit");
            mRTCManager.Leave();
            mRTMManager.Logout();
            mRTMManager.OnDisplayMessage = null;
            mRTMManager.OnSpeechMessage = null;
            VolumeScrollbar.gameObject.SetActive(false);
            Volume.gameObject.SetActive(false);
            Video.gameObject.SetActive(false);
            Micro.gameObject.SetActive(false);
            VideoFeedback.gameObject.SetActive(false);
            Hangup.gameObject.SetActive(false);
        }
    }

}