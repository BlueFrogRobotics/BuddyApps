using UnityEngine;
using Buddy;
using System;
using Buddy.Command;
using UnityEngine.UI;

namespace BuddyApp.RedLightGreenLight
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

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
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

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iSateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;

            if(!mIsQuestionDone)
            {
                Interaction.Mood.Set(MoodType.THINKING);
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("replayState1"));
                //Interaction.TextToSpeech.Say("Do you want to play again?");
                mIsQuestionDone = true;

            }

            if(Interaction.TextToSpeech.HasFinishedTalking && mIsQuestionDone)
            {
                OpenCanvas();
                //LISTENER
                if (mTimer > 5.0F)
                {
                    if ((!mIsAnswerReplayNo || !mIsAnswerReplayYes) && Interaction.SpeechToText.HasFinished && mNeedListen)
                    {
                        mTimer = 0.0F;
                        GetGameObject(2).GetComponent<RLGLListener>().STTRequest(2);
                        mNeedListen = false;
                    }
                }
            }
            if (mIsAnswerReplayYes && !mIsSentenceDone)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Close");
                //GetGameObject(6).SetActive(true);
                mWindowQuestion.GetComponent<Animator>().SetTrigger("Close");
                Interaction.Mood.Set(MoodType.HAPPY);
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("replayState2"));
                //Interaction.TextToSpeech.Say("oh yeah I love games!");
                mIsSentenceDone = true;
            }

            if (Interaction.TextToSpeech.HasFinishedTalking && mIsAnswerReplayYes && mIsSentenceDone)
            {
                Interaction.Mood.Set(MoodType.NEUTRAL);
                iAnimator.GetBehaviour<CountState>().IsOneTurnDone = false;
                iAnimator.SetBool("IsReplayDone", true);
            }

            if (mIsAnswerReplayNo)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Close");
                mWindowQuestion.GetComponent<Animator>().SetTrigger("Close");
                //new HomeCmd().Execute();
                QuitApp();
                //BYOS.Instance.AppManager.Quit();
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsReplayDone", false);
        }

        private void OpenCanvas()
        {
            if (!mCanvasTrigger)
            {
                mBackground.GetComponent<Animator>().SetTrigger("Open");
                //GetGameObject(6).SetActive(false);
                mWindowQuestion.GetComponent<Animator>().SetTrigger("Open");
                Debug.Log("REPLAY STATE APRES OPENQUESTION");
                //mWindowQuestion.GetComponentInChildren<Text>().text = "DO YOU WANT TO PLAY AGAIN?";
                GetGameObject(7).GetComponent<Text>().text = Dictionary.GetString("replayState3");
                //GetGameObject(7).GetComponent<Text>().text = "DO YOU WANT TO PLAY AGAIN?";
                mCanvasTrigger = true;
            }
        }

    }
}

