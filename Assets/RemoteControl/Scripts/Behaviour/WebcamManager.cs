using UnityEngine;
using BuddyOS;
using OpenCVUnity;
using UnityEngine.UI;

namespace BuddyApp.Remote
{
    public class WebcamManager : MonoBehaviour
    {
        private RGBCam mWebcam;
        private MatOfByte mBuffer = new MatOfByte();
        //public RawImage mDebugScreen;
        public int CompressQuality { get { return compressQuality; } set { compressQuality = value; } }
        [Range(1, 100)]
        public int compressQuality;

        public RawImage mDebugScreen;

        // Use this for initialization
        void Start()
        {

            mWebcam = BYOS.Instance.RGBCam;
            mWebcam.Open();
        }

        // Update is called once per frame
        void Update()
        {
            if (mWebcam.FrameMat != null)
            {
                Mat lFrame = mWebcam.FrameMat;
                mDebugScreen.texture = mWebcam.FrameTexture2D;
                Imgproc.resize(lFrame, lFrame, new Size(320, 240));
                MatOfInt compression = new MatOfInt(Highgui.CV_IMWRITE_JPEG_QUALITY, compressQuality);
                //Compression here corresponds to the 'quality' of the jpeg generated
                Highgui.imencode(".jpg", lFrame, mBuffer, compression);
            }
        }

        public MatOfByte GetBuffer()
        {
            return mBuffer;
        }
    }
}