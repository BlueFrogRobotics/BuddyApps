using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;
using BuddyFeature.Vision;
using OpenCVUnity;

namespace BuddyApp.HideAndSeek
{
    [RequireComponent(typeof(FaceCascadeTracker))]
    public class FaceDetector : MonoBehaviour
    {

        private FaceCascadeTracker mFaceTracker;

        public bool HasDetectedFace { get { return mHasDetectedFace; } }

        public Mat CamView { get { return mFaceTracker.FrameMat; } }

        private bool mHasDetectedFace = false;

        // Use this for initialization
        void Start()
        {
            RGBCam lCam = BYOS.Instance.RGBCam;
            mFaceTracker = GetComponent<FaceCascadeTracker>();
            if(!lCam.IsOpen)
            {
                lCam.Open();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (mFaceTracker.TrackedObjects.Count > 0)
            {
                mHasDetectedFace = true;
                Debug.Log("visage!");
            }
            else
            {
                mHasDetectedFace = false;
                Debug.Log("rien!");
            }
        }
    }
}