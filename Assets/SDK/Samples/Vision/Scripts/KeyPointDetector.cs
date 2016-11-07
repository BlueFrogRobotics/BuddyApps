using OpenCVUnity;
using BuddyTools;
using UnityEngine;
using BuddyFeature.Vision;

namespace BuddySample
{
    /// <summary>
    /// Find and display key points from the input image frame.
    /// </summary>
    public class KeyPointDetector : AVisionAlgorithm
    {
        /// <summary>
        /// Detector of key points (not descriptor).
        /// </summary>
        private FeatureDetector mDetector;

        /// <summary>
        /// Mat of key points.
        /// </summary>
        private MatOfKeyPoint mKeypoints;

        /// <summary>
        /// Called one time at the start.
        /// </summary>
        protected override void Init()
        {
            mDetector = FeatureDetector.create(FeatureDetector.ORB);
            mKeypoints = new MatOfKeyPoint();
        }

        /// <summary>
        /// Called at each valuable frame.
        /// </summary>
        /// <remarks>
        /// Detects key points of the current image frame.
        /// </remarks>
        protected override void ProcessFrameImpl(Mat iInputFrameMat, Texture2D iInputFrameTexture)
        {
            mDetector.detect(iInputFrameMat, mKeypoints);
            Features2d.drawKeypoints(iInputFrameMat, mKeypoints, mOutputFrameMat, new Scalar(255, 0, 0), Features2d.DEFAULT);
        }
    }
}