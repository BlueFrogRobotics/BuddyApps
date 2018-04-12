using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLPositionningMovement : AStateMachineBehaviour
    {
        private enum State : int
        {
            FORWARD=0,
            STOP=1,
            PAUSE=2,
            TARGET_CLICKED=3
        }

        private State mState;

        private const float OBSTACLE_DISTANCE = 0.4f;
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private bool mMustStop = false;
        private float mTimer = 0.0f;
        private bool mIsInitialized;
        //private Vector3 mStartingOdometry;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            StartCoroutine(TalkAndInit());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mIsInitialized)
            {
                if (mRLGLBehaviour.Timer > 15)
                {
                    Primitive.Motors.Wheels.Stop();
                    mMustStop = true;
                    Trigger("DisengagementQuestion");
                }
                else if (!mMustStop)
                {
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
                        case State.TARGET_CLICKED:
                            TargetClicked();
                            break;
                        default:
                            Stop();
                            break;
                    }
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetGameObject(1).GetComponent<Animator>().SetTrigger("close");
            //GetGameObject(1).SetActive(false);
        }

        private IEnumerator TalkAndInit()
        {
            mIsInitialized = false;
            yield return SayKeyAndWait("willgo");
            GetGameObject(1).GetComponent<Animator>().SetTrigger("open");
            //GetGameObject(1).SetActive(true);
            mMustStop = false;
            if (!mRLGLBehaviour.CanRecoil)
            {
                mRLGLBehaviour.StartingOdometry = Primitive.Motors.Wheels.Odometry;
                mRLGLBehaviour.TimerMove = 0;
            }
            else
                mRLGLBehaviour.CanRecoil = false;

            mRLGLBehaviour.Timer = 0.0f;
            mTimer = 0.0f;
            mState = State.FORWARD;
            mRLGLBehaviour.TargetClicked = false;
            mIsInitialized = true;
        }

        private bool ObstacleInFront()
        {
            if (Primitive.IRSensors.Middle.Distance < OBSTACLE_DISTANCE && Primitive.IRSensors.Middle.Distance != 0)
                return true;

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
            if (mRLGLBehaviour.TargetClicked)
                mState = State.TARGET_CLICKED;
        }

        private void Stop()
        {
            Primitive.Motors.Wheels.Stop();
            Interaction.TextToSpeech.SayKey("recoilplease");
            mTimer = 0.0f;
            mState = State.PAUSE;
        }

        private void Pause()
        {
            mTimer += Time.deltaTime;
            if (mTimer > 5.0f)
                mState = State.FORWARD;
            if (mRLGLBehaviour.TargetClicked && Interaction.TextToSpeech.HasFinishedTalking)
                mState = State.TARGET_CLICKED;
        }

        private void TargetClicked()
        {
            Primitive.Motors.Wheels.Stop();
            Trigger("WantToPlay");
            mMustStop = true;
        }
    }
}

