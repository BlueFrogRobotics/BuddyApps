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
        private RawImage rawVideoLocal;

        [SerializeField]
        private Image buttonEnableVideo;

        [SerializeField]
        private Image buttonEnableAudio;

        //private string mChannel;
        public bool mVideoIsEnabled;
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

            InitRTC();
        }

        private void Update()
        {
            
        }

        public void InitRTC()
        {
            Buddy.WebServices.Agoraio.LoadEngine(CoursTelepresenceBehaviour.APP_ID);
            mRtcEngine = Buddy.WebServices.Agoraio.RtcEngine;
            mRtcEngine.OnStreamMessage = OnStreamMessage;
            mVideoIsEnabled = true;
            mAudioIsEnabled = true;
            rawVideo.gameObject.SetActive(false);
        }

        public void InitButtons()
        {
            mVideoIsEnabled = true;
            mAudioIsEnabled = true;
            buttonEnableVideo.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconVideoOn");//EnableVideoSprite;
            buttonEnableAudio.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconMicroOn"); //EnableAudioSprite;

        }

        public void Join(string iChannel)
        {
            mRtcEngine.OnRemoteVideoStateChanged = OnRemoteVideoStateChanged;
            mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
            mRtcEngine.OnUserJoined = OnUserJoined;
            mRtcEngine.OnUserOffline = OnUserOffline;
            //mRtcEngine.OnFirstRemoteVideoDecoded = OnFirstRemoteVideoFrame;
            mRtcEngine.OnLocalVideoStats = OnLocalVideoStats;
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
            if (rawVideo.GetComponent<VideoSurface>() != null)
                Destroy(rawVideo.GetComponent<VideoSurface>());
            rawVideo.texture = null;
            rawVideo.gameObject.SetActive(false);
        }

        public void SwitchVideoState()
        {
            if (mVideoIsEnabled) {
                mRtcEngine.MuteLocalVideoStream(true);
                buttonEnableVideo.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconVideoOff");//DisableSprite;
            } else {
                mRtcEngine.MuteLocalVideoStream(false);
                buttonEnableVideo.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconVideoOn");//EnableVideoSprite;
            }
            mVideoIsEnabled = !mVideoIsEnabled;
        }

        public void SwitchAudioState()
        {
            if (mAudioIsEnabled) {
                //mRtcEngine.MuteLocalAudioStream(false);
                mRtcEngine.DisableAudio();
                buttonEnableAudio.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconMicroOff");
            } else {
                //mRtcEngine.MuteLocalAudioStream(true);
                mRtcEngine.EnableAudio();
                buttonEnableAudio.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconMicroOn"); //EnableAudioSprite;

            }
            mAudioIsEnabled = !mAudioIsEnabled;
        }

        public void SendPicture(Texture2D iTexture)
        {
            int lDataId = mRtcEngine.CreateDataStream(true, true); 
            byte[] iDataByte = iTexture.EncodeToPNG();
            string lDataString = System.Text.Encoding.UTF8.GetString(iDataByte, 0, iDataByte.Length);
            mRtcEngine.SendStreamMessage(lDataId, lDataString);
        }

        public void DestroyRTC()
        {
            if (mRtcEngine != null) {
                // Destroy the IRtcEngine object.
                IRtcEngine.Destroy();
                mRtcEngine = null;
            }
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
            if (lVideoSurface == null && uid != mUid) {
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
            Debug.LogWarning("[AGORAIO] onUserOffline: uid = " + uid+" reason: "+reason);
            if (reason == USER_OFFLINE_REASON.QUIT)
            {
                Destroy(rawVideo.GetComponent<VideoSurface>());
                rawVideo.texture = null;
                rawVideo.gameObject.SetActive(false);
                OnEndUserOffline();
            }
        }

        private void OnRemoteVideoStateChanged(uint uid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
        {
            Debug.Log("remote video " + state);
            if (uid == mUid)
                return;
            VideoSurface lVideoSurface = rawVideo.GetComponent<VideoSurface>();
            if (state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STOPPED) {
                if (lVideoSurface != null) {
                    lVideoSurface.SetEnable(false);
                    Destroy(rawVideo.GetComponent<VideoSurface>());
                    rawVideo.texture = null;
                    rawVideo.gameObject.SetActive(false);
                }
            } else if (state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STARTING || state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_DECODING) {
                if (lVideoSurface == null) {
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
            float lAspectRatio = height / width;
            Debug.LogError("first remote frame: " + width + " " + height+" "+ lAspectRatio);
            if (rawVideo.texture == null || uid == mUid)
                return;
            rawVideo.rectTransform.sizeDelta = new Vector2(WIDTH, WIDTH * lAspectRatio); 
        }

        private void OnFirstLocalVideoFrame(int width, int height, int elapsed)
        {
            float lAspectRatio = height / width;
            Debug.LogError("first local frame: " + width + " " + height+ " " + lAspectRatio);
            if (rawVideoLocal.texture == null)
                return;
            rawVideoLocal.rectTransform.sizeDelta = new Vector2(360, 360 * lAspectRatio);
        }

        private void OnStreamMessage(uint userId, int streamId, string data, int length)
        {
            Texture2D tex = new Texture2D(16, 16, TextureFormat.PVRTC_RGBA4, false);
            tex.LoadRawTextureData(System.Text.Encoding.UTF8.GetBytes(data));
            tex.Apply();
            VideoSurface lVideoSurface = rawVideo.GetComponent<VideoSurface>();
            if (lVideoSurface != null)
            {
                lVideoSurface.SetEnable(false);
                Destroy(rawVideo.GetComponent<VideoSurface>());
            }
            rawVideo.gameObject.SetActive(true);
            rawVideo.texture = tex;
        }

        private void OnLocalVideoStats(LocalVideoStats iLocalVideoStats)
        {
            float lAspectRatio = (float)iLocalVideoStats.encodedFrameHeight / (float)iLocalVideoStats.encodedFrameWidth;
            Debug.LogError("first local stats: " + iLocalVideoStats.encodedFrameWidth + " " + iLocalVideoStats.encodedFrameHeight + " " + lAspectRatio);
            if (rawVideoLocal.texture == null)
                return;
            //rawVideoLocal.rectTransform.sizeDelta = new Vector2(360, 360 * lAspectRatio);
            //rawVideoLocal.rectTransform.sizeDelta = new Vector2(iLocalVideoStats.encodedFrameWidth, 640);
        }

        void OnApplicationQuit()
        {
            if (mRtcEngine != null) {
                // Destroy the IRtcEngine object.
                Leave();
                IRtcEngine.Destroy();
                mRtcEngine = null;
            }
        }

    }
}
