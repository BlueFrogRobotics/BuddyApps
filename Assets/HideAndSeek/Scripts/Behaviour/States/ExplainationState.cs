using UnityEngine;
using BuddyOS.App;
using System.Collections;

namespace BuddyApp.HideAndSeek
{
    public class ExplainationState : AStateMachineBehaviour
    {
        private ExplainationWindow mExplainWindow;
        private BackgroundBlackWindow mBackgroundBlackWindow;
        private float mTimer = 0.0f;
        private bool mTimerIsEnded = false;
        private bool mHasClosed = false;

        public override void Init()
        {
            mExplainWindow = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<ExplainationWindow>();
            mBackgroundBlackWindow= GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<BackgroundBlackWindow>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mExplainWindow.Open();
            mBackgroundBlackWindow.Open();
            mTimer = 0.0f;
            mTimerIsEnded = false;
            mHasClosed = false;
            mTTS.Say("Dans la prochaine étape, tourne ta tête de gauche à droite pour m'aider à te reconnaitre");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;

            if (mTimer>3.0f && !mTimerIsEnded && mTTS.HasFinishedTalking())
            {
                mTimerIsEnded = true;
                
            }
            else if(mTimerIsEnded && !mHasClosed)
            {
                mExplainWindow.Close();
                mHasClosed = true;
            }
            else if(mHasClosed && mExplainWindow.IsOff())
                iAnimator.SetTrigger("ChangeState");

        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.ResetTrigger("ChangeState");
        }
    }
}