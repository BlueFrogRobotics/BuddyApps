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

        private bool mDoExitApp;
        private bool mDoGoToParameters;

        private GameObject HeadAdjustmentObject;
        private GameObject mWindoAppOverBlack;

        private RawImage mRGBCamRawImage;

        private Button mTopButton;
        private Button mLowButton;
        private Button mRightButton;
        private Button mLeftButton;

        private Button mValidateButton;


        public override void Init()
        {
            HeadAdjustmentObject = GetGameObject(4);
            mWindoAppOverBlack = GetGameObject(10);
            mRGBCamRawImage = GetGameObject(9).GetComponent<RawImage>();

            mTopButton = GetGameObject(5).GetComponent<Button>();
            mLowButton = GetGameObject(6).GetComponent<Button>();
            mRightButton = GetGameObject(7).GetComponent<Button>();
            mLeftButton = GetGameObject(8).GetComponent<Button>();



            mTopButton.onClick.AddListener(MoveYesToUp);
            mLowButton.onClick.AddListener(MoveYesToDown);
            mRightButton.onClick.AddListener(MoveNoToRight);
            mLeftButton.onClick.AddListener(MoveNoToLeft);



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

            mDoExitApp = false;
            mDoGoToParameters = false;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            HeadAdjustmentObject.SetActive(true);
            mWindoAppOverBlack.SetActive(true);

            mRGBCam.Open();
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mRGBCam.Close();
            HeadAdjustmentObject.SetActive(false);
            mWindoAppOverBlack.SetActive(false);
            iAnimator.SetBool("DoHeadAdjust", false);
            iAnimator.SetFloat("ForwardState", 2);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            ControlNoAxis();
            ControlYesAxis();
            mRGBCamRawImage.texture = mRGBCam.FrameTexture2D;
            if(mDoExitApp)
                iAnimator.SetBool("DoExit", true);
            if (mDoGoToParameters)
                iAnimator.SetBool("DoSetParameters", true);
                
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


    }
}
