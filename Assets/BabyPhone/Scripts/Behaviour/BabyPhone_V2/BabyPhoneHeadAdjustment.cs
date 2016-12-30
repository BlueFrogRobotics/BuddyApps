using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneHeadAdjustment : MonoBehaviour
    {
        [SerializeField]
        private RawImage RGBCamRawImage;

        private Motors mMotors;
        private RGBCam mRGBCam;
        private Face mFace;
        private Speaker mSpeaker;

        private float mNoSpeed;
        private float mYesSpeed;
        private float mYesAngle;
        private float mNoAngle;

        private bool mYesUp;
        private bool mYesDown;
        private bool mNoLeft;
        private bool mNoRight;


        void Start()
        {
            mMotors = BYOS.Instance.Motors;
            mRGBCam = BYOS.Instance.RGBCam;
            mFace = BYOS.Instance.Face;
            mSpeaker = BYOS.Instance.Speaker;
        }

        void OnEnable()
        {
            mYesUp = false;
            mYesDown = false;
            mNoLeft = false;
            mNoRight = false;

            mYesAngle = 0.0F;
            mNoAngle = 0.0F;

            mNoSpeed = 100.0F;
            mYesSpeed = 100.0F;
        }

        void Update()
        {
            RGBCamRawImage.texture = mRGBCam.FrameTexture2D;
            ControlNoAxis();
            ControlYesAxis();
        }

        /// <summary>
        /// Function to control the head hinge
        /// </summary>
        private void ControlNoAxis()
        {
            if (mNoLeft)
            {
                mNoAngle = mMotors.NoHinge.CurrentAnglePosition + 20;
                mMotors.NoHinge.SetPosition(mNoAngle);
            }

            if (mNoRight)
            {
                mNoAngle = mMotors.NoHinge.CurrentAnglePosition - 20;
                mMotors.NoHinge.SetPosition(mNoAngle);
            }
        }

        /// <summary>
        /// Function to control the neck hinge 
        /// </summary>
        private void ControlYesAxis()
        {
            if (mYesDown)
            {
                mYesAngle = mMotors.YesHinge.CurrentAnglePosition + 10;
                mMotors.YesHinge.SetPosition(mYesAngle, mYesSpeed);
            }

            if (mYesUp)
            {
                mYesAngle = mMotors.YesHinge.CurrentAnglePosition - 10;
                mMotors.YesHinge.SetPosition(mYesAngle, mYesSpeed);
            }
               
        }

        public void NoLeftButtonDown()
        {
            mNoLeft = true;
            mFace.LookAt(FaceLookAt.TOP_RIGHT);
            mSpeaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
        }
        public void NoRightButtonDown()
        {
            mNoRight = true;
            mFace.LookAt(-600, 600);
            mSpeaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
        }

        public void YesUpButtonDown()
        {
            mYesUp = true;
            mFace.LookAt(600, 600);
            mSpeaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);

        }
        public void YesDownButtonDown()
        {
            mYesDown = true;
            mFace.LookAt(600, -600);
            mSpeaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
        }

        public void NoLeftButtonUp()
        {
            mNoLeft = false;
            mFace.LookAt(FaceLookAt.CENTER);
        }
        public void NoRightButtonUp()
        {
            mNoRight = false;
            mFace.LookAt(FaceLookAt.CENTER);
        }
        public void YesUpButtonUp()
        {
            mYesUp = false;
            mFace.LookAt(FaceLookAt.CENTER);
        }
        public void YesDownButtonUp()
        {
            mYesDown = false;
            mFace.LookAt(FaceLookAt.CENTER);
        }

        ///// <summary>
        ///// Function to control the head hinge
        ///// </summary>
        //private void ControlNoAxis()
        //{
        //    bool lChanged = false;
        //    if (mNoLeft)
        //    {
        //        mNoAngle = mMotors.NoHinge.CurrentAnglePosition + 20;
        //        lChanged = true;
        //    }

        //    if (mNoRight)
        //    {
        //        mNoAngle = mMotors.NoHinge.CurrentAnglePosition - 20;
        //        lChanged = true;
        //    }

        //    if (lChanged)
        //    {
        //        mMotors.NoHinge.SetPosition(mNoAngle);
        //    }
        //}


    }
}
