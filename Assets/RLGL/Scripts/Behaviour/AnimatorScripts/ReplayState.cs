using UnityEngine;
using BuddyOS.App;
using System;
using BuddyOS;
using BuddyOS.Command;
using UnityEngine.UI;

namespace BuddyApp.RLGL
{
    public class ReplayState : AStateMachineBehaviour
    {

        private GameObject mWindowQuestion;
        private GameObject mBackground;
        private float mTimer;
        private bool mIsSentenceDone;
        private bool mIsQuestionDone;

        private bool mIsAnswerReplayYes;
        public bool IsAnswerReplayYes { get { return mIsAnswerReplayYes; } set { mIsAnswerReplayYes = value; } }

        private bool mIsAnswerReplayNo;
        public bool IsAnswerReplayNo { get { return mIsAnswerReplayNo; } set { mIsAnswerReplayNo = value; } }

        private bool mCanvasTrigger;

        private bool mNeedListen;
        public bool NeedListen { get { return mNeedListen; } set { mNeedListen = value; } }

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetComponent<RLGLBehaviour>().Index = 2;
            mNeedListen = true;
            mTimer = 0.0F;
            mWindowQuestion = GetGameObject(3);
            mBackground = GetGameObject(1);
            mIsQuestionDone = false;
            mIsSentenceDone = false;
            mCanvasTrigger = false;
            mIsAnswerReplayNo = false;
            mIsAnswerReplayYes = false;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iSateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;

            if(!mIsQuestionDone)
            {
                mMood.Set(MoodType.THINKING);
                mTTS.Say(mDictionary.GetRandomString("replayState1"));
                //mTTS.Say("Do you want to play again?");
                mIsQuestionDone = true;

            }

            if(mTTS.HasFinishedTalking && mIsQuestionDone)
            {
                OpenCanvas();
                //LISTENER
                if (mTimer > 5.0F)
                {
                    if ((!mIsAnswerReplayNo || !mIsAnswerReplayYes) && mSTT.HasFinished && mNeedListen)
                    {
                        mTimer = 0.0F;
                        GetGameObject(2).GetComponent<RLGLListener>().STTRequest(2);
                        mNeedListen = false;
                    }
                }
            }
            if (mIsAnswerReplayYes && !mIsSentenceDone)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Close_BG");
                GetGameObject(6).SetActive(true);
                mWindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
                mMood.Set(MoodType.HAPPY);
                mTTS.Say(mDictionary.GetRandomString("replayState2"));
                //mTTS.Say("oh yeah I love games!");
                mIsSentenceDone = true;
            }

            if (mTTS.HasFinishedTalking && mIsAnswerReplayYes && mIsSentenceDone)
            {
                mMood.Set(MoodType.NEUTRAL);
                iAnimator.GetBehaviour<CountState>().IsOneTurnDone = false;
                iAnimator.SetBool("IsReplayDone", true);
            }

            if (mIsAnswerReplayNo)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Close_BG");
                mWindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
                new HomeCmd().Execute();
                //BYOS.Instance.AppManager.Quit();
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsReplayDone", false);
        }

        private void OpenCanvas()
        {
            if (!mCanvasTrigger)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Open_BG");
                GetGameObject(6).SetActive(false);
                mWindowQuestion.GetComponent<Animator>().SetTrigger("Open_WQuestion");
                //mWindowQuestion.GetComponentInChildren<Text>().text = "DO YOU WANT TO PLAY AGAIN?";
                GetGameObject(7).GetComponent<Text>().text = mDictionary.GetString("replayState3");
                //GetGameObject(7).GetComponent<Text>().text = "DO YOU WANT TO PLAY AGAIN?";
                mCanvasTrigger = true;
            }
        }

    }
}

