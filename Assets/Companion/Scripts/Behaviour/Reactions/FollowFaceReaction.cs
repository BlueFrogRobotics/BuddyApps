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
        private Mood mMood;
        private NoHinge mNoHinge;
        private YesHinge mYesHinge;

        void Start()
        {
            mFaceTracker = GetComponent<FaceCascadeTracker>();
            mMood = BYOS.Instance.Mood;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
        }

        void OnEnable()
        {
            Debug.Log("FaceFollow Enabled");

            //Init the different variables
            mMood.Set(MoodType.NEUTRAL);
            mUpdateTime = Time.time;
            mTrackedObjects = mFaceTracker.TrackedObjects;
            mCameraWidthCenter = BYOS.Instance.RGBCam.Width / 2;
            mCameraHeightCenter = BYOS.Instance.RGBCam.Height / 2;
            mHeadNoAngle = mNoHinge.CurrentAnglePosition;
            mHeadYesAngle = mYesHinge.CurrentAnglePosition;
        }

        void Update()
        {
            if (!CompanionData.Instance.CanMoveHead || Time.time - mUpdateTime < 0.08F)
                return;

            mTrackedObjects = mFaceTracker.TrackedObjects;

            if (mTrackedObjects.Count > 0)
            {
                float lXCenter = mTrackedObjects[0].x + mTrackedObjects[0].width / 2;
                float lYCenter = mTrackedObjects[0].y + mTrackedObjects[0].height / 2;
                Debug.Log("Tracking face : XCenter " + lXCenter + " / YCenter " + lYCenter);

                if (!(mCameraWidthCenter - 25 < lXCenter && lXCenter < mCameraWidthCenter + 5))
                    mHeadNoAngle -= Mathf.Sign(lXCenter - mCameraWidthCenter) * 1.5F;
                if (!(mCameraHeightCenter - 5 < lYCenter && lYCenter < mCameraHeightCenter + 25))
                    mHeadYesAngle += Mathf.Sign(lYCenter - mCameraHeightCenter) * 1.5F;

                Debug.Log("Setting angles Yes : " + Mathf.Sign(lYCenter - mCameraHeightCenter) + 
                    " / No : " + Mathf.Sign(lXCenter - mCameraWidthCenter));
                
                mYesHinge.SetPosition(mHeadYesAngle);
                mNoHinge.SetPosition(mHeadNoAngle);
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