using UnityEngine;
using System.Collections;
using Buddy;
using System;
using UnityEngine.UI;
namespace BuddyApp.RedLightGreenLight
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

        private bool mNeedListen;
        public bool NeedListen { get { return mNeedListen; } set { mNeedListen = value; } }


        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.Face.SetExpression(MoodType.NEUTRAL);
            GetComponent<RLGLBehaviour>().Index = 1;
            mNeedListen = true;
            mTimer = 0.0F;
            mIsSentenceDone = false;
            mIsQuestionDone = false;
            mIsAnswerRuleYes = false;
            mIsAnswerRuleNo = false;
            mCanvasTrigger = false;
            mWindowQuestionRule = GetGameObject(3);
            mBackground = GetGameObject(1);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        { 
            mTimer += Time.deltaTime;
            if (!mIsSentenceDone)
                StartCoroutine(SayRulesAndExit());   
            if(Interaction.TextToSpeech.HasFinishedTalking && mIsSentenceDone && !mIsQuestionDone)
            {
                StartCoroutine(Restart());
            } 

            if (Interaction.TextToSpeech.HasFinishedTalking && mIsSentenceDone && mIsQuestionDone)
            {
                OpenCanvas();

                if (mTimer > 5.0F)
                {
                    Debug.Log(mNeedListen);
                    if ((!mIsAnswerRuleYes || !mIsAnswerRuleNo) && Interaction.SpeechToText.HasFinished && mNeedListen)
                    {
                        mTimer = 0.0F;
                        GetGameObject(2).GetComponent<RLGLListener>().STTRequest(1);
                        mNeedListen = false;
                    }
                }

            }

            if (mIsAnswerRuleNo)
            {
                Interaction.Face.SetExpression(MoodType.NEUTRAL);
                mBackground.GetComponent<Animator>().SetTrigger("Close");
                mWindowQuestionRule.GetComponent<Animator>().SetTrigger("Close");
                //GetGameObject(6).SetActive(true);
                iAnimator.GetBehaviour<CountState>().IsOneTurnDone = false;
                iAnimator.SetBool("IsRulesDone", true);

            }
            if (mIsAnswerRuleYes)
            {
                Interaction.Face.SetExpression(MoodType.NEUTRAL);
                mBackground.GetComponent<Animator>().SetTrigger("Close");
                mWindowQuestionRule.GetComponent<Animator>().SetTrigger("Close");
                //GetGameObject(6).SetActive(true);
                iAnimator.Play("RulesState", 0, 0.0F);
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsRulesDone", false);
        }

        IEnumerator SayRulesAndExit()
        {
            yield return new WaitForSeconds(2.0F);
            if(!mIsSentenceDone)
            {
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ruleState1"));
                //Interaction.TextToSpeech.Say("Okay, I will explain the game for you my friend. Step back by around sixteen feet and" +
                //    " sometimes I will say green light and your goal is to touch my face " +
                //        " before I say red light.");
                mIsSentenceDone = true;
            }
        }

        IEnumerator Restart()
        {
            Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ruleState2"));
            //Interaction.TextToSpeech.Say("Do you want me to repeat the rules?");
            mIsQuestionDone = true;
            yield return new WaitForSeconds(3.0F);
        }

        private void OpenCanvas()
        {
            if (!mCanvasTrigger)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Open");
                //GetGameObject(6).SetActive(false);
                mWindowQuestionRule.GetComponent<Animator>().SetTrigger("Open");
                Debug.Log("RULES STATE APRES OPENQUESTION");
                GetGameObject(7).GetComponent<Text>().text = Dictionary.GetString("ruleState3");
                //GetGameObject(7).GetComponent<Text>().text = "DO YOU WANT ME TO REPEAT THE RULES ?";
                mCanvasTrigger = true;
            }


        }
    }
}
