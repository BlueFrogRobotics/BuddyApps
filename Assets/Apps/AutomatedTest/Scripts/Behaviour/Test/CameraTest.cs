using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using OpenCVUnity;

namespace BuddyApp.AutomatedTest
{
    public sealed class CameraTest : AModuleTest
    {
        public override string Name
        {
            get
            {
                return (Buddy.Resources.GetString("camera"));
            }
        }

        // Coefficient use to adjust all skeleton point, on the image.
        private const float COEFF_X = 1.7F;
        private const float COEFF_Y = 2.45F;

        // Detection layer
        private Mat mMatDetect;

        // This texture will be filled with the camera data
        private Texture2D mTextureCam;

        // Color to use for detection layer
        private Scalar mColorOfDisplay;

        private bool mTestAutoEvaluated;

        //  --- Method ---

        public override void InitTestList()
        {
            mAvailableTest = new List<string>();
            mAvailableTest.Add("motiondetect");
            //mAvailableTest.Add("facedetect");
            mAvailableTest.Add("humandetect");
            mAvailableTest.Add("skeletondetect");
            mAvailableTest.Add("takephotorgb");
            mAvailableTest.Add("takephotohd");
            return;
        }

        public override void InitPool()
        {
            mTestPool = new Dictionary<string, TestRoutine>();
            mTestPool.Add("motiondetect", MotionDetectTests);
            //mTestPool.Add("facedetect", FaceDetectTests);
            mTestPool.Add("humandetect", HumanDetectTests);
            mTestPool.Add("skeletondetect", SkeletonDetectTests);
            mTestPool.Add("takephotorgb", TakePhotoRgbTests);
            mTestPool.Add("takephotohd", TakePhotoHdTests);
            return;
        }

        private void Awake()
        {
            mColorOfDisplay = new Scalar(UnityEngine.Random.Range(128, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255));
        }

        // On each frame captured by the camera this function is called, with the matrix of pixel.
        #region CAMERA_CALLBACK
        private void OnFrameCaptured(RGBCameraFrame iInput)
        {
            if (iInput == null)
                return;
            // Always clone the input matrix, this avoid to working with the original matrix, when the C++ part wants to modify it.
            Mat lMatSrc = iInput.Mat.clone();

            // Fill the detection layer texture with the image
            mMatDetect = iInput.Mat.clone();

            // Flip to avoid mirror effect.
            // More information from opencv doc for the flipcode : 
            // A value of 0 means flipping around the x-axis.
            // A value > 0 means flipping around y-axis. 
            // A value < 0 means flipping around both axes.
            Core.flip(lMatSrc, lMatSrc, 1);
            // Use matrice format, to scale the texture.
            mTextureCam = Utils.ScaleTexture2DFromMat(lMatSrc, mTextureCam);
            // Use matrice to fill the texture.
            Utils.MatToTexture2D(lMatSrc, mTextureCam);
        }
        #endregion

        // All TestRoutine of this module:

        #region MOTION_DETECT
        public IEnumerator MotionDetectTests()
        {
            //  --- INIT ---
            mTestAutoEvaluated = false;
            // Initialize texture.
            mTextureCam = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // Setting of the callback to use camera data
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            // RegionOfInterest represents where the detection will be done in the current frame.
            MotionDetectorParameter mMotionDetectorParameter = new MotionDetectorParameter();
            mMotionDetectorParameter.RegionOfInterest = new OpenCVUnity.Rect(0, 0, Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // SensibilityThreshold represents the threshold for the detection
            mMotionDetectorParameter.SensibilityThreshold = 2.5F;
            // OnDetect opens the camera itself so we don't have to do it. Default resolution is 640*480
            Buddy.Perception.MotionDetector.OnDetect.AddP(OnMovementDetected, mMotionDetectorParameter);

            //  --- CODE ---
            DebugColor("MotionDetect work in progress", "blue");
            DisplayTestUi("motiondetect", mTextureCam);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            // --- Wait Before Quit in AUTO ---
            if (Mode == TestMode.M_AUTO)
                yield return new WaitForSeconds(3F);

            //  --- EXIT ---
            Buddy.Perception.MotionDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
        }

        // Callback when there is motions detected
        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            if (mMatDetect == null)
                return true;
            //Draw circle on every motions detected on the detect layer
            foreach (MotionEntity lEntity in iMotions)
            {
                Imgproc.circle(mMatDetect, Utils.Center(lEntity.RectInFrame), 3, mColorOfDisplay, 3);
            }

            // Flip to avoid mirror effect.
            Core.flip(mMatDetect, mMatDetect, 1);

            // Use matrice format of the detect layer, to scale the texture.
            mTextureCam = Utils.ScaleTexture2DFromMat(mMatDetect, mTextureCam);
            // Use matrice of the detect layer to fill the texture.
            Utils.MatToTexture2D(mMatDetect, mTextureCam);

            // Valid the test in auto mode, at the first detection
            if (Mode == TestMode.M_AUTO && mTestAutoEvaluated == false)
            {
                mTestAutoEvaluated = true;
                mResultPool.Add("motiondetect", true);
                mTestInProcess = false;
            }

            return true;
        }
        #endregion

        #region FACE_DETECT
        public IEnumerator FaceDetectTests()
        {
            //  --- INIT ---
            mTestAutoEvaluated = false;
            // Initialize texture.
            mTextureCam = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // Setting of the callback to use camera data
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            Buddy.Perception.FaceDetector.OnDetect.AddP(OnFaceDetect);

            //  --- CODE ---
            DebugColor("FaceDetectDetect work in progress", "blue");
            DisplayTestUi("facedetect", mTextureCam);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            // --- Wait Before Quit in AUTO ---
            if (Mode == TestMode.M_AUTO)
                yield return new WaitForSeconds(3F);

            //  --- EXIT ---
            Buddy.Perception.FaceDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
        }

        private bool OnFaceDetect(FaceEntity[] iFaces)
        {
            if (mMatDetect == null)
                return true;
            //Draw rectangle on each human detected on the detect layer
            foreach (FaceEntity lEntity in iFaces)
            {
                Imgproc.rectangle(mMatDetect, lEntity.BoundingBox.tl(), lEntity.BoundingBox.br(), mColorOfDisplay);
            }

            // Flip to avoid mirror effect.
            Core.flip(mMatDetect, mMatDetect, 1);

            // Use matrice format of the detect layer, to scale the texture.
            mTextureCam = Utils.ScaleTexture2DFromMat(mMatDetect, mTextureCam);
            // Use matrice of the detect layer to fill the texture.
            Utils.MatToTexture2D(mMatDetect, mTextureCam);

            // Valid the test in auto mode, at the first detection
            if (Mode == TestMode.M_AUTO && mTestAutoEvaluated == false)
            {
                mTestAutoEvaluated = true;
                mResultPool.Add("facedetect", true);
                mTestInProcess = false;
            }

            return true;
        }
        #endregion

        #region HUMAN_DETECT
        public IEnumerator HumanDetectTests()
        {
            //  --- INIT ---
            mTestAutoEvaluated = false;
            // Initialize texture.
            mTextureCam = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // Setting of the callback to use camera data
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            HumanDetectorParameter lParameters = new HumanDetectorParameter();
            lParameters.SensorMode = SensorMode.VISION;
            lParameters.YOLO = new YOLOParameter();
            // Region of Interest for the tracking, 0 rectangle will use all the field of view.
            lParameters.YOLO.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 0, 0); ;
            // Also use thermal camera or not.
            lParameters.YOLO.UseThermal = false;
            Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect, lParameters);

            //  --- CODE ---
            DebugColor("HumanDetectDetect work in progress", "blue");
            DisplayTestUi("humandetect", mTextureCam);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            // --- Wait Before Quit in AUTO ---
            if (Mode == TestMode.M_AUTO)
                yield return new WaitForSeconds(3F);

            //  --- EXIT ---
            Buddy.Perception.HumanDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
        }

        private bool OnHumanDetect(HumanEntity[] iHumans)
        {
            if (mMatDetect == null)
                return true;
            //Draw rectangle on each human detected on the detect layer
            foreach (HumanEntity lEntity in iHumans)
            {
                Imgproc.rectangle(mMatDetect, lEntity.BoundingBox.tl(), lEntity.BoundingBox.br(), mColorOfDisplay);
            }

            // Flip to avoid mirror effect.
            Core.flip(mMatDetect, mMatDetect, 1);
            // Use matrice format of the detect layer, to scale the texture.
            mTextureCam = Utils.ScaleTexture2DFromMat(mMatDetect, mTextureCam);
            // Use matrice of the detect layer to fill the texture.
            Utils.MatToTexture2D(mMatDetect, mTextureCam);

            // Valid the test in auto mode, at the first detection
            if (Mode == TestMode.M_AUTO && mTestAutoEvaluated == false)
            {
                mTestAutoEvaluated = true;
                mResultPool.Add("humandetect", true);
                mTestInProcess = false;
            }

            return true;
        }
        #endregion

        #region SKELETON_DETECT
        public IEnumerator SkeletonDetectTests()
        {
            //  --- INIT ---
            // Initialize texture.
            mTestAutoEvaluated = false;
            mTextureCam = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // Setting of the callback to use camera data
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            if (Buddy.Perception.SkeletonDetector.OnDetect.Count == 0)
            {
                Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320X240_30FPS_RGB);
                Buddy.Perception.SkeletonDetector.OnDetect.AddP(OnSkeletonDetect);
            }

            //  --- CODE ---
            DebugColor("SkeletonDetectDetect work in progress", "blue");
            DisplayTestUi("skeletondetect", mTextureCam);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            // --- Wait Before Quit in AUTO ---
            if (Mode == TestMode.M_AUTO)
                yield return new WaitForSeconds(3F);

            //  --- EXIT ---
            Buddy.Perception.SkeletonDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
            Buddy.Sensors.DepthCamera.OnNewFrame.Clear();
            Buddy.Sensors.DepthCamera.Close();
        }

        private bool OnSkeletonDetect(SkeletonEntity[] iSkeleton)
        {
            if (mMatDetect == null)
                return true;

            int lWidth = mMatDetect.cols();
            int lHeight = mMatDetect.rows();
            // Calcul the center of the img
            Point lCenter = new Point(lWidth / 2, lHeight / 2);

            foreach (SkeletonEntity lSkeleton in iSkeleton)
            {
                foreach (SkeletonJoint lJoint in lSkeleton.Joints)
                {
                    // Calcul the local position of the joint
                    Point lLocal = new Point(lJoint.WorldPosition.x / lJoint.WorldPosition.z, lJoint.WorldPosition.y / lJoint.WorldPosition.z);
                    // Conversion of the local position, in the img
                    lLocal.x *= COEFF_X * lWidth / 2;
                    lLocal.y *= COEFF_Y * lHeight / 2;

                    // Draw a circle with the joint point as center
                    // 10 is a constant, choose after some test, to get a base for the size.
                    // Divide the z coordinate by 1000 to get value in millimeter
                    // The result of the pow operation is used to divide a constant, so adding 0.1 avoid a zero division.
                    // The pow operation purpose, is to increase the influence of the depth
                    Imgproc.circle(mMatDetect, lCenter - lLocal, (int)(10 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)), new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                }
            }

            // Flip to avoid mirror effect.
            Core.flip(mMatDetect, mMatDetect, 1);
            // Use matrice format of the detect layer, to scale the texture.
            mTextureCam = Utils.ScaleTexture2DFromMat(mMatDetect, mTextureCam);
            // Use matrice of the detect layer to fill the texture.
            Utils.MatToTexture2D(mMatDetect, mTextureCam);

            // Valid the test in auto mode, at the first detection
            if (Mode == TestMode.M_AUTO && mTestAutoEvaluated == false)
            {
                mTestAutoEvaluated = true;
                mResultPool.Add("skeletondetect", true);
                mTestInProcess = false;
            }
            return true;
        }
        #endregion

        // Doesn't work yet because TakePhotograph is broken
        #region TAKE_PHOTO
        public IEnumerator TakePhotoRgbTests()
        {
            //  --- INIT RGBCam & TakePhoto with it ---
            DebugColor("---- TAKEPHOTOGRAPH RGBCAM ----", "red");
            Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_640X480_30FPS_RGB);

            yield return new WaitForSeconds(0.5F);

            Buddy.Sensors.RGBCamera.TakePhotograph(OnFinish);
            DisplayTestUi("takephotorgb");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            DebugColor("---- TAKEPHOTOGRAPH RGBCAM  END----", "red");
            Buddy.Sensors.RGBCamera.Close();
        }

        public IEnumerator TakePhotoHdTests()
        {
            // --- INIT HDCam & TakePhoto with it ---
            DebugColor("---- TAKEPHOTOGRAPH HDCAM ----", "red");
            Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640X480_30FPS_RGB);
            Buddy.Sensors.HDCamera.TakePhotograph(OnFinish);
            DisplayTestUi("takephotohd");

            yield return new WaitForSeconds(0.5F);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            DebugColor("---- TAKEPHOTOGRAPH HDCAM  END----", "red");
            Buddy.Sensors.HDCamera.Close();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
        }

        private void OnFinish(Photograph iMyPhoto)
        {
            if (iMyPhoto == null)
            {
                DebugColor("OnFinish take photo, iPhoto null", "red");
                return;
            }
            // test with that
            Sprite mPhotoSprite = Sprite.Create(iMyPhoto.Image.texture, new UnityEngine.Rect(0, 0, iMyPhoto.Image.texture.width, iMyPhoto.Image.texture.height), new Vector2(0.5F, 0.5F));
            Buddy.GUI.Toaster.Display<PictureToast>().With(iMyPhoto.Image);
        }
        #endregion
    }
}
