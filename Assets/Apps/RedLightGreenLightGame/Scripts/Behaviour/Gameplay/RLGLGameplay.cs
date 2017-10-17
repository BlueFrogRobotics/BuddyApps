using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using OpenCVUnity;
using UnityEngine.UI;

namespace BuddyApp.RedLightGreenLightGame
{
    // TODO: When player detected -> stopallondetect for the callback
    public class RLGLGameplay : AStateMachineBehaviour
    {
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private RGBCam mCam;
        private Texture2D mTexture;
        private RawImage mRaw;
        private bool mIsDetectedMouv;
        private int mIdLevel;
        private int mTargetScale;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ON STATE ENTER RLGL GAMEPLAY");
            mCam = Primitive.RGBCam;
            mTexture = new Texture2D(mCam.Width, mCam.Height);
            Perception.Motion.OnDetect(OnMovementDetected);
            //WARNING : pour après quand on aura le parseur de level ect
            //mIdLevel = mRLGLBehaviour.idLevel;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbFaFailedacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            if(mIdLevel >= 0 && mIdLevel < 3)
            {
                //se retourne
            }
            else if (mIdLevel > 2)
            {
                //ferme les yeux
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ON STATE EXIT RLGL GAMEPLAY");
        }



        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            Mat lCurrentFrame = mCam.FrameMat.clone();
            foreach (MotionEntity lEntity in iMotions)
            {
                Imgproc.circle(lCurrentFrame, Utils.Center(lEntity.RectInFrame), 10, new Scalar(255, 255, 0), -1);
            }

            Utils.MatToTexture2D(lCurrentFrame, Utils.ScaleTexture2DFromMat(lCurrentFrame, mTexture));
            mRaw.texture = mTexture;

            if (iMotions.Length > 30)
                mIsDetectedMouv = true;
            return false;
        }

    }
}

