using BlueQuark;

using UnityEngine;

using OpenCVUnity;

namespace BuddyApp.BabyPhone
{
    /// <summary>
    /// State where the user can test the movement detection sensibility
    /// </summary>
    public sealed class MovementDetectionTestState : AStateMachineBehaviour
    {
        private Texture2D mTexture;
        private Mat mMatSrc;

        private FButton mLeftButton;
        private FButton mValidateButton;
        private FLabeledHorizontalSlider mSlider;

        private MotionDetector mMovementTracker;
        private bool mInit;

        public override void Start()
        {
            mInit = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("setmotionsensitivity"));
            mInit = false;

            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));

            mSlider = Buddy.GUI.Footer.CreateOnMiddle<FLabeledHorizontalSlider>();
            mSlider.SlidingValue = BabyPhoneData.Instance.MovementDetectionThreshold;
            mSlider.OnSlide.Add(OnSlideChange);
            mSlider.SetLabel(Buddy.Resources.GetString("threshold"));

            mLeftButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            mLeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
            mLeftButton.SetBackgroundColor(Color.white);
            mLeftButton.SetIconColor(Color.black);
            mLeftButton.OnClick.Add(() => { Trigger("MovementDetection"); });

            mValidateButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            mValidateButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));
            mValidateButton.SetBackgroundColor(Utils.BUDDY_COLOR);
            mValidateButton.SetIconColor(Color.white);
            mValidateButton.OnClick.Add(() => { SaveAndQuit(); });

            MotionDetectorParameter lMotionParam = new MotionDetectorParameter();
            lMotionParam.SensibilityThreshold = BabyPhoneData.Instance.MovementDetectionThreshold * DetectionManager.MAX_MOVEMENT_THRESHOLD / 100;
            lMotionParam.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 320, 240);

            mMovementTracker = Buddy.Perception.MotionDetector;
            mMovementTracker.OnDetect.AddP(OnMovementDetected, lMotionParam);

            mTexture = new Texture2D(320, 240);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mInit)
            {
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTexture);
                mInit = true;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Sensors.RGBCamera.OnNewFrame.Remove((iInput) => OnFrameCaptured(iInput));
            mMovementTracker.OnDetect.RemoveP(OnMovementDetected);
            mSlider.OnSlide.Remove(OnSlideChange);
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Remove(mLeftButton);
            Buddy.GUI.Footer.Remove(mValidateButton);
            Buddy.GUI.Footer.Remove(mSlider);
        }

        private void OnFrameCaptured(RGBCameraFrame iFrame)
        {
            if (iFrame.Mat.width() > 0 && iFrame.Mat.height() > 0)
            {
                Mat lTest = iFrame.Mat.clone();
                if (lTest.empty())
                    Debug.Log("on new frame empty");
                mMatSrc = lTest.clone();
                Core.flip(mMatSrc, mMatSrc, 1);
                mTexture = Utils.ScaleTexture2DFromMat(mMatSrc, mTexture);
                Utils.MatToTexture2D(mMatSrc, mTexture);
            }
        }

        private void SaveAndQuit()
        {
            BabyPhoneData.Instance.MovementDetectionThreshold = (int)mSlider.SlidingValue;
            Trigger("MovementDetection");
        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            if (iMotions.Length > 1)
                Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);

            Core.flip(mMatSrc, mMatSrc, 1);

            foreach (MotionEntity lEntity in iMotions)
            {
                Imgproc.circle(mMatSrc, Utils.Center(lEntity.RectInFrame), 3, new Scalar(255, 0, 0), -1);
            }

            Core.flip(mMatSrc, mMatSrc, 1);
            Utils.MatToTexture2D(mMatSrc, Utils.ScaleTexture2DFromMat(mMatSrc, mTexture));

            return true;
        }

        private void OnSlideChange(float iValue)
        {
            MotionDetectorParameter lMotionParam = new MotionDetectorParameter();
            lMotionParam.SensibilityThreshold = iValue * DetectionManager.MAX_MOVEMENT_THRESHOLD / 100;
            lMotionParam.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 320, 240);

            mMovementTracker.OnDetect.ModifyParameterP(OnMovementDetected, lMotionParam);
        }
    }
}