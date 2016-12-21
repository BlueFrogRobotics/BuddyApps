using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;

namespace BuddyApp.Companion
{
    public class IdleReaction : MonoBehaviour
    {
        private float mHeadMoveTime;
        private float mEmoteTime;

        private Face mFace;
        private Mood mMood;
        private NoHinge mNoHinge;
        private YesHinge mYesHinge;

        void Start()
        {
            mFace = BYOS.Instance.Face;
            mMood = BYOS.Instance.Mood;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
        }

        void OnEnable()
        {
            mHeadMoveTime = Time.time;
            mEmoteTime = Time.time;
            mMood.Set(MoodType.NEUTRAL);
        }

        void Update()
        {
            if (Time.time - mHeadMoveTime < 1.5F)
                return;

            mHeadMoveTime = Time.time;

            if (Random.Range(0, 2) == 0)
            {
                float lRandomAngle = Random.Range(-45F, 45F);
                mNoHinge.SetPosition(lRandomAngle);
            }
            else
            {
                float lRandomAngle = Random.Range(-20F, 15F);
                mYesHinge.SetPosition(lRandomAngle);
            }
            
            if (Time.time - mEmoteTime > 15F)
            {
                mFace.SetEvent(FaceEvent.SMILE);
                mEmoteTime = Time.time;
            }
        }

        void OnDisable()
        {
            mNoHinge.SetPosition(0F);
            mYesHinge.SetPosition(0F);
        }
    }
}