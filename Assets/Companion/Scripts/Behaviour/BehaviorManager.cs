using UnityEngine;
using BuddyOS;
using BuddyFeature.Vocal;
using System;
using System.Collections.Generic;

namespace BuddyApp.Companion
{
    [RequireComponent(typeof(CliffDetector))]
    [RequireComponent(typeof(FaceDetector))]
    [RequireComponent(typeof(IRDetector))]
    [RequireComponent(typeof(Reaction))]
    [RequireComponent(typeof(SpeechDetector))]
    [RequireComponent(typeof(ThermalDetector))]
    [RequireComponent(typeof(USDetector))]
    [RequireComponent(typeof(BuddyFaceDetector))]
    [RequireComponent(typeof(AccelerometerDetector))]
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

            mActionInProgress = false;
            mInactiveTime = Time.time;
            //mReaction.ActionFinished = PopHead;
            mReaction.ActionFinished = OnActionFinished;
            mAccelerometerDetector.OnDetection += mReaction.IsBeingLifted;
        }

        void Update()
        {
            //if (mThermalDetector.ThermalDetected && !mFaceDetector.FaceDetected)
            //    PushInStack(mReaction.StepBackHelloReaction);

            //if (mThermalDetector.ThermalDetected && mFaceDetector.FaceDetected)
            //    PushInStack(mReaction.FollowFace);

            if (mFaceDetector.FaceDetected)
                mReaction.FollowFace();
            else
                mReaction.StopFollowFace();

            //if (mIRDetector.IRDetected || mUSDetector.USFrontDetected)
            //    mReaction.StopWheels();

            if (mSpeechDetector.SomeoneTalkingDetected)
                Debug.Log("Someone started to talk !");

            //if (mBuddyFaceDetector.FaceTouched)
            //    mReaction.StopEverything();

            if (mBuddyFaceDetector.FaceSmashed)
                PushInStack(mReaction.Pout);

            if (mCurrentAction != null) {
                mInactiveTime = Time.time;
                mReaction.StopWandering();
            }

            if (!mFaceDetector.FaceDetected && Time.time - mInactiveTime > 45F) {
                //mReaction.AskSomething();
                mReaction.StartWandering();
                //mInactiveTime = Time.time;
            }

            Behave();
        }

        private void OnActionFinished()
        {
            Debug.Log("Current reaction is finished");
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
    }
}