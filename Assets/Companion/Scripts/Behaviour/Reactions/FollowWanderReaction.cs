using UnityEngine;
using BuddyOS;
using System.Collections;

namespace BuddyApp.Companion
{
    [RequireComponent(typeof(WanderReaction))]
    [RequireComponent(typeof(FollowPersonReaction))]
    public class FollowWanderReaction : MonoBehaviour
    {
        //The purpose of this wander style is to prioritize the thermal following.
        //If the robot loses track of a heating source, we switch to normal wander style.
        //When heat is detected once again, we switch to following it

        private bool mInitialized;
        private bool mIsFollowing;
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
            if (!mInitialized)
                Start();

            mIsFollowing = true;
        }
        
        void Update()
        {
            if (!mThermalDetector.ThermalDetected)
                SwitchToNormalWander();
            else
                SwitchToFollowWander();
        }

        void OnDisable()
        {
            mWanderReaction.enabled = false;
            mThermalFollowReaction.enabled = false;
        }

        private void SwitchToNormalWander()
        {
            mThermalFollowReaction.enabled = false;
            mWanderReaction.enabled = true;
        }

        private void SwitchToFollowWander()
        {
            mThermalFollowReaction.enabled = true;
            mWanderReaction.enabled = false;
        }
    }
}