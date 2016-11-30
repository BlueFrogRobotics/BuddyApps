using UnityEngine;
using BuddyOS;
using BuddyFeature.Vision;

namespace BuddyApp.Companion
{
    public class FaceDetector : MonoBehaviour
    {
        public bool FaceRecognizedDetected { get { return mFaceRecognizedDetected; } }
        public bool FaceDetected { get { return mFaceDetected; } }

        private bool mFaceRecognizedDetected;
        private bool mFaceDetected;
        private RGBCam mCamera;
        private FaceRecognizer mFaceReco;
        private FaceShiftTracker mFaceTracker;

        void Start()
        {
            mFaceRecognizedDetected = false;
            mFaceDetected = false;
            mCamera = BYOS.Instance.RGBCam;

            if (!mCamera.IsOpen)
                mCamera.Open();
        }
        
        void Update()
        {
            if (mFaceTracker.NbTrackedObjects > 0)
                mFaceDetected = true;
            else
                mFaceDetected = false;

            if (mFaceReco.RecognizedProfils.Count > 0)
                mFaceRecognizedDetected = true;
            else
                mFaceRecognizedDetected = false;
        }
    }
}