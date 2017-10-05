using UnityEngine;
using System.Collections;
using Buddy;
using UnityEngine.UI;
using System;
namespace BuddyApp.RedLightGreenLight
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

        private bool mBack;

        private bool mObjectDetected;

        private bool mTestUS;

        private float mTimerDifficulty;

      
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
            mBack = false;
            mTestUS = false;
            mObjectDetected = false;
            mDiffDebugMovement = 0;
            mTimerMovement = 0;
            Interaction.Face.SetExpression(MoodType.NEUTRAL);
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

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimerDebug += Time.deltaTime;
            if (!mIsOneTurnDone)
            {
                if (Interaction.TextToSpeech.HasFinishedTalking && !mFirstSentence)
                {
                    StartCoroutine(WaitTenSecondsAtStart());
                }

                if (mStartCount)
                {
                    if (!mCountTen)
                    {
                        GetGameObject(4).GetComponent<Animator>().SetTrigger("Open_WTimer");
                        GetGameObject(1).GetComponent<Animator>().SetTrigger("Open");
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
                            Interaction.TextToSpeech.Say(Dictionary.GetRandomString("countState1"));
                            //Interaction.TextToSpeech.Say("Hurry up, it will be fun");
                            mTimerMovement = mTimerDebugInt + 4500;
                            Primitive.Motors.Wheels.SetWheelsSpeed(150.0F, 150.0F, 4500);
                            mFirstMove = true;
                        }
                        if(Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL)
                        {
                            mBack = true;
                        }

                        if (mFirstMove && !mSecondeMove && !mObjectDetected)
                        {
                            if (Primitive.IRSensors.Middle.Distance <= 0.4F/* || Primitive.USSensors.Left.Distance <= 0.5F || Primitive.USSensors.Right.Distance <= 0.5F*/)
                            {
                                Interaction.TextToSpeech.Silence(0);
                                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("countState2"));
                                //Interaction.TextToSpeech.Say("Don't stay here and place yourself at 26 feet in front of me");
                                //mDiffDebugMovement = mTimerMovement - (int)mTimerDebug;
                               
                                //Primitive.Motors.Wheels.StopWheels();
                                Primitive.Motors.Wheels.SetWheelsSpeed(0.0F, 0.0F, 10);
                                if (/*Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS ||*/ Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL )
                                {
                                    mDiffDebugMovement = mTimerMovement - mTimerDebugInt;
                                    Debug.Log(mDiffDebugMovement);
                                    mObjectDetected = true;
                                    mBack = true;
                                }
                            }
                        }

                        if (mFirstMove && !mSecondeMove && mBack /*|| (Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS)*/)
                        {
                            Debug.Log(Primitive.USSensors.Back.Distance + " " + mTestUS);
                            if(Primitive.USSensors.Back.Distance <= 0.25F && mTestUS)
                            {
                                Debug.Log("STOP MOUVEMENT US");
                                Primitive.Motors.Wheels.SetWheelsSpeed(0.0F, 0.0F, 10);
                                mSecondeMove = true;
                            }
                            if(!mTestUS)
                            {
                                Debug.Log("RECUL");
                                mTestUS = true;
                                Primitive.Motors.Wheels.SetWheelsSpeed(-150.0F, -150.0F, 4500 - mDiffDebugMovement);
                                
                            }
                            Debug.Log(Primitive.USSensors.Back.Distance + " " + mTestUS + " " + Primitive.Motors.Wheels.Status);
                            //if(Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL)
                            //{
                            //    mSecondeMove = true;
                            //}

                        }
                    }
                    else if (mTimerDebug > 10.9F)
                    {
                        GetGameObject(4).GetComponent<Animator>().SetTrigger("Close_WTimer");
                        GetGameObject(1).GetComponent<Animator>().SetTrigger("Close");
                        mStartCount = false;
                    }

                }

                if (Interaction.TextToSpeech.HasFinishedTalking && mFirstSentence && !mSecondSentence)
                {
                    Interaction.Mood.Set(MoodType.NEUTRAL); 
                    mCount = 0;
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("countState3"));
                    //Interaction.TextToSpeech.Say("Ok let's go!");
                    mSecondSentence = true;
                }
            }
            if (mIsOneTurnDone)
            {
                if (Interaction.TextToSpeech.HasFinishedTalking && !mFirstSentenceNotDetected)
                {
                    StartCoroutine(NotDetected());
                }

            }
            if (mSecondSentence || mFirstSentenceNotDetected)
            {
                if (Interaction.TextToSpeech.HasFinishedTalking)
                {
                    StartCoroutine(GreenLightMomentAndTurn());
                    mCountGreenLight = 0;
                }

                if (((Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL || Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS)
                    && mIsReachedGoal) 
                    && mIsCoroutineDone)
                {
                    mIsMovementDone = true;
                }

                if (mIsMovementDone && Interaction.TextToSpeech.HasFinishedTalking)
                {
                    StartCoroutine(ChangeState(TimerWithDifficulty(), iAnimator));
                }
            }
        }


        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCanvasUIToWin.SetActive(false);
        }

        private float TimerWithDifficulty()
        {
            int mDifficulty = RedLightGreenLightData.Instance.Difficulty;

            if(mDifficulty == 0)
            {
                mTimerDifficulty = 2.5F;
                Debug.Log("EASY : " + mTimerDifficulty);
            }
            else if (mDifficulty == 1)
            {
                mTimerDifficulty = 1.5F;
                Debug.Log("MEDIUM : " + mTimerDifficulty);
            }
            else if (mDifficulty == 2)
            {
                mTimerDifficulty = 1.0F;
                Debug.Log("HARD : " + mTimerDifficulty);
            }
            else if (mDifficulty == 3)
            {
                mTimerDifficulty = 0.5F;
                Debug.Log("IMPOSSIBLE : " + mTimerDifficulty);
            }
            
            return mTimerDifficulty;
        }

        private IEnumerator WaitTenSecondsAtStart()
        {
            if (mCount == 0 && Interaction.TextToSpeech.HasFinishedTalking)
            {
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("countState4"));
                //Interaction.TextToSpeech.Say("Okay let's play together! You have ten seconds to walk away by about fifteen feet, gogo! ");
                mCount++;
                Primitive.Motors.YesHinge.SetPosition(45.0F, 150.0F);
            }

            //Debug.Log(mTimerDebug);
            if (Interaction.TextToSpeech.HasFinishedTalking)
            {
                mStartCount = true;
                yield return new WaitForSeconds(10.0F);
                mFirstSentence = true;
            }
        }

        private IEnumerator NotDetected()
        {
            if (Interaction.TextToSpeech.HasFinishedTalking && mCountGreenLight == 0 && !mFirstSentenceNotDetected)
            {
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("countState5"));
                //Interaction.TextToSpeech.Say("You are really good at this game!");
                mCountGreenLight++;
            }
            yield return new WaitForSeconds(2.0F);
            mFirstSentenceNotDetected = true;
        }

        private IEnumerator GreenLightMomentAndTurn()
        {
            yield return new WaitForSeconds(3.0F);
            if (Interaction.TextToSpeech.HasFinishedTalking && mCount == 0 && mCountGreenLight == 0)
            {
                mCanvasUIToWin.SetActive(true);
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("countState6"));
                //Interaction.TextToSpeech.Say("Green Light !");
                Primitive.Motors.Wheels.TurnAngle(180.0F, 250.0F, 0.02F);
                mCount++;
                mIsCoroutineDone = true;
            }
            yield return new WaitForSeconds(1.5F);
            mIsReachedGoal = true;
        }

        private IEnumerator ChangeState(float iSecondToWait, Animator iAnimator)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
            yield return new WaitForSeconds(iSecondToWait);
            iAnimator.SetBool("IsCountDone", true);
        }

    }
}

