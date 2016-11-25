using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
using BuddyFeature.Vision;
using BuddyOS;
using BuddyTools;


public class MotionGame : AVisionAlgorithm
{

    //private List<Point> mPlayerPositions;

    #region Var to adjust the moving mask
    [Range(1, 100000)]
    public int mSobelKernelSize;
    #endregion
    [SerializeField]
    private RawImage mDebugScreen;
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
    private Mat mSobelResult;

    private Mat mKernelRect;
    private Mat mKernelCross;
    private Mat mKernelEllipse;
    private Mat mKernelCustom;
    private Mat mKernel;

    private Mat mRawImage;
    private Mat mTest;
    private Point mPositionOLD;

    [SerializeField]
    private bool mIsMoving;
    #endregion
    private float mTime;

    //Kalman

    // Use this for initialization

    private RGBCam mRGBCam;

    void OnEnable()
    {
        mRGBCam = BYOS.Instance.RGBCam;
        if (!mRGBCam.IsOpen)
            mRGBCam.Open();
    }

    protected override void Init()
    {

        //Game
        mIsMoving = false;
        //Motion Detector
        //mCurrentFrame = new Mat();
        //mPreviousFrame = new Mat();
        mDiffResult = new Mat();
        mRawImage = new Mat();
        //mTest = new Mat();

        mBlurredImage = new Mat();
        mBinaryImage = new Mat();
        mSobelResult = new Mat();
        mPositionOLD = new Point(1000, 1000);
        mSobelKernelSize = 3;

        mTime = Time.time;

        #region Kernel Shape: Testing different kernel Shape
        mKernelRect = Imgproc.getStructuringElement(Imgproc.CV_SHAPE_RECT, new Size(11, 11));
        mKernelCross = Imgproc.getStructuringElement(Imgproc.CV_SHAPE_CROSS, new Size(11, 11));
        mKernelEllipse = Imgproc.getStructuringElement(Imgproc.CV_SHAPE_ELLIPSE, new Size(11, 11));
        mKernelCustom = Imgproc.getStructuringElement(Imgproc.CV_SHAPE_CUSTOM, new Size(11, 11));
        #endregion
    }

    // Update is called once per frame
    protected override void ProcessFrameImpl(Mat iInputFrameMat, Texture2D iInputFrameTexture)
    {
        if (mPreviousFrame != null)
        {
            Imgproc.cvtColor(iInputFrameMat, iInputFrameMat, Imgproc.COLOR_BGR2GRAY);
            Core.absdiff(iInputFrameMat, mPreviousFrame, mDiffResult);
            //Imgproc.blur(mDiffResult, mBlurredImage, new Size(3, 3));
            Imgproc.threshold(mDiffResult, mBinaryImage, 35, 255, Imgproc.THRESH_BINARY);
            mDebugScreen.texture = Utils.MatToTexture2D(mDiffResult);
            Point lCenterOfMass = new Point();
            Moments M = Imgproc.moments(mBinaryImage);
            lCenterOfMass.x = (int)(M.get_m10() / M.get_m00());
            lCenterOfMass.y = (int)(M.get_m01() / M.get_m00());
            if (mPositionOLD.x != 1000)
            {
                float lDiffX = Mathf.Abs((float)(lCenterOfMass.x - mPositionOLD.x));
                float lDiffY = Mathf.Abs((float)(lCenterOfMass.y - mPositionOLD.y));
                if (lDiffX == 0 && lDiffY == 0)
                    mIsMoving = false;
                else
                    mIsMoving = true;
            }
            mPositionOLD = lCenterOfMass;
        }
        mPreviousFrame = iInputFrameMat.clone();

    }
    public bool IsMoving()
    {
        return mIsMoving;
    }

    void OnDisable()
    {
        mRGBCam.Close();
    }
}

