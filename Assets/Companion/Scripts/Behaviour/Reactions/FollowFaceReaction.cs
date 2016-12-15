using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using BuddyFeature.Vision;
using OpenCVUnity;
using System.Collections.Generic;
using Rect = OpenCVUnity.Rect;


namespace BuddyApp.Companion
{
    [RequireComponent(typeof(FaceCascadeTracker))]
    public class FollowFaceReaction : MonoBehaviour
    {
        private int mCameraWidthCenter;
        private int mCameraHeightCenter;
        private float mUpdateTime;
        private float mHeadYesAngle;
        private float mHeadNoAngle;
        List<Rect> mTrackedObjects;
        private FaceCascadeTracker mFaceTracker;

        void Start()
        {
            mFaceTracker = GetComponent<FaceCascadeTracker>();
        }

        void OnEnable()
        {
            Debug.Log("FaceFollow Enabled");

            //Init the different variables
            new SetMoodCmd(MoodType.NEUTRAL).Execute();
            mUpdateTime = Time.time;
            mTrackedObjects = mFaceTracker.TrackedObjects;
            mCameraWidthCenter = BYOS.Instance.RGBCam.Width / 2;
            mCameraHeightCenter = BYOS.Instance.RGBCam.Height / 2;
            mHeadNoAngle = BYOS.Instance.Motors.NoHinge.CurrentAnglePosition;
            mHeadYesAngle = BYOS.Instance.Motors.YesHinge.CurrentAnglePosition;
        }

        void Update()
        {
            if (!CompanionData.Instance.CanMoveHead || Time.time - mUpdateTime < 0.08F)
                return;

            mTrackedObjects = mFaceTracker.TrackedObjects;
            //Write here some code to make sure that one face is centered in the camera
            if (mTrackedObjects.Count > 0)
            {
                float lXCenter = mTrackedObjects[0].x + mTrackedObjects[0].width / 2;
                float lYCenter = mTrackedObjects[0].y + mTrackedObjects[0].height / 2;
                Debug.Log("Tracking face : XCenter " + lXCenter + " / YCenter " + lYCenter);

                if (!(mCameraWidthCenter - 25 < lXCenter && lXCenter < mCameraWidthCenter + 5))
                    mHeadNoAngle -= Mathf.Sign(lXCenter - mCameraWidthCenter) * 2F;
                if (!(mCameraHeightCenter - 5 < lYCenter && lYCenter < mCameraHeightCenter + 25))
                    mHeadYesAngle += Mathf.Sign(lYCenter - mCameraHeightCenter) * 2F;

                Debug.Log("Setting angles Yes : " + Mathf.Sign(lYCenter - mCameraHeightCenter) + 
                    " / No : " + Mathf.Sign(lXCenter - mCameraWidthCenter));

                new SetPosYesCmd(mHeadYesAngle).Execute();
                new SetPosNoCmd(mHeadNoAngle).Execute();
                //yield return new WaitForSeconds(0.1F);

                //if (Time.time - mFaceAndTalkTime > 30F)
                //{
                //    AskSomething();
                //    mFaceAndTalkTime = Time.time;
                //}
            }
        }

        void OnDisable()
        {
            GetComponent<Reaction>().ActionFinished();
        }
    }
}