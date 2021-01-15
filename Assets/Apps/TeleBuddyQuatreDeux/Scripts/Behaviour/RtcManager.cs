﻿using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using OpenCVUnity;

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

namespace BuddyApp.TeleBuddyQuatreDeux
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

        public bool mVideoIsEnabled;
        private bool mAudioIsEnabled;

        private IRtcEngine mRtcEngine;

        private uint mUid;
        private int mIndexImage;
        private const int WIDTH = 1280;
        private const int HEIGHT = 800;
        private bool mFrameProcessing;


        private int mCamType;
        private string mToken;

        private const string GET_TOKEN_URL = "https://teamnet-bfr.ey.r.appspot.com/rtcToken?channelName=";
        public Action OnEndUserOffline { get; set; }
        private int lastSample = 0;
        private AudioClip lAudioClip;
        private int idSound = 0;
        private bool mIsConnecting = false;
        private long id = 0;
        public bool mCurrentCameraWide;

        private void Awake()
        {
            if (Application.isEditor)
                Application.runInBackground = true;
        }

        // Use this for initialization
        //void Start()
        //{
        //    //mIndexImage = 100;
        //    Debug.Log("MICRO avant");
        //    //lAudioClip = Microphone.Start(Microphone.devices[0], true, 10, 44100);// Buddy.Sensors.Microphones.SamplingRate);//44100

        //    AudioRecordingDeviceManager audioRecordingDeviceManager;
        //    audioRecordingDeviceManager = (AudioRecordingDeviceManager)mRtcEngine.GetAudioRecordingDeviceManager();
        //    Debug.Log("MICRO apres");
        //    if (audioRecordingDeviceManager.CreateAAudioRecordingDeviceManager())
        //    {
        //        int i = audioRecordingDeviceManager.GetAudioRecordingDeviceCount();
        //        Debug.Log(" MICROGetAudioRecordingDeviceCount = " + i);
        //    }
        //    else
        //    {
        //        Debug.Log("MICRO probleme GetAudioRecordingDeviceCount ");
        //    }
        //    //InitRTC();
        //}

        //private void Update()
        //{
        //    if (mIsConnecting)
        //    {
        //        AudioFrame audioFrame = new AudioFrame();
        //        Debug.Log("MICRO1");
        //        //Buddy.Sensors.Microphones.
        //        int pos = Microphone.GetPosition(null);
        //        Debug.Log("MICRO2 " + pos);
        //        if (lastSample > pos)
        //            lastSample = 0;
        //        int diff = pos - lastSample;
        //        Debug.Log("MICRO2bis " + diff);

        //        if (diff > 0)
        //        {
        //            float[] samples = new float[diff * lAudioClip.channels];
        //            Debug.Log("MICRO3 "+ lAudioClip.channels);
        //            lAudioClip.GetData(samples, lastSample);
        //            audioFrame.buffer = ToByteArray(samples);
        //            Debug.Log("MICRO4 "+ samples[0]);
        //            audioFrame.channels = 1;
        //            audioFrame.bytesPerSample = 4;
        //            audioFrame.renderTimeMs = id;
        //            id++;
        //            audioFrame.type = AUDIO_FRAME_TYPE.FRAME_TYPE_PCM16;
        //            mRtcEngine.PushAudioFrame(audioFrame);
        //        }
        //        Debug.Log("MICRO5");
        //        lastSample = pos;
        //    }
        //}

        private byte[] ToByteArray(float[] floatArray)
        {
            int len = floatArray.Length * 4;
            byte[] byteArray = new byte[len];
            int pos = 0;
            foreach (float f in floatArray) {
                byte[] data = System.BitConverter.GetBytes(f);
                System.Array.Copy(data, 0, byteArray, pos, 4);
                pos += 4;
            }
            return byteArray;
        }

        public string TakePhoto()
        {
            Utils.DeleteFile(Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");
            Debug.Log("take photo in rtc maanger");
            string iPathPhoto = "";
            Texture2D iPhotoTaken = (Texture2D)rawVideo.GetComponent<RawImage>().texture;
            Utils.SaveTextureToFile(iPhotoTaken, Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");
            iPathPhoto = Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg";
            return iPathPhoto;
        }


        public void InitRTC()
        {
            Buddy.WebServices.Agoraio.LoadEngine(TeleBuddyQuatreDeuxBehaviour.APP_ID);//TODO WALID: attendre que la requete zoho soit terminé avant etremplacer par l'app id recu //TODO MC : j'ai créé un nouvel init qui prend l'app id en fonction du user choisi dans la liste donc appelé dans ConnectingState ButtonClick()
            mRtcEngine = Buddy.WebServices.Agoraio.RtcEngine;
            mRtcEngine.OnStreamMessage = OnStreamMessage;
            //mRtcEngine.GetAudioRecordingDeviceManager().SetAudioRecordingDevice("1");
            mVideoIsEnabled = true;
            mAudioIsEnabled = true;
            rawVideo.gameObject.SetActive(false);

        }

        public void InitNewVersionRTC(string iAppID)
        {
            Buddy.WebServices.Agoraio.LoadEngine(iAppID);
            mRtcEngine = Buddy.WebServices.Agoraio.RtcEngine;
            mRtcEngine.OnStreamMessage = OnStreamMessage;
            mVideoIsEnabled = true;
            mAudioIsEnabled = true;
            rawVideo.gameObject.SetActive(false);
            Debug.Log("before sdk version: ");
            Debug.Log("sdk version: " + IRtcEngine.GetSdkVersion());
            AudioRecordingDeviceManager audioRecordingDeviceManager;
            audioRecordingDeviceManager = (AudioRecordingDeviceManager)mRtcEngine.GetAudioRecordingDeviceManager();
            Debug.Log("MICRO apres");
            if (audioRecordingDeviceManager.CreateAAudioRecordingDeviceManager()) {
                int i = audioRecordingDeviceManager.GetAudioRecordingDeviceCount();
                Debug.Log(" MICRO GetAudioRecordingDeviceCount = " + i);
            } else {
                Debug.Log("MICRO probleme GetAudioRecordingDeviceCount ");
            }
            //Debug.Log("NUM OF RECORDING DEVICE: " + mRtcEngine.GetAudioRecordingDeviceManager().GetAudioRecordingDeviceCount());
            //string name = "";
            //string id = "";
            //mRtcEngine.GetAudioRecordingDeviceManager().GetCurrentRecordingDeviceInfo(ref name, ref id);
            //Debug.Log("CURRENT RECORDING DEVICE NAME " + name + " ID: " + id);
        }

        public void SwitchCam()
        {
            Debug.Log("RTC MANAGER : SWITCH CAMERA");
            mCurrentCameraWide = !mCurrentCameraWide;
            mRtcEngine.SwitchCamera();
        }

        public void InitButtons()
        {
            mVideoIsEnabled = true;
            mAudioIsEnabled = true;
            buttonEnableVideo.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconVideoOn");//EnableVideoSprite;
            buttonEnableAudio.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconMicroOn"); //EnableAudioSprite;

        }

        public void SetMicrophone(string iId)
        {
            mRtcEngine.GetAudioRecordingDeviceManager().SetAudioRecordingDevice(iId);
            Debug.Log("NUM OF RECORDING DEVICE: " + mRtcEngine.GetAudioRecordingDeviceManager().GetAudioRecordingDeviceCount());
            string name = "";
            string id = "";
            mRtcEngine.GetAudioRecordingDeviceManager().GetCurrentRecordingDeviceInfo(ref name, ref id);
            Debug.Log("CURRENT RECORDING DEVICE NAME " + name + " ID: " + id);
        }

        public void Join(string iChannel)
        {
            // Start camera RGB
            mFrameProcessing = false;
            // TODO uncomment to Change camera
            //Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320X240_15FPS_RGB);
            //Buddy.Sensors.RGBCamera.OnNewFrame.Add(UpdateVideoFrame);

            //if (Buddy.Sensors.HDCamera.IsOpen)
            //    Buddy.Sensors.HDCamera.Close();
            //mCamType = 0;
            //Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640X480_30FPS_RGB, (HDCameraType)mCamType);
            //Buddy.Sensors.HDCamera.OnNewFrame.Add((iFrame) => UpdateVideoFrame(iFrame));
            mCurrentCameraWide = true;
            mRtcEngine.OnRemoteVideoStateChanged = OnRemoteVideoStateChanged;
            mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess; 
            mRtcEngine.OnUserJoined = OnUserJoined;
            mRtcEngine.OnUserOffline = OnUserOffline;
            VideoEncoderConfiguration videoEncoder = new VideoEncoderConfiguration {
                degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_QUALITY,
                dimensions = new VideoDimensions() {
                    height = 1080,
                    width = 1920
                }
            };

            //mRtcEngine.AdjustRecordingSignalVolume(120);
            //mRtcEngine.SetVideoProfile(VIDEO_PROFILE_TYPE.VIDEO_PROFILE_LANDSCAPE_4K, false);

            mRtcEngine.SetVideoEncoderConfiguration(videoEncoder);
            //mRtcEngine.SetAudioProfile(AUDIO_PROFILE_TYPE.AUDIO_PROFILE_MUSIC_HIGH_QUALITY, AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);
            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();
            //mRtcEngine.SetExternalAudioSource(true, 44100, 1);
            mRtcEngine.EnableAudio();
            mRtcEngine.SetParameters("{\"che.audio.opensl\":true}");

            // TODO uncomment to Change camera
            //mRtcEngine.SetExternalVideoSource(true, false);
            //mRtcEngine.JoinChannel(iChannel, null, 0);
            mRtcEngine.OnError = OnError;
            InitButtons();
            StartCoroutine(JoinAsync(iChannel));
        }

        private void UpdateVideoFrame(HDCameraFrame iCameraFrame)
        {
            if (!mFrameProcessing) {
                if (iCameraFrame != null && mVideoIsEnabled) {
                    mFrameProcessing = true;

                    byte[] bytes4 = new byte[iCameraFrame.Texture.GetRawTextureData().Length / 2];
                    EncodeYUV420SP(bytes4, iCameraFrame.Texture.GetRawTextureData(), iCameraFrame.Width, iCameraFrame.Height);
                    // Gives enough space for the bytes array.
                    int size = Marshal.SizeOf(bytes4[0]) * bytes4.Length;
                    // Checks whether the IRtcEngine instance is existed.
                    if (mRtcEngine != null) {
                        // Creates a new external video frame.
                        ExternalVideoFrame externalVideoFrame = new ExternalVideoFrame();
                        // Sets the buffer type of the video frame.
                        externalVideoFrame.type = ExternalVideoFrame.VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
                        // Sets the format of the video pixel.

                        //externalVideoFrame.format = ExternalVideoFrame.VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_I420;
                        externalVideoFrame.format = ExternalVideoFrame.VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_NV12;

                        // Applies raw data.
                        externalVideoFrame.buffer = bytes4;
                        //// Sets the width (pixel) of the video frame.
                        //externalVideoFrame.stride = Buddy.Sensors.RGBCamera.Frame.Texture.width;
                        //// Sets the height (pixel) of the video frame.
                        //externalVideoFrame.height = Buddy.Sensors.RGBCamera.Frame.Texture.height;


                        // Sets the width (pixel) of the video frame.
                        externalVideoFrame.stride = iCameraFrame.Width;
                        // Sets the height (pixel) of the video frame.
                        externalVideoFrame.height = iCameraFrame.Height;

                        // Rotates the video frame (0, 90, 180, or 270)
                        externalVideoFrame.rotation = 180;
                        // Increments i with the video timestamp.
                        externalVideoFrame.timestamp = mIndexImage++;
                        // Pushes the external video frame with the frame you create.
                        mRtcEngine.PushVideoFrame(externalVideoFrame);
                        mFrameProcessing = false;
                    } else {
                        Debug.LogWarning("No frame on HD camera");
                    }
                }
            }
        }

        private void EncodeYUV420SP(byte[] yuv420sp, byte[] rgb, int width, int height)
        {
            int frameSize = width * height;

            int yIndex = 0;
            int uvIndex = frameSize;

            int R, G, B, Y, U, V;
            int index = 0;

            for (int j = 0; j < height; j++) {
                for (int i = 0; i < width; i++) {

                    R = rgb[3 * index];
                    G = rgb[3 * index + 1];
                    B = rgb[3 * index + 2];

                    // well known RGB to YUV algorithm
                    Y = ((66 * R + 129 * G + 25 * B + 128) >> 8) + 16;
                    U = ((-38 * R - 74 * G + 112 * B + 128) >> 8) + 128;
                    V = ((112 * R - 94 * G - 18 * B + 128) >> 8) + 128;

                    // NV21 has a plane of Y and interleaved planes of VU each sampled by a factor of 2
                    //    meaning for every 4 Y pixels there are 1 V and 1 U.  Note the sampling is every other
                    //    pixel AND every other scanline.
                    yuv420sp[yIndex++] = (byte)((Y < 0) ? 0 : ((Y > 255) ? 255 : Y));
                    if (j % 2 == 0 && index % 2 == 0) {
                        yuv420sp[uvIndex++] = (byte)((U < 0) ? 0 : ((U > 255) ? 255 : U));
                        yuv420sp[uvIndex++] = (byte)((V < 0) ? 0 : ((V > 255) ? 255 : V));
                    }
                    index++;
                }
            }
        }

        public void MuteVideo(bool iBool)
        {
            mRtcEngine.MuteLocalVideoStream(iBool);
            if(iBool)
            {
                mRtcEngine.DisableVideo();
                mRtcEngine.DisableVideoObserver();
            }
            else
            {
                mRtcEngine.EnableVideo();
                mRtcEngine.EnableVideoObserver();
            }

        }

        

        public void Leave()
        {
            //TODO uncomment to change camera
            //Buddy.Sensors.RGBCamera.OnNewFrame.Remove(UpdateVideoFrame);
            //Buddy.Sensors.RGBCamera.Close();
            mIsConnecting = false;
            if (mRtcEngine == null)
                return;

            mRtcEngine.OnNetworkQuality = null;
            mRtcEngine.LeaveChannel();

            mRtcEngine.DisableVideoObserver();
            if (rawVideo.GetComponent<VideoSurface>() != null)
                Destroy(rawVideo.GetComponent<VideoSurface>());
            rawVideo.texture = null;
            rawVideo.gameObject.SetActive(false);
        }

        public void SwitchVideoState()
        {
            Debug.Log("SWITCH VIDEO STATE");
            if (mVideoIsEnabled) {
                mRtcEngine.MuteLocalVideoStream(true);
                buttonEnableVideo.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconVideoOff");
            } else {
                mRtcEngine.MuteLocalVideoStream(false);
                buttonEnableVideo.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconVideoOn");
            }
            mVideoIsEnabled = !mVideoIsEnabled;
        }

        public void SwitchAudioState()
        {
            Debug.Log("SWITCH AUDIO STATE");
            if (mAudioIsEnabled) {
                mRtcEngine.MuteLocalAudioStream(true);
                buttonEnableAudio.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconMicroOff");
            } else {
                mRtcEngine.MuteLocalAudioStream(false);
                buttonEnableAudio.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconMicroOn");
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

        public void SetProfilePicture(string iData, int iWidth, int iHeight)
        {
            Debug.Log("Set profile picture");
            byte[] newBytes = Convert.FromBase64String(iData);
            Texture2D tex = new Texture2D(iWidth, iHeight);
            tex.LoadImage(newBytes);
            tex.Apply();
            FlipTextureVertically(tex);
            VideoSurface lVideoSurface = rawVideo.GetComponent<VideoSurface>();
            if (lVideoSurface != null) {
                lVideoSurface.SetEnable(false);
                Destroy(rawVideo.GetComponent<VideoSurface>());
            }
            rawVideo.gameObject.SetActive(true);
            rawVideo.texture = tex;

            Debug.Log("***********HEIGHT : " + rawVideo.texture.height + " ********WIDTH : " + rawVideo.texture.width);
        }

        private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
            if (mRtcEngine.OnNetworkQuality != null)
                mRtcEngine.OnNetworkQuality += (ID, TX, RX) => QualityCheck(ID, TX, RX);
            mUid = uid;
        }

        private void QualityCheck(uint iID, int iTX, int iRX)
        {

            //if((iTX == 6 || iRX == 6) && iID != 0)
            //{
            //    TeleBuddyQuatreDeuxData.Instance.IsQualityNetworkGood = false;
            //}
            //else
            //{
            //    TeleBuddyQuatreDeuxData.Instance.IsQualityNetworkGood = true;
            //}
        }

        private void OnError(int iIdError, string iMessage)
        {
            Debug.LogError("ON ERROR RTC MANAGER id + Message : " + iIdError + " - " + iMessage);
            CheckConnectivity.Instance.OnErrorAgoraio(iIdError);
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
            }
        }

        private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
        {
            // remove video stream
            Debug.Log("[AGORAIO] onUserOffline: uid = " + uid);
            Debug.LogWarning("[AGORAIO] onUserOffline: uid = " + uid + " reason: " + reason);
            if (reason == USER_OFFLINE_REASON.QUIT || reason == USER_OFFLINE_REASON.DROPPED) {
                if (rawVideo.GetComponent<VideoSurface>() != null)
                    Destroy(rawVideo.GetComponent<VideoSurface>());
                rawVideo.texture = null;
                rawVideo.gameObject.SetActive(false);
                OnEndUserOffline();
            }
        }

        private void OnRemoteVideoStateChanged(uint uid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
        {
            Debug.Log("RTC MANAGER : remote video " + state + " reason " + reason);
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
            } else if (state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STARTING || state == REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_DECODING /*&& reason != REMOTE_VIDEO_STATE_REASON.REMOTE_VIDEO_STATE_REASON_INTERNAL*/) {
                rawVideo.gameObject.SetActive(true);
                if (lVideoSurface != null) {
                    return;
                    lVideoSurface.SetEnable(false);
                    rawVideo.texture = null;
                    Destroy(rawVideo.GetComponent<VideoSurface>());
                } else if (lVideoSurface == null) {
                    lVideoSurface = rawVideo.gameObject.AddComponent<VideoSurface>();
                    rawVideo.texture = null;
                }
                mRtcEngine.OnStreamMessage = OnStreamMessage;
                lVideoSurface.SetForUser(uid);
                lVideoSurface.SetEnable(true);
                lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                lVideoSurface.SetGameFps(30);
            }
        }


        private void OnFirstRemoteVideoFrame(uint uid, int width, int height, int elapsed)
        {
            float lAspectRatio = height / width;
            Debug.Log("first remote frame: " + width + " " + height + " " + lAspectRatio);
            if (rawVideo.texture == null || uid == mUid)
                return;
            rawVideo.rectTransform.sizeDelta = new Vector2(WIDTH, WIDTH * lAspectRatio);
        }

        private void OnFirstLocalVideoFrame(int width, int height, int elapsed)
        {
            float lAspectRatio = height / width;
            Debug.Log("first local frame: " + width + " " + height + " " + lAspectRatio);
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
            if (lVideoSurface != null) {
                lVideoSurface.SetEnable(false);
                Destroy(rawVideo.GetComponent<VideoSurface>());
            }
            rawVideo.gameObject.SetActive(true);
            rawVideo.texture = tex;
        }

        private void FlipTextureVertically(Texture2D iTexture)
        {
            var originalPixels = iTexture.GetPixels();

            Color[] newPixels = new Color[originalPixels.Length];

            int width = iTexture.width;
            int rows = iTexture.height;

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < rows; y++) {
                    newPixels[x + y * width] = originalPixels[x + (rows - y - 1) * width];
                }
            }

            iTexture.SetPixels(newPixels);
            iTexture.Apply();
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

        void OnTokenPrivilegeWillExpire(string token)
        {
            StartCoroutine(RenewTokenAsync());
        }

        private IEnumerator GetToken(string lId)
        {
            yield return new WaitForSeconds(0.1F);
            //mToken = DBManager.Instance.ListUserStudent[TeleBuddyQuatreDeuxData.Instance.IndexTablet].RTCToken; 
            mToken = DBManager.Instance.mRobotTokenRTC;

            //TODO WALID: remplacer la requete par la requete zoho pour le token rtc //TODO MC : du coup le token est récup avec le user dans ConnectingState ButtonClick()
            //string request = GET_TOKEN_URL + lId;
            //using (UnityWebRequest www = UnityWebRequest.Get(request))
            //{
            //    yield return www.SendWebRequest();
            //    if (www.isHttpError || www.isNetworkError)
            //    {
            //        Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
            //    }
            //    else
            //    {
            //        string lRecoJson = www.downloadHandler.text;
            //        Newtonsoft.Json.Linq.JObject lJsonNode = Utils.UnserializeJSONtoObject(lRecoJson);
            //        Debug.LogError("token: " + lJsonNode["key"]);
            //        mToken = (string)lJsonNode["key"];
            //    }
            //}
        }

        private IEnumerator JoinAsync(string iChannel)
        {
            yield return GetToken(iChannel);
            string lId = TeleBuddyQuatreDeuxBehaviour.EncodeToSHA256(TeleBuddyQuatreDeuxBehaviour.EncodeToMD5(/*Buddy.Platform.RobotUID*/Buddy.IO.MobileData.IMEI()));
            //mRtcEngine.JoinChannelWithUserAccount(mToken, iChannel, lId);
            mRtcEngine.JoinChannelWithUserAccount(mToken, lId, lId);
            mRtcEngine.OnTokenPrivilegeWillExpire = OnTokenPrivilegeWillExpire;
            mIsConnecting = true;
            Debug.Log("join");
        }

        private IEnumerator RenewTokenAsync()
        {
            yield return GetToken(/*Buddy.Platform.RobotUID*/Buddy.IO.MobileData.IMEI());
            mRtcEngine.RenewToken(mToken);
            Debug.Log("renew");
        }

        private void OnDestroy()
        {
            Debug.Log("ON DESTROY RTC LEAVE");
            Leave();
        }
    }
}
