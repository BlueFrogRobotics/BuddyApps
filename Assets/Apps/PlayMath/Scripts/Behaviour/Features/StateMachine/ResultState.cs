﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.PlayMath{
    public class ResultState : AStateMachineBehaviour {

		private const int DURATION = 2; // in sec

		private Animator mBackgroundAnimator;
		private Mood mPreviousMood;

		private float mEndTime = -1;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mBackgroundAnimator = GameObject.Find("UI/Background_Black").GetComponent<Animator>();
			mBackgroundAnimator.SetTrigger("close");

			BYOS.Instance.Interaction.Mood.Set(MoodType.HAPPY);

			mEndTime = Time.time + DURATION;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			if (mEndTime != -1 && mEndTime < Time.time) {
				//if win
				animator.SetTrigger("TakePhoto");
				//else
				// animator.SetTrigger("Score");

				mEndTime = -1;
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
    }
}
