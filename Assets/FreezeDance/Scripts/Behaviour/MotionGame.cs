using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
using BuddyFeature.Vision;
using BuddyOS;
using BuddyTools;

namespace BuddyApp.FreezeDance
{
    public class MotionGame : AVisionAlgorithm
    {
        private bool mIsRLGL;
        public bool IsRLGL { get { return mIsRLGL; } set { mIsRLGL = value; } }

        [SerializeField]
        private bool isMoving;

        #region Mats needed to process the image
        //detect motion

        private RGBCam mRGBCam;

        private Point mTrackingPoint;

        private Mat mCurrentFrame;
        private Mat mPreviousFrame;
        private Mat mDiffResult;

        private Mat mBlurredImage;
        private Mat mBinaryImage;

        private Mat mKernel;

        private Mat mRawImage;
        private Mat mTest;
        private Point mPositionOLD;

        ////debug
        //[SerializeField]
        //private RawImage mDebugRawImg;

        #endregion
        private double mThresh;

        //Kalman

        // Use this for initialization

        //private Mat mDebugMatRLGL;
        [SerializeField]
        private RawImage mDebugRawImg;

        protected override void Init()
        {
            isMoving = false;
            mCurrentFrame = new Mat();
            mPreviousFrame = new Mat();
            mDiffResult = new Mat();
            mRawImage = new Mat();
            mTest = new Mat();
            
            mBlurredImage = new Mat();
            mBinaryImage = new Mat();
            mPositionOLD = new Point(1000, 1000);
            mIsRLGL = false;
            //sobelKernelSize = 3;

            ////Debug for RLGL
            //mDebugMatRLGL = new Mat();

            mRGBCam = BYOS.Instance.RGBCam;
            mRGBCam.Open();
        }

        // Update is called once per frame
        protected override void ProcessFrameImpl(Mat iInputFrameMat, Texture2D iInputFrameTexture)
        {

            //mThresh = threshBar.value;
            //need to change mThresh to adjust difficulty
            if (mIsRLGL)
            {
                switch (RLGL.RLGLData.Instance.Difficulty)
                {
                    case RLGL.RLGLData.Level.LEVEL_EASY:
                        mThresh = 125;
                        break;
                    case RLGL.RLGLData.Level.LEVEL_MEDIUM:
                        mThresh = 100;
                        break;
                    case RLGL.RLGLData.Level.LEVEL_HARD:
                        mThresh = 75;
                        break;
                    case RLGL.RLGLData.Level.LEVEL_IMPOSSIBLE:
                        mThresh = 50;
                        break;
                    default:
                        mThresh = 100;
                        break;
                }
            }
            else
                mThresh = 100;


            mRawImage = iInputFrameMat.clone();
            mTest = iInputFrameMat.clone();

            Imgproc.cvtColor(mRawImage, mCurrentFrame, Imgproc.COLOR_BGR2GRAY);
            if (mPreviousFrame.width() != 0) {
                Core.absdiff(mCurrentFrame, mPreviousFrame, mDiffResult);
                Imgproc.blur(mDiffResult, mBlurredImage, new Size(3, 3));
                Imgproc.threshold(mBlurredImage, mBinaryImage, mThresh, 255, Imgproc.THRESH_BINARY);
                //mDebugRawImg.texture = Utils.MatToTexture2D(mBinaryImage);

                Point lCenterOfMass = new Point();
                Moments lMoments = Imgproc.moments(mBinaryImage);
                lCenterOfMass.x = (int)(lMoments.get_m10() / lMoments.get_m00());
                lCenterOfMass.y = (int)(lMoments.get_m01() / lMoments.get_m00());
                Imgproc.circle(mTest, lCenterOfMass, 10, new Scalar(254, 254, 254), -1);
                //Imgproc.rectangle(mTest, );
                if (mPositionOLD.x != 1000) {
                    float lDiffX = Mathf.Abs((float)(lCenterOfMass.x - mPositionOLD.x));
                    float lDiffY = Mathf.Abs((float)(lCenterOfMass.y - mPositionOLD.y));
                    if (lDiffX == 0 && lDiffY == 0)
                    {
                        mDebugRawImg.enabled = false;
                        isMoving = false;
                    }
                    else
                    {
                        mDebugRawImg.enabled = true;
                        mDebugRawImg.texture = Utils.MatToTexture2D(mTest);
                        isMoving = true;
                    }
                        
                }
                mPositionOLD = lCenterOfMass;
            }

            mPreviousFrame = mCurrentFrame.clone();
            mPreviousFrame.convertTo(mPreviousFrame, CvType.CV_8UC1);
            mOutputFrameMat = mTest;
        }

        public bool IsMoving()
        {
            return isMoving;
        }
    }
}