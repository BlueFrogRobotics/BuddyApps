using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;

namespace BuddyApp.Companion
{
    public class WanderReaction : MonoBehaviour
    {
        private const float MIN_DIST = 0.4f;

        private bool mIsSearchingPoint;
        private bool mHeadSearchPlaying;
        private bool mChangingDirection;
        private float mUpdateTime;
        private float mWanderTime;
        private float mRandomWanderTime;

        private IRSensors mIRSensors;
        private Mood mMood;
        private NoHinge mNoHinge;
        private USSensors mUSSensors;
        private Wheels mWheels;
        private YesHinge mYesHinge;

        void Start()
        {
            mIRSensors = BYOS.Instance.IRSensors;
            mMood = BYOS.Instance.Mood;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mUSSensors = BYOS.Instance.USSensors;
            mWheels = BYOS.Instance.Motors.Wheels;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
        }

        void OnEnable()
        {
            mHeadSearchPlaying = false;
            mChangingDirection = false;
            mIsSearchingPoint = true;
            mUpdateTime = Time.time;
            mWanderTime = Time.time;
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

            mUpdateTime = Time.time;

            if(mIsSearchingPoint && Time.time - mWanderTime < mRandomWanderTime) {
                PlaySearchingHeadAnimation();
                if (!AnyObstructionsInfrared())
                    mWheels.SetWheelsSpeed(200F, 200F, 200);
                else
                    FaceRandomDirection();
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
                float lHeadDirection = Random.Range(-45F, 45F);
                mNoHinge.SetPosition(lHeadDirection);

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
            float lRandomAngle = Random.Range(-45F, 45F);
            mNoHinge.SetPosition(lRandomAngle);

            yield return new WaitForSeconds(1.5F);
            
            mWheels.TurnAngle(lRandomAngle, 100F, 0.02F);
            mNoHinge.SetPosition(0F);

            yield return new WaitForSeconds(1.5F);

            mIsSearchingPoint = true;
            mRandomWanderTime = Random.Range(15F, 30F);
            mWanderTime = Time.time;
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

        private bool IsCollisionEminent(float iCollisionDistance)
        {
            return iCollisionDistance != 0.0F && iCollisionDistance < MIN_DIST;
        }

        private void FaceRandomDirection()
        {
            float lRandomAngle = Random.Range(60F, 300F);
            if (lRandomAngle > 180F)
                lRandomAngle = 360F - lRandomAngle;
            mWheels.TurnAngle(lRandomAngle, 100F, 0.02F);
        }
    }
}