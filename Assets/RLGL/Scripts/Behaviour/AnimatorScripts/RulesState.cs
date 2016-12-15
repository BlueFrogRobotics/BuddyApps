﻿using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;
namespace BuddyApp.RLGL
{
    public class RulesState : AStateMachineBehaviour
    {
        private bool mIsSentenceDone;
        private bool mIsQuestionDone;

        private bool mIsAnswerRuleYes;
        public bool IsAnswerRuleYes { get { return mIsAnswerRuleYes; } set { mIsAnswerRuleYes = value; } }

        private bool mIsAnswerRuleNo;
        public bool IsAnswerRuleNo { get { return mIsAnswerRuleNo; } set { mIsAnswerRuleNo = value; } }
        
        private float mTimer;

        private GameObject mWindowQuestionRule;
        private GameObject mBackground;

        private bool mCanvasTrigger;

        public override void Init()
        {
            
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetComponent<RLGLBehaviour>().Index = 1;
            mTimer = 0.0F;
            mIsSentenceDone = false;
            mIsQuestionDone = false;
            mIsAnswerRuleYes = false;
            mIsAnswerRuleNo = false;
            mCanvasTrigger = false;
            mWindowQuestionRule = GetGameObject(3);
            mBackground = GetGameObject(1);
            //mWindowQuestionRule.SetActive(false);
            mBackground.GetComponent<Animator>().ResetTrigger("Close_BG");
            mWindowQuestionRule.GetComponent<Animator>().ResetTrigger("Close_WQuestion");
            Debug.Log("RULES STATE : ON ENTER");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("RULES STATE : ON UPDATE");
            mTimer += Time.deltaTime;
            if (!mIsSentenceDone)
                StartCoroutine(SayRulesAndExit());   
            if(mTTS.HasFinishedTalking() && mIsSentenceDone && !mIsQuestionDone )
            {
                StartCoroutine(Restart());
            }

            if (mTTS.HasFinishedTalking() && mIsSentenceDone && mIsQuestionDone)
            {
                OpenCanvas();
                //mBackground.GetComponent<Animator>().ResetTrigger("Close_BG");
                if (mTimer > 5.0F)
                {
                    if ((!mIsAnswerRuleYes || !mIsAnswerRuleNo) && mSTT.HasFinished)
                    {
                        mTimer = 0.0F;
                        GetGameObject(2).GetComponent<RLGLListener>().STTRequest(1);
                    }
                }

            }

            if (mIsAnswerRuleNo)
            {
                iAnimator.GetBehaviour<CountState>().IsOneTurnDone = false;
                iAnimator.SetBool("IsRulesDone", true);

            }
            if (mIsAnswerRuleYes)
            {
                iAnimator.Play("RulesState", 0, 0.0F);
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsRulesDone", false);
            mBackground.GetComponent<Animator>().SetTrigger("Close_BG");
            mWindowQuestionRule.GetComponent<Animator>().SetTrigger("Close_WQuestion");
            //mBackground.GetComponent<Animator>().ResetTrigger("Close_BG");
            Debug.Log("RULES STATE : ON EXIT");
        }

        IEnumerator SayRulesAndExit()
        {
            yield return new WaitForSeconds(2.0F);
            if(!mIsSentenceDone)
            {
                mTTS.Say("Okay, I will explain the game for you my friend. Decreases by about fifteen feets and" +
                    " sometimes I will say green light and your goal is to touch my face " +
                        " before I say red light.");
                mIsSentenceDone = true;
            }
        }

        IEnumerator Restart()
        {
            mTTS.Say("Do you want me to repeat the rules?");
            mIsQuestionDone = true;
            yield return new WaitForSeconds(3.0F);
        }

        private void OpenCanvas()
        {
            if (!mCanvasTrigger)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Open_BG");
                mWindowQuestionRule.GetComponent<Animator>().SetTrigger("Open_WQuestion");
                mCanvasTrigger = true;
            }


        }
    }
}