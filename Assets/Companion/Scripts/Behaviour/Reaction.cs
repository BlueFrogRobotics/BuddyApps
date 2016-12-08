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

    [RequireComponent(typeof(RoombaNavigation))]
    [RequireComponent(typeof(FollowFaceReaction))]
    [RequireComponent(typeof(VocalChat))]
    [RequireComponent(typeof(WanderReaction))]
    public class Reaction : MonoBehaviour
    {
        private bool mIsPouting;
        private bool mIsTrackingFace;
        private float mHeadNoAngle;
        private float mHeadYesAngle;
        internal ReactionFinished ActionFinished;
        private FollowFaceReaction mFollowFace;
        private FaceCascadeTracker mFaceTracker;
        private RoombaNavigation mNavigation;
        private TextToSpeech mTTS;
        private VocalChat mVocalChat;
        private WanderReaction mWander;

        void Start()
        {
            mIsPouting = false;
            mIsTrackingFace = false;
            mTTS = BYOS.Instance.TextToSpeech;
            mFollowFace = GetComponent<FollowFaceReaction>();
            mFaceTracker = GetComponent<FaceCascadeTracker>();
            mNavigation = GetComponent<RoombaNavigation>();
            mVocalChat = GetComponent<VocalChat>();
            mWander = GetComponent<WanderReaction>();

            mWander.enabled = false;
            mNavigation.enabled = false;
            mVocalChat.OnQuestionTypeFound = SortQuestionType;
        }

        void Update()
        {

        }

        public void AskSomething()
        {
            if (mIsPouting)
                return;

            StartCoroutine(AskSomethingCo());
        }

        private IEnumerator AskSomethingCo()
        {
            mTTS.Say("Que puis-je faire pour vous ?");
            yield return new WaitForSeconds(2F);

            mVocalChat.StartDialogue();
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

        //private IEnumerator FollowFaceCo()
        //{
        //    mTTS.Say("Bonjour !");
        //    float mFaceAndTalkTime = Time.time;
        //    //Write here some code to make sure that one face is centered in the camera
        //    List<Rect> mTrackedObjects = mFaceTracker.TrackedObjects;
        //    mHeadNoAngle = BYOS.Instance.Motors.NoHinge.CurrentAnglePosition;
        //    mHeadYesAngle = BYOS.Instance.Motors.YesHinge.CurrentAnglePosition;
        //    int mCameraWidthCenter = BYOS.Instance.RGBCam.Width / 2;
        //    int mCameraHeightCenter = BYOS.Instance.RGBCam.Height / 2;

        //    while (mTrackedObjects.Count > 0)
        //    {
        //        float lXCenter = mTrackedObjects[0].x + mTrackedObjects[0].width / 2;
        //        float lYCenter = mTrackedObjects[0].y + mTrackedObjects[0].height / 2;
        //        Debug.Log("Tracking face : XCenter " + lXCenter);
        //        Debug.Log("Tracking face : YCenter " + lYCenter);

        //        if (!(mCameraWidthCenter - 25 < lXCenter && lXCenter < mCameraWidthCenter + 5))
        //            mHeadNoAngle -= Mathf.Sign(lXCenter - mCameraWidthCenter) * 1.5F;
        //        if (!(mCameraHeightCenter - 5 < lYCenter && lYCenter < mCameraHeightCenter + 25))
        //            mHeadYesAngle += Mathf.Sign(lYCenter - mCameraHeightCenter) * 1.5F;

        //        new SetPosYesCmd(mHeadYesAngle).Execute();
        //        new SetPosNoCmd(mHeadNoAngle).Execute();
        //        yield return new WaitForSeconds(0.1F);

        //        if (Time.time - mFaceAndTalkTime > 30F)
        //        {
        //            AskSomething();
        //            mFaceAndTalkTime = Time.time;
        //        }

        //        mTrackedObjects = mFaceTracker.TrackedObjects;
        //    }
        //    mIsTrackingFace = false;
        //    ActionFinished();
        //}

        public void IsBeingLifted()
        {
            new SetMoodFaceCmd(MoodType.SCARED).Execute();
            BYOS.Instance.Face.SetMouthEvent(MouthEvent.SCREAM);
        }
        
        public void Pout()
        {
            if (mIsPouting)
                return;

            mIsPouting = true;
            Debug.Log("Face smashed ! I will pout");

            if (CompanionData.Instance.CanMoveBody)
                StartCoroutine(PoutBodyCo());
            else
                StartCoroutine(PoutHeadCo());
        }

        private IEnumerator PoutBodyCo()
        {
            new SetMoodFaceCmd(MoodType.ANGRY).Execute();

            //Small animation to make he seem angry
            for (int i = 0; i < 2; i++)
            {
                new SetWheelsSpeedCmd(200F, -200F, 200).Execute();
                yield return new WaitForSeconds(0.2F);
            }
            BYOS.Instance.Face.SetMouthEvent(MouthEvent.SCREAM);
            for (int i = 0; i < 2; i++)
            {
                new SetWheelsSpeedCmd(-200F, 200F, 200).Execute();
                yield return new WaitForSeconds(0.2F);
            }

            //Turn around and run away (set route and avoid obstacles is better)
            for (int i = 0; i < 5; i++)
            {
                new SetWheelsSpeedCmd(200F, -200F, 200).Execute();
                yield return new WaitForSeconds(0.2F);
            }
            new SetMoodFaceCmd(MoodType.GRUMPY).Execute();

            StartWandering();
            yield return new WaitForSeconds(5F);

            //for (int i = 0; i < 50; i++)
            //{
            //    new SetWheelsSpeedCmd(200F, 200F, 200).Execute();
            //    yield return new WaitForSeconds(0.2F);
            //}

            StopWandering();
            new SetMoodFaceCmd(MoodType.NEUTRAL).Execute();
            mIsPouting = false;
            ActionFinished();
        }

        private IEnumerator PoutHeadCo()
        {
            new SetMoodFaceCmd(MoodType.ANGRY).Execute();

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

                BYOS.Instance.Face.SetMouthEvent(MouthEvent.SCREAM);
                new SetPosYesCmd(-5F).Execute();
                new SetPosNoCmd(0F).Execute();

                yield return new WaitForSeconds(0.5F);

                new SetPosYesCmd(5F).Execute();
            }
            new SetMoodFaceCmd(MoodType.NEUTRAL).Execute();
            mIsPouting = false;
            ActionFinished();
        }

        public void StartWandering()
        {
            if (mWander.enabled)
                return;
            Debug.Log("Start Wandering");
            //mNavigation.enabled = true;
            mWander.enabled = true;
        }

        public void StopWandering()
        {
            if (!mWander.enabled)
                return;
            Debug.Log("Stop Wandering");
            //mNavigation.enabled = false;
            mWander.enabled = false;
        }

        public void StopEverything()
        {
            StopAllCoroutines();
            StopWheels();
            mNavigation.enabled = false;
            mFollowFace.enabled = false;
        }

        public void StopWheels()
        {
            new SetWheelsSpeedCmd(0F, 0F).Execute();
        }

        public void StepBackHelloReaction()
        {
            Debug.Log("Someone is there. I say hello !");
            new SetWheelsSpeedCmd(-200F, -200F, 100).Execute();
            new SetPosYesCmd(0).Execute();
            mTTS.Say("Bonjour !");
            ActionFinished();
        }

        private void SortQuestionType(string iType)
        {
            Debug.Log("Question Type found : " + iType);
        }
    }
}