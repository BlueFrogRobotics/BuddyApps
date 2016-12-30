using UnityEngine;
using BuddyOS.App;
using System.Collections;

namespace BuddyApp.HideAndSeek
{
    public class QuestionAddPlayerState : AStateMachineBehaviour
    {
        private enum Answer : int
        {
            NONE,
            YES,
            NO
        }

        private QuestionAddPlayerWindow mQuestionWindow;
        private BackgroundBlackWindow mBackgroundBlackWindow;
        private FaceRecognition mFaceReco;
        private bool mHasClosed = false;
        private bool mIsListening = false;
        private Answer mAnswer;

        public override void Init()
        {
            mQuestionWindow = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<QuestionAddPlayerWindow>();
            mFaceReco = GetGameObject((int)HideAndSeekData.ObjectsLinked.FACE_RECO).GetComponent<FaceRecognition>();
            mBackgroundBlackWindow = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<BackgroundBlackWindow>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponent<WindowLinker>().SetAppWhite();
            mQuestionWindow.Open();
            mQuestionWindow.ButtonYes.onClick.AddListener(Yes);
            mQuestionWindow.ButtonNo.onClick.AddListener(No);
            mAnswer = Answer.NONE;
            mHasClosed = false;
            mIsListening = false;
            mSTT.OnBestRecognition.Add(VocalProcessing);
            mSTT.OnBeginning.Add(StartListening);
            mSTT.OnEnd.Add(StopListening);
            mTTS.Say(mDictionary.GetString("askAddPlayer"));
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
            else if (mAnswer!=Answer.NONE && !mHasClosed)
            {
                mQuestionWindow.Close();
                if (mAnswer == Answer.NO)
                    mBackgroundBlackWindow.Close();
                mHasClosed = true;
            }
            else if (mHasClosed && mQuestionWindow.IsOff())
            {
                if(mAnswer==Answer.YES)
                    iAnimator.SetTrigger("AddPlayer");
                else
                    iAnimator.SetTrigger("ChangeState");
            }
                
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mQuestionWindow.ButtonYes.onClick.RemoveAllListeners();
            mQuestionWindow.ButtonNo.onClick.RemoveAllListeners();
            iAnimator.ResetTrigger("ChangeState");
            iAnimator.ResetTrigger("AddPlayer");
            mSTT.OnBestRecognition.Remove(VocalProcessing);
            mSTT.OnBeginning.Remove(StartListening);
            mSTT.OnEnd.Remove(StopListening);
        }

        private void Yes()
        {
            if(mAnswer==Answer.NONE)
                mAnswer = Answer.YES;
        }

        private void No()
        {
            if (mAnswer == Answer.NONE)
            {
                mAnswer = Answer.NO;
                mFaceReco.Train();
            }
        }

        private void VocalProcessing(string iRequest)
        {
            string lRequest = iRequest.ToLower();
            if (lRequest.Contains(mDictionary.GetString("yes")) )
            {
                Yes();
            }
            else if(lRequest.Contains(mDictionary.GetString("no")))
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