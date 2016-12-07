using UnityEngine;
using System.Collections;
using BuddyOS;
using System;

namespace BuddyApp.RLGL
{
    public class StartState : AStateMachineBehaviour
    {
        //private bool mIsRule;
        private GameObject mWindowQuestion;

        //private bool mIsDone;
        
        private bool mIsAnswerYes;
        public bool IsAnswerYes { get { return mIsAnswerYes; } set { mIsAnswerYes = value; } }
        private bool mIsAnswerNo;
        public bool IsAnswerNo { get { return mIsAnswerNo; } set { mIsAnswerNo = value; } }

        private int mCount;
        private bool mSentenceDone;

        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCount = 0;
            mSentenceDone = false;
            mIsAnswerNo = false;
            mIsAnswerYes = false;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(!mSentenceDone)
            {
                StartCoroutine(SayStart());
            }

            if (mTTS.HasFinishedTalking() && mSentenceDone)
            {
                mWindowQuestion = GetGameObject(3);
                if(!mWindowQuestion.activeSelf)
                    mWindowQuestion.SetActive(true);
                StartCoroutine(StartRules(0));
            }

            if( mIsAnswerYes)
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
            mWindowQuestion.SetActive(false);
        }

        private IEnumerator StartRules(int iIndex)
        {
            Debug.Log("START STATE STTREQUEST");
            GetGameObject(2).GetComponent<RLGLListener>().STTRequest(iIndex);
            while (!mIsAnswerNo || !mIsAnswerYes)
            {
                yield return null;
            }
        }

        private IEnumerator SayStart()
        {
            yield return new WaitForSeconds(2.0F);
            if(mCount < 1)
            {
                mTTS.Say("Hey! Hello my friend, we will play together but before I need to know if you want to know the rules! So do you want to listen to the rules?");
                mCount++;
            }
           
            if(mTTS.HasFinishedTalking() && mCount > 0)
            {
                mSentenceDone = true;
                yield return null;
            }
        }

        public void StartRuleState(Animator iAnimator)
        {
            iAnimator.SetBool("IsStartDoneAndRules", true);
        }

        public void StartCountState(Animator iAnimator)
        {
            iAnimator.SetBool("IsStartDoneAndNoRules", true);
        }

    }
}
