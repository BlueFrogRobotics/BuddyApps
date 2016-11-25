using UnityEngine;
using System.Collections;
using BuddyOS;
using System;
using BuddyFeature.Vision;

namespace BuddyApp.Companion
{
    public class IDLEState : AStateMachineBehaviour
    {
        private FaceCascadeTracker mTracker;

        public override void Init()
        {
            mTracker = GetComponent<FaceCascadeTracker>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mRGBCam.Open();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iSateInfo, int iLayerIndex)
        {
            //Debug.Log(mTracker.TrackedObjects.Count);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mRGBCam.Close();
        }
    }
}