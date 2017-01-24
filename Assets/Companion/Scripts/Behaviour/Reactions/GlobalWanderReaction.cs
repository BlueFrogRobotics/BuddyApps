using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    [RequireComponent(typeof(WanderReaction))]
    [RequireComponent(typeof(FollowPersonReaction))]
    public class GlobalWanderReaction : MonoBehaviour
    {
        private bool mInitialized;
        private bool mIsFollowing;
        private float mActiveTime;
        private float mRandomThermal;
        private float mFollowTime;
        private int[] mBadHotspot;

        private WanderReaction mWanderReaction;
        private FollowPersonReaction mThermalFollowReaction;
        private ThermalDetector mThermalDetector;

        void Start()
        {
            mWanderReaction = GetComponent<WanderReaction>();
            mThermalFollowReaction = GetComponent<FollowPersonReaction>();
            mThermalDetector = GetComponent<ThermalDetector>();
            mBadHotspot = new int[] { -1, -1 };
            mInitialized = true;
        }

        void OnEnable()
        {
            if(!mInitialized)
                Start();

            mIsFollowing = false;
            mActiveTime = Time.time;
            mFollowTime = Time.time;
            mRandomThermal = Random.Range(40, 60);
            mWanderReaction.enabled = true;
            mThermalFollowReaction.enabled = false;
        }
        
        void Update()
        {
            if (!CompanionData.Instance.CanMoveBody)
                enabled = false;

            //After a while, if there is thermal activity, we track it until it disappears
            if (!mIsFollowing && Time.time - mActiveTime > mRandomThermal && mThermalDetector.PositionHotSpot != mBadHotspot) {
                mWanderReaction.enabled = false;
                mThermalFollowReaction.enabled = true;
                mIsFollowing = true;
                mFollowTime = Time.time;
            }
            //We track the heat source until it disappears
            else if (mIsFollowing && Time.time - mFollowTime > 15F)
            {
                mWanderReaction.enabled = true;
                mThermalFollowReaction.enabled = false;
                mRandomThermal = Random.Range(40, 60);
                mIsFollowing = false;
                mActiveTime = Time.time;
            }
        }

        void OnDisable()
        {
            mWanderReaction.enabled = false;
            mThermalFollowReaction.enabled = false;
        }
    }
}