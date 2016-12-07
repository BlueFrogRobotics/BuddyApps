using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;
using BuddyFeature.Vision;
using OpenCVUnity;

namespace BuddyApp.HideAndSeek
{
    public class FaceDetector : MonoBehaviour
    {
        [SerializeField]
        private FaceCascadeTracker faceTracker;

        public bool HasDetectedFace { get { return mHasDetectedFace; } }

        private bool mHasDetectedFace = false;

        // Use this for initialization
        void Start()
        {
            RGBCam lCam = BYOS.Instance.RGBCam;
            if(!lCam.IsOpen)
            {
                lCam.Open();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (faceTracker.TrackedObjects.Count > 0)
                mHasDetectedFace = true;
            else
                mHasDetectedFace = false;
        }
    }
}