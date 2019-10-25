using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


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
        private bool mFirst = true;

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
            if (!Buddy.Sensors.RGBCamera.IsOpen)
                Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320X240_15FPS_RGB);
            SetBool("ScoreBool", true);
            if (mFreezeBehaviour.ChangeMusic)
            {
                mMusicPlayer.ReinitMusic(Random.Range(0, mMusicPlayer.NbClips));
                mFreezeBehaviour.ChangeMusic = false;
            }

            mMusicPlayer.Play();
            mTime = Time.time;
            mRandomStopDelay = 0;
            mEnd = false;
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            mHasDetected = false;
            mTimer = 0.0f;
            mFreezeBehaviour.OnMovementDetect += OnDetect;
            mInGame.GetComponent<Animator>().SetTrigger("open");
            mIcon.GetComponent<Animator>().SetTrigger("on");
            mHasTalked = false;
            mFinishedState = false;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            LEDColor color = (LEDColor)Random.Range(0, 12);
            Buddy.Actuators.LEDs.SetHeartLight(color);
            Buddy.Actuators.LEDs.SetShouldersLights(color);

            //mHasDetected = true;

            if (!mFinishedState)
            {
                mTimer += Time.deltaTime;
                float lTime = Time.time;
                if (mRandomStopDelay == 0)
                    mRandomStopDelay = Random.Range(4, 8);
                if (lTime - mTime > 3.5f && !mHasDetected && !mHasTalked)
                {
                    //Buddy.Behaviour.SetMood(Mood.ANGRY);
                    Buddy.Vocal.Say(Buddy.Resources.GetRandomString("move"));
                    mHasTalked = true;
                }


                if (mTimer > 0.25f && mHasDetected)
                {
                    mScoreManager.WinLife();
                    mTimer = 0.0f;
                    mHasDetected = false;
                }

                if (mTimer > 0.5f && !mHasDetected)
                {
                    mTimer = 0.0f;
                    mScoreManager.LoseLife();
                }
                //Debug.Log("!!!!!!time " + (lTime - mTime)+" random: "+ mRandomStopDelay);
                if (!mEnd && lTime - mTime > mRandomStopDelay)
                {
                    mEnd = true;
                    mMusicPlayer.Pause(Buddy.Resources.GetString("dontmove"));
                    Trigger("Detection");
                }

                if (mMusicPlayer.IsStopped())
                {
                    mFinishedState = true;
                    //mRanking.AddPlayer((int)mScoreManager.Score);
                    mInGame.GetComponent<Animator>().SetTrigger("close");
                    Trigger("EndGame");
                    if (Buddy.Sensors.RGBCamera.IsOpen)
                        Buddy.Sensors.RGBCamera.Close();
                    mMusicPlayer.Restart();
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Actuators.LEDs.SetHeartLight(LEDColor.BLUE_NEUTRAL);
            Buddy.Actuators.LEDs.SetShouldersLights(LEDColor.BLUE_NEUTRAL);

            mFreezeBehaviour.OnMovementDetect -= OnDetect;
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            ResetTrigger("Detection");
            ResetTrigger("EndGame");
            mInGame.GetComponent<Animator>().ResetTrigger("open");
            mIcon.GetComponent<Animator>().ResetTrigger("on");
        }

        private void OnDetect()
        {
            mHasDetected = true;
        }
    }
}