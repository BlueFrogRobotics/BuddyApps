using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BuddyApp.RLGL
{
    public class StartState : AStateMachineBehaviour
    {
        //private bool mIsRule;
        private GameObject mWindowQuestion;
        private GameObject mBackground;

        //private bool mIsDone;

        private bool mIsAnswerYes;
        public bool IsAnswerYes { get { return mIsAnswerYes; } set { mIsAnswerYes = value; } }
        private bool mIsAnswerNo;
        public bool IsAnswerNo { get { return mIsAnswerNo; } set { mIsAnswerNo = value; } }

        private int mCount;
        private bool mSentenceDone;

        private bool mCanvasTrigger;
        
        private float mTimer;

        private bool mNeedListen;
        public bool NeedListen { get { return mNeedListen; } set { mNeedListen = value; } }

        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(5).GetComponent<RLGLMenu>().enabled = false;
            GetComponent<RLGLBehaviour>().Index = 0;
            mNeedListen = true;
            mTimer = 0.0F;
            mCount = 0;
            mSentenceDone = false;
            mIsAnswerNo = false;
            mIsAnswerYes = false;
            Debug.Log("START STATE : ON ENTER");
            mCanvasTrigger = false;
            mWindowQuestion = GetGameObject(3);
            mBackground = GetGameObject(1);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if (!mSentenceDone)
            {
                StartCoroutine(SayStart());
            }

            if(mTTS.HasFinishedTalking && mSentenceDone)
            {
                OpenCanvas();
                if (mTimer > 5.0F)
                {
                    if ((!mIsAnswerNo || !mIsAnswerYes) && mSTT.HasFinished && mNeedListen)
                    {
                        mTimer = 0.0F;
                        GetGameObject(2).GetComponent<RLGLListener>().STTRequest(0);
                        mNeedListen = false;
                    }
                }
            }

            if (mIsAnswerYes)
            {
                StartRuleState(iAnimator);
            }
            if (mIsAnswerNo)
            {
                StartCountState(iAnimator);
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsStartDoneAndRules", false);
            iAnimator.SetBool("IsStartDoneAndNoRules", false);
            //mWindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
            //mBackground.GetComponent<Animator>().SetTrigger("Close_BG");
            
            //mWindowQuestion.GetComponent<Animator>().ResetTrigger("Close_WQuestion");
            Debug.Log("START STATE : ON EXIT");
        }
        
        private IEnumerator SayStart()
        {
            yield return new WaitForSeconds(2.0F);
            if (mCount < 1)
            {
                mTTS.Say("I am so happy, we will play together but before I need to know if you want to know the rules! So do you want to hear them?");
                mCount++;
            }

            if (mTTS.HasFinishedTalking && mCount > 0)
            {
                mSentenceDone = true;
                yield return null;
            }
        }

        public void StartRuleState(Animator iAnimator)
        {
            mWindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
            mBackground.GetComponent<Animator>().SetTrigger("Close_BG");
            iAnimator.SetBool("IsStartDoneAndRules", true);
        }

        public void StartCountState(Animator iAnimator)
        {
            mWindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
            mBackground.GetComponent<Animator>().SetTrigger("Close_BG");
            iAnimator.SetBool("IsStartDoneAndNoRules", true);
        }

        private void OpenCanvas()
        {
            if (!mCanvasTrigger)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Open_BG");
                mWindowQuestion.GetComponent<Animator>().SetTrigger("Open_WQuestion");
                mWindowQuestion.GetComponentInChildren<Text>().text = "DO YOU WANT TO HEAR THE RULES?";
                mCanvasTrigger = true;
            }
        }
    }
}
