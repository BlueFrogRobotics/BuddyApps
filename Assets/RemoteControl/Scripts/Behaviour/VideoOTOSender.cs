using UnityEngine;
using OpenCVUnity;

namespace BuddyApp.Remote
{
    [RequireComponent(typeof(WebcamManager))]
    public class VideoOTOSender : OTONetSender
    {
        [SerializeField]
        private OTONetwork OTO;
        private WebcamManager mWebcam;
        private MatOfByte mBuffer;
        [Range(1, 100)]
        public int mCompressQuality;
        [Range(5, 30)]
        public float mRequestedFPS;
        private float mLastTime;

        void Start()
        {
            mWebcam = GetComponent<WebcamManager>();
            mLastTime = Time.time;
        }

        void Update()
        {
            if (!OTO.HasAPeer || !isActiveAndEnabled)
                return;

            if (Time.time - mLastTime < 1 / mRequestedFPS) return;
            mLastTime = Time.time;

            mWebcam.CompressQuality = mCompressQuality;
            mBuffer = mWebcam.GetBuffer();
            byte[] lData = mBuffer.toArray();
            if (lData == null) return;
            SendData(lData, lData.Length);
        }
    }
}