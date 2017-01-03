using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;

namespace BuddyApp.Companion
{
    public class SearchFaceReaction : MonoBehaviour
    {
        private bool mLookLeft;
        private bool mLookUp;
        private float mTime;
        private float mHeadNoAngle;
        private FaceDetector mFaceDetector;
        private NoHinge mNoHinge;
        private YesHinge mYesHinge;

        void Start()
        {
            mFaceDetector = GetComponent<FaceDetector>();
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
        }

        void OnEnable()
        {
            if(mNoHinge == null)
                Start();

            mLookLeft = true;
            mLookUp = true;
            mTime = Time.time;
            mHeadNoAngle = mNoHinge.CurrentAnglePosition;
            mYesHinge.SetPosition(-5F);
        }
        
        void Update()
        {
            if (mFaceDetector.FaceDetected) {
                enabled = false;
                return;
            }

            if (Time.time - mTime < 0.1F)
                return;

            mTime = Time.time;

            if (mLookLeft) {
                mHeadNoAngle += 4F;

                if (mHeadNoAngle > 30F) {
                    mHeadNoAngle = 30F;
                    mLookLeft = false;
                    ChangeYesPosition();
                }
                mNoHinge.SetPosition(mHeadNoAngle);
            }
            else {
                mHeadNoAngle -= 4F;

                if (mHeadNoAngle < -30F)
                {
                    mHeadNoAngle = -30F;
                    mLookLeft = true;
                    ChangeYesPosition();
                }
                mNoHinge.SetPosition(mHeadNoAngle);
            }
        }

        void OnDisable()
        {
            GetComponent<Reaction>().ActionFinished();
        }

        private void ChangeYesPosition()
        {
            float lCurrPos = mYesHinge.CurrentAnglePosition;
            if (lCurrPos < -10F && mLookUp)
                mLookUp = false;
            else if (lCurrPos > 25F && !mLookUp)
                mLookUp = true;

            if (mLookUp)
                mYesHinge.SetPosition(lCurrPos - 7F);
            else
                mYesHinge.SetPosition(lCurrPos + 7F);
        }
    }
}