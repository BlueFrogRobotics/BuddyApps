using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Buddy;
using UnityEngine.UI;
using OpenCVUnity;

namespace BuddyApp.Companion
{
    public class Mirror : AStateMachineBehaviour
    {
        private RGBCam mCam;
        private Mat mMat;
        private Texture2D mTexture = null;
        private bool mHasShowWindow;
        private float mTime;
        private float mEndTime;

        // Use this for initialization
        public override void Start()
        {
            mCam = BYOS.Instance.Primitive.RGBCam;
            mCam.Resolution = RGBCamResolution.W_176_H_144;
            mMat = new Mat();
            mHasShowWindow = false;
            mTime = 0F;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCam.IsOpen)
                mCam.Open(mCam.Resolution);
            if (iAnimator.GetInteger("Duration") < 10F)
                mEndTime = 10F;
            else if ((iAnimator.GetInteger("Duration") > 60F))
                mEndTime = 60;
            else
                mEndTime = iAnimator.GetInteger("Duration");

        }

        // Update is called once per frame
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTime += Time.deltaTime;
            if (mCam.IsOpen)
            {
                if (!mHasShowWindow)
                {
                    mHasShowWindow = true;
                    Mat mMatSrc = mCam.FrameMat;
                    Core.flip(mMatSrc, mMat, 1);
                    mTexture = Utils.MatToTexture2D(mMat);
                    Debug.Log("MIRROR : Toast opened");
                    Toaster.Display<Buddy.UI.PictureToast>().With(Sprite.Create(mTexture, new UnityEngine.Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f)));
                }
                else
                {
                    Mat mMatSrc = mCam.FrameMat.clone();
                    Core.flip(mMatSrc, mMat, 1);
                    Texture2D lTexture = Utils.MatToTexture2D(mMat);
                    mTexture.SetPixels(lTexture.GetPixels());
                    Debug.Log("MIRROR : New flux");
                }
            }

            mTexture.Apply();

            if (mTime > mEndTime)
                Trigger("VOCALCOMMAND");
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Toaster.Hide();
            mHasShowWindow = false;
            mTime = 0F;
            iAnimator.SetInteger("Duration", 0);

            if (mCam.IsOpen)
                mCam.Close();
        }
    }
}