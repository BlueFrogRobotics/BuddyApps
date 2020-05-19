﻿using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{

    public sealed class CallState : AStateMachineBehaviour
    {

        private static int STREAMCALL;
        private static int FLAGSHOWUI = 0;

        private Slider VolumeScrollbar;
        private Button Volume;
        private Button Video;
        private Button Micro;
        private Button VideoFeedbackButton;
        public Image VideoFeedbackIcon;

        private Button Hangup;
        private Text Message;

        private RTCManager mRTCManager;
        private RTMManager mRTMManager;
        private float mTimeVolume;

        private float mTimer;
        private bool mDisplayed;
        private float mTimeMessage;

        private HDCamera mHDCam;
        private GameObject VideoFeedbackImage;
        private bool mHandUp;

        // Use this for initialization
        override public void Start()
        {

            mDisplayed = false;

            mRTCManager = GetComponent<RTCManager>();
            mRTMManager = GetComponent<RTMManager>();

            VolumeScrollbar = GetGameObject(4).GetComponentInChildren<Slider>();
            Volume = GetGameObject(5).GetComponentInChildren<Button>();
            Video = GetGameObject(6).GetComponentInChildren<Button>();
            Micro = GetGameObject(7).GetComponentInChildren<Button>();
            VideoFeedbackButton = GetGameObject(8).GetComponentInChildren<Button>();
            VideoFeedbackIcon = GetGameObject(13).GetComponentInChildren<Image>();
            Hangup = GetGameObject(9).GetComponentInChildren<Button>();
            Message = GetGameObject(11).GetComponentInChildren<Text>();
            VideoFeedbackImage = GetGameObject(12).GetComponentInChildren<RawImage>().gameObject;


            mRTCManager.InitButtons();

            VolumeScrollbar.onValueChanged.AddListener(
                (lValue) => {
                    Debug.LogWarning("PRE Volume call" + GetCallVolume());
                    Debug.LogWarning("PRE Volume call set to " + lValue);
                    SetCallVolume(lValue);
                    //Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
                    Debug.LogWarning("POST Volume set to " + GetCallVolume());
                    mTimeVolume = Time.time;
                });

            Volume.onClick.AddListener(
                () => {
                    VolumeScrollbar.value = GetCallVolume();
                    Volume.gameObject.SetActive(false);
                    VolumeScrollbar.gameObject.SetActive(true);
                    mTimeVolume = Time.time;
                    // TODO update button image
                }
                );

            VideoFeedbackButton.onClick.AddListener(OnFeedBackButtonClick);
            Hangup.onClick.AddListener(OnHangup);

            mRTMManager.OnWheelsMotion = OnWheelsMotion;
            mRTMManager.OnHeadNoAbsolute = Buddy.Actuators.Head.No.SetPosition;
            mRTMManager.OnHeadYesAbsolute = Buddy.Actuators.Head.Yes.SetPosition;
            mRTMManager.OnHeadNo = (lAngle) => Buddy.Actuators.Head.No.SetPosition(lAngle + Buddy.Actuators.Head.No.Angle);
            mRTMManager.OnHeadYes = (lAngle) => Buddy.Actuators.Head.Yes.SetPosition(lAngle + Buddy.Actuators.Head.Yes.Angle);
        }

        private void OnFeedBackButtonClick()
        {
            Debug.LogWarning("FeedbackButtonclicked");
            if (GetGameObject(12).activeInHierarchy) {
                Debug.LogWarning("Atlas_Education_IconOpenFeedback");
                GetGameObject(12).SetActive(false);
                VideoFeedbackIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconOpenFeedback");

            } else {
                Debug.LogWarning("Atlas_Education_IconCloseFeedback");
                GetGameObject(12).SetActive(true);
                VideoFeedbackIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconCloseFeedback");

            }
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
                ManageGUIClose();
                Trigger("IDLE");
            }, "OK"
            );
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer = 0F;
            mHandUp = false;
            Debug.LogError("call state");
            mTimeMessage = -1F;

            VideoSurface lVideoSurface = VideoFeedbackImage.AddComponent<VideoSurface>();
            lVideoSurface.SetForUser(0);
            lVideoSurface.SetEnable(true);
            lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            lVideoSurface.SetGameFps(30);
            //lVideoSurface.EnableFilpTextureApply(false, true);

            GetGameObject(12).SetActive(true);
            VideoFeedbackIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconCloseFeedback");

            mRTCManager.OnEndUserOffline = () => Buddy.GUI.Dialoger.Display<IconToast>("Communication coupée").
                    With(Buddy.Resources.Get<Sprite>("os_icon_phoneoff_big"),
                        () => {
                            ManageGUIClose();
                            Trigger("IDLE");
                            Buddy.GUI.Dialoger.Hide();
                        },
                        null,
                        () => {
                            ManageGUIClose();
                            Trigger("IDLE");
                            Buddy.GUI.Dialoger.Hide();
                        }
                        );

            mRTMManager.OnDisplayMessage = (lMessage) => {
                Message.text = lMessage;
                if (mHandUp)
                    StopRaiseHand();
                if (mTimeMessage <= 0F)
                    TriggerGUI("MESSAGE START");
                mTimeMessage = 10F;
            };


            mRTMManager.OnRaiseHand = (lHandUp) => {
                if (lHandUp) {
                    if (mTimeMessage > 0F)
                        StopMessage();

                    Debug.LogWarning("Start raising hand");
                    mHandUp = true;
                    Buddy.Actuators.LEDs.FlashIntensity = 0.03F;
                    Buddy.Actuators.LEDs.SetBodyLights(60, 100, 93);
                    Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.BASIC_BLINK);
                    TriggerGUI("HANDSUP START");
                } else {
                    StopRaiseHand();
                }
            };

            mRTMManager.OnSpeechMessage = (lMessage) => {
                float lCallVolume = GetCallVolume();
                SetCallVolume(0F);
                Buddy.Vocal.Say(lMessage, (lOutput) => SetCallVolume(lCallVolume));
            };

            mRTMManager.OnActivateZoom = (lZoom) =>
              {
                  mRTCManager.SendPicture(Buddy.Sensors.HDCamera.Frame.Texture);
              };

            Volume.gameObject.SetActive(true);
            Video.gameObject.SetActive(true);
            Micro.gameObject.SetActive(true);
            VideoFeedbackButton.gameObject.SetActive(true);
            Hangup.gameObject.SetActive(true);

        }

        private void OnWheelsMotion(WheelsMotion iWheelsMotion)
        {
            if (Math.Abs(iWheelsMotion.speed) < 0.2F)
                iWheelsMotion.speed = 0F;

            if (Math.Abs(iWheelsMotion.angularVelocity) < 0.2F)
                iWheelsMotion.angularVelocity = 0F;

            Buddy.Actuators.Wheels.SetVelocities(Wheels.MAX_LIN_VELOCITY * iWheelsMotion.speed,
                Wheels.MAX_ANG_VELOCITY * iWheelsMotion.angularVelocity);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Buddy.WebServices.HasInternetAccess) {
                mTimer += Time.deltaTime;
                if (mTimer >= 30F && !mDisplayed) {
                    mDisplayed = true;
                    Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                        TText lText = iBuilder.CreateWidget<TText>();
                        lText.SetLabel(Buddy.Resources.GetString("edunotconnected"));
                    }, null, null,
                    () => {
                        mDisplayed = false;
                        Buddy.GUI.Toaster.Hide();
                        mTimer = 0F;
                    }
                    );
                } else if (mTimer > 38F) {
                    mDisplayed = false;
                    Buddy.GUI.Toaster.Hide();
                    ManageGUIClose();
                    Trigger("IDLE");
                }
            } else {
                mTimer = 0F;
                if (mDisplayed) {
                    mDisplayed = false;
                    Buddy.GUI.Toaster.Hide();
                }
            }

            if (mTimeMessage >= 0) {
                mTimeMessage -= Time.deltaTime;
                if (mTimeMessage < 0)
                    StopMessage();
            }

            if (VolumeScrollbar.gameObject.activeInHierarchy && Time.time - mTimeVolume > 5.0F) {
                VolumeScrollbar.gameObject.SetActive(false);
                Volume.gameObject.SetActive(true);
            }


            if (mRTCManager.mVideoIsEnabled && !VideoFeedbackButton.gameObject.activeInHierarchy) {
                GetGameObject(12).SetActive(true);
                VideoFeedbackButton.gameObject.SetActive(true);
                VideoFeedbackIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconCloseFeedback");
            } else if (!mRTCManager.mVideoIsEnabled && VideoFeedbackButton.gameObject.activeInHierarchy) {
                GetGameObject(12).SetActive(false);
                VideoFeedbackButton.gameObject.SetActive(false);
            }


            if (!mRTMManager.mStaticSteering)
                if (Math.Abs(Buddy.Actuators.Head.No.Angle) > 5 && !Buddy.Actuators.Head.IsBusy) {
                    Buddy.Navigation.Run<DisplacementStrategy>().Rotate(Buddy.Actuators.Head.No.Angle, 200F);
                    Buddy.Actuators.Head.No.SetPosition(0F, 200F);
                }

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //ManageGUIClose();
            if (mTimeMessage >= 0)
                StopMessage();
            if (mHandUp)
            {
                StopRaiseHand();
            }
            Debug.LogError("call state exit");
            //mRTMManager.Logout();
            mRTMManager.OnDisplayMessage = null;
            mRTMManager.OnSpeechMessage = null;
            mRTCManager.Leave();
            Destroy(VideoFeedbackImage.GetComponent<VideoSurface>());
            VideoFeedbackImage.GetComponent<RawImage>().texture = null;
            //mRTCManager.DestroyRTC();
            VolumeScrollbar.gameObject.SetActive(false);
            Volume.gameObject.SetActive(false);
            Video.gameObject.SetActive(false);
            Micro.gameObject.SetActive(false);
            GetGameObject(12).SetActive(false);
            Hangup.gameObject.SetActive(false);
            Debug.LogError("fin call state exit");
        }



        private void StopRaiseHand()
        {
            mHandUp = false;
            TriggerGUI("HANDSUP END");
            Buddy.Actuators.LEDs.Flash = false;
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            Debug.LogWarning("Stop raising hand");
        }

        private void StopMessage()
        {
            TriggerGUI("MESSAGE END");
            mTimeMessage = -1F;
        }

        private void ManageGUIClose()
        {
            if (mTimeMessage >= 0)
                StopMessage();
            if (mHandUp)
                StopRaiseHand();
            if (mDisplayed)
                Buddy.GUI.Toaster.Hide();
        }


        //////////////// For Volume
        ///

        private static AndroidJavaObject audioManager;

        private static AndroidJavaObject deviceAudio
        {
            get
            {
                if (audioManager == null) {
                    AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                    AndroidJavaClass audioManagerClass = new AndroidJavaClass("android.media.AudioManager");
                    AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context");

                    STREAMCALL = audioManagerClass.GetStatic<int>("STREAM_VOICE_CALL");
                    string Context_AUDIO_SERVICE = contextClass.GetStatic<string>("AUDIO_SERVICE");

                    audioManager = context.Call<AndroidJavaObject>("getSystemService", Context_AUDIO_SERVICE);

                    if (audioManager != null)
                        Debug.Log("[AndroidNativeVolumeService] Android Audio Manager successfully set up");
                    else
                        Debug.LogWarning("[AndroidNativeVolumeService] Could not read Audio Manager");
                }
                return audioManager;
            }

        }

        private static int GetDeviceMaxVolume()
        {
            return deviceAudio.Call<int>("getStreamMaxVolume", STREAMCALL);
        }

        private float GetCallVolume()
        {
            int deviceVolume = deviceAudio.Call<int>("getStreamVolume", STREAMCALL);
            float scaledVolume = (deviceVolume / (float)GetDeviceMaxVolume());

            return scaledVolume;
        }

        private void SetCallVolume(float volumeValue)
        {
            int scaledVolume = (int)(volumeValue * GetDeviceMaxVolume());
            deviceAudio.Call("setStreamVolume", STREAMCALL, scaledVolume, FLAGSHOWUI);
        }





    }


}