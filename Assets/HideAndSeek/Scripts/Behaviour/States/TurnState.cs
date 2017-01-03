using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class TurnState : AStateMachineBehaviour
    {
        private string mFoundMove;
        private float mTimer;
        private int mCount = 0;
        private bool mMovedLeft = false;
        private bool mMovedRight = false;
        private bool mHasTurned = false;
        private bool mHasClosed = false;

        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0.0f;
            mHasTurned = false;
            mMovedLeft = false;
            mMovedRight = false;
            mHasClosed = false;
            mCount = 0;
            if(!mRGBCam.IsOpen)
                mRGBCam.Open();
            mFoundMove = mDictionary.GetString("foundMovement");
            // mWheels.MoveDistance(100, 100, 10, 0.1f);

        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;

            if (mCount < 20)
            {
                MovementDetector.Direction lDir = (MovementDetector.Direction)iAnimator.GetInteger("MovingDetect");
                if (lDir == MovementDetector.Direction.LEFT)
                    mMovedLeft = true;
                else if (lDir == MovementDetector.Direction.RIGHT)
                    mMovedRight = true;
                mCount++;
            }

            else if (mCount >= 20 && !mHasTurned)
            {
                if (mMovedLeft && !mMovedRight)
                {
                    mWheels.TurnAngle(45.0f, 200.0f, 0.02f);
                    mTTS.Say(mFoundMove);// "J'ai vu un mouvement");
                }
                else if (!mMovedLeft && mMovedRight)
                {
                    mWheels.TurnAngle(-45.0f, 200.0f, 0.02f);
                    mTTS.Say(mFoundMove);
                }
                else if(mMovedLeft && mMovedRight)
                    mTTS.Say(mFoundMove);
                else
                    mWheels.TurnAngle(-180.0f, 200.0f, 0.02f);

                mHasTurned = true;
                mTimer = 0.0f;
            }

            //mWheels.TurnAngle(30.0f, 200.0f, 0.1f);
            else if (mHasTurned && mTimer > 1.5f && mWheels.Status == MovingState.MOTIONLESS && !mHasClosed)
            {
                iAnimator.SetTrigger("ChangeState");
                mHasClosed = true;
                if (mRGBCam.IsOpen)
                    mRGBCam.Close();
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mRGBCam.IsOpen)
                mRGBCam.Close();
            iAnimator.ResetTrigger("ChangeState");
        }
    }
}