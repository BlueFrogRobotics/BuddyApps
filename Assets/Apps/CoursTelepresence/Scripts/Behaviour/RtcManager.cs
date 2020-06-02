using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using OpenCVUnity;

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

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
        private int mIndexImage;
        private Texture2D mTexture;
        private const int WIDTH = 1280;
        private const int HEIGHT = 800;
        //private UnityEngine.Rect mRect;
        //private Texture2D mTextureShared;
        private bool mFrameProcessing;
        //private int mDownSample;

        private string mToken;

        private const string GET_TOKEN_URL = "https://teamnet-bfr.ey.r.appspot.com/rtcToken?channelName=";


        public Action OnEndUserOffline { get; set; }

        private void Awake()
        {
            if (Application.isEditor)
                Application.runInBackground = true;
        }

        // Use this for initialization
        void Start()
        {
            mIndexImage = 100;
            Debug.Log("test " + Time.time);

            InitRTC();
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
            // Start camera RGB
            mFrameProcessing = false;
            // TODO uncomment to Change camera
            //Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320X240_15FPS_RGB);
            //Buddy.Sensors.RGBCamera.OnNewFrame.Add(UpdateVideoFrame);

            mRtcEngine.OnRemoteVideoStateChanged = OnRemoteVideoStateChanged;
            mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
            mRtcEngine.OnUserJoined = OnUserJoined;
            mRtcEngine.OnUserOffline = OnUserOffline;
            //mRtcEngine.OnFirstRemoteVideoDecoded = OnFirstRemoteVideoFrame;
            //mRtcEngine.OnLocalVideoStats = OnLocalVideoStats;
            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();

            // TODO uncomment to Change camera
            //mRtcEngine.SetExternalVideoSource(true, false);
            mRtcEngine.JoinChannel(iChannel, null, 0);

            //mRtcEngine.OnTokenPrivilegeWillExpire = OnTokenPrivilegeWillExpire;
           
            //mRtcEngine.JoinChannelWithUserAccount(mToken, iChannel, Buddy.Platform.RobotUID);

        }

        //IEnumerator ShareScreen()
        //{
        //    mFrameProcessing = true;
        //    yield return new WaitForEndOfFrame();

        //    if(mRect == null)
        //        mRect = new UnityEngine.Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height);

        //    if (mTextureShared == null)
        //        mTextureShared = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGB24, false);

        //    // Reads the Pixels of the rectangle you create.
        //    mTextureShared.ReadPixels(mRect, 0, 0);
        //    // Applies the Pixels read from the rectangle to the texture.
        //    mTextureShared.Apply();
        //    // Gets the Raw Texture data from the texture and apply it to an array of bytes.
        //    //byte[] bytes = mTextureShared.GetRawTextureData();

        //    // RGB24 to BGRA32Graphics.ConvertTexture
        //    //byte[] bytes4 = new byte[(4 * bytes.Length) / 3];
        //    byte[] bytes4 = new byte[mTextureShared.GetRawTextureData().Length / 2];
        //    EncodeYUV420SP(bytes4, mTextureShared.GetRawTextureData(), mTextureShared.width, mTextureShared.height);

        //    //int j = 0;
        //    //for (int i = 0; i < bytes4.Length; i++) {
        //    //    if (i % 4 == 3)
        //    //        bytes4[i] = 255;
        //    //    else {
        //    //        if (j % 3 == 0)
        //    //            bytes4[i] = bytes[j + 2];
        //    //        else if (j % 3 == 1)
        //    //            bytes4[i] = bytes[j];
        //    //        else if (j % 3 == 2)
        //    //            bytes4[i] = bytes[j - 2];
        //    //        j++;
        //    //    }
        //    //}



        //    // Gives enough space for the bytes array.
        //    int size = Marshal.SizeOf(bytes4[0]) * bytes4.Length;
        //    // Checks whether the IRtcEngine instance is existed.
        //    IRtcEngine rtc = IRtcEngine.QueryEngine();
        //    if (rtc != null) {
        //        // Creates a new external video frame.
        //        ExternalVideoFrame externalVideoFrame = new ExternalVideoFrame();
        //        // Sets the buffer type of the video frame.
        //        externalVideoFrame.type = ExternalVideoFrame.VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
        //        // Sets the format of the video pixel.
        //        externalVideoFrame.format = ExternalVideoFrame.VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_NV12;
        //        // Applies raw data.
        //        externalVideoFrame.buffer = bytes4;
        //        // Sets the width (pixel) of the video frame.
        //        externalVideoFrame.stride = (int)mRect.width;
        //        // Sets the height (pixel) of the video frame.
        //        externalVideoFrame.height = (int)mRect.height;
        //        // Removes pixels from the sides of the frame
        //        externalVideoFrame.cropLeft = 10;
        //        externalVideoFrame.cropTop = 10;
        //        externalVideoFrame.cropRight = 10;
        //        externalVideoFrame.cropBottom = 10;
        //        // Rotates the video frame (0, 90, 180, or 270)
        //        externalVideoFrame.rotation = 180;
        //        // Increments i with the video timestamp.
        //        externalVideoFrame.timestamp = mIndexImage++;
        //        // Pushes the external video frame with the frame you create.
        //        rtc.PushVideoFrame(externalVideoFrame);
        //        Debug.LogWarning("Shared screen");
        //        mFrameProcessing = false;
        //    }
        //}

        private void UpdateVideoFrame(RGBCameraFrame iCameraFrame)
        {
            //yield return new WaitForEndOfFrame();
            // TMP hack
            //mDownSample++;
            if (!mFrameProcessing/* && mDownSample == 4*/)
            {
                //mDownSample = 0;
                //if (mAudioIsEnabled) {
                if (iCameraFrame != null && mVideoIsEnabled)
                {
                    mFrameProcessing = true;

                    // Convert texture RGB24 to BGRA32
                    //mTexture = new Texture2D(Buddy.Sensors.RGBCamera.Frame.Width, Buddy.Sensors.RGBCamera.Frame.Height, TextureFormat.BGRA32, false);
                    //mTexture.SetPixels(Buddy.Sensors.RGBCamera.Frame.Texture.GetPixels());
                    //mTexture.Apply();

                    // ERROR: unsupported format, this is likely because the device doesn't support this format as a rendertarget format
                    //if (mTexture == null) {
                    //    Debug.LogWarning("Pre texture BGRA32 ");
                    //    mTexture = new Texture2D(Buddy.Sensors.RGBCamera.Frame.Width, Buddy.Sensors.RGBCamera.Frame.Height, TextureFormat.RGBA32, false);
                    //    Debug.LogWarning("post texture BGRA32 ");
                    //}

                    //ERROR wrong figures for total and elemsize negative
                    //Debug.LogWarning("Pre texture convertion SUPPORT BGRA?" + SystemInfo.SupportsTextureFormat(TextureFormat.BGRA32));
                    //Graphics.ConvertTexture(iCameraFrame.Texture, mTexture);
                    //Debug.LogWarning("pre mat");
                    //Mat converted_image = iCameraFrame.Mat.clone();
                    //Debug.LogWarning("mat height " + iCameraFrame.Mat.height());
                    //Debug.LogWarning("mat width " + iCameraFrame.Mat.width());
                    //Debug.LogWarning("total " + iCameraFrame.Mat.total());
                    //Debug.LogWarning("mat elem size " + iCameraFrame.Mat.elemSize());
                    //Debug.LogWarning("pre convert");
                    //Imgproc.cvtColor(iCameraFrame.Mat.clone(), converted_image, Imgproc.COLOR_RGB2YUV_I420);
                    //Debug.LogWarning("mat elem size " + iCameraFrame.Mat.elemSize());
                    ////Imgproc.cvtColor(iCameraFrame.Mat, converted_image, Imgproc.COLOR_RGB2YUV_YV12);

                    //// Gets the Raw Texture data from the texture and apply it to an array of bytes.
                    //Debug.LogWarning("pre mat to bytes");
                    //byte[] bytes4 = matToBytes(converted_image);
                    //Debug.LogWarning("post mat to byte");
                    //byte[] bytes = iCameraFrame.Texture.GetRawTextureData();
                    byte[] bytes4 = new byte[iCameraFrame.Texture.GetRawTextureData().Length / 2];
                    EncodeYUV420SP(bytes4, iCameraFrame.Texture.GetRawTextureData(), iCameraFrame.Width, iCameraFrame.Height);
                    // RGB24 to BGRA32
                    //byte[] bytes4 = new byte[(4 * bytes.Length) / 3];

                    //int j = 0;
                    //for (int i = 0; i < bytes4.Length; i++) {
                    //    if (i % 4 == 3)
                    //        bytes4[i] = 255;
                    //    else {
                    //        if (j % 3 == 0)
                    //            bytes4[i] = bytes[j + 2];
                    //        else if (j % 3 == 1)
                    //            bytes4[i] = bytes[j];
                    //        else if (j % 3 == 2)
                    //            bytes4[i] = bytes[j - 2];
                    //        j++;
                    //    }
                    //}


                    // Gives enough space for the bytes array.
                    int size = Marshal.SizeOf(bytes4[0]) * bytes4.Length;
                    // Checks whether the IRtcEngine instance is existed.
                    if (mRtcEngine != null)
                    {
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

                        // Removes pixels from the sides of the frame
                        //externalVideoFrame.cropLeft = 10;
                        //externalVideoFrame.cropTop = 10;
                        //externalVideoFrame.cropRight = 10;
                        //externalVideoFrame.cropBottom = 10;
                        // Rotates the video frame (0, 90, 180, or 270)
                        externalVideoFrame.rotation = 180;
                        // Increments i with the video timestamp.
                        externalVideoFrame.timestamp = mIndexImage++;
                        // Pushes the external video frame with the frame you create.
                        mRtcEngine.PushVideoFrame(externalVideoFrame);
                        Debug.LogWarning("RGB cam");
                        mFrameProcessing = false;
                    }
                    else
                    {
                        Debug.LogWarning("No frame on RGB camera");
                    }
                    //TMP hack
                }
                //} else
                //StartCoroutine(ShareScreen());
            }

        }

        //private byte[] matToBytes(Mat image)
        //{
        //    Debug.LogWarning("mat to byte");
        //    Debug.LogWarning("mat to byte height " + image.height());
        //    Debug.LogWarning("mat to byte width " + image.width());
        //    Debug.LogWarning("mat to byte");
        //    int size = image.height() * image.width() * 12;
        //    Debug.LogWarning("mat to byte size " + size);
        //    Debug.LogWarning("mat to byte 2 + total " + image.total());
        //    Debug.LogWarning("mat to byte 2 + elemsize " + image.elemSize());
        //    Debug.LogWarning("mat to byte 2 + size " + size);
        //    byte[] bytes = new byte[size];  // you will have to delete[] that later
        //    Debug.LogWarning("mat to byte 3");
        //    image.get(0, 0, bytes);
        //    Debug.LogWarning("mat to byte 4");
        //    return bytes;
        //}

        private void EncodeYUV420SP(byte[] yuv420sp, byte[] rgb, int width, int height)
        {
            int frameSize = width * height;

            int yIndex = 0;
            int uvIndex = frameSize;

            int R, G, B, Y, U, V;
            int index = 0;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {

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
                    if (j % 2 == 0 && index % 2 == 0)
                    {
                        yuv420sp[uvIndex++] = (byte)((U < 0) ? 0 : ((U > 255) ? 255 : U));
                        yuv420sp[uvIndex++] = (byte)((V < 0) ? 0 : ((V > 255) ? 255 : V));
                    }
                    index++;
                }
            }
        }

        public void Leave()
        {
            //TODO uncomment to change camera
            //Buddy.Sensors.RGBCamera.OnNewFrame.Remove(UpdateVideoFrame);
            //Buddy.Sensors.RGBCamera.Close();

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

        public void SetProfilePicture(string iData)
        {
            Debug.LogWarning("Set profile picture");

            //byte[] decodedString = Base64.decode(ImgName, Base64.DEFAULT);
            //Bitmap decodedByte = BitmapFactory.decodeByteArray(decodedString, 0, decodedString.length);
            byte[] newBytes = Convert.FromBase64String(iData);

            Texture2D tex = new Texture2D(16, 16);
            tex.LoadImage(newBytes);
            tex.Apply();
            FlipTextureVertically(tex);
            VideoSurface lVideoSurface = rawVideo.GetComponent<VideoSurface>();
            if (lVideoSurface != null)
            {
                lVideoSurface.SetEnable(false);
                Destroy(rawVideo.GetComponent<VideoSurface>());
            }
            rawVideo.gameObject.SetActive(true);
            rawVideo.texture = tex;
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
                if(rawVideo.GetComponent<VideoSurface>()!=null)
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
                    lVideoSurface = rawVideo.gameObject.AddComponent<VideoSurface>();
                }
                    rawVideo.gameObject.SetActive(true);
                    lVideoSurface.SetForUser(uid);
                    lVideoSurface.SetEnable(true);
                    lVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                    lVideoSurface.SetGameFps(30);
                    //VideoSurface.rectTransform.sizeDelta = SizeToParent(VideoSurface);
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

        //private void OnLocalVideoStats(LocalVideoStats iLocalVideoStats)
        //{
        //    float lAspectRatio = (float)iLocalVideoStats.encodedFrameHeight / (float)iLocalVideoStats.encodedFrameWidth;
        //    Debug.LogError("first local stats: " + iLocalVideoStats.encodedFrameWidth + " " + iLocalVideoStats.encodedFrameHeight + " " + lAspectRatio);
        //    if (rawVideoLocal.texture == null)
        //        return;
        //    //rawVideoLocal.rectTransform.sizeDelta = new Vector2(360, 360 * lAspectRatio);
        //    //rawVideoLocal.rectTransform.sizeDelta = new Vector2(iLocalVideoStats.encodedFrameWidth, 640);
        //}

        private void FlipTextureVertically(Texture2D iTexture)
        {
            var originalPixels = iTexture.GetPixels();

            Color[] newPixels = new Color[originalPixels.Length];

            int width = iTexture.width;
            int rows = iTexture.height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < rows; y++)
                {
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
            string request = GET_TOKEN_URL + lId;
            using (UnityWebRequest www = UnityWebRequest.Get(request))
            {
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
                }
                else
                {
                    string lRecoJson = www.downloadHandler.text;
                    Newtonsoft.Json.Linq.JObject lJsonNode = Utils.UnserializeJSONtoObject(lRecoJson);
                    Debug.LogError("token: " + lJsonNode["key"]);
                    mToken = (string)lJsonNode["key"];
                }
            }
        }

        private IEnumerator JoinAsync(string iChannel)
        {
            yield return GetToken(Buddy.Platform.RobotUID);
            mRtcEngine.JoinChannelWithUserAccount(mToken, iChannel, Buddy.Platform.RobotUID);
            Debug.Log("join");
        }

        private IEnumerator RenewTokenAsync()
        {
            yield return GetToken(Buddy.Platform.RobotUID);
            mRtcEngine.RenewToken(mToken);
            Debug.Log("renew");
        }

    }
}
