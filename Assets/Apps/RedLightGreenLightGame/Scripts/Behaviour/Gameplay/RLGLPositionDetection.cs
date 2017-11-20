using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Buddy;
using Buddy.UI;
using OpenCVUnity;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLPositionDetection : AStateMachineBehaviour
    {
        private RGBCam mCam;
        private MotionDetection mMotion;
        private Mat mMat;
        private Mat mMatDetection;
        private Texture2D mTexture;
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private int mDetectionCount = 0;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Interaction.TextToSpeech.Silence(1000);
            mMotion = Perception.Motion;
            mMotion.enabled = true;
            mCam = Primitive.RGBCam;
            mMotion.OnDetect(OnMovementDetected, 3f);
            //Texture2D truc=new Texture2D()
            //mTexture = mCam.FrameTexture2D;
            mMat = mCam.FrameMat;//Utils.Texture2DToMat(mTexture, OpenCVUnity.CvType.CV_8UC3);
            mTexture = Utils.MatToTexture2D(mMat);
            mMatDetection = null;
            Toaster.Display<PictureToast>().With(Dictionary.GetString("lookphoto"), Sprite.Create(mTexture, new UnityEngine.Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f)));
            mRLGLBehaviour.Timer = 0;
            mDetectionCount = 0;
            //StartCoroutine(Defeat());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mRLGLBehaviour.Timer > 0.1 && mDetectionCount <= 12)
            {
                if (mMatDetection == null)
                {
                    mMat = mCam.FrameMat.clone();//Utils.Texture2DToMat(mTexture, OpenCVUnity.CvType.CV_8UC3);
                    Imgproc.rectangle(mMat, new Point((int)(mMat.width() / 3), 0), new Point((int)(mMat.width() * 2 / 3), mMat.height()), new Scalar(255, 255, 0), 3);
                    Texture2D lTexture = Utils.MatToTexture2D(mMat);
                    mTexture.SetPixels(lTexture.GetPixels());
                }
                else
                {
                    Texture2D lTexture = Utils.MatToTexture2D(mMatDetection);
                    mTexture.SetPixels(lTexture.GetPixels());
                    mMatDetection = null;
                }

                mTexture.Apply();
                mRLGLBehaviour.Timer = 0;
            }

            if (mDetectionCount > 12)
            {
                Debug.Log("------------------------------------");
                mRLGLBehaviour.Gameplay = false;
                Trigger("StartGame");
            }
                
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
            mMotion.StopOnDetect(OnMovementDetected);
            Toaster.Hide();
        }


        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            Debug.Log("detection mouvement");
            if (iMotions.Length > 2)
            {
                BYOS.Instance.Primitive.Speaker.FX.Play(FXSound.BEEP_1);
                mMatDetection = mCam.FrameMat.clone();//Utils.Texture2DToMat(mTexture, OpenCVUnity.CvType.CV_8UC3);
                Imgproc.rectangle(mMatDetection, new Point((int)(mMatDetection.width() / 3), 0), new Point((int)(mMatDetection.width() * 2 / 3), mMatDetection.height()), new Scalar(255, 0, 0), 3);

                bool lInRectangle = false;
                foreach (MotionEntity lEntity in iMotions)
                {
                    Imgproc.circle(mMatDetection, Utils.Center(lEntity.RectInFrame), 3, new Scalar(255, 0, 0), 3);
                    if (lEntity.RectInFrame.x > (mMatDetection.width() / 3) && lEntity.RectInFrame.x < (mMatDetection.width() * 2 / 3))
                        lInRectangle = true;
                }
                if (lInRectangle)
                    mDetectionCount++;
            }
            //mTexture = Utils.MatToTexture2D(mMat);
            return true;
        }

     }

}
