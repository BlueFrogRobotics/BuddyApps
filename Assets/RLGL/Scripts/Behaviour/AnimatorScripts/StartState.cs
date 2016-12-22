using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

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

        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetComponent<RLGLBehaviour>().Index = 0;
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
            if (!mSentenceDone) {
                StartCoroutine(SayStart());
            }

            if (mTTS.HasFinishedTalking && mSentenceDone) {
                OpenCanvas();
                if (mTimer > 5.0F) {
                    if ((!mIsAnswerNo || !mIsAnswerYes) && mSTT.HasFinished) {
                        mTimer = 0.0F;
                        GetGameObject(2).GetComponent<RLGLListener>().STTRequest(0);
                    }
                }

            }

            if (mIsAnswerYes) {
                StartRuleState(iAnimator);
            }
            if (mIsAnswerNo) {
                StartCountState(iAnimator);
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsStartDoneAndRules", false);
            iAnimator.SetBool("IsStartDoneAndNoRules", false);
            mWindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
            mBackground.GetComponent<Animator>().SetTrigger("Close_BG");

            //mWindowQuestion.GetComponent<Animator>().ResetTrigger("Close_WQuestion");
            Debug.Log("START STATE : ON EXIT");
        }

        private IEnumerator SayStart()
        {
            yield return new WaitForSeconds(2.0F);
            if (mCount < 1) {
                mTTS.Say("Hello my friend, we will play together but before I need to know if you want to know the rules! So do you want to listen to the rules?");
                mCount++;
            }

            if (mTTS.HasFinishedTalking && mCount > 0) {
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

        private void OpenCanvas()
        {
            if (!mCanvasTrigger) {
                mBackground.GetComponent<Animator>().SetTrigger("Open_BG");
                mWindowQuestion.GetComponent<Animator>().SetTrigger("Open_WQuestion");
                mCanvasTrigger = true;
            }
        }
    }
}
