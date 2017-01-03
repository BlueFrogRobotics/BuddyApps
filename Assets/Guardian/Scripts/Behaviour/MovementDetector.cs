using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
//using BuddyAPI;
using BuddyOS;
using System.Collections.Generic;
using BuddyFeature.Vision;
using System;
using System.IO;

namespace BuddyApp.Guardian
{
    public class MovementDetector : AVisionAlgorithm, IDetector
    {

        //private List<Point> mPlayerPositions;

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


        public event Action OnDetection;

        private Queue<Mat> mBufferVideo;
        private float mMaxBufferSize;

        private float mMinThreshold = 10.0f;
        private float mMaxThreshold = 190f;
        private float mThreshold = 35f;

        private VideoWriter mVideoWriter;

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
            mBufferVideo = new Queue<Mat>();
            mMaxBufferSize = 30.0f * 4.0f;
        }
        // Update is called once per frame
        protected override void ProcessFrameImpl(Mat iInputFrameMat, Texture2D iInputFrameTexture)
        {
            //mThresh = mThreshBar.value;
            double lThresh = mThreshold;
            mRawImage = iInputFrameMat.clone();
            mTest = iInputFrameMat.clone();

            mBufferVideo.Enqueue(mTest);
            if (mBufferVideo.Count > mMaxBufferSize)
                mBufferVideo.Dequeue();

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
                    mIsMoving = false;
                else
                {
                    mIsMoving = true;
                    //Debug.Log("mouvement magique");
                    if (OnDetection != null)
                        OnDetection();
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

            Debug.Log("threshold mouv: " + mThreshold);
        }

        public void Save(string iFilename)
        {
            Mat[] lListMat = mBufferVideo.ToArray();
            if (lListMat.Length > 0)
            {
                int codec = VideoWriter.fourcc('M', 'J', 'P', 'G');
                //int codec = VideoWriter.fourcc('H', '2', '6', '4');
                Debug.Log("codec: " + codec);
                Debug.Log("3");
                double fps = 30.0;                          // framerate of the created video stream
                //string filename = "monitoring.avi";

                string filepath = Application.persistentDataPath + "/" + iFilename;//Utils.GetStreamingAssetFilePath(filename);
                
                //File.Create(filepath);
                if (mVideoWriter == null)
                    mVideoWriter = new VideoWriter(filepath, codec, fps, lListMat[0].size());

                if (!mVideoWriter.isOpened())
                {
                    mVideoWriter.open(filepath, codec, fps, lListMat[0].size());
                    Debug.Log("a du open avec size: "+ lListMat[0].size());
                }
                if (mVideoWriter.isOpened())
                {
                    Debug.Log("truc");
                    Mat lFrame = new Mat();
                    for (int i = 0; i < lListMat.Length; i++)
                    {
                        Imgproc.cvtColor(lListMat[i], lFrame, Imgproc.COLOR_RGB2BGR);
                        mVideoWriter.write(lFrame);
                    }
                }
                mVideoWriter.Dispose();
                mVideoWriter = null;
            }
        }
    }
}
