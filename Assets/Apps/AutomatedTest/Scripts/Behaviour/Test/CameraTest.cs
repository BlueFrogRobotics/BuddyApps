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

        // Detection layer
        private Mat mMatDetect;

        // This texture will be filled with the camera data
        private Texture2D mTextureCam;

        // Color to use for detection layer
        private Color mColorOfDisplay;

        private bool mTestInProcess;

        public override void InitTestList()
        {
            mAvailableTest = new List<string>();
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

        // All TestRoutine of this module:

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
            mColorOfDisplay = new Color(Random.Range(128, 255), Random.Range(0, 255), Random.Range(0, 255));

            //  --- CODE ---
            Debug.LogWarning("MotionDetect work in progress");
            Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTextureCam, () => { mTestInProcess = false; });

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Toaster.Hide();
            Buddy.Perception.MotionDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
            yield return new WaitUntil(() => Buddy.GUI.Toaster.IsBusy);
        }

        // Callback when there is motions detected
        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            //Draw circle on every motions detected on the detect layer
            foreach (MotionEntity lEntity in iMotions) {
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
            Debug.LogWarning("FaceDetectDetect work in progress");
            Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTextureCam, () => { mTestInProcess = false; });

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Toaster.Hide();
            Buddy.Perception.FaceDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
            yield return new WaitUntil(() => Buddy.GUI.Toaster.IsBusy);
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
            Debug.LogWarning("HumanDetectDetect work in progress");
            Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTextureCam, () => { mTestInProcess = false; });

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Toaster.Hide();
            Buddy.Perception.HumanDetector.OnDetect.Clear();
            Buddy.Sensors.RGBCamera.OnNewFrame.Clear();
            Buddy.Sensors.RGBCamera.Close();
            yield return new WaitUntil(() => Buddy.GUI.Toaster.IsBusy);
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
            Debug.LogWarning("SkeletonDetect not implemented yet");
            //while (false)
            //    yield return null;
            yield break;
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
            yield return new WaitUntil(() => Buddy.GUI.Toaster.IsBusy);

            // --- INIT HDCam & TakePhoto with it ---
            mTestInProcess = true;
            Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);
            Buddy.Sensors.HDCamera.TakePhotograph(OnFinish);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Toaster.Hide();
            Buddy.Sensors.HDCamera.Close();
            yield return new WaitUntil(() => Buddy.GUI.Toaster.IsBusy);
        }

        private void OnFinish(Photograph iMyPhoto)
        {
            if (iMyPhoto == null)
            {
                mTestInProcess = false;
                return;
            }
            Buddy.GUI.Toaster.Display<PictureToast>().With(iMyPhoto.Image, () => { mTestInProcess = false; });
        }
        #endregion
    }
}
