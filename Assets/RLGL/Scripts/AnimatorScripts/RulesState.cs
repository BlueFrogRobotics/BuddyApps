using UnityEngine;
using System.Collections;
using BuddyOS;
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

        private GameObject mWindowQuestionRule;

        public override void Init()
        {
            
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mIsSentenceDone = false;
            mIsQuestionDone = false;
            mIsAnswerRuleYes = false;
            mIsAnswerRuleNo = false;
            mWindowQuestionRule = GetGameObject(4);
            mWindowQuestionRule.SetActive(false);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(!mIsSentenceDone)
                StartCoroutine(SayRulesAndExit());   
            if(mTTS.HasFinishedTalking() && mIsSentenceDone && !mIsQuestionDone )
            {
                StartCoroutine(Restart());
            }
            //mettre sil a finit de parler et deux coroutine done -> request + question
            if(mTTS.HasFinishedTalking() && mIsSentenceDone && mIsQuestionDone)
            {
                if(!mWindowQuestionRule.activeSelf)
                    mWindowQuestionRule.SetActive(true);
                StartCoroutine(RestartAskingRule(1));
                if(mIsAnswerRuleNo)
                {
                    iAnimator.SetBool("IsRulesDone", true);
                }
                else if(mIsAnswerRuleYes)
                {
                    iAnimator.Play("RulesState", 0, 0.0F);
                }
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsRulesDone", false);
            mWindowQuestionRule.SetActive(false);
        }

        IEnumerator SayRulesAndExit()
        {
            yield return new WaitForSeconds(2.0F);
            if(!mIsSentenceDone)
            {
                mTTS.Say("Okay, I will explain the game for you my friend. Decreases by about fifteen feets and sometimes I will say green light and your goal is to touch my face " +
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

        private IEnumerator RestartAskingRule(int iIndex)
        {
            Debug.Log("RULE STATE STTREQUEST");
            GetGameObject(2).GetComponent<RLGLListener>().STTRequest(iIndex);
            while (!mIsAnswerRuleYes || !mIsAnswerRuleNo)
            {
                yield return null;
            }
        }
    }
}
