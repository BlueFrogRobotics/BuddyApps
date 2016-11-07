using OpenCVUnity;
using UnityEngine;
using BuddyFeature.Vision;

namespace BuddySample
{
    /// <summary>
    /// Applies Canny filter on the input frame.
    /// </summary>
    public class CannyFilter : AVisionAlgorithm
    {
        /// <summary>
        /// Called at each valuable frame.
        /// </summary>
        /// <remarks>        
        /// Applies a Canny filter on the frame coming from the
        /// (simulate or real) RGB camera of the robot.
        /// </remarks>
        protected override void ProcessFrameImpl(Mat iInputFrameMat, Texture2D iInputFrameTexture)
        {
            Imgproc.blur(iInputFrameMat, iInputFrameMat, new Size(5, 5));
            Imgproc.Canny(iInputFrameMat, mOutputFrameMat, 10, 100);
        }
    }
}