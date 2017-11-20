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
        private GameObject mInGame;
        private GameObject mIcon;
        private ScoreManager mScoreManager;
        private float mTimer;

        public override void Start()
        {
            mFreezeBehaviour = GetComponent<FreezeDanceBehaviour>();
            mScoreManager = GetComponent<ScoreManager>();
            mInGame = GetGameObject(0);
            mIcon = GetGameObject(1);
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
            mTimer = 0.0f;
            mFreezeBehaviour.OnMovementDetect += OnDetect;
            mInGame.GetComponent<Animator>().SetTrigger("open");
            mIcon.GetComponent<Animator>().SetTrigger("on");
            Debug.Log("name user: " + DataBase.GetUsers()[0].FirstName);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            float lTime = Time.time;
            if (mRandomStopDelay==0)
                mRandomStopDelay = Random.Range(5, 10);
            if(lTime - mTime>3.5f && !mHasDetected)
            {
                Interaction.Mood.Set(MoodType.ANGRY);
            }


            if (mTimer > 1.0f)
            {
                mScoreManager.LoseLife();// WinLife();
                mTimer = 0.0f;
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
            mInGame.GetComponent<Animator>().ResetTrigger("open");
            mIcon.GetComponent<Animator>().ResetTrigger("on");
        }

        private void OnDetect()
        {
            mHasDetected = true;
        }
    }
}