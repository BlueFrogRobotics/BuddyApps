using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;

namespace BuddyApp.Companion
{
    public class WanderReaction : MonoBehaviour
    {
        public float HeadPosition { get { return mHeadPos; } set { mHeadPos = value; } }

        [SerializeField]
        private Emotion mEmotion;

        private const float MIN_DIST = 0.4f;

        private bool mInitialized;
        private bool mIsSearchingPoint;
        private bool mHeadSearchPlaying;
        private bool mChangingDirection;
        private float mEmoteTime;
        private float mHeadPos;
        private float mUpdateTime;
        private float mWanderTime;
        private float mRandomWanderTime;
        private float mTTSTime;
        private float mRandomSpeechTime;

        private Dictionary mDict;
        private Face mFace;
        private IRSensors mIRSensors;
        private Mood mMood;
        private NoHinge mNoHinge;
        private TextToSpeech mTTS;
        private USSensors mUSSensors;
        private Wheels mWheels;
        private YesHinge mYesHinge;

        void Start()
        {
            mInitialized = true;
            mHeadPos = -10F;
            mDict = BYOS.Instance.Dictionary;
            mFace = BYOS.Instance.Face;
            mIRSensors = BYOS.Instance.IRSensors;
            mMood = BYOS.Instance.Mood;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mTTS = BYOS.Instance.TextToSpeech;
            mUSSensors = BYOS.Instance.USSensors;
            mWheels = BYOS.Instance.Motors.Wheels;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
        }

        void OnEnable()
        {
            if(!mInitialized) {
                Start();
                mInitialized = true;
            }

            mHeadSearchPlaying = false;
            mChangingDirection = false;
            mIsSearchingPoint = true;
            mUpdateTime = Time.time;
            mWanderTime = Time.time;
            mTTSTime = Time.time;
            mEmoteTime = Time.time;
            mRandomSpeechTime = Random.Range(40F, 80F);
            mRandomWanderTime = Random.Range(10F, 30F);
            mMood.Set(MoodType.NEUTRAL);
            mYesHinge.SetPosition(10F);
            FaceRandomDirection();
        }

        void Update()
        {
            if (Time.time - mUpdateTime < 0.1F)
                return;

            if (!CompanionData.Instance.CanMoveBody)
                enabled = false;

            if (Time.time - mTTSTime > mRandomSpeechTime)
                SaySomething();

            mUpdateTime = Time.time;

            if(mIsSearchingPoint && Time.time - mWanderTime < mRandomWanderTime) {
                PlaySearchingHeadAnimation();
                if (!AnyObstructionsInfrared())
                    mWheels.SetWheelsSpeed(200F, 200F, 200);
                else
                    FaceRandomDirection();

                if (Time.time - mEmoteTime > 8F) {
                    switch (Random.Range(0, 3)) {
                        case 0:
                            mFace.SetEvent(FaceEvent.SMILE);
                            break;
                        case 1:
                            mFace.SetEvent(FaceEvent.YAWN);
                            break;
                        case 2:
                            mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
                            break;
                    }
                    mEmoteTime = Time.time;
                }
            }
            else {
                StopSearchingHeadAnimation();
                mIsSearchingPoint = false;
                ChangeDirection();
            }
        }

        void OnDisable()
        {
            mHeadSearchPlaying = false;
            mIsSearchingPoint = false;
            mNoHinge.SetPosition(0F);
            mYesHinge.SetPosition(0F);
            //new SetWheelsSpeedCmd(0F, 0F).Execute();
            //StopAllCoroutines();
            GetComponent<Reaction>().ActionFinished();
        }

        private void PlaySearchingHeadAnimation()
        {
            if (mHeadSearchPlaying)
                return;

            mHeadSearchPlaying = true;
            StartCoroutine(SearchingHeadCo());
        }

        private void StopSearchingHeadAnimation()
        {
            mHeadSearchPlaying = false;
        }

        private IEnumerator SearchingHeadCo()
        {
            while(mHeadSearchPlaying) {
                switch(Random.Range(0, 2)) {
                    case 0:
                        float lHeadNo = Random.Range(-45F, 45F);
                        mNoHinge.SetPosition(lHeadNo);
                        break;
                    case 1:
                        float lHeadYes = Random.Range(mHeadPos - 10F, mHeadPos + 10F);
                        mYesHinge.SetPosition(lHeadYes);
                        break;
                }
                yield return new WaitForSeconds(2F);
            }
        }

        private void ChangeDirection()
        {
            if (mChangingDirection)
                return;

            mChangingDirection = true;
            StartCoroutine(ChangeDirectionCo());
        }

        private IEnumerator ChangeDirectionCo()
        {
            switch(Random.Range(0, 6)) {
                case 0:
                    mMood.Set(MoodType.TIRED);
                    break;
                case 1:
                    mMood.Set(MoodType.GRUMPY);
                    break;
                case 2:
                    mMood.Set(MoodType.HAPPY);
                    break;
                default:
                    mMood.Set(MoodType.NEUTRAL);
                    break;
            }

            switch (Random.Range(0, 5))
            {
                case 0:
                    mEmotion.EnableChoregraph();
                    mEmotion.SetEvent(EmotionEvent.SHY);
                    yield return new WaitForSeconds(5F);
                    mEmotion.DisableChoregraph();
                    break;
                case 1:
                    mEmotion.EnableChoregraph();
                    mEmotion.SetEvent(EmotionEvent.DISCOVER);
                    yield return new WaitForSeconds(3.2F);
                    mEmotion.DisableChoregraph();
                    break;
                default:
                    break;
            }

            mYesHinge.SetPosition(15F);
            float lRandomAngle = Random.Range(-45F, 45F);
            mNoHinge.SetPosition(lRandomAngle);

            yield return new WaitForSeconds(1.5F);
            
            mWheels.TurnAngle(lRandomAngle, 100F, 0.02F);
            mNoHinge.SetPosition(0F);

            yield return new WaitForSeconds(1.5F);

            mIsSearchingPoint = true;
            mRandomWanderTime = Random.Range(15F, 30F);
            mWanderTime = Time.time;
            mEmoteTime = Time.time;
            mChangingDirection = false;
        }

        private bool AnyObstructionsInfrared()
        {
            float lLeftDistance = mIRSensors.Left.Distance;
            float lMiddleDistanceMiddle = mIRSensors.Middle.Distance;
            float lRightDistance = mIRSensors.Right.Distance;
            return IsCollisionEminent(lLeftDistance)
                || IsCollisionEminent(lMiddleDistanceMiddle)
                || IsCollisionEminent(lRightDistance);
        }

        private bool IsCollisionEminent(float iCollisionDistance, float iThreshold = MIN_DIST)
        {
            return iCollisionDistance != 0.0F && iCollisionDistance < iThreshold;
        }

        private void FaceRandomDirection()
        {
            float lRandomAngle = Random.Range(60F, 300F);
            if (lRandomAngle > 180F)
                lRandomAngle = 360F - lRandomAngle;
            mWheels.TurnAngle(lRandomAngle, 100F, 0.02F);
        }

        private void SaySomething()
        {
            string lSentence = "";
            switch(Random.Range(0,5))
            {
                case 0:
                    lSentence = mDict.GetString("iAmBored");
                    break;
                case 1:
                    lSentence = mDict.GetString("playWithMe");
                    break;
                case 2:
                    lSentence = mDict.GetString("iWantCuddle");
                    break;
                case 3:
                    lSentence = mDict.GetString("iWantToTalk");
                    break;
                case 4:
                    lSentence = mDict.GetString("nicePlace");
                    break;
            }
            mTTS.Say(lSentence, true);
            mTTSTime = Time.time;
            mRandomSpeechTime = Random.Range(40F, 80F);
        }
    }
}