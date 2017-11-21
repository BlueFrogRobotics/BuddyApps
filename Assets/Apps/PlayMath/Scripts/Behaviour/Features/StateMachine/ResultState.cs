using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using Buddy.UI;

namespace BuddyApp.PlayMath{
    public class ResultState : AStateMachineBehaviour {

		private const float DURATION = 2f; // in sec
        // add some time to notification message display due to next scene load time
        private const float DURATION_NOT = DURATION + 0.5f;

		private Animator mBackgroundAnimator;
		private Mood mPreviousMood;

        private bool mEndOnce;
        private string mTTSKey;

        private Result mResult;
        private Score mScore;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mBackgroundAnimator = GameObject.Find("UI/Background_Black").GetComponent<Animator>();
			mBackgroundAnimator.SetTrigger("close");

            mResult = GameObject.Find("UI/Four_Answer").GetComponent<Result>();
            if (mResult.isCorrect()) {
                BYOS.Instance.Interaction.Mood.Set(MoodType.HAPPY);
                mTTSKey = "goodanswer";
            }
            else {
                BYOS.Instance.Interaction.Mood.Set(MoodType.SAD);
                mTTSKey = "badanswer";
            }

            mScore = GameObject.Find("UI/EndGame_Score").GetComponent<Score>();
            mScore.AddResult(mResult);

            string lMessage = mResult.Equation + "=" + mResult.CorrectAnswer;
            BYOS.Instance.Notifier.Display<SimpleNot>(DURATION_NOT).With(lMessage);

            mEndOnce = false;
            BYOS.Instance.Interaction.TextToSpeech.SayKey(mTTSKey,true);
            AnnounceResult(lMessage);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (BYOS.Instance.Interaction.TextToSpeech.HasFinishedTalking && !mEndOnce)
            {
                Debug.Log("Has finished talking");
                if (mResult.Last)
                {
                    if (mScore.IsPerfect())
                        animator.SetTrigger("TakePhoto");
                    else
                        animator.SetTrigger("Score");
                }
                else
                    animator.SetTrigger("NextQuestion");
                mEndOnce = true;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mBackgroundAnimator.SetTrigger("open");

			MoodType lLastMood = BYOS.Instance.Interaction.Mood.LastMood;
			BYOS.Instance.Interaction.Mood.Set(lLastMood);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        private void AnnounceResult(string statement)
        {
            if (statement.Contains("÷"))
                statement = statement.Replace("÷", BYOS.Instance.Dictionary.GetString("dividedby"));
            if (statement.Contains("×"))
                statement = statement.Replace("×", BYOS.Instance.Dictionary.GetString("xtimesy"));
            if (statement.Contains("-"))
                statement = statement.Replace("-", BYOS.Instance.Dictionary.GetString("minus"));
            if(statement.Contains("="))
                statement = statement.Replace("=",BYOS.Instance.Dictionary.GetString("equal"));

            BYOS.Instance.Interaction.TextToSpeech.Say(statement,true);
        }
    }
}
