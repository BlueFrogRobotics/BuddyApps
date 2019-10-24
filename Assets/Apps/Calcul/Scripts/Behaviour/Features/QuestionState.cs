using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BlueQuark;

namespace BuddyApp.Calcul
{
    public class QuestionState : AStateMachineBehaviour
    {
        private QuestionBehaviour mQuestionBehaviour;
        private Scrollbar mTimeScrollbar;

        private bool mIsQuestionAsked;
        private bool mNextStepActivated;

        public override void Start()
        {
            mQuestionBehaviour = GetComponent<QuestionBehaviour>();
            mTimeScrollbar = GameObject.Find("AppGUI/Time_Progress").GetComponent<Scrollbar>();
            Buddy.GUI.Header.OnClickParameters.Add(() =>
            {
                mQuestionBehaviour.Clear();
                Trigger("Parameters");
            });
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mNextStepActivated = false;

            mIsQuestionAsked = false;

            mTimeScrollbar.gameObject.SetActive(true);
            mTimeScrollbar.size = 0;
            mTimeScrollbar.value = 0;

            mQuestionBehaviour.InitState();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mNextStepActivated)
                return;
            if (!mIsQuestionAsked)
            {
                mIsQuestionAsked = true;

                mQuestionBehaviour.AskNextQuestion();
            }
            else if (mIsQuestionAsked)
            {
                double lElapsedTime = mQuestionBehaviour.ElapsedTimeSinceStart();
                int lMaxTime = User.Instance.GameParameters.Timer;

                if (!mQuestionBehaviour.HasAnswer)
                {
                    if (lElapsedTime < lMaxTime)
                    {
                        mTimeScrollbar.size = (float)(lElapsedTime / lMaxTime);
                    }
                    else
                    {   //TimeOut
                        mQuestionBehaviour.TimeOut();
                        mNextStepActivated = true;
                        Trigger("Result");
                    }
                }
                else
                {
                    mNextStepActivated = true;
                    Trigger("Result");
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Footer.Hide();
            mTimeScrollbar.gameObject.SetActive(false);
            mIsQuestionAsked = false;
        }
    }
}
