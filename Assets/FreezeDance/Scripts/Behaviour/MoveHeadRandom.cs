﻿using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.FreezeDance
{
    public class MoveHeadRandom : MonoBehaviour
    {
        private Motors mMotors;
        private float mTime;
        private float mDelayBeforeMove;
        private float mRandomStop;
        private bool mIsRandomStopSet;

        // Use this for initialization
        void Awake()
        {
            mMotors = BYOS.Instance.Motors;
        }

        void Start()
        {
            mTime = Time.time;
            mIsRandomStopSet = false;
            mMotors.YesHinge.SetPosition(0);
            mMotors.NoHinge.SetPosition(0);
        }

        // Update is called once per frame
        void Update()
        {
            float lTime = Time.time;
            mDelayBeforeMove = lTime - mTime;
            if (!mIsRandomStopSet) {
                mRandomStop = Random.Range(1f, 3f);
                mIsRandomStopSet = true;
            }

            int lYesOrNo = Random.Range(1, 2);

            if (mDelayBeforeMove > mRandomStop && lYesOrNo == 1) {
                mMotors.YesHinge.SetPosition(Random.Range(-20, 20));
                mIsRandomStopSet = false;
                mTime = Time.time;
            }

            if (mDelayBeforeMove > mRandomStop && lYesOrNo == 2) {
                mMotors.YesHinge.SetPosition(Random.Range(-70, 70));
                mIsRandomStopSet = false;
                mTime = Time.time;
            }
        }

        void OnEnable()
        {
            mTime = Time.time;
            mIsRandomStopSet = false;
        }

        void OnDisable()
        {
            mMotors.YesHinge.SetPosition(0);
            mMotors.NoHinge.SetPosition(0);
        }
    }
}