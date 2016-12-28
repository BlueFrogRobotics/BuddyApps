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
    [RequireComponent(typeof(HeadForcedDetector))]
    [RequireComponent(typeof(IRDetector))]
    [RequireComponent(typeof(Reaction))]
    [RequireComponent(typeof(SpeechDetector))]
    [RequireComponent(typeof(ThermalDetector))]
    [RequireComponent(typeof(USDetector))]
    [RequireComponent(typeof(VocalChat))]
    public class BehaviorManager : MonoBehaviour
    {
        private bool mAskedSomething;
        private bool mVocalWanderOrder;
        private bool mActionInProgress;
		private bool mIsRobotIsTrackingSomeone;
		private bool mAreEyesTrackingThermal;
        private float mInactiveTime;
        private Stack<Action> mActionStack;
        private Action mCurrentAction;

        private CompanionData mCompanionData;
        private Dictionary mDictionary;
        private VocalActivation mVocalActivation;
        private VocalChat mVocalChat;
        private YesHinge mYesHinge;

        private AccelerometerDetector mAccelerometerDetector;
        private BuddyFaceDetector mBuddyFaceDetector;
        private CliffDetector mCliffDetector;
        private FaceDetector mFaceDetector;
        private HeadForcedDetector mHeadForcedDetector;
        private IRDetector mIRDetector;
        private Reaction mReaction;
        private SpeechDetector mSpeechDetector;
        private ThermalDetector mThermalDetector;
        private USDetector mUSDetector;

        void Start()
        {
            mCurrentAction = null;
            mActionStack = new Stack<Action>();

            mAccelerometerDetector = GetComponent<AccelerometerDetector>();
            mBuddyFaceDetector = GetComponent<BuddyFaceDetector>();
            mCliffDetector = GetComponent<CliffDetector>();
            mFaceDetector = GetComponent<FaceDetector>();
            mHeadForcedDetector = GetComponent<HeadForcedDetector>();
            mIRDetector = GetComponent<IRDetector>();
            mReaction = GetComponent<Reaction>();
            mSpeechDetector = GetComponent<SpeechDetector>();
            mThermalDetector = GetComponent<ThermalDetector>();
            mUSDetector = GetComponent<USDetector>();
            mVocalChat = GetComponent<VocalChat>();

            mCompanionData = CompanionData.Instance;
            mDictionary = BYOS.Instance.Dictionary;
            mVocalActivation = BYOS.Instance.VocalActivation;
            mYesHinge = BYOS.Instance.Motors.YesHinge;

            mVocalActivation.enabled = true;
            mVocalActivation.StartRecoWithTrigger();
            mVocalChat.WithNotification = true;
            mVocalChat.OnQuestionTypeFound = SortQuestionType;

            mVocalWanderOrder = false;
            mActionInProgress = false;
            mInactiveTime = Time.time;
            //mReaction.ActionFinished = PopHead;
            mReaction.ActionFinished = OnActionFinished;
            mAccelerometerDetector.OnDetection += mReaction.IsBeingLifted;
			mIsRobotIsTrackingSomeone = true;
			// by default the robot is following the poeple with his eyes
			mAreEyesTrackingThermal = true;
        }

        void Update()
        {
            //if (mSpeechDetector.SomeoneTalkingDetected)
            //    Debug.Log("Someone started to talk !");

            //if (mBuddyFaceDetector.FaceTouched)
            //    mReaction.StopEverything();

            if (mFaceDetector.FaceDetected)
                mReaction.FollowFace();
            else
                mReaction.StopFollowFace();

            if (mBuddyFaceDetector.FaceSmashed)
                PushInStack(mReaction.Pout);

            if (mHeadForcedDetector.HeadForcedDetected)
                PushInStack(mReaction.HeadForced);

            //if (mThermalDetector.ThermalDetected && !mFaceDetector.FaceDetected && mCurrentAction == null)
            //    mReaction.StepBackHelloReaction();
            //else if (!mThermalDetector.ThermalDetected)
            //    mReaction.DisableSayHelloReaction();

            if (mBuddyFaceDetector.EyeTouched || mFaceDetector.FaceDetected ||
                (mCurrentAction != null && !mVocalWanderOrder) || !mIsRobotIsTrackingSomeone) {
                //Debug.Log("Interaction with Buddy");
                mVocalWanderOrder = false;
                mInactiveTime = Time.time;
                mReaction.StopMoving();
            }

            if(Time.time - mInactiveTime > 10F) {// && Time.time - mInactiveTime < 30F) {
                if(mCompanionData.CanMoveBody) {
                    mReaction.StopIdle();
                    mReaction.StartWandering();
                }
                else if (mCompanionData.CanMoveHead)
                    mReaction.StartIdle();
            }
            //else if(!mAskedSomething) {
            //    mReaction.AskSomething();
            //    mInactiveTime = Time.time;
            //    mAskedSomething = true;
            //}
            //else if (Time.time - mInactiveTime > 30F && mCompanionData.CanMoveBody) {
            //    //mReaction.AskSomething();
            //    //mInactiveTime = Time.time;
            //    mReaction.StopIdle();
            //    mReaction.StartWandering();
            //}

			if (mIsRobotIsTrackingSomeone){
				mReaction.StartFollowing ();
			}else{
				mReaction.StopFollowing ();
			}

			if (mAreEyesTrackingThermal) {
				mReaction.StartEyesFollow ();
			} else {
				mReaction.StopEyesFollow ();
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
                    mCompanionData.CanMoveBody = true;
                    break;

				case "DontMove":
					mVocalWanderOrder = false;
	                    //mCompanionData.CanMoveBody = false;
					mReaction.StopMoving ();

					if (mIsRobotIsTrackingSomeone)
						mIsRobotIsTrackingSomeone = false;
	                break;

				case "FollowMe":
					if (!mIsRobotIsTrackingSomeone)
						mIsRobotIsTrackingSomeone = true;
					break;

                case "HeadUp":
                    GetComponent<IdleReaction>().HeadPosition = -10F;
                    GetComponent<WanderReaction>().HeadPosition = -10F;
                    break;

                case "HeadDown":
                    GetComponent<IdleReaction>().HeadPosition = 15F;
                    GetComponent<WanderReaction>().HeadPosition = 15F;
                    break;

                case "VolumeUp":
                    BYOS.Instance.Speaker.VolumeUp();
                    break;

                case "VolumeDown":
                    BYOS.Instance.Speaker.VolumeDown();
                    break;

                case "LookAtMe":
                    PushInStack(mReaction.SearchFace);
                    break;

				case "Photo":
					new LoadAppBySceneCmd("TakePhotoApp").Execute();
					break;

                case "Calcul":
                    new LoadAppBySceneCmd("CalculGameApp").Execute();
                    break;

                case "Babyphone":
                    new LoadAppBySceneCmd("BabyApp").Execute();
                    break;

                case "FreezeDance":
                    new LoadAppBySceneCmd("FreezeDanceApp").Execute();
                    break;

                case "Guardian":
                    new LoadAppBySceneCmd("Guardian").Execute();
                    break;

                case "IOT":
                    new LoadAppBySceneCmd("IOT").Execute();
                    break;

                case "Jukebox":
                    new LoadAppBySceneCmd("JukeboxApp").Execute();
                    break;

                case "Recipe":
                    new LoadAppBySceneCmd("Recipe").Execute();
                    break;

                case "RLGL":
                    new LoadAppBySceneCmd("RLGLApp").Execute();
                    break;

                case "Memory":
                    new LoadAppBySceneCmd("MemoryGameApp").Execute();
                    break;

				case "Quizz":
                    break;

                case "Colors":
                    break;

                default:
                    break;
            }

            mInactiveTime = Time.time;
        }
    }
}