using UnityEngine;
using System.Collections;
using BuddyOS.App;
using UnityEngine.UI;
using System;
namespace BuddyApp.RLGL
{
    public class CountState : AStateMachineBehaviour
    {
        private GameObject mCanvasUIToWin;
        private Button mButtonToWin;
        private bool mFirstSentence;
        private bool mSecondSentence;
        private bool mIsCoroutineDone;
        private bool mIsMovementDone;
        private bool mFirstSentenceNotDetected;
        private bool mIsReachedGoal;
        private bool mStartCount;


        private bool mIsOneTurnDone;
        public bool IsOneTurnDone { get { return mIsOneTurnDone; } set { mIsOneTurnDone = value; } }

        private int mCount;
        private int mCountGreenLight;
        private bool mCountTen;

        private bool mFirstMove;
        private bool mSecondeMove;

        private float mTimerDebug;
        private int mTimerDebugInt;
        private int mTimerMovement;
        private int mDiffDebugMovement;

        private bool mObjectDetected;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mObjectDetected = false;
            mDiffDebugMovement = 0;
            mTimerMovement = 0;
            mFace.SetExpression(MoodType.NEUTRAL);
            mCountTen = false;
            mStartCount = false;
            mFirstMove = false;
            mSecondeMove = false;
            mTimerDebug = 0;
            mCanvasUIToWin = GetGameObject(0);
            mIsCoroutineDone = false;
            mIsMovementDone = false;
            mFirstSentence = false;
            mSecondSentence = false;
            mIsReachedGoal = false;
            mFirstSentenceNotDetected = false;
            mCount = 0;
            mCountGreenLight = 0;
            iAnimator.SetBool("IsCountDone", false);
            iAnimator.SetBool("IsWon", false);

        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimerDebug += Time.deltaTime;
            if (!mIsOneTurnDone)
            {
                if (mTTS.HasFinishedTalking && !mFirstSentence)
                {
                    StartCoroutine(WaitTenSecondsAtStart());
                }

                if (mStartCount)
                {
                    if (!mCountTen)
                    {
                        GetGameObject(4).GetComponent<Animator>().SetTrigger("Open_WTimer");
                        GetGameObject(1).GetComponent<Animator>().SetTrigger("Open_BG");
                    }
                    if (!mCountTen)
                        mTimerDebug = 0.0F;
                    mCountTen = true;
                    if (mTimerDebug <= 11.0F)
                    {
                        mTimerDebugInt = (int)(mTimerDebug * 1000F);
                        GetGameObject(4).GetComponentInChildren<Text>().text = (10 - (int)mTimerDebug).ToString();
                        if (!mFirstMove)
                        {
                            mTTS.Say("Hurry up, it will be fun");
                            
                            
                            mTimerMovement = mTimerDebugInt + 4500;
                            mWheels.SetWheelsSpeed(150.0F, 150.0F, 4500);
                            mFirstMove = true;
                        }
                        if (mFirstMove && !mSecondeMove && !mObjectDetected)
                        {
                            if (mIRSensors.Middle.Distance <= 0.4F/* || mUSSensors.Left.Distance <= 0.5F || mUSSensors.Right.Distance <= 0.5F*/)
                            {

                                mTTS.Silence(0);
                                mTTS.Say("Don't stay here and place yourself at 26 feet in front of me");
                                //mDiffDebugMovement = mTimerMovement - (int)mTimerDebug;
                               
                                //mWheels.StopWheels();
                                mWheels.SetWheelsSpeed(0.0F, 0.0F, 10);
                                Debug.Log("FEAR : " + mWheels.Status);
                                if (/*mWheels.Status == MovingState.MOTIONLESS ||*/ mWheels.Status == MovingState.REACHED_GOAL )
                                {
                                    mDiffDebugMovement = mTimerMovement - mTimerDebugInt;
                                    Debug.Log(mDiffDebugMovement);
                                    mObjectDetected = true;
                                }
                                
                            }
                        }

                        if (mFirstMove && !mSecondeMove && ((mWheels.Status == MovingState.REACHED_GOAL) || (mWheels.Status == MovingState.MOTIONLESS && mTimerDebug > 3.0F)))
                        {
                            
                            mWheels.SetWheelsSpeed(-150.0F, -150.0F, 4500 - mDiffDebugMovement);
                            mSecondeMove = true;
                        }
                        //Debug.Log((10 - (int)mTimerDebug).ToString());
                    }
                    else if (mTimerDebug > 10.9F)
                    {
                        GetGameObject(4).GetComponent<Animator>().SetTrigger("Close_WTimer");
                        GetGameObject(1).GetComponent<Animator>().SetTrigger("Close_BG");
                        mStartCount = false;
                    }

                }

                if (mTTS.HasFinishedTalking && mFirstSentence && !mSecondSentence)
                {
                    mMood.Set(MoodType.NEUTRAL);
                    mCount = 0;
                    mTTS.Say("Ok let's go!");
                    mSecondSentence = true;
                }
            }
            if (mIsOneTurnDone)
            {
                if (mTTS.HasFinishedTalking && !mFirstSentenceNotDetected)
                {
                    StartCoroutine(NotDetected());

                }

            }
            if (mSecondSentence || mFirstSentenceNotDetected)
            {
                if (mTTS.HasFinishedTalking)
                {
                    StartCoroutine(GreenLightMomentAndTurn());
                    mCountGreenLight = 0;
                }

                if (((mWheels.Status == MovingState.REACHED_GOAL || mWheels.Status == MovingState.MOTIONLESS) && mIsReachedGoal) && mIsCoroutineDone)
                {
                    mIsMovementDone = true;
                }

                if (mIsMovementDone && mTTS.HasFinishedTalking)
                {
                    StartCoroutine(ChangeState(1.5F, iAnimator));
                }
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCanvasUIToWin.SetActive(false);
        }

        private IEnumerator WaitTenSecondsAtStart()
        {
            if (mCount == 0 && mTTS.HasFinishedTalking)
            {
                mTTS.Say("Okay let's play together! You have ten seconds to walk away by about fifteen feet, gogo! ");
                mCount++;
                mYesHinge.SetPosition(45.0F, 150.0F);

            }

            //Debug.Log(mTimerDebug);
            if (mTTS.HasFinishedTalking)
            {
                mStartCount = true;
                yield return new WaitForSeconds(10.0F);
                mFirstSentence = true;
            }

        }

        private IEnumerator NotDetected()
        {
            if (mTTS.HasFinishedTalking && mCountGreenLight == 0 && !mFirstSentenceNotDetected)
            {
                mTTS.Say("You are really good at this game!");
                mCountGreenLight++;
            }
            yield return new WaitForSeconds(2.0F);
            mFirstSentenceNotDetected = true;

        }

        private IEnumerator GreenLightMomentAndTurn()
        {
            yield return new WaitForSeconds(3.0F);
            if (mTTS.HasFinishedTalking && mCount == 0 && mCountGreenLight == 0)
            {
                mCanvasUIToWin.SetActive(true);
                mTTS.Say("Green Light !");
                mWheels.TurnAngle(180.0F, 250.0F, 0.02F);
                mCount++;
                mIsCoroutineDone = true;
            }
            yield return new WaitForSeconds(1.5F);
            mIsReachedGoal = true;


        }

        private IEnumerator ChangeState(float iSecondToWait, Animator iAnimator)
        {
            mMood.Set(MoodType.NEUTRAL);
            yield return new WaitForSeconds(iSecondToWait);
            iAnimator.SetBool("IsCountDone", true);
        }

    }
}

