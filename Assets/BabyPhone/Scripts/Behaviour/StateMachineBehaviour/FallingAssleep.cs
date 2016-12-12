using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;
using System;

namespace BuddyApp.BabyPhone
{
    public class FallingAssleep : AStateMachineBehaviour
    {
        private AudioSource mSpeaker;

        public override void Init()
        {
            mSpeaker = GetGameObject(2).GetComponent<AudioSource>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            BuddyListen();
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mSpeaker.isPlaying)
                mSpeaker.Stop();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        private void BuddyListen()
        {
            StartCoroutine(SetListenMood());
            if (!mSpeaker.isPlaying)
                mSpeaker.Play();
        }

        private IEnumerator SetListenMood()
        {
            yield return new WaitForSeconds(0.5F);
            mFace.SetExpression(MoodType.LISTENING);
        }

    }
}
