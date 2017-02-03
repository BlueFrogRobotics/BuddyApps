using UnityEngine;
using BuddyOS;
using BuddyFeature.Vision;
using OpenCVUnity;
using System.Collections.Generic;
using Rect = OpenCVUnity.Rect;

namespace BuddyApp.Companion
{
    /// <summary>
    /// Behavior to make Buddy's head to be in front of a detected Face using Cascade Face Tracker
    /// </summary>
    [RequireComponent(typeof(FaceCascadeTracker))]
    public class FollowFaceReaction : MonoBehaviour
    {
        [SerializeField]
        private GameObject mTrackingText;

        private int mCameraWidthCenter;
        private int mCameraHeightCenter;
        private float mUpdateTime;
        private float mHeadYesAngle;
        private float mHeadNoAngle;
        List<Rect> mTrackedObjects;

        private bool mInitialized;
        private FaceCascadeTracker mFaceTracker;
        private Mood mMood;
        private NoHinge mNoHinge;
        private YesHinge mYesHinge;

        void Start()
        {
            mInitialized = true;
            mFaceTracker = GetComponent<FaceCascadeTracker>();
            mMood = BYOS.Instance.Mood;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
        }

        void OnEnable()
        {
            //Initialize everything
            if (!mInitialized) {
                Start();
                mInitialized = true;
            }

            Debug.Log("FaceFollow Enabled");

            //Init the different variables
            mUpdateTime = Time.time;
            mTrackedObjects = mFaceTracker.TrackedObjects;
            //Get the center of the camera
            mCameraWidthCenter = BYOS.Instance.RGBCam.Width / 2;
            mCameraHeightCenter = BYOS.Instance.RGBCam.Height / 2;
            //Get head position for precise control
            mHeadNoAngle = mNoHinge.CurrentAnglePosition;
            mHeadYesAngle = mYesHinge.CurrentAnglePosition;
            mTrackingText.SetActive(true);
        }

        void Update()
        {
            if (!CompanionData.Instance.CanMoveHead || Time.time - mUpdateTime < 0.08F)
                return;

            //Get tracked objects detected by the Cascade Tracker
            mTrackedObjects = mFaceTracker.TrackedObjects;

            //If something is recognized
            if (mTrackedObjects.Count > 0)
            {
                //Get the center of the recognized area
                float lXCenter = mTrackedObjects[0].x + mTrackedObjects[0].width / 2;
                float lYCenter = mTrackedObjects[0].y + mTrackedObjects[0].height / 2;

                //Control the head depending on where the area is.
                //Note : since the camera is on Buddy's right side, the control takes this into account
                if (!(mCameraWidthCenter - 25 < lXCenter && lXCenter < mCameraWidthCenter + 5))
                    mHeadNoAngle -= Mathf.Sign(lXCenter - mCameraWidthCenter) * 1F;
                if (!(mCameraHeightCenter - 5 < lYCenter && lYCenter < mCameraHeightCenter + 25))
                    mHeadYesAngle += Mathf.Sign(lYCenter - mCameraHeightCenter) * 1.5F;
                                
                mYesHinge.SetPosition(mHeadYesAngle);
                mNoHinge.SetPosition(mHeadNoAngle);                
            }
        }

        void OnDisable()
        {
            mTrackingText.SetActive(false);
            GetComponent<Reaction>().ActionFinished();
        }
    }
}