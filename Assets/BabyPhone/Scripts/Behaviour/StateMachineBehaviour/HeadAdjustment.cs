using UnityEngine;
using UnityEngine.UI;
using BuddyOS;
using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class HeadAdjustment : AStateMachineBehaviour
    {
        private float mNormalSpeed;
        private float mRotationSpeed;
        private float mNoSpeed;

        private float mYesSpeed;
        private float mYesAngle;
        private float mNoAngle;

        private bool mYesUp;
        private bool mYesDown;
        private bool mNoLeft;
        private bool mNoRight;

        private GameObject HeadAdjustmentObject;
        private RawImage mRGBCamRawImage;
        private Button mTopButton;
        private Button mLowButton;
        private Button mRightButton;
        private Button mLeftButton;
        private Button mValidateButton;
        private Button mQuitButton;

        public override void Init()
        {
            HeadAdjustmentObject = GetGameObject(0);
            mRGBCamRawImage = GetGameObject(1).GetComponent<RawImage>();

            mTopButton = GetGameObject(2).GetComponent<Button>();
            mLowButton = GetGameObject(3).GetComponent<Button>();
            mRightButton = GetGameObject(4).GetComponent<Button>();
            mLeftButton = GetGameObject(5).GetComponent<Button>();
            mQuitButton = GetGameObject(6).GetComponent<Button>();

            mTopButton.onClick.AddListener(MoveYesToUp);
            mLowButton.onClick.AddListener(MoveYesToDown);
            mRightButton.onClick.AddListener(MoveNoToRight);
            mLeftButton.onClick.AddListener(MoveNoToLeft);
            mQuitButton.onClick.AddListener(Quit);

            mNormalSpeed = 0.6F;
            mRotationSpeed = 0.2F;
            mNoSpeed = 20.0F;

            mYesSpeed = 20.0F;
            mYesAngle = 0.0F;
            mNoAngle = 0.0F;

            mYesUp = false;
            mYesDown = false;
            mNoLeft = false;
            mNoRight = false;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            HeadAdjustmentObject.SetActive(true);
            mRGBCam.Open();
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mRGBCam.Close();
            HeadAdjustmentObject.SetActive(false);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            ControlNoAxis();
            ControlYesAxis();
            mRGBCamRawImage.texture = mRGBCam.FrameTexture2D;
        }

        /// <summary>
        /// Function to control the head hinge
        /// </summary>
        private void ControlNoAxis()
        {
            bool lChanged = false;
            if (mNoLeft)
            {
                mNoAngle = mNoHinge.CurrentAnglePosition + 1;
                lChanged = true;
                mNoLeft = false;
            }

            if (mNoRight)
            {
                mNoAngle = mNoHinge.CurrentAnglePosition - 1;
                lChanged = true;
                mNoRight = false;
            }

            if (lChanged)
            {
                mNoHinge.SetPosition(mNoAngle, mNoSpeed);
            }
        }

        /// <summary>
        /// Function to control the neck hinge 
        /// </summary>
        private void ControlYesAxis()
        {
            bool lChanged = false;
            if (mYesDown)
            {
                mYesAngle = mYesHinge.CurrentAnglePosition + 2;
                lChanged = true;
                mYesDown = false;
            }

            if (mYesUp)
            {
                mYesAngle = mYesHinge.CurrentAnglePosition - 2;
                lChanged = true;
                mYesUp = false;
            }

            if (lChanged)
                mYesHinge.SetPosition(mYesAngle, mYesSpeed);
        }

        public void MoveNoToLeft()
        {
            mNoLeft = true;
        }
        public void MoveNoToRight()
        {
            mNoRight = true;
        }
        public void MoveYesToUp()
        {
            mYesUp = true;
        }
        public void MoveYesToDown()
        {
            mYesDown = true;
        }
        public void OpenMenu()
        {

        }
        public void Quit()
        {
            BYOS.Instance.AppManager.Quit();
        }

    }
}
