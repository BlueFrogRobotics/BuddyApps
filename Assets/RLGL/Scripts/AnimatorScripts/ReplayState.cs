using UnityEngine;
using BuddyOS.App;
using System;

namespace BuddyApp.RLGL
{
    public class ReplayState : AStateMachineBehaviour
    {

        private GameObject mWindowQuestion;
        private float mTimer;
        private bool mIsSentenceDone;
        private bool mIsQuestionDone;

        private bool mIsAnswerYes;
        public bool IsAnswerYes { get { return mIsAnswerYes; } set { mIsAnswerYes = value; } }


        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("REPLAY STATE : ON ENTER");
            mTimer = 0.0f;
            mWindowQuestion = GetGameObject(6);
            mIsQuestionDone = false;
            mIsSentenceDone = false;
            mIsAnswerYes = false;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iSateInfo, int iLayerIndex)
        {
            Debug.Log("REPLAY STATE : ON UPDATE");
            mTimer += Time.deltaTime;

            if(mTimer < 12.0F && !mIsAnswerYes && !mIsQuestionDone)
            {
                mMood.Set(MoodType.THINKING);
                mTTS.Say("Do you want to replay the game with me?");
                mWindowQuestion.SetActive(true);
                mIsQuestionDone = true;
            }

            if (mTTS.HasFinishedTalking() &&  mTimer < 12.0f && mIsAnswerYes && !mIsSentenceDone && mIsQuestionDone)
            {
                mMood.Set(MoodType.NEUTRAL);
                mTTS.Say("Ok I let you ten second to go back at the start!");
                mIsSentenceDone = true;
            }

            if (mTimer > 22.0f && mTTS.HasFinishedTalking() && mIsAnswerYes)
            {
                iAnimator.SetBool("IsReplayDone", true);
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("REPLAY STATE : ON EXIT");
            mWindowQuestion.SetActive(false);
            iAnimator.SetBool("IsReplayDone", false);
        }

    }
}

