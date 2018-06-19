using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class ReplacePlayer : AStateMachineBehaviour
    {
        private enum State : int
        {
            FORWARD = 0,
            STOP = 1,
            PAUSE = 2,
            BACKWARD = 3,
            FINISHED = 4
        }

        private State mState;

        private const float OBSTACLE_DISTANCE = 0.4f;
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private bool mMustStop = false;
        private float mTimer = 0.0f;
        private float mDistance = 0.0f;
        private Vector3 mBeginningOdometry;
        private bool mGoForward;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Vector2 lDist = mRLGLBehaviour.EndingOdometry - mRLGLBehaviour.StartingOdometry;
            mDistance = lDist.magnitude;
            mBeginningOdometry = Primitive.Motors.Wheels.Odometry;
            mGoForward = true;
            //GetGameObject(1).GetComponent<Animator>().SetTrigger("open");
            //GetGameObject(1).SetActive(true);
            mMustStop = false;
            

            mRLGLBehaviour.Timer = 0.0f;
            mTimer = 0.0f;
            mState = State.FORWARD;
            mRLGLBehaviour.TargetClicked = false;

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //if (mRLGLBehaviour.Timer > 15)
            //{
            //    Primitive.Motors.Wheels.Stop();
            //    mMustStop = true;
            //    Trigger("DisengagementQuestion");
            //}
            //else if (!mMustStop)
            //{
                switch (mState)
                {
                    case State.FORWARD:
                        GoForward();
                        break;
                    case State.STOP:
                        Stop();
                        break;
                    case State.PAUSE:
                        Pause();
                        break;
                    case State.BACKWARD:
                        GoBackward();
                        break;
                    case State.FINISHED:
                        Finished();
                        break;
                    default:
                        Stop();
                        break;
                }
           // }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //GetGameObject(1).GetComponent<Animator>().SetTrigger("close");
            //GetGameObject(1).SetActive(false);
        }

        private bool ObstacleInFront()
        {
            if (Primitive.IRSensors.Middle.Distance < OBSTACLE_DISTANCE && Primitive.IRSensors.Middle.Distance != 0)
            { 
                Debug.Log("RLGL OBSTACLE : " + Primitive.IRSensors.Middle.Distance);
                return true;

            }

            return false;
        }


        private void GoForward()
        {
            Primitive.Motors.Wheels.SetWheelsSpeed(200f);
            mRLGLBehaviour.TimerMove += Time.deltaTime;

            if (ObstacleInFront())
            {
                mState = State.STOP;
            }
            Vector2 lDistVector = Primitive.Motors.Wheels.Odometry - mBeginningOdometry;
            float lDistance = lDistVector.magnitude;
            if (Mathf.Abs(lDistance-mDistance)<0.1f)
            {
                Primitive.Motors.Wheels.Stop();
                mGoForward = false;
                mBeginningOdometry = Primitive.Motors.Wheels.Odometry;
                mState = State.BACKWARD;
            }
        }

        private void GoBackward()
        {
            Primitive.Motors.Wheels.SetWheelsSpeed(-200f);
            mRLGLBehaviour.TimerMove += Time.deltaTime;

            //if (ObstacleInFront())
            //{
            //    mState = State.STOP;
            //}
            Vector2 lDistVector =  mBeginningOdometry - Primitive.Motors.Wheels.Odometry;
            float lDistance = lDistVector.magnitude;
            if (Mathf.Abs(lDistance - mDistance) < 0.1f)
            {
                Primitive.Motors.Wheels.Stop();
                mState = State.FINISHED;
            }
        }

        private void Stop()
        {
            Primitive.Motors.Wheels.Stop();
            Interaction.TextToSpeech.SayKey("recoil");
            mTimer = 0.0f;
            mState = State.PAUSE;
        }

        private void Pause()
        {
            mTimer += Time.deltaTime;
            if (mTimer > 5.0f && mGoForward)
                mState = State.FORWARD;
            else if (mTimer > 5.0f && !mGoForward)
                mState = State.BACKWARD;
        }

        private void Finished()
        {
            Primitive.Motors.Wheels.Stop();
            Trigger("StartGame");
            mMustStop = true;
        }
    }
}

