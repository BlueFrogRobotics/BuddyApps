using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Buddy;
using UnityEngine.UI;
using OpenCVUnity;

namespace BuddyApp.Companion
{
    public class Colour : AStateMachineBehaviour
    {
        private RGBCam mCam;
        private Mat mMat;
        private Texture2D mTexture;
        private ShadeEntity mColor;
        private bool mHasShowWindow;
        private float mTime;
        private float mEndTime;

        // Use this for initialization
        public override void Start()
        {
            mCam = BYOS.Instance.Primitive.RGBCam;
            mCam.Resolution = RGBCamResolution.W_176_H_144;
            mMat = new Mat();
            mTexture = new Texture2D(160, 120);
            mColor = new ShadeEntity();
            mHasShowWindow = false;
            mTime = 0F;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iAnimatorStateInfo, int iLayerIndex)
        {
            if (!mCam.IsOpen)
                mCam.Open(mCam.Resolution);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iAnimatorStateInfo, int iLayerIndex)
        {
            if (mCam.IsOpen)
            {
                mTime += Time.deltaTime;

                if (mTime > 1F && !mHasShowWindow)
                {
                    mColor = BYOS.Instance.Perception.Shade.GetColor(mCam.FrameMat);
                    for (int width = 0; width < mTexture.width; ++width)
                        for (int height = 0; height < mTexture.height; ++height)
                            mTexture.SetPixel(width, height, mColor.Color);
                    Toaster.Display<Buddy.UI.PictureToast>().With(Sprite.Create(mTexture, new UnityEngine.Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f)));
                    mTexture.Apply();

                    BYOS.Instance.Primitive.LED.SetBodyLight(mColor.Color);
                    string lSentence = BYOS.Instance.Dictionary.GetRandomString("colourseen").Replace("[colour]", BYOS.Instance.Dictionary.GetRandomString(mColor.NearestColor.ToString().ToLower()));
                    BYOS.Instance.Interaction.TextToSpeech.Say(lSentence);

                    mHasShowWindow = true;
                }

                if (mTime > 5F)
                    Trigger("VOCALCOMMAND");
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iAnimatorStateInfo, int iLayerIndex)
        {
            mHasShowWindow = false;
            mTime = 0F;
            Toaster.Hide();
            if (mCam.IsOpen)
                mCam.Close();
        }
    }
}