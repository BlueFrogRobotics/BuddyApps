using UnityEngine;
using BuddyOS.App;
using System;
using BuddyOS;

namespace BuddyApp.RLGL
{
    public class ReplayState : AStateMachineBehaviour
    {

        private GameObject mWindowQuestion;
        private GameObject mBackground;
        private float mTimer;
        private bool mIsSentenceDone;
        private bool mIsQuestionDone;

        private bool mIsAnswerYes;
        public bool IsAnswerYes { get { return mIsAnswerYes; } set { mIsAnswerYes = value; } }

        private bool mIsAnswerNo;
        public bool IsAnswerNo { get { return mIsAnswerNo; } set { mIsAnswerNo = value; } }

        private bool mCanvasTrigger;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("REPLAY STATE : ON ENTER");
            mTimer = 0.0F;
            mWindowQuestion = GetGameObject(3);
            mBackground = GetGameObject(1);
            mIsQuestionDone = false;
            mIsSentenceDone = false;
            mCanvasTrigger = false; 
            mIsAnswerYes = false;
            mBackground.GetComponent<Animator>().ResetTrigger("Close_BG");
            mWindowQuestion.GetComponent<Animator>().ResetTrigger("Close_WQuestion");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iSateInfo, int iLayerIndex)
        {
            Debug.Log("REPLAY STATE : ON UPDATE");
            mTimer += Time.deltaTime;

            if(!mIsQuestionDone)
            {
                Debug.Log("REPLAY STATE : UPDATE : QUESTION REJOUER");
                mMood.Set(MoodType.THINKING);
                mTTS.Say("Do you want to replay the game with me?");
                mIsQuestionDone = true;

            }

            if(mTTS.HasFinishedTalking() && mIsQuestionDone)
            {
                OpenCanvas();

                //LISTENER
                if (mTimer > 5.0F)
                {
                    if ((!mIsAnswerNo || !mIsAnswerYes) && mSTT.HasFinished)
                    {
                        mTimer = 0.0F;
                        GetGameObject(2).GetComponent<RLGLListener>().STTRequest(0);
                    }
                }
            }
            Debug.Log(mIsAnswerYes + " " + mIsAnswerNo);
            if (mIsAnswerYes && !mIsSentenceDone)
            {
                Debug.Log("REPLAY STATE : UPDATE : HAPPY");
                mMood.Set(MoodType.HAPPY);
                mTTS.Say("Ok it will be fun!");
                mIsSentenceDone = true;
            }

            if (mTTS.HasFinishedTalking() && mIsAnswerYes && mIsSentenceDone)
            {
                Debug.Log("REPLAY STATE : UPDATE : REJOUER TRUE");
                iAnimator.GetBehaviour<CountState>().IsOneTurnDone = false;
                iAnimator.SetBool("IsReplayDone", true);
            }

            if (mIsAnswerNo )
            {
                //quitter
                BYOS.Instance.AppManager.Quit();
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("REPLAY STATE : ON EXIT");
            mBackground.GetComponent<Animator>().SetTrigger("Close_BG");
            mWindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
            iAnimator.SetBool("IsReplayDone", false);
        }

        private void OpenCanvas()
        {
            if (!mCanvasTrigger)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Open_BG");
                mWindowQuestion.GetComponent<Animator>().SetTrigger("Open_WQuestion");
                mCanvasTrigger = true;
            }
        }

    }
}

