using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using OpenCVUnity;

using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.CoursTelepresence
{
    public class RtcManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject ObjVideoSurface;

        [SerializeField]
        private Sprite EnableSprite;

        [SerializeField]
        private Sprite DisableSprite;

        [SerializeField]
        private Button buttonEnableVideo;

        private string mChannel;
        private bool mVideoIsEnabled;

        private IRtcEngine mRtcEngine;

        private uint mUid;

        private void Awake()
        {
            if (Application.isEditor)
                Application.runInBackground = true;
        }

        // Use this for initialization
        void Start()
        {

            Buddy.WebServices.Agoraio.LoadEngine("dc949460a57e4fb0990a219b799ccf13");
            mRtcEngine = Buddy.WebServices.Agoraio.RtcEngine;
            mRtcEngine.OnRemoteVideoStateChanged = OnRemoteVideoStateChanged;
            mRtcEngine.OnStreamMessage = OnStreamMessage;
            mChannel = "a";
            Join();
            buttonEnableVideo.GetComponent<Image>().sprite = EnableSprite;
            mVideoIsEnabled = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Join()
        {
            mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
            mRtcEngine.OnUserJoined = onUserJoined;
            mRtcEngine.OnUserOffline = onUserOffline;
            //mRtcEngine.OnRemoteVideoStateChanged = OnRemoteVideoStateChanged;
            // enable video
            mRtcEngine.EnableVideo();
            // allow camera output callback
            mRtcEngine.EnableVideoObserver();

            // join channel
            mRtcEngine.JoinChannel(mChannel, null, 0);

        }

        public void Leave()
        {
            Buddy.WebServices.Agoraio.Leave();
        }

        public void SwitchVideoState()
        {
            if (!mVideoIsEnabled)
            {
                Buddy.WebServices.Agoraio.RtcEngine.MuteLocalVideoStream(false);
                buttonEnableVideo.GetComponent<Image>().sprite = EnableSprite;
            }
            else
            {
                Buddy.WebServices.Agoraio.RtcEngine.MuteLocalVideoStream(true);
                buttonEnableVideo.GetComponent<Image>().sprite = DisableSprite;
            }
            mVideoIsEnabled = !mVideoIsEnabled;
        }

        private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
            mUid = uid;
            //GameObject textVersionGameObject = GameObject.Find("VersionText");
            //textVersionGameObject.GetComponent<Text>().text = "Version : " + getSdkVersion();
        }

        private void onUserJoined(uint uid, int elapsed)
        {
            Debug.Log("[AGORAIO] onUserJoined: uid = " + uid + " elapsed = " + elapsed);
            VideoSurface lVideoSurface = ObjVideoSurface.GetComponent<VideoSurface>();
            if (lVideoSurface == null && uid != mUid)
            {
                lVideoSurface = ObjVideoSurface.AddComponent<VideoSurface>();
                lVideoSurface.SetForUser(uid);
                lVideoSurface.SetEnable(true);
                lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                lVideoSurface.SetGameFps(30);
            }
        }

        private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
        {
            // remove video stream
            Debug.Log("[AGORAIO] onUserOffline: uid = " + uid);
            Destroy(ObjVideoSurface.GetComponent<VideoSurface>());
            ObjVideoSurface.GetComponent<RawImage>().texture = null;
        }

        private void OnRemoteVideoStateChanged(uint uid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
        {
            Debug.Log("remote video " + state);
            if (uid == mUid)
                return;
            VideoSurface lVideoSurface = ObjVideoSurface.GetComponent<VideoSurface>();
            if (state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STOPPED)
            {
                if (lVideoSurface != null)
                {
                    //lVideoSurface.SetForUser(uid);
                    lVideoSurface.SetEnable(false);
                    //lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                    //lVideoSurface.SetGameFps(30);
                    Destroy(ObjVideoSurface.GetComponent<VideoSurface>());
                    ObjVideoSurface.GetComponent<RawImage>().texture = null;
                    ObjVideoSurface.SetActive(false);
                }
            }
            else if (state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STARTING || state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_DECODING)
            {
                if (lVideoSurface == null)
                {
                    ObjVideoSurface.SetActive(true);
                    lVideoSurface = ObjVideoSurface.AddComponent<VideoSurface>();
                    lVideoSurface.SetForUser(uid);
                    lVideoSurface.SetEnable(true);
                    lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                    lVideoSurface.SetGameFps(30);
                }
            }
        }

        private void OnStreamMessage(uint userId, int streamId, string data, int length)
        {
            Texture2D tex = new Texture2D(16, 16, TextureFormat.PVRTC_RGBA4, false);
            tex.LoadRawTextureData(System.Text.Encoding.UTF8.GetBytes(data));
            tex.Apply();
            ObjVideoSurface.SetActive(true);
            ObjVideoSurface.GetComponent<RawImage>().texture = tex;
        }

    }
}