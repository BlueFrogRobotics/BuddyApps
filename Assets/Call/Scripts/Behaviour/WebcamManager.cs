using UnityEngine;
using BuddyOS;
using OpenCVUnity;
using UnityEngine.UI;

namespace BuddyApp.Call
{
    public class WebcamManager : MonoBehaviour
    {
        [Range(1, 100)]
        public int compressQuality;

        private RGBCam mWebcam;
        private MatOfByte mBuffer;
        public RawImage mDebugScreen;
        
        void Start()
        {
            mBuffer = new MatOfByte();
            mWebcam = BYOS.Instance.RGBCam;

            if(!mWebcam.IsOpen)
                mWebcam.Open();
        }
        
        void Update()
        {
            if (mWebcam.FrameMat != null) {
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