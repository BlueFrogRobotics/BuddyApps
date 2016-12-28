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
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            if (mAnswer!=Answer.NONE && !mHasClosed)
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
        }

        private void Yes()
        {
            mAnswer = Answer.YES;
        }

        private void No()
        {
            mAnswer = Answer.NO;
            mFaceReco.Train();
        }
    }
}