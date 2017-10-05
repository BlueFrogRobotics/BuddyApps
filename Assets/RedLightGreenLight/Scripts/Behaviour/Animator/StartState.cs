﻿using UnityEngine;
using System.Collections;
using Buddy;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BuddyApp.RedLightGreenLight
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

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Primitive.RGBCam.IsOpen)
                Primitive.RGBCam.Close();
            Primitive.RGBCam.Resolution = RGBCamResolution.W_320_H_240;

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

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if (!mSentenceDone)
            {
                StartCoroutine(SayStart());
            }

            if(Interaction.TextToSpeech.HasFinishedTalking && mSentenceDone)
            {
                OpenCanvas();
                if (mTimer > 5.0F)
                {
                    if ((!mIsAnswerNo || !mIsAnswerYes) && Interaction.SpeechToText.HasFinished && mNeedListen)
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

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsStartDoneAndRules", false);
            iAnimator.SetBool("IsStartDoneAndNoRules", false);
        }
        
        private IEnumerator SayStart()
        {
            yield return new WaitForSeconds(1.0F);
            if (mCount < 1)
            {
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("startState1"));
                //Interaction.TextToSpeech.Say("I am so happy, we will play together but before I need to know if you want to hear the rules! So do you want to hear them?");
                mCount++;
            }

            if (Interaction.TextToSpeech.HasFinishedTalking && mCount > 0)
            {
                mSentenceDone = true;
                yield return null;
            }
        }

        public void StartRuleState(Animator iAnimator)
        {
            Interaction.Face.SetExpression(MoodType.NEUTRAL);
            mWindowQuestion.GetComponent<Animator>().SetTrigger("Close");
            mBackground.GetComponent<Animator>().SetTrigger("Close");
            //GetGameObject(6).SetActive(true);
            iAnimator.SetBool("IsStartDoneAndRules", true);
        }

        public void StartCountState(Animator iAnimator)
        {
            Interaction.Face.SetExpression(MoodType.NEUTRAL);
            mWindowQuestion.GetComponent<Animator>().SetTrigger("Close");
            mBackground.GetComponent<Animator>().SetTrigger("Close");
            iAnimator.SetBool("IsStartDoneAndNoRules", true);
        }

        private void OpenCanvas()
        {
            if (!mCanvasTrigger)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Open");
                mWindowQuestion.GetComponent<Animator>().SetTrigger("Open");
                //mWindowQuestion.GetComponentInChildren<Text>().text = "DO YOU WANT TO HEAR THE RULES?";
                GetGameObject(7).GetComponent<Text>().text = Dictionary.GetString("startState2");
                //GetGameObject(7).GetComponent<Text>().text = "DO YOU WANT TO HEAR THE RULES?";
                mCanvasTrigger = true;
            }
        }
    }
}
