using UnityEngine;
using OpenCVUnity;

namespace BuddyApp.Remote
{
    public class VideoOTOSender : OTONetSender
    {
        [SerializeField]
        private OTONetwork OTO;
        [SerializeField]
        private WebcamManager Webcam;
        private MatOfByte mBuffer;
        [Range(1, 100)]
        public int mCompressQuality;
        [Range(5, 30)]
        public float mRequestedFPS;
        private float mLastTime;

        void Start()
        {
            mLastTime = Time.time;
        }

        void Update()
        {
            if (!OTO.HasAPeer || !isActiveAndEnabled)
                return;

            if (Time.time - mLastTime < 1 / mRequestedFPS) return;
            mLastTime = Time.time;

            Webcam.CompressQuality = mCompressQuality;
            mBuffer = Webcam.GetBuffer();
            byte[] lData = mBuffer.toArray();
            if (lData == null) return;
            SendData(lData, lData.Length);
        }
    }
}