using UnityEngine;
using BlueQuark;
using System;

using Random = System.Random;

namespace BuddyApp.Tutorial
{ 
    enum Step : int
    {
        FIRST_STEP = 0,
        SECOND_STEP = 1,
        LAST_STEP = 2,
        DONE = 3
    }

    public sealed class IntroductionState : AStateMachineBehaviour
    {
        private Step mStep;
        private bool mIsFirstSentenceDone;
        private Random mRandom;
        private Array mValue;
        private float mTimer;
        private int mNumberOfRandom;

		public override void Start()
		{
			mStep = Step.FIRST_STEP;
		}

		//private int mRandom;
		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mNumberOfRandom = 0;
            mTimer = 0F;
            mIsFirstSentenceDone = false;
            mValue =  Enum.GetValues(typeof(Mood));
            mRandom = new Random();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mTimer += Time.deltaTime;
            if(mStep == Step.FIRST_STEP)
            {
                Buddy.Vocal.SayKey("introstatehello", true);
                mStep = Step.SECOND_STEP;
            }
            else if(mStep == Step.SECOND_STEP && !Buddy.Vocal.IsBusy /*&& mTimer > 2F*/)
            {
                mNumberOfRandom++;
                //mTimer = 0F;
                if (!mIsFirstSentenceDone)
                {
                    Buddy.Vocal.SayKey("introstateseconstep");
                    mIsFirstSentenceDone = true;
                }
                Buddy.Behaviour.SetMood((Mood)mValue.GetValue(mRandom.Next(mValue.Length)));
                if (mNumberOfRandom > 5)
                    mStep = Step.LAST_STEP;
            }
            else if(mStep == Step.LAST_STEP && !Buddy.Vocal.IsBusy)
            {
                Buddy.Vocal.SayKey("introstatethirdstep");
                //Mettre les BML quand ils fonctionneront dans l'OS
                mStep = Step.DONE;
            }
            else if (mStep == Step.DONE)
                Trigger("MenuTrigger");


        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }
    }
}

