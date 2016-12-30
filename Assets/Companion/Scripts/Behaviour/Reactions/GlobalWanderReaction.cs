using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    [RequireComponent(typeof(WanderReaction))]
    [RequireComponent(typeof(FollowPersonReaction))]
    public class GlobalWanderReaction : MonoBehaviour
    {
        private bool mInitialized;
        private float mActiveTime;
        private float mRandomThermal;
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
            if(!mInitialized) {
                Start();
            }

            mActiveTime = Time.time;
            mRandomThermal = Random.Range(50, 90);
            mWanderReaction.enabled = true;
            mThermalFollowReaction.enabled = false;
        }
        
        void Update()
        {
            //After a while, if there is thermal activity, we track it until it disappears
            if (Time.time - mActiveTime > mRandomThermal && mThermalDetector.ThermalDetected) {
                mWanderReaction.enabled = false;
                mThermalFollowReaction.enabled = true;
            }
            //We track the heat source until it disappears
            else if (Time.time - mActiveTime > mRandomThermal && mThermalDetector.PositionHotSpot == mBadHotspot)
            {
                mWanderReaction.enabled = true;
                mThermalFollowReaction.enabled = false;
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