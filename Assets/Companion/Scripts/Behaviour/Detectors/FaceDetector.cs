﻿using UnityEngine;
using BuddyOS;
using BuddyFeature.Vision;

namespace BuddyApp.Companion
{
    [RequireComponent(typeof(FaceCascadeTracker))]
    public class FaceDetector : MonoBehaviour
    {
        public bool FaceRecognizedDetected { get { return mFaceRecognizedDetected; } }
        public bool FaceDetected { get { return mFaceDetected; } }

        private bool mFaceRecognizedDetected;
        private bool mFaceDetected;
        private RGBCam mCamera;
        private FaceRecognizer mFaceReco;
        private FaceCascadeTracker mFaceTracker;

        void Start()
        {
            mFaceRecognizedDetected = false;
            mFaceDetected = false;
            mCamera = BYOS.Instance.RGBCam;
            mCamera.Resolution = RGBCamResolution.W_176_H_144;
            mFaceTracker = GetComponent<FaceCascadeTracker>();

            if (!mCamera.IsOpen)
                mCamera.Open();
        }
        
        void Update()
        {
            if (!mCamera.IsOpen) {
                mFaceDetected = false;
                return;
            }

            if (mFaceTracker.TrackedObjects.Count > 0)
                mFaceDetected = true;
            else
                mFaceDetected = false;

            //if (mFaceReco.RecognizedProfils.Count > 0)
            //    mFaceRecognizedDetected = true;
            //else
            //    mFaceRecognizedDetected = false;
        }
    }
}