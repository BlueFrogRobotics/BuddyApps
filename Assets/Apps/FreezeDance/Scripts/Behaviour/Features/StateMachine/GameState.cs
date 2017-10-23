using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.FreezeDance
{
    public class GameState : AStateMachineBehaviour
    {
        private MusicPlayer mMusicPlayer;
        private float mTime;
        int mRandomStopDelay;
        private bool mEnd = false;
        private FreezeDanceBehaviour mFreezeBehaviour;
        private bool mHasDetected = false;

        public override void Start()
        {
            mFreezeBehaviour = GetComponent<FreezeDanceBehaviour>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMusicPlayer = GetComponent<MusicPlayer>();
            mMusicPlayer.Play();
            mTime = Time.time;
            mRandomStopDelay = 0;
            mEnd = false;
            Interaction.Mood.Set(MoodType.NEUTRAL);
            mHasDetected = false;
            mFreezeBehaviour.OnMovementDetect += OnDetect;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            float lTime = Time.time;
            if (mRandomStopDelay==0)
                mRandomStopDelay = Random.Range(5, 10);
            if(lTime - mTime>3.5f && !mHasDetected)
            {
                Interaction.Mood.Set(MoodType.ANGRY);
            }

            //Debug.Log("!!!!!!time " + (lTime - mTime)+" random: "+ mRandomStopDelay);
            if (!mEnd && lTime - mTime > mRandomStopDelay)
            {
                mEnd = true;
                mMusicPlayer.Pause(Dictionary.GetString("dontmove"));
                Trigger("Detection");
            }

            if (mMusicPlayer.IsStopped())
            {
                Trigger("Win");
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mFreezeBehaviour.OnMovementDetect -= OnDetect;
            Interaction.Mood.Set(MoodType.NEUTRAL);
            ResetTrigger("Detection");
            ResetTrigger("Win");
        }

        private void OnDetect()
        {
            mHasDetected = true;
        }
    }
}