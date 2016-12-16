﻿using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
//using BuddyAPI;
using BuddyOS;
using System.Collections.Generic;
using BuddyFeature.Vision;
using System;
using System.IO;

namespace BuddyApp.HideAndSeek
{
    public class MovementDetector : AVisionAlgorithm
    {

        public enum Direction : int
        {
            NONE,
            LEFT,
            RIGHT
        }

        #region Var to adjust the moving mask




        [Range(1, 100000)]
        public int mSobelKernelSize;
        #endregion
        //[SerializeField]
        //private RawImage mDebugScreen;
        [SerializeField]
        private Slider mThreshBar;

        private double mThresh;

        public Point mTrackingPoint;

        #region Mats needed to process the image
        //detect motion
        private Mat mCurrentFrame;
        private Mat mPreviousFrame;
        private Mat mDiffResult;

        private Mat mBlurredImage;
        private Mat mBinaryImage;

        public Mat BinaryImage { get { return mBinaryImage; } }

        private Mat mKernel;

        private Mat mRawImage;
        private Mat mTest;
        private Point mPositionOLD;

        public Point PositionMoment { get { return mPositionOLD; } }

        private bool mIsMoving;

        public bool IsMovementDetected { get { return mIsMoving; } }



        #endregion

        private RGBCam mCam;

        private Direction mDirectionMov;
        public Direction DirectionMov { get { return mDirectionMov; } }


        public event Action OnDetection;

        private float mMinThreshold = 0.0f;
        private float mMaxThreshold = 200f;
        private float mThreshold = 35f;

        //Kalman

        //void Awake()
        //{
        //    mWebcam = BuddyOS.BuddyOperatingSystem.Instance.BuddyAPI.RGBCam;
        //}

        // Use this for initialization
        protected override void Init()
        {


            mPositionOLD = new Point(0, 0);
            //Game
            mIsMoving = false;
            //Motion Detector
            mCurrentFrame = new Mat();
            mPreviousFrame = new Mat();
            mDiffResult = new Mat();
            mRawImage = new Mat();
            mTest = new Mat();

            mBlurredImage = new Mat();
            mBinaryImage = new Mat();

            mSobelKernelSize = 3;


            mCam = BYOS.Instance.RGBCam;
            mPreviousFrame = mCurrentFrame.clone();
            if (mCam != null)
            {
                mCam.Open();
                //mPreviousFrame = mCam.FrameMat.clone();
            }
        }
        // Update is called once per frame
        protected override void ProcessFrameImpl(Mat iInputFrameMat, Texture2D iInputFrameTexture)
        {
            //mThresh = mThreshBar.value;
            double lThresh = mThreshold;
            mRawImage = iInputFrameMat.clone();
            mTest = iInputFrameMat.clone();

            Imgproc.cvtColor(mRawImage, mCurrentFrame, Imgproc.COLOR_BGR2GRAY);
            if (mPreviousFrame.width() != 0)
            {
                Core.absdiff(mCurrentFrame, mPreviousFrame, mDiffResult);
                Imgproc.blur(mDiffResult, mBlurredImage, new Size(3, 3));
                Imgproc.threshold(mBlurredImage, mBinaryImage, lThresh, 255, Imgproc.THRESH_BINARY);
                Point lCenterOfMass = new Point();
                Moments M = Imgproc.moments(mBinaryImage);
                if (M.get_m00() != 0)
                {
                    lCenterOfMass.x = (int)(M.get_m10() / M.get_m00());
                    lCenterOfMass.y = (int)(M.get_m01() / M.get_m00());
                }
                else
                {
                    lCenterOfMass = mPositionOLD;
                }
                Imgproc.circle(mTest, lCenterOfMass, 1, new Scalar(254, 254, 254), -1);
                float lDiffX = Mathf.Abs((float)(lCenterOfMass.x - mPositionOLD.x));
                float lDiffY = Mathf.Abs((float)(lCenterOfMass.y - mPositionOLD.y));
                if (lDiffX == 0 && lDiffY == 0)
                {
                    mIsMoving = false;
                    mDirectionMov = Direction.NONE;
                }
                else
                {
                    mIsMoving = true;
                    //Debug.Log("mouvement magique");
                    if (OnDetection != null)
                        OnDetection();
                    //Debug.Log("pos center: " + lCenterOfMass.x);
                    if (lCenterOfMass.x < mCurrentFrame.width()/2)
                        mDirectionMov = Direction.LEFT;
                    else
                        mDirectionMov = Direction.RIGHT;
                    /*Debug.Log("moment: " + M.get_m00());
                    Debug.Log("centre masse x: " + lCenterOfMass.x + " y: " + lCenterOfMass.y);
                    Debug.Log("position old x: " + mPositionOLD.x + " y: " + mPositionOLD.y);
                    Debug.Log("difference masse x: " + lDiffX + " y: " + lDiffY);*/
                }
                //}
                mPositionOLD = lCenterOfMass;

            }

            mPreviousFrame = mCurrentFrame.clone();
            mPreviousFrame.convertTo(mPreviousFrame, CvType.CV_8UC1);
            mOutputFrameMat = mTest;
            //mDebugScreen.texture = mOutputFrameTexture;
        }

        public float GetMinThreshold()
        {
            return mMinThreshold;
        }

        public float GetMaxThreshold()
        {
            return mMaxThreshold;
        }

        public float GetThreshold()
        {
            return mThreshold;
        }

        public void SetThreshold(float iThreshold)
        {
            if (iThreshold < mMinThreshold)
                mThreshold = mMinThreshold;
            else if (iThreshold > mMaxThreshold)
                mThreshold = mMaxThreshold;
            else
                mThreshold = iThreshold;

            //Debug.Log("threshold sound: " + mThreshold);
        }
    }
}
