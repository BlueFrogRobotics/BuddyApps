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
        /*
        *  Dependances (SDK tools used by this module):
        *  Buddy.GUI.Toaster.Hide();
        *  Buddy.GUI.Toaster.IsBusy;
        *  Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTextureCam);
        *  Buddy.GUI.Toaster.Display<PictureToast>().With(img, onFinish);
        */

        public override string Name
        {
            get
            {
                return ("Camera Test");
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
        private Color mColorOfDisplay;

        private bool mTestInProcess;

        //  --- Method ---

        public override void InitTestList()
        {
            mAvailableTest = new List<string>();
            mAvailableTest.Add("CameraView");
            mAvailableTest.Add("MotionDetect");
            mAvailableTest.Add("FaceDetect");
            mAvailableTest.Add("HumanDetect");
            mAvailableTest.Add("SkeletonDetect");
            mAvailableTest.Add("TakePhoto");
            return;
        }

        public override void InitPool()
        {
            mTestPool = new Dictionary<string, TestRoutine>();
            mTestPool.Add("CameraView", CameraView);
            mTestPool.Add("MotionDetect", MotionDetectTests);
            mTestPool.Add("FaceDetect", FaceDetectTests);
            mTestPool.Add("HumanDetect", HumanDetectTests);
            mTestPool.Add("SkeletonDetect", SkeletonDetectTests);
            mTestPool.Add("TakePhoto", TakePhotoTests);
            return;
        }

        public CameraTest()
        {
            mTestInProcess = false;
        }

        // On each frame captured by the camera this function is called, with the matrix of pixel.
        #region CAMERA_CALLBACK
        private void OnFrameCaptured(RGBCameraFrame iInput)
        {
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

        // Common interface for all MotionTest
        #region COMMON_UI
        private void DisplayTestUi(string iTestTitle, string iTest, bool iCameraView = true)
        {
            if (!string.IsNullOrEmpty(iTestTitle))
                Buddy.GUI.Header.DisplayLightTitle(iTestTitle);
            if (iCameraView)
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTextureCam);

            // Fail button
            FButton lFailButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lFailButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_close"));
            lFailButton.SetBackgroundColor(Color.red);
            lFailButton.SetIconColor(Color.white);
            lFailButton.OnClick.Add(() => { mTestInProcess = false; mResultPool.Add(iTest, false); });

            // Success button
            FButton lSuccessButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lSuccessButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));
            lSuccessButton.SetBackgroundColor(Color.green);
            lSuccessButton.SetIconColor(Color.white);
            lSuccessButton.OnClick.Add(() => { mTestInProcess = false; mResultPool.Add(iTest, true); });
        }
        #endregion

        // All TestRoutine of this module:
        #region CAMERA_VIEW
        public IEnumerator CameraView()
        {
            //  --- INIT ---
            mTestInProcess = true;
            // Initialize texture.
            mTextureCam = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // Setting of the callback to use camera data
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));

            //  --- CODE ---
            DebugColor("CameraView work in progress", "blue");
            DisplayTestUi("Camera View", "CameraView");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }
        #endregion

        #region MOTION_DETECT
        public IEnumerator MotionDetectTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
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
            mColorOfDisplay = new Color(UnityEngine.Random.Range(128, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255));

            //  --- CODE ---
            DebugColor("MotionDetect work in progress", "blue");
            DisplayTestUi("Motion Detect", "MotionDetect");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Perception.MotionDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }

        // Callback when there is motions detected
        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            //Draw circle on every motions detected on the detect layer
            foreach (MotionEntity lEntity in iMotions)
            {
                Imgproc.circle(mMatDetect, Utils.Center(lEntity.RectInFrame), 3, new Scalar(mColorOfDisplay), 3);
            }

            // Flip to avoid mirror effect.
            Core.flip(mMatDetect, mMatDetect, 1);

            // Use matrice format of the detect layer, to scale the texture.
            mTextureCam = Utils.ScaleTexture2DFromMat(mMatDetect, mTextureCam);
            // Use matrice of the detect layer to fill the texture.
            Utils.MatToTexture2D(mMatDetect, mTextureCam);
            return true;
        }
        #endregion

        #region FACE_DETECT
        public IEnumerator FaceDetectTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            // Initialize texture.
            mTextureCam = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // Setting of the callback to use camera data
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            Buddy.Perception.FaceDetector.OnDetect.AddP(OnFaceDetect);

            //  --- CODE ---
            DebugColor("FaceDetectDetect work in progress", "blue");
            DisplayTestUi("Face Detect", "FaceDetect");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Perception.FaceDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }

        private bool OnFaceDetect(FaceEntity[] iFaces)
        {
            //Draw rectangle on each human detected on the detect layer
            foreach (FaceEntity lEntity in iFaces)
            {
                Imgproc.rectangle(mMatDetect, lEntity.BoundingBox.tl(), lEntity.BoundingBox.br(), new Scalar(mColorOfDisplay));
            }

            // Flip to avoid mirror effect.
            Core.flip(mMatDetect, mMatDetect, 1);

            // Use matrice format of the detect layer, to scale the texture.
            mTextureCam = Utils.ScaleTexture2DFromMat(mMatDetect, mTextureCam);
            // Use matrice of the detect layer to fill the texture.
            Utils.MatToTexture2D(mMatDetect, mTextureCam);
            return true;
        }
        #endregion

        #region HUMAN_DETECT
        public IEnumerator HumanDetectTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
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
            DisplayTestUi("Human Detect", "HumanDetect");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Perception.HumanDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }

        private bool OnHumanDetect(HumanEntity[] iHumans)
        {
            //Draw rectangle on each human detected on the detect layer
            foreach (HumanEntity lEntity in iHumans)
            {
                Imgproc.rectangle(mMatDetect, lEntity.BoundingBox.tl(), lEntity.BoundingBox.br(), new Scalar(mColorOfDisplay));
            }

            // Flip to avoid mirror effect.
            Core.flip(mMatDetect, mMatDetect, 1);
            // Use matrice format of the detect layer, to scale the texture.
            mTextureCam = Utils.ScaleTexture2DFromMat(mMatDetect, mTextureCam);
            // Use matrice of the detect layer to fill the texture.
            Utils.MatToTexture2D(mMatDetect, mTextureCam);
            return true;
        }
        #endregion

        #region SKELETON_DETECT
        public IEnumerator SkeletonDetectTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            // Initialize texture.
            mTextureCam = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // Setting of the callback to use camera data
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320X240_30FPS_RGB);
            Buddy.Perception.SkeletonDetector.OnDetect.AddP(OnSkeletonDetect);

            //  --- CODE ---
            DebugColor("SkeletonDetectDetect work in progress", "blue");
            DisplayTestUi("Skeleton Detect", "SkeletonDetect");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Perception.SkeletonDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
            Buddy.Sensors.DepthCamera.Close();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }

        private bool OnSkeletonDetect(SkeletonEntity[] iSkeleton)
        {
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
            return true;
        }
        #endregion

        // Doesn't work yet - TODO: Debug to know if the bug is here or in the SDK
        #region TAKE_PHOTO
        public IEnumerator TakePhotoTests()
        {
            //  --- INIT RGBCam & TakePhoto with it ---
            mTestInProcess = true;
            Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_640X480_30FPS_RGB);
            Buddy.Sensors.RGBCamera.TakePhotograph(OnFinish);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            // --- TRANSITION ---
            Buddy.GUI.Toaster.Hide();
            Buddy.Sensors.RGBCamera.Close();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);

            // --- INIT HDCam & TakePhoto with it ---
            mTestInProcess = true;
            Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);
            Buddy.Sensors.HDCamera.TakePhotograph(OnFinish);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Sensors.HDCamera.Close();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }

        private void OnFinish(Photograph iMyPhoto)
        {
            if (iMyPhoto == null)
            {
                mTestInProcess = false;
                return;
            }
            Buddy.GUI.Toaster.Display<PictureToast>().With(iMyPhoto.Image);
            //Show Ui Button but disable VideoToaster
            DisplayTestUi("Take Photo", "TakePhoto", false);
        }
        #endregion
    }
}
