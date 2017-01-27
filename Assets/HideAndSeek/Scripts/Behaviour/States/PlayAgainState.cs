using UnityEngine;
using BuddyOS.App;
using System.Collections;

namespace BuddyApp.HideAndSeek
{
    public class PlayAgainState : AStateMachineBehaviour
    {
        private enum Answer : int
        {
            NONE,
            YES,
            NO
        }

        private QuestionPlayAgainWindow mQuestionWindow;
        private BackgroundBlackWindow mBackgroundBlackWindow;
        private Players mPlayers;
        private WindowLinker mWindowLinker;
        private bool mHasClosed = false;
        private bool mIsListening = false;
        private Answer mAnswer;

        public override void Init()
        {
            mWindowLinker = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponent<WindowLinker>();
            mQuestionWindow = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<QuestionPlayAgainWindow>();
            mBackgroundBlackWindow = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<BackgroundBlackWindow>();
            mPlayers = GetComponent<Players>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindowLinker.SetAppWhite();
            mBackgroundBlackWindow.Open();
            mQuestionWindow.Open();
            mQuestionWindow.ButtonYes.onClick.AddListener(Yes);
            mQuestionWindow.ButtonNo.onClick.AddListener(No);
            mAnswer = Answer.NONE;
            mHasClosed = false;
            mIsListening = false;
            mSTT.OnBestRecognition.Add(VocalProcessing);
            mSTT.OnBeginning.Add(StartListening);
            mSTT.OnEnd.Add(StopListening);
            mTTS.Say(mDictionary.GetString("playAgain"));
            mMood.Set(MoodType.NEUTRAL);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mAnswer == Answer.NONE && mTTS.HasFinishedTalking)
            {
                //Debug.Log("dans if");
                if (!mIsListening)
                {
                    //mVocalActivation.StartInstantReco();
                    mSTT.Request();
                }
            }
            else if (mAnswer != Answer.NONE && !mHasClosed)
            {
                mQuestionWindow.Close();
                //if (mAnswer == Answer.NO)
                mBackgroundBlackWindow.Close();
                
                mHasClosed = true;
            }
            else if (mHasClosed && mQuestionWindow.IsOff())
            {
                mWindowLinker.SetAppBlack();
                mSTT.OnBestRecognition.Remove(VocalProcessing);
                mSTT.OnBeginning.Remove(StartListening);
                mSTT.OnEnd.Remove(StopListening);

                if (mAnswer == Answer.YES)
                {
                    mPlayers.ResetGame();
                    iAnimator.SetTrigger("PlayAgain");
                    mYesHinge.SetPosition(20);
                }
                else
                    iAnimator.SetTrigger("ChangeState");
            }

        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mQuestionWindow.ButtonYes.onClick.RemoveAllListeners();
            mQuestionWindow.ButtonNo.onClick.RemoveAllListeners();
            iAnimator.ResetTrigger("ChangeState");
            iAnimator.ResetTrigger("PlayAgain");
            
        }

        private void Yes()
        {
            if (mAnswer == Answer.NONE)
                mAnswer = Answer.YES;
        }

        private void No()
        {
            if (mAnswer == Answer.NONE)
            {
                mAnswer = Answer.NO;

            }
        }

        private void VocalProcessing(string iRequest)
        {
            string lRequest = iRequest.ToLower();
            if (lRequest.Contains(mDictionary.GetString("yes")))
            {
                Yes();
            }
            else if (lRequest.Contains(mDictionary.GetString("no")))
            {
                No();
            }
        }

        private void StartListening()
        {
            mIsListening = true;
        }

        private void StopListening()
        {
            mIsListening = false;
        }
    }
}