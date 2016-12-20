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
    [RequireComponent(typeof(IdleReaction))]
    [RequireComponent(typeof(SayHelloReaction))]
    [RequireComponent(typeof(SearchFaceReaction))]
    [RequireComponent(typeof(WanderReaction))]
    public class Reaction : MonoBehaviour
    {
        private bool mIsPouting;
        private bool mIsTrackingFace;
        internal ReactionFinished ActionFinished;
        private FollowFaceReaction mFollowFace;
        private FaceCascadeTracker mFaceTracker;
        private IdleReaction mIdleReaction;
        private SayHelloReaction mHelloReaction;
        private SearchFaceReaction mSearchFaceReaction;
        private TextToSpeech mTTS;
        private WanderReaction mWanderReaction;

        private Dictionary mDictionary;

        void Start()
        {
            mIsPouting = false;
            mIsTrackingFace = false;
            mTTS = BYOS.Instance.TextToSpeech;
            mFollowFace = GetComponent<FollowFaceReaction>();
            mFaceTracker = GetComponent<FaceCascadeTracker>();
            mIdleReaction = GetComponent<IdleReaction>();
            mHelloReaction = GetComponent<SayHelloReaction>();
            mSearchFaceReaction = GetComponent<SearchFaceReaction>();
            mWanderReaction = GetComponent<WanderReaction>();

            mDictionary = BYOS.Instance.Dictionary;

            mIdleReaction.enabled = false;
            mHelloReaction.enabled = false;
            mWanderReaction.enabled = false;
        }

        void Update()
        {

        }

        public void HeadForced()
        {
            new TurnRelaCmd(BYOS.Instance.Motors.NoHinge.CurrentAnglePosition, 80F, 0.5F).Execute();
            new SayTTSCmd("Hey ! Arrête ça !").Execute();
            ActionFinished();
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
            new SetMoodCmd(MoodType.SCARED).Execute();
            new SetFaceEvntCmd(FaceEvent.SCREAM);
            //BYOS.Instance.Face.SetMouthEvent(MouthEvent.SCREAM);
            mTTS.Say(mDictionary.GetString("putMeDown"));
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
            new SetMoodCmd(MoodType.ANGRY).Execute();

            //Small animation to make he seem angry
            for (int i = 0; i < 2; i++)
            {
                new SetWheelsSpeedCmd(200F, -200F, 200).Execute();
                yield return new WaitForSeconds(0.2F);
            }
            new SetFaceEvntCmd(FaceEvent.SCREAM).Execute();
            //BYOS.Instance.Face.SetMouthEvent(MouthEvent.SCREAM);
            for (int i = 0; i < 2; i++)
            {
                new SetWheelsSpeedCmd(-200F, 200F, 200).Execute();
                yield return new WaitForSeconds(0.2F);
            }

            //Turn around and run away (set route and avoid obstacles is better)
            new TurnRelaCmd(180F, 120F, 0.2F).Execute();
            new SetMoodCmd(MoodType.GRUMPY).Execute();
            
            yield return new WaitForSeconds(5F);

            //for (int i = 0; i < 50; i++)
            //{
            //    new SetWheelsSpeedCmd(200F, 200F, 200).Execute();
            //    yield return new WaitForSeconds(0.2F);
            //}

            new TurnRelaCmd(-180F, 80F, 0.2F).Execute();
            new SetMoodCmd(MoodType.NEUTRAL).Execute();
            mIsPouting = false;
            ActionFinished();
        }

        private IEnumerator PoutHeadCo()
        {
            new SetMoodCmd(MoodType.ANGRY).Execute();

            yield return new WaitForSeconds(0.1F);

            if (CompanionData.Instance.CanMoveHead)
            {
                new SetPosYesCmd(5F).Execute();
                new SetPosNoCmd(0F).Execute();

                yield return new WaitForSeconds(0.5F);

                new SetPosNoCmd(-5F).Execute();

                yield return new WaitForSeconds(0.5F);

                new SetPosNoCmd(5F).Execute();

                yield return new WaitForSeconds(0.5F);

                BYOS.Instance.Face.SetEvent(FaceEvent.SCREAM);
                new SetPosYesCmd(-5F).Execute();
                new SetPosNoCmd(0F).Execute();

                yield return new WaitForSeconds(0.5F);

                new SetPosYesCmd(5F).Execute();
            }
            new SetMoodCmd(MoodType.NEUTRAL).Execute();
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
            new SetWheelsSpeedCmd(0F, 0F).Execute();
        }

        public void StepBackHelloReaction()
        {
            if (mHelloReaction.enabled == true)
                return;
            mHelloReaction.enabled = true;
        }

        public void SearchFace()
        {
            if (mSearchFaceReaction.enabled)
                return;

            mSearchFaceReaction.enabled = false;
        }
        
        public void LookRight()
        {
            float lHeadNoAngle = BYOS.Instance.Motors.NoHinge.CurrentAnglePosition;
            lHeadNoAngle -= 20;
            new SetPosNoCmd(lHeadNoAngle).Execute();
        }

        public void LookLeft()
        {
            float lHeadNoAngle = BYOS.Instance.Motors.NoHinge.CurrentAnglePosition;
            lHeadNoAngle += 20;
            new SetPosNoCmd(lHeadNoAngle).Execute();
        }
    }
}