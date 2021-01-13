using System.Collections.Generic;
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

        private string mPhotoSentPath;

        private static AndroidJavaObject audioManager;
        // Use this for initialization
        override public void Start()
        {
            mSliderVolumeEnabled = false;
            mDisplayed = false;
            mEndCallDisplay = false;
            mRTCManager = GetComponent<RTCManager>();
            mRTMManager = GetComponent<RTMManager>();

            if (Buddy.Sensors.Microphones.CurrentMicrophone.Code != "DEVICE_IN_USB_DEVICE") {
                //set microphone 360 to true
                Buddy.Sensors.Microphones.SwitchMicrophone("DEVICE_IN_WIRED_HEADSET", false);
                Buddy.Sensors.Microphones.SwitchMicrophone("DEVICE_IN_USB_DEVICE", true);
                mRTCManager.SetMicrophone("1");
            }
            Buddy.WebServices.Agoraio.ImageReceived = (width, height, data) => mRTCManager.SetProfilePicture(data, width, height);

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

            mRTMManager.OnHeadNoAbsolute = (lAngle) => {
                float lMaxValue;
                if (!mRTCManager.mCurrentCameraWide)
                    lMaxValue = 40F;
                else
                    lMaxValue = 60F;


                Debug.LogWarning("Angle No received, we go at " + (Buddy.Actuators.Head.No.Angle - lAngle * lMaxValue) + " from " + Buddy.Actuators.Head.No.Angle);
                Buddy.Actuators.Head.No.SetPosition(Buddy.Actuators.Head.No.Angle - lAngle * lMaxValue);
            };

            mRTMManager.OnHeadYesAbsolute = (lAngle) => {

                float lMaxValue;
                if (!mRTCManager.mCurrentCameraWide)
                    lMaxValue = 25F;
                else
                    lMaxValue = 40F;

                Debug.LogWarning("Angle Yes received, we go at " + (Buddy.Actuators.Head.Yes.Angle + lAngle * lMaxValue) + " from " + Buddy.Actuators.Head.Yes.Angle);
                Buddy.Actuators.Head.Yes.SetPosition(Buddy.Actuators.Head.Yes.Angle + lAngle * lMaxValue);

            };

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
            // Just to be sure:
            Buddy.Vocal.OnTrigger.Clear();
            Buddy.Vocal.ListenOnTrigger = false;

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
            lVideoSurface.EnableFilpTextureApply(false, true);

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
                    //ResetTrigger("HANDSUP START");
                } else {
                    StopRaiseHand();
                }
            };

            mRTMManager.OnFrontalListening = (lFrontalListening) => {
                if (lFrontalListening) {
                    //set microphone 360 to false
                    Buddy.Sensors.Microphones.SwitchMicrophone("DEVICE_IN_USB_DEVICE", false);
                    //set the front microphone to true
                    Buddy.Sensors.Microphones.SwitchMicrophone("DEVICE_IN_WIRED_HEADSET", true);
                    mRTCManager.SetMicrophone("0");
                    Debug.Log("MICRO ENABLED : " + Buddy.Sensors.Microphones.CurrentMicrophone.Code);

                } else {
                    //set the front microphone to false
                    Buddy.Sensors.Microphones.SwitchMicrophone("DEVICE_IN_WIRED_HEADSET", false);
                    //set microphone 360 to true
                    Buddy.Sensors.Microphones.SwitchMicrophone("DEVICE_IN_USB_DEVICE", true);
                    mRTCManager.SetMicrophone("1");
                    Debug.Log("MICRO ENABLED : " + Buddy.Sensors.Microphones.CurrentMicrophone.Code);
                }
            };

            mRTMManager.OnSpeechMessage = (lMessage) => {
                //VOCON
                float lCallVolume = GetCallVolume();
                SetCallVolume(0F);
                Buddy.Vocal.Say(lMessage, (lOutput) =>
                {
                    SetCallVolume(lCallVolume);
                });
            };

            mRTMManager.OnActivateZoom = (lZoom) => {
                mRTCManager.SwitchCam();
            };

            mRTMManager.OnMood = (lMood) =>
            {
                Debug.LogError("ON MOOD profil set active false");
                GetGameObject(22).SetActive(false);
            };

            mRTMManager.OnTakePhoto = (lTakePhoto) => {
                Debug.LogError("CALLSTATE TAKE PHOTO WITH switchcam ");
                //Save image from open camera on the robot
                Utils.DeleteFile(Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");
                Texture2D iPhotoFromRobot = (Texture2D)GetGameObject(12).GetComponentInChildren<RawImage>().texture;
                Utils.SaveTextureToFile(iPhotoFromRobot, Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");
                Debug.LogError("CALLSTATE TAKE PHOTO WITH TAKEPHOTOGRAPH PATH : " + Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");
                string iPathPhotoSaved = Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg";
                //iPathPhotoSaved = mRTCManager.TakePhoto();
                Debug.LogError("path photo saved : " + iPathPhotoSaved);
                //test
                //Buddy.WebServices.Agoraio.SendPicture(DBManager.Instance.ListUIDTablet[TeleBuddyQuatreDeuxData.Instance.IndexTablet],/* Buddy.Resources.AppSpritesPath + "background.jpg"*/iPathPhotoSaved);
                Buddy.WebServices.Agoraio.SendPicture(mRTMManager.IdConnectionTablet,/* Buddy.Resources.AppSpritesPath + "background.jpg"*/iPathPhotoSaved);

                Debug.LogError("CALLSTATE TAKE PHOTO WITH TAKEPHOTOGRAPH WITH PATH BACKGROUND : " + Buddy.Resources.AppSpritesPath + "background.jpg");





                //new test take photo
                //HDCamera mHDCam;
                //mHDCam = Buddy.Sensors.HDCamera;
                //mHDCam.OnNewFrame.Clear();
                //mHDCam.Close();
                //Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam ");
                //Utils.DeleteFile(Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");
                //Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 1");
                //mRTCManager.MuteVideo(true);
                //Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 4");
                //mHDCam.Open(HDCameraMode.COLOR_4224x3136_30FPS_RGB, HDCameraType.FRONT);
                //Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 5");
                //Texture2D iPhotoFromRobotTest;
                //mHDCam.OnNewFrame.Add((iInput) => {
                //    Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 6");
                //    if(iInput.Texture != null)
                //    {
                //        Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 6 : height " + iInput.Texture.height + " width : " + iInput.Texture.width);
                //        iPhotoFromRobotTest = iInput.Texture;
                //        Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 7 : " + Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");
                //        Utils.SaveTextureToFile(iPhotoFromRobotTest, Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");

                //    }
                //    Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 8");
                //    string iPathPhotoSavedTest = Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg";
                //    Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 9 : " + iPathPhotoSavedTest);
                //    Buddy.WebServices.Agoraio.SendPicture(mRTMManager.IdConnectionTablet, iPathPhotoSavedTest);
                //    Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 10");
                //    mHDCam.OnNewFrame.Clear();
                //    mHDCam.Close();
                //    mRTCManager.MuteVideo(false);
                //    Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 11");

                //});


            };

            //mRTMManager.OnPictureReceived = (data) => mRTCManager.SetProfilePicture(data);

            Volume.gameObject.SetActive(true);
            Video.gameObject.SetActive(true);
            Micro.gameObject.SetActive(true);
            VideoFeedbackButton.gameObject.SetActive(true);
            Hangup.gameObject.SetActive(true);
        }
        
        private void OnPhotoTaken(Photograph iMyPhoto)
        {
            if (iMyPhoto == null) {
                Debug.LogError("OnFinish take photo, iPhoto null");
                return;
            }
            Debug.LogError("CALL STATE : PHOTO TAKEN");
            // test to display photo
            Sprite mPhotoSprite = Sprite.Create(iMyPhoto.Image.texture, new UnityEngine.Rect(0, 0, iMyPhoto.Image.texture.width, iMyPhoto.Image.texture.height), new Vector2(0.5F, 0.5F));
            //OU (la deuxième version bugguait à un moment)
            Sprite mPhotoSpriteSecondVersion = iMyPhoto.Image;
            //test pour voir l'image
            //Buddy.GUI.Toaster.Display<PictureToast>().With(iMyPhoto.Image);

            iMyPhoto.Save();
            mPhotoSentPath = iMyPhoto.FullPath;
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
                Buddy.Actuators.Wheels.SetVelocities(0F, 0F, AccDecMode.HIGH);
                mTimeSinceMovement = Time.time;
            }

            if(!mRTMManager.mStaticSteering)
            {
                if (OneOrMoreCliff())
                    Buddy.Actuators.Wheels.UnlockWheels();

            }

            if (DBManager.Instance.CanEndCourse) {
                mTimerEndCall += Time.deltaTime;
                if (!mEndCallDisplay)
                {
                    mEndCallDisplay = true;
                    Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
                    {
                        TText lText = iBuilder.CreateWidget<TText>();
                        lText.SetLabel(Buddy.Resources.GetString("eduendcall"));
                    }, null, null);
                }


                if (mTimerEndCall > 8F) {

                    Buddy.GUI.Toaster.Hide();
                    if (DBManager.Instance.ListUIDTablet.Count > 1)
                    {
                        GetGameObject(21).SetActive(true);
                    }
                    ManageGUIClose();
                    Trigger("IDLE");
                }
            }

            if (mHideTime > 0 && (Time.time - mHideTime > 2F))
                Buddy.GUI.Dialoger.Hide();


            //if (!TeleBuddyQuatreDeuxData.Instance.IsQualityNetworkGood) {
            //    mTimer += Time.deltaTime;
            //    if (mTimer >= 30F && !mDisplayed) {
            //        mDisplayed = true;
            //        Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
            //            TText lText = iBuilder.CreateWidget<TText>();
            //            lText.SetLabel(Buddy.Resources.GetString("edunotconnected"));
            //        }, null, null,
            //        () => {
            //            mDisplayed = false;
            //            Buddy.GUI.Toaster.Hide();
            //            mTimer = 0F;
            //        }
            //        );
            //    } else if (mTimer > 38F) {
            //        mDisplayed = false;
            //        Buddy.GUI.Toaster.Hide();
            //        ManageGUIClose();
            //        Trigger("IDLE");
            //    }
            //} else {
            //    mTimer = 0F;
            //    if (mDisplayed) {
            //        mDisplayed = false;
            //        Buddy.GUI.Toaster.Hide();
            //    }
            //}

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

            if (!mRTMManager.mStaticSteering) {
                if (Math.Abs(Buddy.Actuators.Head.No.Angle) > 5 && !Buddy.Actuators.Head.IsBusy) {
                    Buddy.Navigation.Run<DisplacementStrategy>().Rotate(Buddy.Actuators.Head.No.Angle, 200F);
                    Buddy.Actuators.Head.No.SetPosition(0F, 200F);
                }
            }

        }

        public bool OneOrMoreCliff()
        {
            return BackCliff() || FrontCliff();
        }
        public bool BackCliff()
        {
            return Buddy.Sensors.CliffSensors.BackLeftFreeWheel.Value > 65F ||
                Buddy.Sensors.CliffSensors.BackRightFreeWheel.Value > 65F ||
                Buddy.Sensors.CliffSensors.BackLeftWheel.Value > 65F ||
                Buddy.Sensors.CliffSensors.BackRightWheel.Value > 65F;
        }
        public bool FrontCliff()
        {
            return Buddy.Sensors.CliffSensors.FrontFreeWheel.Value > 65F ||
                Buddy.Sensors.CliffSensors.FrontLeftWheel.Value > 65F ||
                Buddy.Sensors.CliffSensors.FrontRightWheel.Value > 65F;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mRTMManager.PingReceived = false;
            //DBManager.Instance.CanEndCourse = false;
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
            //ResetTrigger("HANDSUP END");
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
                Buddy.Actuators.Wheels.UnlockWheels();
                mRTMManager.SwapSteering(false);
                mToggleNavigationDynamic.ToggleValue = true;
            }
        }

        private void SetNavigationDynamic(bool iValue)
        {
            if (iValue) {
                Buddy.Actuators.Wheels.UnlockWheels();
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