using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using OpenCVUnity;

using System.Collections;
using System.Collections.Generic;
using System;

namespace BuddyApp.CoursTelepresence
{
    public class RTCManager : MonoBehaviour
    {
        [SerializeField]
        private RawImage rawVideo;

        [SerializeField]
        private Button buttonEnableVideo;

        [SerializeField]
        private Button buttonEnableAudio;

        //private string mChannel;
        private bool mVideoIsEnabled;
        private bool mAudioIsEnabled;

        private IRtcEngine mRtcEngine;

        private uint mUid;

        private const int WIDTH = 1280;
        private const int HEIGHT = 800;


        public Action OnEndUserOffline { get; set; }

        private void Awake()
        {
            if (Application.isEditor)
                Application.runInBackground = true;
        }

        // Use this for initialization
        void Start()
        {
            Debug.Log("test " + Time.time);
            Buddy.WebServices.Agoraio.LoadEngine("dc949460a57e4fb0990a219b799ccf13");
            mRtcEngine = Buddy.WebServices.Agoraio.RtcEngine;
            mRtcEngine.OnStreamMessage = OnStreamMessage;
            mVideoIsEnabled = true;
            mAudioIsEnabled = true;
            rawVideo.gameObject.SetActive(false);

            buttonEnableAudio.onClick.AddListener(SwitchAudioState);
            buttonEnableVideo.onClick.AddListener(SwitchVideoState);
        }
        

        public void Join(string iChannel)
        {
            mRtcEngine.OnRemoteVideoStateChanged = OnRemoteVideoStateChanged;
            mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
            mRtcEngine.OnUserJoined = OnUserJoined;
            mRtcEngine.OnUserOffline = OnUserOffline;
            mRtcEngine.OnFirstRemoteVideoFrame = OnFirstRemoteVideoFrame;
            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();
            
            mRtcEngine.JoinChannel(iChannel, null, 0);

        }

        public void Leave()
        {
            if (mRtcEngine == null)
                return;

            mRtcEngine.LeaveChannel();
            mRtcEngine.DisableVideoObserver();
            if(rawVideo.GetComponent<VideoSurface>()!=null)
                Destroy(rawVideo.GetComponent<VideoSurface>());
            rawVideo.texture = null;
            rawVideo.gameObject.SetActive(false);
        }

        public void SwitchVideoState()
        {
            if (!mVideoIsEnabled)
            {
                mRtcEngine.MuteLocalVideoStream(false);
                buttonEnableVideo.GetComponent<Image>().sprite = Buddy.Resources.Get<Sprite>("os_icon_video_on");//EnableVideoSprite;
            }
            else
            {
                mRtcEngine.MuteLocalVideoStream(true);
                buttonEnableVideo.GetComponent<Image>().sprite = Buddy.Resources.Get<Sprite>("os_icon_video_off");//DisableSprite;
            }
            mVideoIsEnabled = !mVideoIsEnabled;
        }

        public void SwitchAudioState()
        {
            if (!mAudioIsEnabled)
            {
                mRtcEngine.MuteLocalAudioStream(false);
                buttonEnableAudio.GetComponent<Image>().sprite = Buddy.Resources.Get<Sprite>("os_icon_micro_on"); //EnableVideoSprite;
            }
            else
            {
                mRtcEngine.MuteLocalAudioStream(true);
                buttonEnableAudio.GetComponent<Image>().sprite = Buddy.Resources.Get<Sprite>("os_icon_micro_off"); ;
            }
            mAudioIsEnabled = !mAudioIsEnabled;
        }

        private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
            mUid = uid;
        }

        private void OnUserJoined(uint uid, int elapsed)
        {
            Debug.Log("[AGORAIO] onUserJoined: uid = " + uid + " elapsed = " + elapsed);
            VideoSurface lVideoSurface = rawVideo.GetComponent<VideoSurface>();
            if (lVideoSurface == null && uid != mUid)
            {
                rawVideo.gameObject.SetActive(true);
                lVideoSurface = rawVideo.gameObject.AddComponent<VideoSurface>();
                lVideoSurface.SetForUser(uid);
                lVideoSurface.SetEnable(true);
                lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                lVideoSurface.SetGameFps(30);
                //VideoSurface.rectTransform.sizeDelta = SizeToParent(VideoSurface);
            }
        }

        private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
        {
            // remove video stream
            Debug.Log("[AGORAIO] onUserOffline: uid = " + uid);
            Destroy(rawVideo.GetComponent<VideoSurface>());
            rawVideo.texture = null;
            rawVideo.gameObject.SetActive(false);
            OnEndUserOffline();
        }

        private void OnRemoteVideoStateChanged(uint uid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
        {
            Debug.Log("remote video " + state);
            if (uid == mUid)
                return;
            VideoSurface lVideoSurface = rawVideo.GetComponent<VideoSurface>();
            if (state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STOPPED)
            {
                if (lVideoSurface != null)
                {
                    lVideoSurface.SetEnable(false);
                    Destroy(rawVideo.GetComponent<VideoSurface>());
                    rawVideo.texture = null;
                    rawVideo.gameObject.SetActive(false);
                }
            }
            else if (state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STARTING || state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_DECODING)
            {
                if (lVideoSurface == null)
                {
                    rawVideo.gameObject.SetActive(true);
                    lVideoSurface = rawVideo.gameObject.AddComponent<VideoSurface>();
                    lVideoSurface.SetForUser(uid);
                    lVideoSurface.SetEnable(true);
                    lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                    lVideoSurface.SetGameFps(30);
                    //VideoSurface.rectTransform.sizeDelta = SizeToParent(VideoSurface);
                }
            }
        }

        private void OnFirstRemoteVideoFrame(uint uid, int width, int height, int elapsed)
        {
            if (rawVideo.texture == null || uid == mUid)
                return;
            float lAspectRatio = height / width;
            rawVideo.rectTransform.sizeDelta = new Vector2(WIDTH, WIDTH * lAspectRatio);
        }

        private void OnStreamMessage(uint userId, int streamId, string data, int length)
        {
            Texture2D tex = new Texture2D(16, 16, TextureFormat.PVRTC_RGBA4, false);
            tex.LoadRawTextureData(System.Text.Encoding.UTF8.GetBytes(data));
            tex.Apply();
            rawVideo.gameObject.SetActive(true);
            rawVideo.texture = tex;
        }

        void OnApplicationQuit()
        {
            if (mRtcEngine != null)
            {
                // Destroy the IRtcEngine object.
                IRtcEngine.Destroy();
                mRtcEngine = null; 
            }
        }

    }
}