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
        private SettingsWindow mSettings;
        private float mTime;
        int mRandomStopDelay;
        private bool mEnd = false;
        private FreezeDanceBehaviour mFreezeBehaviour;
        private bool mHasDetected = false;
        private GameObject mInGame;
        private GameObject mIcon;
        private ScoreManager mScoreManager;
        private float mTimer;
        private Ranking mRanking;
        private bool mHasTalked;
        private bool mFinishedState;

        public override void Start()
        {
            mFreezeBehaviour = GetComponent<FreezeDanceBehaviour>();
            mScoreManager = GetComponent<ScoreManager>();
            mRanking = GetComponent<Ranking>();
            mMusicPlayer = GetComponent<MusicPlayer>();
            mSettings = GetComponent<SettingsWindow>();
            mInGame = GetGameObject(0);
            mIcon = GetGameObject(1);
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
            mMusicPlayer.ReinitMusic(mSettings.MusicId);
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
            mHasTalked = false;
            mFinishedState = false;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mFinishedState)
            {
                mTimer += Time.deltaTime;
                float lTime = Time.time;
                if (mRandomStopDelay == 0)
                    mRandomStopDelay = Random.Range(10, 15);
                if (lTime - mTime > 3.5f && !mHasDetected && !mHasTalked)
                {
                    //Interaction.Mood.Set(MoodType.ANGRY);
                    Interaction.TextToSpeech.Say("bouge");
                    mHasTalked = true;
                }


                if (mTimer > 0.25f && mHasDetected)
                {
                    mScoreManager.WinLife();
                    mTimer = 0.0f;
                    mHasDetected = false;
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
                    mFinishedState = true;
                    mRanking.AddPlayer((int)mScoreManager.Score);
                    mInGame.GetComponent<Animator>().SetTrigger("close");
                    Trigger("Ranking");
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mFreezeBehaviour.OnMovementDetect -= OnDetect;
            Interaction.Mood.Set(MoodType.NEUTRAL);
            ResetTrigger("Detection");
            ResetTrigger("Ranking");
            mInGame.GetComponent<Animator>().ResetTrigger("open");
            mIcon.GetComponent<Animator>().ResetTrigger("on");
        }

        private void OnDetect()
        {
            mHasDetected = true;
        }
    }
}