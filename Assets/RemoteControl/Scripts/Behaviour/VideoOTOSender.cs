using UnityEngine;
using OpenCVUnity;

namespace BuddyApp.Remote
{
    [RequireComponent(typeof(WebcamManager))]
    public class VideoOTOSender : OTONetSender
    {
        [Range(1, 100)]
        public int mCompressQuality;

        [Range(5, 30)]
        public float mRequestedFPS;

        [SerializeField]
        private OTONetwork OTO;

        private float mLastTime;
        private WebcamManager mWebcam;
        private MatOfByte mBuffer;

        void Start()
        {
            mWebcam = GetComponent<WebcamManager>();
            mLastTime = Time.time;
        }

        void Update()
        {
            if (!OTO.HasAPeer || !isActiveAndEnabled)
                return;

            if (Time.time - mLastTime < 1 / mRequestedFPS)
                return;

            //Debug.Log("Video OTO has peer " + OTO.HasAPeer + " and sending");
            mLastTime = Time.time;
            mWebcam.compressQuality = mCompressQuality;
            mBuffer = mWebcam.GetBuffer();
            byte[] lData = mBuffer.toArray();

            if (lData == null)
                return;

            SendData(lData, lData.Length);
        }
    }
}