using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;


namespace BuddyApp.Tutorial
{ 
    enum Step : int
    {
        FIRST_STEP = 0,
        SECOND_STEP = 1,
        LAST_STEP = 2,
        DONE = 3
    }

    public class IntroductionState : AStateMachineBehaviour
    {
        private Step mStep = Step.FIRST_STEP;
        private bool mIsFirstSentenceDone;
        private System.Random mRandom;
        private Array mValue;
        private float mTimer;
        private int mNumberOfRandom;
        //private int mRandom;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mNumberOfRandom = 0;
            mTimer = 0F;
            mIsFirstSentenceDone = false;
            mValue =  Enum.GetValues(typeof(Mood));
            mRandom = new System.Random();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //mTimer += Time.deltaTime;
            if(mStep == Step.FIRST_STEP)
            {
                Buddy.Vocal.SayKey("hello", true);
                mStep = Step.SECOND_STEP;
            }
            else if(mStep == Step.SECOND_STEP && !Buddy.Vocal.IsBusy /*&& mTimer > 2F*/)
            {
                mNumberOfRandom++;
                //mTimer = 0F;
                if (!mIsFirstSentenceDone)
                {
                    Buddy.Vocal.SayKey("introseconstep");
                    mIsFirstSentenceDone = true;
                }
                Buddy.Behaviour.SetMood((Mood)mValue.GetValue(mRandom.Next(mValue.Length)));
                if (mNumberOfRandom > 5)
                    mStep = Step.LAST_STEP;
            }
            else if(mStep == Step.LAST_STEP && !Buddy.Vocal.IsBusy)
            {
                Buddy.Vocal.SayKey("introthirdstep");
                //Mettre les BML quand ils fonctionneront dans l'OS
                mStep = Step.DONE;
            }
            else if (mStep == Step.DONE)
                Trigger("MenuTrigger");


        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
    }
}

