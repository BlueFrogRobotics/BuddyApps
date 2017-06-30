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

        public override void Start()
        {
            
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMusicPlayer = GetComponent<MusicPlayer>();
            mMusicPlayer.Play();
            mTime = Time.time;
            mRandomStopDelay = 0;
            mEnd = false;
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            float lTime = Time.time;
            if (mRandomStopDelay==0)
                mRandomStopDelay = Random.Range(5, 10);

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
            ResetTrigger("Detection");
            ResetTrigger("Win");
        }
    }
}