using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class HeadMover : MonoBehaviour
    {

        private Motors mMotors;
        private RGBCam mRGBCam;

        private float mNormalSpeed = 0.6F;
        private float mRotationSpeed = 0.2F;
        private float mNoSpeed = 20.0F;


        private float mYesSpeed = 20.0F;
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
                mNoAngle = mMotors.NoHinge.CurrentAnglePosition + 1;
                lChanged = true;
                mNoLeft = false;
            }

            if (mNoRight)
            {
                mNoAngle = mMotors.NoHinge.CurrentAnglePosition - 1;
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
                mYesAngle = mMotors.YesHinge.CurrentAnglePosition + 2;
                lChanged = true;
                mYesDown = false;
            }

            if (mYesUp)
            {
                mYesAngle = mMotors.YesHinge.CurrentAnglePosition - 2;
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