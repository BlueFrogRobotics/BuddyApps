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

        private const float MIN_DIST_IR = 0.4f;
        private const float MIN_DIST_US = 0.5f;

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
        private USSensors mUSSensors;
        private Mood mMood;
        private NoHinge mNoHinge;
        private TextToSpeech mTTS;
        private Wheels mWheels;
        private YesHinge mYesHinge;

        void Start()
        {
            mHeadPos = -5F;
            mDict = BYOS.Instance.Dictionary;
            mFace = BYOS.Instance.Face;
            mIRSensors = BYOS.Instance.IRSensors;
            mMood = BYOS.Instance.Mood;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mTTS = BYOS.Instance.TextToSpeech;
            mUSSensors = BYOS.Instance.USSensors;
            mWheels = BYOS.Instance.Motors.Wheels;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
            mInitialized = true;
        }

        void OnEnable()
        {
            if (!mInitialized)
                Start();

            mHeadSearchPlaying = false;
            mChangingDirection = false;
            mIsSearchingPoint = true;
            mUpdateTime = Time.time;
            mWanderTime = Time.time;
            mTTSTime = Time.time;
            mEmoteTime = Time.time;
            mRandomSpeechTime = Random.Range(20F, 40F);
            mRandomWanderTime = Random.Range(10F, 30F);
            mMood.Set(MoodType.NEUTRAL);
            mYesHinge.SetPosition(mHeadPos);
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

            if (mIsSearchingPoint && Time.time - mWanderTime < mRandomWanderTime) {
                PlaySearchingHeadAnimation();
                if (!AnyObstructionsInfrared())
                    mWheels.SetWheelsSpeed(200F, 200F);
                else
                    FaceRandomDirection();

                if (Time.time - mEmoteTime > 5F) {
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
                        case 3:
                            StartCoroutine(LoveFaceCo());
                            break;
                        case 4:
                            StartCoroutine(HappyFaceCo());
                            break;
                        case 5:
                            StartCoroutine(AngryFaceCo());
                            break;
                    }
                    mEmoteTime = Time.time;
                }
            } else {
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
            //GetComponent<Reaction>().ActionFinished();
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
            while (mHeadSearchPlaying) {
                switch (Random.Range(0, 2)) {
                    case 0:
                        TurnHeadNo();
                        break;
                    case 1:
                        TurnHeadYes();
                        break;
                }
                yield return new WaitForSeconds(1.3F);
            }
        }

        private void TurnHeadNo()
        {
            float lHeadNo = Random.Range(10F, 30F);

            if (mNoHinge.CurrentAnglePosition > 0F)
                lHeadNo = -lHeadNo;

            mNoHinge.SetPosition(lHeadNo);
        }

        private void TurnHeadYes()
        {
            float lHeadYes = Random.Range(mHeadPos - 10F, mHeadPos + 20F);
            mYesHinge.SetPosition(lHeadYes);
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
            if (!BYOS.Instance.VocalActivation.RecognitionTriggered) {
                switch (Random.Range(0, 6)) {
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
            }

            switch (Random.Range(0, 5)) {
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
            mRandomWanderTime = Random.Range(10F, 30F);
            mWanderTime = Time.time;
            mEmoteTime = Time.time;
            mChangingDirection = false;
        }

        private bool AnyObstructionsInfrared()
        {
            float lLeftIRDistance = mIRSensors.Left.Distance;
            float lMiddleIRDistance = mIRSensors.Middle.Distance;
            float lRightIRDistance = mIRSensors.Right.Distance;
            float lRightUSDistance = mUSSensors.Right.Distance;
            float lLeftUSDistance = mUSSensors.Left.Distance;
            return IsCollisionEminent(lLeftIRDistance, MIN_DIST_IR)
                || IsCollisionEminent(lMiddleIRDistance, MIN_DIST_IR)
                || IsCollisionEminent(lRightIRDistance, MIN_DIST_IR);
            //|| IsCollisionEminent(lRightUSDistance, MIN_DIST_US)
            //|| IsCollisionEminent(lLeftUSDistance, MIN_DIST_US);
        }

        private bool IsCollisionEminent(float iCollisionDistance, float iThreshold = MIN_DIST_IR)
        {
            return iCollisionDistance != 0.0F && iCollisionDistance < iThreshold;
        }

        private void FaceRandomDirection()
        {
            float lRandomAngle = Random.Range(60F, 300F);
            if (lRandomAngle > 180F)
                lRandomAngle = 360F - lRandomAngle;
            mWheels.TurnAngle(lRandomAngle, 130F, 0.02F);
        }

        private void SaySomething()
        {
            string lSentence = "";
            switch (Random.Range(0, 5)) {
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
            mRandomSpeechTime = Random.Range(20F, 40F);
        }

        private IEnumerator AngryFaceCo()
        {
            mMood.Set(MoodType.ANGRY);
            yield return new WaitForSeconds(1.0F);
            mMood.Set(MoodType.NEUTRAL);
        }

        private IEnumerator HappyFaceCo()
        {
            mMood.Set(MoodType.HAPPY);
            yield return new WaitForSeconds(1.0F);
            mMood.Set(MoodType.NEUTRAL);
        }

        private IEnumerator LoveFaceCo()
        {
            mMood.Set(MoodType.LOVE);
            yield return new WaitForSeconds(1.0F);
            mMood.Set(MoodType.NEUTRAL);
        }
    }
}