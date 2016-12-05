using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;
using BuddyOS.Command;
using BuddyFeature.Vision;
using BuddyFeature.Vocal;
using Rect = OpenCVUnity.Rect;

namespace BuddyApp.Companion
{
    internal delegate void ReactionFinished();

    public class Reaction : MonoBehaviour
    {
        public bool IsTrackingFace { get { return mIsTrackingFace; } }

        private float mHeadYesAngle;
        private float mHeadNoAngle;
        private bool mIsTrackingFace;
        private bool mIsPouting;
        internal ReactionFinished ActionFinished;
        private FaceCascadeTracker mFaceTracker;
        private TextToSpeech mTTS;
        private VocalChat mVocalChat;

        void Start()
        {
            mIsTrackingFace = false;
            mIsPouting = false;
            mTTS = BYOS.Instance.TextToSpeech;
            mFaceTracker = GetComponent<FaceCascadeTracker>();
            mVocalChat = GetComponent<VocalChat>();
        }

        void Update()
        {

        }

        public void StopEverything()
        {
            StopAllCoroutines();
            StopWheels();
            mIsTrackingFace = false;
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
            new SetMoodFaceCmd(FaceMood.ANGRY).Execute();

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
            for (int i=0; i<5; i++) {
                new SetWheelsSpeedCmd(200F, -200F, 200).Execute();
                yield return new WaitForSeconds(0.2F);
            }
            new SetMoodFaceCmd(FaceMood.GRUMPY).Execute();

            for (int i = 0; i < 50; i++)
            {
                new SetWheelsSpeedCmd(200F, 200F, 200).Execute();
                yield return new WaitForSeconds(0.2F);
            }
            new SetMoodFaceCmd(FaceMood.NEUTRAL).Execute();
            mIsPouting = false;
            ActionFinished();
        }

        private IEnumerator PoutHeadCo()
        {
            new SetMoodFaceCmd(FaceMood.ANGRY).Execute();

            yield return new WaitForSeconds(0.1F);

            if(CompanionData.Instance.CanMoveHead)
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
            mIsPouting = false;
            ActionFinished();
        }

        public void FollowFace()
        {
            if (mIsTrackingFace || !CompanionData.Instance.CanMoveHead)
                return;
            Debug.Log("Found a face to track");
            mIsTrackingFace = true;
            StartCoroutine(FollowFaceCo());
        }

        private IEnumerator FollowFaceCo()
        {
            mTTS.Say("Bonjour !");
            float mFaceAndTalkTime = Time.time;
            //Write here some code to make sure that one face is centered in the camera
            List<Rect> mTrackedObjects = mFaceTracker.TrackedObjects;
            mHeadNoAngle = BYOS.Instance.Motors.NoHinge.CurrentAnglePosition;
            mHeadYesAngle = BYOS.Instance.Motors.YesHinge.CurrentAnglePosition;
            int mCameraWidthCenter = BYOS.Instance.RGBCam.Width / 2;
            int mCameraHeightCenter = BYOS.Instance.RGBCam.Height / 2;

            while (mTrackedObjects.Count > 0) {
                float lXCenter = mTrackedObjects[0].x + mTrackedObjects[0].width / 2;
                float lYCenter = mTrackedObjects[0].y + mTrackedObjects[0].height / 2;
                Debug.Log("Tracking face : XCenter " + lXCenter);
                Debug.Log("Tracking face : YCenter " + lYCenter);

                if (!(mCameraWidthCenter - 25 < lXCenter && lXCenter < mCameraWidthCenter + 5))
                    mHeadNoAngle -= Mathf.Sign(lXCenter - mCameraWidthCenter) * 1.5F;
                if (!(mCameraHeightCenter - 5 < lYCenter && lYCenter < mCameraHeightCenter + 25))
                    mHeadYesAngle += Mathf.Sign(lYCenter - mCameraHeightCenter) * 1.5F;

                new SetPosYesCmd(mHeadYesAngle).Execute();
                new SetPosNoCmd(mHeadNoAngle).Execute();
                yield return new WaitForSeconds(0.1F);

                //if(Time.time - mFaceAndTalkTime > 30F) {
                //    AskSomething();
                //    mFaceAndTalkTime = Time.time;
                //}

                mTrackedObjects = mFaceTracker.TrackedObjects;
            }
            mIsTrackingFace = false;     
            ActionFinished();
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
    }
}