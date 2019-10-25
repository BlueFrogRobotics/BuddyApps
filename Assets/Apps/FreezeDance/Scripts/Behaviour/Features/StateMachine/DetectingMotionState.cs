using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.FreezeDance
{
    public class DetectingMotionState : AStateMachineBehaviour
    {
        private float mTime;
        private float mTimer;
        private bool mLost;
        private FreezeDanceBehaviour mFreezeBehaviour;
        private bool mHasDetected = false;
        private GameObject mIcon;
        private ScoreManager mScoreManager;
        private float mTimerPause;

        public override void Start()
        {
            mFreezeBehaviour = GetComponent<FreezeDanceBehaviour>();
            mIcon = GetGameObject(1);
            mScoreManager = GetComponent<ScoreManager>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimerPause = Random.Range(1.5F, 6.0F);
            mTime = 0.0f;
            mTimer = 0.0f;
            mLost = false;
            mFreezeBehaviour.OnMovementDetect += OnDetect;
            mHasDetected = false;
            mIcon.GetComponent<Animator>().SetTrigger("off");
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTime += Time.deltaTime;
            mTimer += Time.deltaTime;
            //if (!mLost && mTime > 1.1f && mHasDetected)
            //{
            //    Trigger("Lose");
            //    mLost = true;

            //}

            if (mHasDetected && mTimer>0.25f)
            {
                mScoreManager.LoseLife();
                mHasDetected = false;
                mTimer = 0.0f;
            }

            if (!mLost && mTime > mTimerPause) {
                mLost = true;
                Buddy.GUI.Toaster.Hide();
                Trigger("Detection");
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mFreezeBehaviour.OnMovementDetect -= OnDetect;
            ResetTrigger("Detection");
            mIcon.GetComponent<Animator>().ResetTrigger("off");
        }

        private void OnDetect()
        {
            Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
            mHasDetected = true;
        }
    }
}