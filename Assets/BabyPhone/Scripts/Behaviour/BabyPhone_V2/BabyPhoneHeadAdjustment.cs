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

        private float mNormalSpeed = 100F;
        private float mRotationSpeed = 100F;

        private float mNoSpeed = 100.0F;
        private float mYesSpeed = 100.0F;
        private float mYesAngle = 0.0F;
        private float mNoAngle = 0.0F;

        private bool mYesUp = false;
        private bool mYesDown = false;
        private bool mNoLeft = false;
        private bool mNoRight = false;


        void Start()
        {
            mMotors = BYOS.Instance.Motors;
            mRGBCam = BYOS.Instance.RGBCam;
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
            bool lChanged = false;
            if (mNoLeft)
            {
                mNoAngle = mMotors.NoHinge.CurrentAnglePosition + 5;
                lChanged = true;
                mNoLeft = false;
            }

            if (mNoRight)
            {
                mNoAngle = mMotors.NoHinge.CurrentAnglePosition - 5;
                lChanged = true;
                mNoRight = false;
            }

            if (lChanged)
            {
                mMotors.NoHinge.SetPosition(mNoAngle, mNoSpeed);
            }
        }

        /// <summary>
        /// Function to control the neck hinge 
        /// </summary>
        private void ControlYesAxis()
        {
            bool lChanged = false;
            if (mYesDown)
            {
                mYesAngle = mMotors.YesHinge.CurrentAnglePosition + 5;
                lChanged = true;
                mYesDown = false;
            }

            if (mYesUp)
            {
                mYesAngle = mMotors.YesHinge.CurrentAnglePosition - 5;
                lChanged = true;
                mYesUp = false;
            }

            if (lChanged)
                mMotors.YesHinge.SetPosition(mYesAngle, mYesSpeed);
        }

        public void MoveNoLeft()
        {
            mNoLeft = true;
        }
        public void MoveNoRight()
        {
            mNoRight = true;
        }
        public void MoveYesUp()
        {
            mYesUp = true;
        }
        public void MoveYesDown()
        {
            mYesDown = true;
        }

    }
}
