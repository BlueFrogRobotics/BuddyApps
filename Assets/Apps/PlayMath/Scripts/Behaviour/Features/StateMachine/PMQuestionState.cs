using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.PlayMath{
    public class PMQuestionState : AnimatorSyncState {

		private Animator mQuestionAnimator;
        private QuestionBehaviour mQuestionBehaviour;
        private Scrollbar mTimeScrollbar;

        private bool mIsOpen;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mQuestionAnimator = GameObject.Find("UI/Four_Answer").GetComponent<Animator>();
            mQuestionBehaviour = GameObject.Find("UI/Four_Answer").GetComponent<QuestionBehaviour>();

            mPreviousStateBehaviours.Add(GameObject.Find("UI/Set_Table").GetComponent<SelectTableBehaviour>());
            mPreviousStateBehaviours.Add(GameObject.Find("UI/Menu").GetComponent<MainMenuBehaviour>());
            mPreviousStateBehaviours.Add(GameObject.Find("UI/Settings").GetComponent<SettingsBehaviour>());

            mIsOpen = false;

            mTimeScrollbar = GameObject.Find("UI/Four_Answer/Bottom_UI/Time_Progress").GetComponent<Scrollbar>();
            mTimeScrollbar.size = 0;
            mTimeScrollbar.value = 0;
            // Disable user interaction on scrollbar (mainly drag)
            mTimeScrollbar.interactable = false;

            mQuestionBehaviour.InitState();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (PreviousBehaviourHasEnded() && !mIsOpen)
            {
                mQuestionAnimator.SetTrigger("open");
                mIsOpen = true;

                mQuestionBehaviour.AskNextQuestion(); 
            }
            else if (mIsOpen)
            {
                double lElapsedTime = mQuestionBehaviour.ElapsedTimeSinceStart();
                int lMaxTime = User.Instance.GameParameters.Timer;

                if (!mQuestionBehaviour.HasAnswer)
                {
                    if (lElapsedTime < lMaxTime)
                        mTimeScrollbar.size = (float)(lElapsedTime / lMaxTime);
                    else //TimeOut
                       mQuestionBehaviour.TimeOut();
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mQuestionAnimator.SetTrigger("close");
            mIsOpen = false;
        }
    }
}
