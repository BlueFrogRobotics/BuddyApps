using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLPositionningPlayerMovement : AStateMachineBehaviour
    {
        private const float OBSTACLE_DISTANCE = 1.1f;
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private bool mMustStop = false;
        private float mTimer = 0.0f;
        //private Vector3 mStartingOdometry;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetGameObject(1).SetActive(true);
            mMustStop = false;
            mRLGLBehaviour.StartingOdometry = Primitive.Motors.Wheels.Odometry;
            mRLGLBehaviour.Timer = 0.0f;
            mTimer = 0.0f;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mMustStop)
            {
                if (ObstacleInFront() && mRLGLBehaviour.Timer - mTimer > 4.5f)
                {
                    Debug.Log("obstacle!!!!!!");
                    //mMustStop = true;
                    Interaction.TextToSpeech.SayKey("recoilplease");
                    Primitive.Motors.Wheels.Stop();
                    mTimer = mRLGLBehaviour.Timer;
                    //mRLGLBehaviour.Timer = 0.0f;
                    //mTimer = 0.0f;
                    //Trigger("ObstacleDetected");

                }
                else if(ObstacleInFront() && mRLGLBehaviour.Timer - mTimer <= 4.5f)
                {
                    Debug.Log("toujours obstacle!!!!!!");
                    if(mRLGLBehaviour.TargetClicked)
                    {
                        Primitive.Motors.Wheels.Stop();
                        Trigger("WantToPlay");
                        mMustStop = true;
                    }
                }
                else
                {
                    if (!mRLGLBehaviour.TargetClicked)
                    {
                        Primitive.Motors.Wheels.SetWheelsSpeedAtMedium();
                        Vector3 lDist = Primitive.Motors.Wheels.Odometry - mRLGLBehaviour.StartingOdometry;
                        Debug.Log("distance: " + lDist.magnitude);
                    }
                    else
                    {
                        Primitive.Motors.Wheels.Stop();
                        Trigger("WantToPlay");
                        mMustStop = true;
                    }
                }
                
                if(mRLGLBehaviour.Timer>10.0f)
                {
                    Trigger("DisengagementQuestion");
                    GetGameObject(1).SetActive(false);
                    mRLGLBehaviour.TargetClicked = false;
                    Primitive.Motors.Wheels.Stop();
                    mMustStop = true;

                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private bool ObstacleInFront()
        {
            //if (Primitive.IRSensors.Left.Distance < OBSTACLE_DISTANCE && Primitive.IRSensors.Left.Distance != 0)
            //    return true;
            //if (Primitive.IRSensors.Right.Distance < OBSTACLE_DISTANCE && Primitive.IRSensors.Right.Distance != 0)
            //    return true;
            if (Primitive.IRSensors.Middle.Distance < OBSTACLE_DISTANCE && Primitive.IRSensors.Middle.Distance != 0)
                return true;

            return false;
        }

    }
}

