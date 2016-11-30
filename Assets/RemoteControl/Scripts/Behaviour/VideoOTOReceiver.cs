using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
using BuddyTools;

namespace BuddyApp.Remote
{
    public class VideoOTOReceiver : OTONetReceiver
    {
        [SerializeField]
        private RawImage WebcamShowStream;

        private int mNbReceived = 0;
        private float mTime;
        private Mat mDecodedImage;
        private Texture2D mTemp2DTexture;

        void Start()
        {
            if (mTemp2DTexture == null)
                mTemp2DTexture = new Texture2D(320, 240);
            mTime = Time.time;
        }

        void Update()
        {
            if (WebcamShowStream.IsActive()) {
                if (Time.time - mTime > 2f)
                    WebcamShowStream.gameObject.SetActive(false);
            }
        }

        public override void ReceiveData(byte[] data, int ndata)
        {
            mNbReceived++;

            if (!WebcamShowStream.IsActive())
                WebcamShowStream.gameObject.SetActive(true);
            
            mDecodedImage = Highgui.imdecode(new MatOfByte(data), 3);

            if (mDecodedImage.total() != 0) {
                Utils.MatToTexture2D(mDecodedImage, mTemp2DTexture);
                WebcamShowStream.texture = mTemp2DTexture;
                mTime = Time.time;
            }
        }
    }
}