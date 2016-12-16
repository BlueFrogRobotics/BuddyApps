using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using BuddyFeature.Vocal;
using System;
using System.Collections.Generic;

namespace BuddyApp.Companion
{
    [RequireComponent(typeof(AccelerometerDetector))]
    [RequireComponent(typeof(BuddyFaceDetector))]
    [RequireComponent(typeof(CliffDetector))]
    [RequireComponent(typeof(FaceDetector))]
    [RequireComponent(typeof(IRDetector))]
    [RequireComponent(typeof(Reaction))]
    [RequireComponent(typeof(SpeechDetector))]
    [RequireComponent(typeof(ThermalDetector))]
    [RequireComponent(typeof(USDetector))]
    [RequireComponent(typeof(VocalChat))]
    public class BehaviorManager : MonoBehaviour
    {
        private AccelerometerDetector mAccelerometerDetector;
        private BuddyFaceDetector mBuddyFaceDetector;
        private CliffDetector mCliffDetector;
        private FaceDetector mFaceDetector;
        private IRDetector mIRDetector;
        private Reaction mReaction;
        private SpeechDetector mSpeechDetector;
        private ThermalDetector mThermalDetector;
        private USDetector mUSDetector;
        private VocalChat mVocalChat;

        private Dictionary mDictionary;
        private CompanionData mCompanionData;

        private bool mAskedSomething;
        private bool mVocalWanderOrder;
        private bool mActionInProgress;
        private float mInactiveTime;
        private Stack<Action> mActionStack;
        private Action mCurrentAction;
        //private VocalActivation mVocalActivation;

        void Start()
        {
            mCurrentAction = null;
            mActionStack = new Stack<Action>();
            mAccelerometerDetector = GetComponent<AccelerometerDetector>();
            mBuddyFaceDetector = GetComponent<BuddyFaceDetector>();
            mCliffDetector = GetComponent<CliffDetector>();
            mFaceDetector = GetComponent<FaceDetector>();
            mIRDetector = GetComponent<IRDetector>();
            mReaction = GetComponent<Reaction>();
            mSpeechDetector = GetComponent<SpeechDetector>();
            mThermalDetector = GetComponent<ThermalDetector>();
            mUSDetector = GetComponent<USDetector>();
            mVocalChat = GetComponent<VocalChat>();

            mDictionary = BYOS.Instance.Dictionary;
            mCompanionData = CompanionData.Instance;

            mVocalChat.WithNotification = true;
            mVocalChat.OnQuestionTypeFound = SortQuestionType;

            mVocalWanderOrder = false;
            mActionInProgress = false;
            mInactiveTime = Time.time;
            //mReaction.ActionFinished = PopHead;
            mReaction.ActionFinished = OnActionFinished;
            mAccelerometerDetector.OnDetection += mReaction.IsBeingLifted;
            mBuddyFaceDetector.RightSideTouched += mReaction.LookRight;
            mBuddyFaceDetector.LeftSideTouched += mReaction.LookLeft;
        }

        void Update()
        {
            //if (mThermalDetector.ThermalDetected && mFaceDetector.FaceDetected)
            //    PushInStack(mReaction.FollowFace);

            //if (mIRDetector.IRDetected || mUSDetector.USFrontDetected)
            //    mReaction.StopWheels();

            //if (mSpeechDetector.SomeoneTalkingDetected)
            //    Debug.Log("Someone started to talk !");

            //if (mBuddyFaceDetector.FaceTouched)
            //    mReaction.StopEverything();

            if (mThermalDetector.ThermalDetected && !mFaceDetector.FaceDetected)
                PushInStack(mReaction.StepBackHelloReaction);

            if (mFaceDetector.FaceDetected)
                mReaction.FollowFace();
            else
                mReaction.StopFollowFace();

            if (mBuddyFaceDetector.FaceSmashed)
                PushInStack(mReaction.Pout);

            if ((mSpeechDetector.SomeoneTalkingDetected && !mVocalWanderOrder)
                || mBuddyFaceDetector.FaceTouched || mFaceDetector.FaceDetected ||
                mCurrentAction != null ) {
                //Debug.Log("Interaction with Buddy");
                mInactiveTime = Time.time;
                mReaction.StopMoving();
            }

            if(Time.time - mInactiveTime > 10F && Time.time - mInactiveTime < 50F) {
                mReaction.StartIdle();
            }
            //else if(!mAskedSomething) {
            //    mReaction.AskSomething();
            //    mInactiveTime = Time.time;
            //    mAskedSomething = true;
            //}
            else if (Time.time - mInactiveTime > 50F && mCompanionData.CanMoveBody) {
                //mReaction.AskSomething();
                //mInactiveTime = Time.time;
                mReaction.StopIdle();
                mReaction.StartWandering();
            }

            Behave();
        }

        private void OnActionFinished()
        {
            Debug.Log("Current reaction is finished");
            mAskedSomething = false;
            mCurrentAction = null;
            mActionInProgress = false;
        }

        private void Behave()
        {
            if(!mActionInProgress && mActionStack.Count != 0) {
                mActionInProgress = true;
                mCurrentAction = mActionStack.Pop();
                mCurrentAction.Invoke();
            }
        }

        private void PushInStack(Action iAction)
        {
            if(iAction == mCurrentAction)
                return;

            foreach (Action lStackAction in mActionStack) {
                if (lStackAction == iAction)
                    return;
            }
            Debug.Log("Inserted new Action " + iAction.Method.Name);
            mActionStack.Push(iAction);
        }
        
        private void PopHead()
        {
            mActionStack.Pop();
        }

        private void SortQuestionType(string iType)
        {
            Debug.Log("Question Type found : " + iType);
            mVocalWanderOrder = false;
            switch(iType)
            {
                case "Wander":
                    BYOS.Instance.TextToSpeech.Say(mDictionary.GetString("wander"));
                    mVocalWanderOrder = true;
                    mReaction.StartWandering();
                    break;

                case "CanMove":
                    CompanionData.Instance.CanMoveBody = true;
                    break;

                case "DontMove":
                    CompanionData.Instance.CanMoveBody = false;
                    break;

                case "LookAtMe":
                    PushInStack(mReaction.SearchFace);
                    break;

                case "Quizz":
                    break;

                case "Colors":
                    break;

                case "Memory":
                    break;

                default:
                    break;
            }

            mInactiveTime = Time.time;
        }
    }
}