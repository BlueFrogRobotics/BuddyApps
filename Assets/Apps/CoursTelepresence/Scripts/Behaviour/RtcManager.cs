using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.CoursTelepresence
{
    public class RtcManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject ObjVideoSurface;

        private string mChannel;

        // Use this for initialization
        void Start()
        {
            Buddy.WebServices.Agoraio.LoadEngine("dc949460a57e4fb0990a219b799ccf13");
            //IRtcEngine rtc = Buddy.WebServices.Agoraio.RtcEngine;
            Buddy.WebServices.Agoraio.RtcEngine.OnRemoteVideoStateChanged = OnRemoteVideoStateChanged;
            Buddy.WebServices.Agoraio.RtcEngine.OnStreamMessage = OnStreamMessage;
            mChannel = "truc";
            Join();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Join()
        {
            Buddy.WebServices.Agoraio.OnUserConnected = onUserJoined;
            Buddy.WebServices.Agoraio.OnUserOffline = onUserOffline;
            //Buddy.WebServices.Agoraio.Join(channel.text);
            Buddy.WebServices.Agoraio.Join(mChannel);

        }

        public void Leave()
        {
            Buddy.WebServices.Agoraio.Leave();
        }

        public void EnableVideo()
        {

        }

        private void onUserJoined(uint uid, int elapsed)
        {
            Debug.Log("[AGORAIO] onUserJoined: uid = " + uid + " elapsed = " + elapsed);

            //VideoSurface lVideoSurface = ObjVideoSurface.AddComponent<VideoSurface>();
            //lVideoSurface.SetForUser(uid);
            //lVideoSurface.SetEnable(true);
            //lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            //lVideoSurface.SetGameFps(30);
        }

        private void onUserOffline(uint uid)
        {
            // remove video stream
            Debug.Log("[AGORAIO] onUserOffline: uid = " + uid);
            Destroy(ObjVideoSurface.GetComponent<VideoSurface>());
            ObjVideoSurface.GetComponent<RawImage>().texture = null;
        }

        private void OnRemoteVideoStateChanged(uint uid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
        {
            if(state==REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STOPPED)
            {
                VideoSurface lVideoSurface = ObjVideoSurface.GetComponent<VideoSurface>();
                if (lVideoSurface != null)
                {
                    //lVideoSurface.SetForUser(uid);
                    lVideoSurface.SetEnable(false);
                    //lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                    //lVideoSurface.SetGameFps(30);
                    Destroy(ObjVideoSurface.GetComponent<VideoSurface>());
                    ObjVideoSurface.GetComponent<RawImage>().texture = null;
                }
            }
            else if(state==REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STARTING)
            {
                VideoSurface lVideoSurface = ObjVideoSurface.AddComponent<VideoSurface>();
                lVideoSurface.SetForUser(uid);
                lVideoSurface.SetEnable(true);
                lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                lVideoSurface.SetGameFps(30);
            }
        }

        private void OnStreamMessage(uint userId, int streamId, string data, int length)
        {
            Texture2D tex = new Texture2D(16, 16, TextureFormat.PVRTC_RGBA4, false);
            tex.LoadRawTextureData(System.Text.Encoding.UTF8.GetBytes(data));
            tex.Apply();
            ObjVideoSurface.GetComponent<RawImage>().texture = tex;
        }

    }
}