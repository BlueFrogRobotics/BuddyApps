using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RedLightGreenLight
{
    public class RLGLCountState : AStateMachineBehaviour
    {

        /// <summary>
        /// Number of players, needs to be defined before this state in parameters. Minimum is 1 and maximum is 2 for now.
        /// </summary>
        public int mNumberPlayers;

        /// <summary>
        /// Time in float to wait before starting the gameplay.
        /// </summary>
        private float mTimeToWait;

        /// <summary>
        /// Boolean to do actions when sentence has been said
        /// </summary>
        private bool mFirstSentenceDone;
        private bool mSecondSentenceDone;
        private bool mSentenceDone;
        private bool mFirstMove;
        private bool mSecondMove;
        private bool mStartCount;
        private bool mBack;
        private bool mObjectDetected;
        private bool mTestUS;
        private bool mFirstSentenceNotDetected;
        private bool mIsReachedGoal;
        private bool mIsCoroutineDone;
        private bool mIsMovementDone;

        private bool mIsOneTurnDone;
        public bool IsOneTurnDone { get { return mIsOneTurnDone; } set { mIsOneTurnDone = value; } }

        /// <summary>
        /// Timer before the game in float
        /// </summary>
        private float mTimer;

        private int mTimerDebugInt;
        private int mTimerMovement;
        private int mDiffDebugMovement;
        private int mCountGreenLight;
        private int mCount;

        private float mTimerDifficulty;

        private GameObject mCanvasUIToWin;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            

            mIsMovementDone = false;
            mIsCoroutineDone = false;
            mTestUS = false;
            mNumberPlayers = 1;
            mTimeToWait = 10.0F;
            mFirstSentenceDone = false;
            mSentenceDone = false;
            mStartCount = false;
            mTimerDebugInt = 0;
            mFirstMove = false;
            mSecondMove = false;
            mTimerMovement = 0;
            mBack = false;
            mObjectDetected = false;
            mDiffDebugMovement = 0;
            mFirstSentenceNotDetected = false;
            mIsReachedGoal = false;
            mCountGreenLight = 0;
            mCount = 0;
            mCanvasUIToWin = GetGameObject(0);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if (!mIsOneTurnDone)
            {
                if (Interaction.TextToSpeech.HasFinishedTalking && !mFirstSentenceDone)
                {
                    StartCoroutine(TimeToWaitAtStart(mTimeToWait));
                }

                if (mTimer <= mTimeToWait + 1.0F && mStartCount)
                {
                    mTimerDebugInt = (int)(mTimer * 1000F);
                    BYOS.Instance.Toaster.Display<CountdownToast>().With((int)mTimeToWait, () => Debug.Log("Start CountDown"));

                    if (!mFirstMove)
                    {
                        Interaction.TextToSpeech.Say(Dictionary.GetRandomString("countState1"));
                        //Interaction.TextToSpeech.Say("Hurry up, it will be fun");
                        mTimerMovement = mTimerDebugInt + 4500;
                        Primitive.Motors.Wheels.SetWheelsSpeed(150.0F, 150.0F, 4500);
                        mFirstMove = true;
                    }
                    if (Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL)
                    {
                        mBack  = true;
                    }
                    if (mFirstMove && !mSecondMove && !mObjectDetected)
                    {
                        if (Primitive.IRSensors.Middle.Distance <= 0.4F)
                        {
                            Interaction.TextToSpeech.Silence(0);
                            Interaction.TextToSpeech.Say(Dictionary.GetRandomString("countState2"));
                            //Interaction.TextToSpeech.Say("Don't stay here and place yourself at 26 feet in front of me");
                            Primitive.Motors.Wheels.SetWheelsSpeed(0.0F, 0.0F, 10);
                            if (Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL)
                            {
                                mDiffDebugMovement = mTimerMovement - mTimerDebugInt;
                                Debug.Log(mDiffDebugMovement);
                                mObjectDetected = true;
                                mBack = true;
                            }
                        }
                    }

                    if (mFirstMove && !mSecondMove && mBack)
                    {
                        Debug.Log(Primitive.USSensors.Back.Distance + " " + mTestUS);
                        if (Primitive.USSensors.Back.Distance <= 0.25F && mTestUS)
                        {
                            Debug.Log("STOP MOUVEMENT US");
                            Primitive.Motors.Wheels.SetWheelsSpeed(0.0F, 0.0F, 10);
                            mSecondMove = true;
                        }
                        if (!mTestUS)
                        {
                            Debug.Log("RECUL");
                            mTestUS = true;
                            Primitive.Motors.Wheels.SetWheelsSpeed(-150.0F, -150.0F, 4500 - mDiffDebugMovement);

                        }
                        Debug.Log(Primitive.USSensors.Back.Distance + " " + mTestUS + " " + Primitive.Motors.Wheels.Status);
                    }
                }
                else if (mTimer > mTimeToWait + 0.9F && mStartCount)
                {
                    mStartCount = false;
                }

                if (Interaction.TextToSpeech.HasFinishedTalking && mFirstSentenceDone && !mSecondSentenceDone)
                {
                    Interaction.Mood.Set(MoodType.NEUTRAL);
                    mCount = 0;
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("countState3"));
                    //Interaction.TextToSpeech.Say("Ok let's go!");
                    mSecondSentenceDone = true;
                }
            }

            if (Interaction.TextToSpeech.HasFinishedTalking && !mFirstSentenceNotDetected && mIsOneTurnDone)
            {
                StartCoroutine(NotDetected());
            }
            

            if (mSecondSentenceDone || mFirstSentenceNotDetected)
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
                    StartCoroutine(ChangeState(TimerWithDifficulty(), animator));
                    
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        /// <summary>
        /// Difficulty of the game, it changes only the timer when the Buddy 
        /// </summary>
        /// <returns></returns>
        private float TimerWithDifficulty()
        {
            int mDifficulty = RedLightGreenLightData.Instance.Difficulty;

            if (mDifficulty == 0)
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

        /// <summary>
        /// Coroutine 
        /// </summary>
        /// <param name="ITimeToWait"></param>
        /// <returns></returns>
        private IEnumerator TimeToWaitAtStart(float ITimeToWait)
        {
           if(!mSentenceDone)
            {
                //Say for example : Let's play together! You have ten seconds to walk away by about fifteen feet, go go!
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("countState4"));
                Primitive.Motors.YesHinge.SetPosition(45.0F, 150.0F);
                mSentenceDone = true;
            }
            mStartCount = true;
            mTimer = 0.0F;
            yield return new WaitForSeconds(ITimeToWait);
            mFirstSentenceDone = true;
        }

        private IEnumerator NotDetected()
        {
            if (mCountGreenLight == 0 && !mFirstSentenceNotDetected)
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
            //iAnimator.SetBool("IsCountDone", true);
            iAnimator.SetTrigger("Gameplay");
        }

    }
}

