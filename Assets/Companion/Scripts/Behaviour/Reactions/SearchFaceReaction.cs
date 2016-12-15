using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;

namespace BuddyApp.Companion
{
    public class SearchFaceReaction : MonoBehaviour
    {
        private bool mLookLeft;
        private float mTime;
        private float mHeadNoAngle;
        private FaceDetector mFaceDetector;

        void Start()
        {
            mFaceDetector = GetComponent<FaceDetector>();
        }

        void OnEnable()
        {
            mLookLeft = true;
            mTime = Time.time;
            mHeadNoAngle = BYOS.Instance.Motors.NoHinge.CurrentAnglePosition;
            new SetPosYesCmd(-5F).Execute();
        }
        
        void Update()
        {
            if (mFaceDetector.FaceDetected) {
                enabled = false;
                return;
            }

            if (Time.time - mTime > 0.5F)
                return;

            if(mLookLeft) {
                mHeadNoAngle += 3F;

                if(mHeadNoAngle > 30F) {
                    mHeadNoAngle = 30F;
                    mLookLeft = false;
                }
                new SetPosNoCmd(mHeadNoAngle).Execute();
            }
            else {
                mHeadNoAngle -= 3F;

                if (mHeadNoAngle < -30F)
                {
                    mHeadNoAngle = -30F;
                    mLookLeft = true;
                }
                new SetPosNoCmd(mHeadNoAngle).Execute();
            }
        }

        void OnDisable()
        {
            GetComponent<Reaction>().ActionFinished();
        }
    }
}