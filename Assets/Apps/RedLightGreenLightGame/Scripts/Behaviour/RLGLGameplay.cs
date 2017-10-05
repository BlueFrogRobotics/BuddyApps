using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using OpenCVUnity;

namespace BuddyApp.RedLightGreenLightGame
{
    // TODO: When player detected -> stopallondetect for the callback
    public class RLGLGameplay : AStateMachineBehaviour
    {
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private RGBCam mCam;
        private Texture2D mTexture;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ON STATE ENTER RLGL GAMEPLAY");
            mRLGLBehaviour = GetComponent<RedLightGreenLightGameBehaviour>();
            mCam = Primitive.RGBCam;
            mTexture = new Texture2D(mCam.Width, mCam.Height);
            mRLGLBehaviour.MotionDetection = BYOS.Instance.Perception.Motion;
            
            //mRLGLBehaviour.MotionDetection.changethreshold
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            if (mRLGLBehaviour.IsPlayerPositionning)
            {
                //Premier cas : le joueur se place en début de jeu
                Positionning();
            }
            else
            {
                Gameplay();
            }

            //mRLGLBehaviour.MotionDetection.OnDetect

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ON STATE EXIT RLGL GAMEPLAY");
        }

        private void Positionning()
        {
            //activer la détecttion et regarder si le joueur bouge les mains
        }

        private void Gameplay()
        {

        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            
            if(!mRLGLBehaviour.IsPlayerPositionning)
            {
                Mat lCurrentMat = mCam.FrameMat.clone();
                foreach (MotionEntity lEntity in iMotions)
                {
                    Imgproc.circle(lCurrentMat, Utils.Center(lEntity.RectInFrame), 10, new Scalar(255, 255, 0), -1);
                }

                Utils.MatToTexture2D(lCurrentMat, Utils.ScaleTexture2DFromMat(lCurrentMat, mTexture));
            }
            else
            {

            }

            return true;
        }

    }
}

