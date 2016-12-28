using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;
using BuddyOS.Command;
using BuddyFeature.Vision;
using BuddyFeature.Vocal;
using BuddyFeature.Navigation;
using Rect = OpenCVUnity.Rect;

namespace BuddyApp.Companion
{
    internal delegate void ReactionFinished();

    [RequireComponent(typeof(FollowFaceReaction))]
    [RequireComponent(typeof(GrumpyReaction))]
    [RequireComponent(typeof(IdleReaction))]
    [RequireComponent(typeof(LiftedReaction))]
    [RequireComponent(typeof(SayHelloReaction))]
	[RequireComponent(typeof(SearchFaceReaction))]
	[RequireComponent(typeof(WanderReaction))]
	[RequireComponent(typeof(FollowPersonReaction))]
	[RequireComponent(typeof(EyesFollowReaction))]
    public class Reaction : MonoBehaviour
    {
        private bool mIsPouting;
        private bool mIsTrackingFace;

        internal ReactionFinished ActionFinished;
        private Dictionary mDictionary;
        private Face mFace;
        private FaceCascadeTracker mFaceTracker;
        private FollowFaceReaction mFollowFace;
        private GrumpyReaction mGrumpyReaction;
        private SayHelloReaction mHelloReaction;
        private IdleReaction mIdleReaction;
        private LiftedReaction mLiftedReaction;
		private FollowPersonReaction mFollowPersonReaction;
        private Mood mMood;
        private NoHinge mNoHinge;
        private SearchFaceReaction mSearchFaceReaction;
        private TextToSpeech mTTS;
        private WanderReaction mWanderReaction;
		private EyesFollowReaction mEyesFollowReaction;
        private Wheels mWheels;
        private YesHinge mYesHinge;


        void Start()
        {
            mDictionary = BYOS.Instance.Dictionary;
            mFace = BYOS.Instance.Face;
            mMood = BYOS.Instance.Mood;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mTTS = BYOS.Instance.TextToSpeech;
            mWheels = BYOS.Instance.Motors.Wheels;
            mYesHinge = BYOS.Instance.Motors.YesHinge;

            mFaceTracker = GetComponent<FaceCascadeTracker>();
            mFollowFace = GetComponent<FollowFaceReaction>();
            mGrumpyReaction = GetComponent<GrumpyReaction>();
            mIdleReaction = GetComponent<IdleReaction>();
            mLiftedReaction = GetComponent<LiftedReaction>();
            mHelloReaction = GetComponent<SayHelloReaction>();
            mSearchFaceReaction = GetComponent<SearchFaceReaction>();
            mWanderReaction = GetComponent<WanderReaction>();
			mFollowPersonReaction = GetComponent<FollowPersonReaction> ();
			mEyesFollowReaction = GetComponent<EyesFollowReaction> ();

            mIsPouting = false;
            mIsTrackingFace = false;
            mIdleReaction.enabled = false;
            mHelloReaction.enabled = false;
            mWanderReaction.enabled = false;
            mGrumpyReaction.enabled = false;
			// TODO : change this on to false;
			mFollowPersonReaction.enabled = false;
			mEyesFollowReaction.enabled = true;
        }

        void Update()
        {

        }

        public void HeadForced()
        {
            //mWheels.TurnAngle(BYOS.Instance.Motors.NoHinge.CurrentAnglePosition, 80F, 0.5F);
            //mTTS.Say("Hey ! Arrête ça !");
            //ActionFinished();

            if (mGrumpyReaction.enabled == true)
                return;

            mGrumpyReaction.enabled = true;
        }

        public void AskSomething()
        {
            if (mIsPouting)
                return;

            mTTS.Say(mDictionary.GetString("playWithMe"));
            //StartCoroutine(AskSomethingCo());
        }

        private IEnumerator AskSomethingCo()
        {
            mTTS.Say("Que puis-je faire pour vous ?");
            yield return new WaitForSeconds(2F);

            //mVocalChat.StartDialogue();
        }

        public void IsBeingLifted()
        {
            if (mLiftedReaction.enabled)
                return;
            mLiftedReaction.enabled = true;
        }

        public void Pout()
        {
            if (mIsPouting)
                return;

            mIsPouting = true;
            Debug.Log("Face smashed ! I will pout");

            if (CompanionData.Instance.CanMoveBody)
                StartCoroutine(PoutBodyCo());
            else if (CompanionData.Instance.CanMoveHead)
                StartCoroutine(PoutHeadCo());
        }

        private IEnumerator PoutBodyCo()
        {
            mMood.Set(MoodType.ANGRY);

            //Small animation to make him seem angry
            for (int i = 0; i < 2; i++)
            {
                mWheels.SetWheelsSpeed(200F, -200F, 200);
                yield return new WaitForSeconds(0.2F);
            }
            mFace.SetEvent(FaceEvent.SCREAM);

            for (int i = 0; i < 2; i++)
            {
                mWheels.SetWheelsSpeed(-200F, 200F, 200);
                yield return new WaitForSeconds(0.2F);
            }

            //Turn around
            mWheels.TurnAngle(180F, 120F, 0.2F);
            mMood.Set(MoodType.GRUMPY);            
            yield return new WaitForSeconds(5F);

            mWheels.TurnAngle(-180F, 120F, 0.2F);
            mMood.Set(MoodType.NEUTRAL);
            mIsPouting = false;
            ActionFinished();
        }

        private IEnumerator PoutHeadCo()
        {
            mMood.Set(MoodType.ANGRY);
            yield return new WaitForSeconds(0.1F);
            
            mYesHinge.SetPosition(5F);
            mNoHinge.SetPosition(0F);
                

            yield return new WaitForSeconds(0.5F);
                
            mYesHinge.SetPosition(-5F);

            yield return new WaitForSeconds(0.5F);
                
            mYesHinge.SetPosition(5F);

            yield return new WaitForSeconds(0.5F);
                
            mFace.SetEvent(FaceEvent.SCREAM);
            mYesHinge.SetPosition(-5F);
            mNoHinge.SetPosition(0F);

            yield return new WaitForSeconds(0.5F);
                
            mYesHinge.SetPosition(5F); 

            mMood.Set(MoodType.NEUTRAL);
            mIsPouting = false;
            ActionFinished();
        }

        public void FollowFace()
        {
            if (mFollowFace.enabled)
                return;

            mFollowFace.enabled = true;

            //if (mIsTrackingFace || !CompanionData.Instance.CanMoveHead)
            //    return;
            ////Debug.Log("Found a face to track");
            //mIsTrackingFace = true;
            //StartCoroutine(FollowFaceCo());
        }

        public void StopFollowFace()
        {
            if (!mFollowFace.enabled)
                return;

            mFollowFace.enabled = false;
        }

        public void StartIdle()
        {
            if (mIdleReaction.enabled == true)
                return;

            Debug.Log("Starting Idle");
            mIdleReaction.enabled = true;
        }

        public void StopIdle()
        {
            if (mIdleReaction.enabled == false)
                return;

            Debug.Log("Stopping Idle");
            mIdleReaction.enabled = false;
        }

        public void StartWandering()
        {
            if (mWanderReaction.enabled || !CompanionData.Instance.CanMoveBody)
                return;
            Debug.Log("Start Wandering");
            //mNavigation.enabled = true;
            mWanderReaction.enabled = true;
        }

        public void StopWandering()
        {
            if (!mWanderReaction.enabled)
                return;
            Debug.Log("Stop Wandering");
            //mNavigation.enabled = false;
            mWanderReaction.enabled = false;
        }

        public void StopMoving()
        {
            StopWandering();
            StopIdle();
        }

        public void StopEverything()
        {
            StopAllCoroutines();
            StopWheels();
            mWanderReaction.enabled = false;
            mFollowFace.enabled = false;
        }

        public void StopWheels()
        {
            mWheels.SetWheelsSpeed(0F, 0F);
        }

        public void StepBackHelloReaction()
        {
            if (mHelloReaction.enabled)
                return;

            mHelloReaction.enabled = true;
        }

        public void DisableSayHelloReaction()
        {
            if (!mHelloReaction.enabled)
                return; 

            mHelloReaction.enabled = false;
        }

        public void SearchFace()
        {
            if (mSearchFaceReaction.enabled)
                return;

            mSearchFaceReaction.enabled = false;
        }
        
        public void LookRight()
        {
            float lHeadNoAngle = mNoHinge.CurrentAnglePosition;
            lHeadNoAngle -= 20;
            mNoHinge.SetPosition(lHeadNoAngle);
        }

        public void LookLeft()
        {
            float lHeadNoAngle = mNoHinge.CurrentAnglePosition;
            lHeadNoAngle += 20;
            mNoHinge.SetPosition(lHeadNoAngle);
        }

		public void StartFollowing ()
		{
			mFollowPersonReaction.enabled = true;
		}
	
		public void StopFollowing ()
		{
			mFollowPersonReaction.enabled = false;
		}

		public void StartEyesFollow()
		{
			mEyesFollowReaction.enabled = true;
		}

		public void StopEyesFollow()
		{
			mEyesFollowReaction.enabled = false;
		}

    }
}