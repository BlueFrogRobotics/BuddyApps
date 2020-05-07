using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{

    public sealed class CallState : AStateMachineBehaviour
    {

        private Slider VolumeScrollbar;
        private Button Volume;
        private Button Video;
        private Button Micro;
        private Button VideoFeedback;
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
            VideoFeedback = GetGameObject(8).GetComponentInChildren<Button>();
            Hangup = GetGameObject(9).GetComponentInChildren<Button>();
            Message = GetGameObject(11).GetComponentInChildren<Text>();
            VideoFeedbackImage = GetGameObject(12).GetComponentInChildren<RawImage>().gameObject;

            //mHDCam.Open(HDCameraMode.COLOR_1920X1080_15FPS_RGB);
            //mHDCam.OnNewFrame.Add((iInput) => { VideoFeedbackImage.GetComponent<RawImage>().texture = iInput.Texture; });
            //VideoFeedbackImage.SetActive(true);

            mRTCManager.InitButtons();

            Debug.LogWarning("7");
            VolumeScrollbar.onValueChanged.AddListener(
                (lValue) => {
                    //Debug.Log("PRE Volume set to " + lValue);
                    //Buddy.Actuators.Speakers.Volume = lValue;
                    //Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_2);
                    //Debug.Log("POST Volume set to " + Buddy.Actuators.Speakers.Volume);
                    mTimeVolume = Time.time;
                });

            Volume.onClick.AddListener(
                () => {
                    VolumeScrollbar.value = Buddy.Actuators.Speakers.Volume;
                    VolumeScrollbar.gameObject.SetActive(!VolumeScrollbar.gameObject.activeInHierarchy);
                    mTimeVolume = Time.time;
                    // TODO update button image
                }
                );


            VideoFeedback.onClick.AddListener(
                () =>
                {
                    // TODO update button image
                    if (GetGameObject(12).activeInHierarchy)
                    {
                        GetGameObject(12).SetActive(false);
                        //mHDCam.OnNewFrame.Clear();
                        //mHDCam.Close();
                    }
                    else
                    {
                        //TODO : Change resolution depending on the connection
                        //mHDCam.Open(HDCameraMode.COLOR_1920X1080_15FPS_RGB);
                        //mHDCam.OnNewFrame.Add((iInput) => { VideoFeedbackImage.GetComponent<RawImage>().texture = iInput.Texture; });
                        GetGameObject(12).SetActive(true);
                    }
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
            mTimer = 0F;
            Debug.LogError("call state");
            mTimeMessage = -1F;

            VideoSurface lVideoSurface = VideoFeedbackImage.AddComponent<VideoSurface>();
            lVideoSurface.SetForUser(0);
            lVideoSurface.SetEnable(true);
            lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            lVideoSurface.SetGameFps(30);

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
                Message.text = lMessage;
                if (mTimeMessage >= 0F)
                    TriggerGUI("MESSAGE START");
                mTimeMessage = 10F;
            };


            mRTMManager.OnRaiseHand = (lHandUp) => {
                if (lHandUp) {
                    Debug.LogWarning("Start raising hand");
                    Buddy.Actuators.LEDs.FlashIntensity = 0.03F;
                    Buddy.Actuators.LEDs.SetBodyLights(60, 100, 93);
                    Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.BASIC_BLINK);
                    TriggerGUI("HANDSUP START");
                } else {
                    Debug.LogWarning("Stop raising hand");
                    TriggerGUI("HANDSUP END");
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
            if (!Buddy.WebServices.HasInternetAccess)
            {
                mTimer += Time.deltaTime;
                if (mTimer <= 6F && !mDisplayed)
                {
                    mDisplayed = true;
                    Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
                    {
                        TText lText = iBuilder.CreateWidget<TText>();
                        lText.SetLabel(Buddy.Resources.GetString("edunotconnected"));
                    }, null, () => Trigger("IDLE"));
                }
                else if (mTimer > 6F)
                {
                    Buddy.GUI.Toaster.Hide();
                }
            }

            if (mTimeMessage >= 0) {
                mTimeMessage -= Time.deltaTime;
                if (mTimeMessage < 0)
                    TriggerGUI("MESSAGE END");
            }

            if (VolumeScrollbar.gameObject.activeInHierarchy && Time.time - mTimeVolume > 5.0F)
                VolumeScrollbar.gameObject.SetActive(false);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
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
    }

}