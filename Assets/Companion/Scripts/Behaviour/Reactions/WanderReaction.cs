using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;

namespace BuddyApp.Companion
{
    /// <summary>
    /// This wandering style makes Buddy go forward until it detects an obstacle.
    /// Once in a while, Buddy plays an animation, says something and changes mood.
    /// </summary>
    public class WanderReaction : MonoBehaviour
    {
        public float HeadPosition { get { return mHeadPos; } set { mHeadPos = value; } }

        [SerializeField]
        private Emotion mEmotion;

        private const float MIN_DIST_IR = 0.4f;
        private const float MIN_DIST_US = 0.5f;

        private bool mInitialized;
        private bool mIsSearchingPoint;
        private bool mIsFacingRandDirection;
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
            //Init all parameters
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
            mIsFacingRandDirection = false;
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

            //Say something once in a while to attract attention
            if (Time.time - mTTSTime > mRandomSpeechTime)
                SaySomething();

            mUpdateTime = Time.time;

            //Make the actual movement
            if (mIsSearchingPoint && Time.time - mWanderTime < mRandomWanderTime) {
                //Head looks at random direction
                PlaySearchingHeadAnimation();
                //If no obstacle, go forward
                if (!AnyObstructionsInfrared() && !mIsFacingRandDirection)
                    mWheels.SetWheelsSpeed(200F, 200F, 400);
                else
                    FaceRandomDirection();

                //Change mood every now and then
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
                //Do a small animation and change direction
                StopSearchingHeadAnimation();
                mIsFacingRandDirection = false;
                mIsSearchingPoint = false;
                ChangeDirection();
            }
        }

        void OnDisable()
        {
            mHeadSearchPlaying = false;
            mIsSearchingPoint = false;
            mIsFacingRandDirection = false;
            mNoHinge.SetPosition(0F);
            mYesHinge.SetPosition(0F);
            mWheels.SetWheelsSpeed(0F, 0F);
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

        //This makes the head look right and left on random angles
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

        //Change mood, play an animation and change direction
        private IEnumerator ChangeDirectionCo()
        {
            if (!BYOS.Instance.VocalManager.RecognitionTriggered) {
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

            //Use the choregraph to perform some animation
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

            //Change direction
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

        //Detects if there is any obstacle ahead (using IR and/or US)
        private bool AnyObstructionsInfrared()
        {
            float lLeftIRDistance = mIRSensors.Left.Distance;
            float lMiddleIRDistance = mIRSensors.Middle.Distance;
            float lRightIRDistance = mIRSensors.Right.Distance;
            float lRightUSDistance = mUSSensors.Right.Distance;
            float lLeftUSDistance = mUSSensors.Left.Distance;
            return IsCollisionEminent(lLeftIRDistance)
                || IsCollisionEminent(lMiddleIRDistance)
                || IsCollisionEminent(lRightIRDistance);
            //|| IsCollisionEminent(lRightUSDistance, MIN_DIST_US)
            //|| IsCollisionEminent(lLeftUSDistance, MIN_DIST_US);
        }

        private bool IsCollisionEminent(float iCollisionDistance, float iThreshold = MIN_DIST_IR)
        {
            return iCollisionDistance != 0.0F && iCollisionDistance < iThreshold;
        }

        private void FaceRandomDirection()
        {
            if (mIsFacingRandDirection)
                return;

            StartCoroutine(FaceRandomDirectionCo());
            mIsFacingRandDirection = true;
        }

        //Reminder : positive angle makes Buddy turn counter-clockwise, and negative makes him turn clockwise
        //The point here is to detect where we can turn to, to avoid having to turn in too random directions
        private IEnumerator FaceRandomDirectionCo()
        {
            bool lCollisionLeft = IsCollisionEminent(mIRSensors.Left.Distance, 0.6F);
            bool lCollisionMiddle = IsCollisionEminent(mIRSensors.Middle.Distance, 0.6F);
            bool lCollisionRight = IsCollisionEminent(mIRSensors.Right.Distance, 0.6F);

            float lRandomAngle = 0F;
            //Everything is obstructed ahead, better to make a full round rotation
            if (lCollisionLeft && lCollisionMiddle && lCollisionRight) {
                lRandomAngle = Random.Range(110F, 250F);
                if (lRandomAngle > 180F)
                    lRandomAngle = lRandomAngle - 360F;
            }
            //Front and left side are obstructed
            else if (lCollisionLeft && lCollisionMiddle)
                lRandomAngle = Random.Range(-120F, -40F);
            //Only left side is blocked
            else if (lCollisionLeft)
                lRandomAngle = Random.Range(-60F, -30F);
            //Front and right side are obstructed
            else if (lCollisionRight && lCollisionMiddle)
                lRandomAngle = Random.Range(40F, 120F);
            //Only right side is blocked
            else if (lCollisionRight)
                lRandomAngle = Random.Range(30F, 60F);
            //This is just in case we gathered bad IR data
            else {
                lRandomAngle = Random.Range(60F, 300F);
                if (lRandomAngle > 180F)
                    lRandomAngle = lRandomAngle - 360F;
            }

            mWheels.TurnAngle(lRandomAngle, 130F, 0.02F);
            Debug.Log("[TURNING] turning " + lRandomAngle);
            while (mWheels.Status != MovingState.REACHED_GOAL) {
                yield return null;
            }
            Debug.Log("[TURNING] reached goal");

            mIsFacingRandDirection = false;
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