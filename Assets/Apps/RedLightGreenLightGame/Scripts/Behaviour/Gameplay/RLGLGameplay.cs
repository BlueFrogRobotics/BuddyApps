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
        private int mIdLevel;
        private int mTargetScale;
        private bool mGameplay;
        private bool mFirstStep;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ON STATE ENTER RLGL GAMEPLAY");
            mGameplay = false;
            mFirstStep = false;
            mCam = Primitive.RGBCam;
            mTexture = new Texture2D(mCam.Width, mCam.Height);
            //Perception.Motion.OnDetect(OnMovementDetected);
            Interaction.TextToSpeech.SayKey("gameplayaller");
            mRLGLBehaviour.OpenFlash();
            //WARNING : pour après quand on aura le parseur de level ect
            //mIdLevel = mRLGLBehaviour.idLevel;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbFaFailedacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!mGameplay)
            {
                //Close the flash, depends on the fact that the flash is auto so it close by itself or if we have to close it
                if (mIdLevel >= 0 && mIdLevel < 3)
                {
                    if(!mFirstStep)
                    {
                        //se retourne
                        //Primitive.Motors.Wheels.TurnAngle(angle du xml, vitesse xml, 0.5F ou 1F);
                        Primitive.Motors.Wheels.TurnAngle(180F, 250F, 1F);
                        Interaction.Face.SetExpression(MoodType.LOVE);
                        mFirstStep = true;
                    }
                    
                    if((Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL || Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS) && mFirstStep)
                        Trigger("TargetGameplay");

                }
                else if (mIdLevel > 2)
                {
                    Interaction.Face.SetExpression(MoodType.LOVE);
                    Trigger("TargetGameplay");
                }
            }
            else
            {
                //put the treshold on the xml 
                Perception.Motion.OnDetect(OnMovementDetected, 5F);
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
            mRLGLBehaviour.PictureMoving = mTexture;
            Trigger("Defeat");

            ////Change the value 30 by the value in XML (maybe useless with the treshold of the motion detection)
            //if (iMotions.Length > 30)
            //    mIsDetectedMouv = true;
            return false;
        }





    }
}

