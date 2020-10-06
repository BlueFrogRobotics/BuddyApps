﻿using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BuddyApp.TeleBuddyQuatreDeux
{

    public sealed class CallState : AStateMachineBehaviour
    {

        private bool mDisplayed;
        private bool mHandUp;
        private bool mEndCallDisplay;
        private bool mSliderVolumeEnabled;

        private static int STREAMCALL;
        private static int FLAGSHOWUI = 0;

        private float mTimerEndCall;
        private float mTimeVolume;
        private float mTimer;
        private float mHideTime;
        private float mPreviousAngle;
        private float mPreviousNoAngleTime;
        private float mPreviousYesAngleTime;
        private float mTimeSinceMovement;
        private float mTimeMessage;

        private Slider VolumeScrollbar;
        private Button Volume;
        private Button Video;
        private Button Micro;
        private Button VideoFeedbackButton;
        public Image VideoFeedbackIcon;
        private Button Hangup;
        private Text Message;
        private GameObject VideoFeedbackImage;
        private TSlider mSliderVolume;
        private TToggle mToggleNavigation;
        private TToggle mToggleNavigationStatic;
        private TToggle mToggleNavigationDynamic;

        private RTCManager mRTCManager;
        private RTMManager mRTMManager;

        private HDCamera mHDCam;

        private static AndroidJavaObject audioManager;
        // Use this for initialization
        override public void Start()
        {
            mSliderVolumeEnabled = false;
            mDisplayed = false;
            mEndCallDisplay = false;
            mRTCManager = GetComponent<RTCManager>();
            mRTMManager = GetComponent<RTMManager>();

            //if(Buddy.Sensors.Microphones.)
                //set microphone 360 to true
                //        Buddy.Sensors.Microphones.SwitchMicrophone( , true);

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
                    SetCallVolume(lValue);
                    mTimeVolume = Time.time;
                });

            Volume.onClick.AddListener(
                () => {
                    mSliderVolumeEnabled = true;
                    VolumeScrollbar.value = GetCallVolume();
                    Volume.gameObject.SetActive(false);
                    VolumeScrollbar.gameObject.SetActive(true);
                    mTimeVolume = Time.time;
                }
                );

            VolumeScrollbar.GetComponentInChildren<Button>().onClick.AddListener(
                () => {
                    VolumeScrollbar.gameObject.SetActive(false);
                    Volume.gameObject.SetActive(true);
                });

            VideoFeedbackButton.onClick.AddListener(OnFeedBackButtonClick);
            Hangup.onClick.AddListener(OnHangup);

            mRTMManager.OnWheelsMotion = OnWheelsMotion;
            mRTMManager.OnHeadNoAbsolute = Buddy.Actuators.Head.No.SetPosition;
            mRTMManager.OnHeadYesAbsolute = Buddy.Actuators.Head.Yes.SetPosition;
            mRTMManager.OnHeadNo = (lAngle) => {
                if (lAngle * mPreviousAngle < 0) {
                    // Dirty way to clean the queue. Need new function in OS to do this
                    Buddy.Actuators.Head.No.Stop();
                    Buddy.Actuators.Head.No.SetPosition(Mathf.Clamp(lAngle + Buddy.Actuators.Head.No.Angle, -80F, 80F), Mathf.Clamp(Math.Abs(lAngle) * 10, 20F, 230F), AccDecMode.HIGH);
                    mPreviousAngle = lAngle;

                    Debug.LogWarning("Time between NO sent command " + (Time.time - mPreviousNoAngleTime));
                    mPreviousNoAngleTime = Time.time;
                } else if (!Buddy.Actuators.Head.No.IsBusy) {
                    Buddy.Actuators.Head.No.SetPosition(Mathf.Clamp(lAngle + Buddy.Actuators.Head.No.Angle, -80F, 80F), Mathf.Clamp(Math.Abs(lAngle) * 10, 20F, 230F), AccDecMode.SMOOTH);
                    mPreviousAngle = lAngle;

                    Debug.LogWarning("Time between NO sent command " + (Time.time - mPreviousNoAngleTime));
                    mPreviousNoAngleTime = Time.time;
                }
            };
            mRTMManager.OnHeadYes = (lAngle) => {
                // If the user changes head direction
                if (lAngle * mPreviousAngle < 0) {
                    // Dirty way to clean the queue. Need new function in OS to do this
                    Buddy.Actuators.Head.Yes.Stop();
                    Buddy.Actuators.Head.Yes.SetPosition(Mathf.Clamp(lAngle + Buddy.Actuators.Head.Yes.Angle, -10F, 37F), Mathf.Clamp(Math.Abs(lAngle) * 4, 5F, 80F), AccDecMode.HIGH);
                    Debug.LogWarning("Time between YES sent command " + (Time.time - mPreviousYesAngleTime));
                    mPreviousYesAngleTime = Time.time;
                    mPreviousAngle = lAngle;
                } else if (!Buddy.Actuators.Head.Yes.IsBusy) {
                    Buddy.Actuators.Head.Yes.SetPosition(Mathf.Clamp(lAngle + Buddy.Actuators.Head.Yes.Angle, -10F, 37F), Mathf.Clamp(Math.Abs(lAngle) * 4, 5F, 80F), AccDecMode.SMOOTH);
                    mPreviousAngle = lAngle;
                    Debug.LogWarning("Time between YES sent command " + (Time.time - mPreviousYesAngleTime));
                    mPreviousYesAngleTime = Time.time;
                }
            };
        }

        private void OnFeedBackButtonClick()
        {
            if (GetGameObject(12).activeInHierarchy) {
                GetGameObject(12).SetActive(false);
                VideoFeedbackIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconOpenFeedback");

            } else {
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
                if (DBManager.Instance.ListUIDTablet.Count > 1) {
                    GetGameObject(21).SetActive(true);
                }
                ManageGUIClose();
                Trigger("IDLE");
            }, "OK"
            );
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            TeleBuddyQuatreDeuxData.Instance.CurrentState = TeleBuddyQuatreDeuxData.States.CALL_STATE;
            mSliderVolumeEnabled = false;
            mTimer = 0F;
            mTimerEndCall = 0F;
            mHideTime = -1F;
            mHandUp = false;
            mTimeMessage = -1F;
            mTimeSinceMovement = Time.time;
            Buddy.GUI.Header.DisplayParametersButton(true);
            VideoSurface lVideoSurface = VideoFeedbackImage.AddComponent<VideoSurface>();
            lVideoSurface.SetForUser(0);
            lVideoSurface.SetEnable(true);
            lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            lVideoSurface.SetGameFps(30);

            GetGameObject(12).SetActive(true);
            GetGameObject(18).SetActive(true);
            VideoFeedbackIcon.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconCloseFeedback");

            mRTCManager.OnEndUserOffline = () => Buddy.GUI.Dialoger.Display<IconToast>("Communication coupée").
            With(Buddy.Resources.Get<Sprite>("os_icon_phoneoff_big"),
                () => {
                    ManageGUIClose();
                    Trigger("IDLE");
                    Buddy.GUI.Dialoger.Hide();
                },
                () => mHideTime = Time.time,
                () => {
                    mRTCManager.Leave();
                    ManageGUIClose();
                    Trigger("IDLE");
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
                    mHandUp = true;
                    Buddy.Actuators.LEDs.FlashIntensity = 0.03F;
                    Buddy.Actuators.LEDs.SetBodyLights(60, 100, 93);
                    Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.BASIC_BLINK);
                    TriggerGUI("HANDSUP START");
                    ResetTrigger("HANDSUP START");
                } else {
                    StopRaiseHand();
                }
            };

            //mRTMManager.OnFrontalListening = (lFrontalListening) =>
            //{
            //    if(lFrontalListening)
            //    {
            //        //set microphone 360 to false
            //        Buddy.Sensors.Microphones.SwitchMicrophone( , false);
            //        //set the front microphone to true
            //        Buddy.Sensors.Microphones.SwitchMicrophone( , true);
            //    }
            //    else
            //    {
            //        //set microphone 360 to true
            //        Buddy.Sensors.Microphones.SwitchMicrophone( , true);
            //        //set the front microphone to false
            //        Buddy.Sensors.Microphones.SwitchMicrophone( , false);
            //    }
            //};

            mRTMManager.OnSpeechMessage = (lMessage) => {
                float lCallVolume = GetCallVolume();
                SetCallVolume(0F);
                Buddy.Vocal.Say(lMessage, (lOutput) => {
                    SetCallVolume(lCallVolume);
                });
            };

            mRTMManager.OnActivateZoom = (lZoom) => {
                mRTCManager.SendPicture(Buddy.Sensors.HDCamera.Frame.Texture);
            };

            mRTMManager.OnPictureReceived = (data) => mRTCManager.SetProfilePicture(data);

            Volume.gameObject.SetActive(true);
            Video.gameObject.SetActive(true);
            Micro.gameObject.SetActive(true);
            VideoFeedbackButton.gameObject.SetActive(true);
            Hangup.gameObject.SetActive(true);
        }

        private void OnWheelsMotion(WheelsMotion iWheelsMotion)
        {
            mTimeSinceMovement = Time.time;
            Buddy.Actuators.Wheels.SetVelocities(Wheels.MAX_LIN_VELOCITY * Mathf.Pow(iWheelsMotion.speed, 3),
                Wheels.MAX_ANG_VELOCITY * Mathf.Pow(iWheelsMotion.angularVelocity, 3));
        }

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Buddy.Actuators.Wheels.Locked && Time.time - mTimeSinceMovement > 0.5F) {
                Buddy.Actuators.Wheels.SetVelocities(0F, 0F);
                mTimeSinceMovement = Time.time;
            }

            if (DBManager.Instance.CanEndCourse) {
                mTimerEndCall += Time.deltaTime;
                if (!mEndCallDisplay) {
                    mEndCallDisplay = true;
                    Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                        TText lText = iBuilder.CreateWidget<TText>();
                        lText.SetLabel(Buddy.Resources.GetString("eduendcall"));
                    }, null, null);
                }


                if (mTimerEndCall > 8F) {
                    Buddy.GUI.Toaster.Hide();
                    DBManager.Instance.CanEndCourse = false;
                    Trigger("IDLE");
                }
            }

            if (mHideTime > 0 && (Time.time - mHideTime > 2F))
                Buddy.GUI.Dialoger.Hide();


            if (!TeleBuddyQuatreDeuxData.Instance.IsQualityNetworkGood) {
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
            Buddy.Actuators.Wheels.Stop();
            Buddy.GUI.Toaster.Hide();
            if (mTimeMessage >= 0)
                StopMessage();
            if (mHandUp) {
                StopRaiseHand();
            }
            mRTMManager.OnDisplayMessage = null;
            mRTMManager.OnSpeechMessage = null;
            mRTCManager.Leave();
            Destroy(VideoFeedbackImage.GetComponent<VideoSurface>());
            VideoFeedbackImage.GetComponent<RawImage>().texture = null;
            VolumeScrollbar.gameObject.SetActive(false);
            Volume.gameObject.SetActive(false);
            Video.gameObject.SetActive(false);
            Micro.gameObject.SetActive(false);
            VideoFeedbackButton.gameObject.SetActive(false);
            GetGameObject(12).SetActive(false);
            Hangup.gameObject.SetActive(false);
            GetGameObject(18).SetActive(false);
        }

        private void StopRaiseHand()
        {
            mHandUp = false;
            TriggerGUI("HANDSUP END");
            ResetTrigger("HANDSUP END");
            Buddy.Actuators.LEDs.Flash = false;
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
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

        private void Lauchparameters()
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                TText lTextVolume = iBuilder.CreateWidget<TText>();
                lTextVolume.SetLabel("Réglage du volume");
                mSliderVolume = iBuilder.CreateWidget<TSlider>();
                mSliderVolume.SlidingValue = Buddy.Actuators.Speakers.Volume * 100F;
                mSliderVolume.OnSlide.Add(UpdateVolume);

                mToggleNavigationStatic = iBuilder.CreateWidget<TToggle>();
                mToggleNavigationStatic.SetLabel("Navigation Statique");
                mToggleNavigationStatic.ToggleValue = mRTMManager.mStaticSteering;
                mToggleNavigationStatic.OnToggle.Add(SetNavigationStatic);

                mToggleNavigationDynamic = iBuilder.CreateWidget<TToggle>();
                mToggleNavigationDynamic.SetLabel("Navigation Dynamique");
                mToggleNavigationDynamic.ToggleValue = !mRTMManager.mStaticSteering;
                mToggleNavigationDynamic.OnToggle.Add(SetNavigationDynamic);

            },
           () => { Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.Get<Sprite>("os_icon_close", Context.OS),
           () => { Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.Get<Sprite>("os_icon_check", Context.OS)
          );

            Buddy.GUI.Header.OnClickParameters.Clear();
            Buddy.GUI.Header.OnClickParameters.Add(CloseParameters);
        }

        private void SetNavigationStatic(bool iValue)
        {
            if (iValue) {
                mRTMManager.SwapSteering(true);
                mToggleNavigationDynamic.ToggleValue = false;
            } else {
                mRTMManager.SwapSteering(false);
                mToggleNavigationDynamic.ToggleValue = true;
            }
        }

        private void SetNavigationDynamic(bool iValue)
        {
            if (iValue) {
                mRTMManager.SwapSteering(false);
                mToggleNavigationStatic.ToggleValue = false;
            } else {
                mRTMManager.SwapSteering(true);
                mToggleNavigationStatic.ToggleValue = true;
            }
        }

        private void CloseParameters()
        {
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Header.OnClickParameters.Clear();
            Buddy.GUI.Header.OnClickParameters.Add(Lauchparameters);
        }

        public void UpdateVolume(float iValue)
        {
            float lValue = iValue / 100F;
            if (Mathf.Abs(Buddy.Actuators.Speakers.Volume - lValue) > 0.05) {
                Buddy.Actuators.Speakers.Volume = lValue;
            }
        }
    }
}