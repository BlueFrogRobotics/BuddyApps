﻿using UnityEngine;
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
        private const int NUMBER_OF_RANDOM = 5;
        private Step mStep;
        private bool mIsFirstSentenceDone;
        private bool mIsSecondSentenceDone;
        private Random mRandom;
        private Array mValue;
        private float mTimer;
        private int mNumberOfRandom;
        private BehaviourAlgorithm mBehaviourAlgorithm;

        private UnityEngine.UI.Button kikoo;

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
            mIsSecondSentenceDone = false;
            mValue =  Enum.GetValues(typeof(Mood));
            mRandom = new Random();

            mBehaviourAlgorithm = new BehaviourAlgorithm();
            mBehaviourAlgorithm.Instructions.Add(new SetMoodBehaviourInstruction() {
                Mood = Mood.HAPPY,
            });
            mBehaviourAlgorithm.Instructions.Add(new MoveBodyBehaviourInstruction() {
                Distance = 1F,
                Speed = 1F,
                IsBlocking = true
            });
            mBehaviourAlgorithm.Instructions.Add(new MoveBodyBehaviourInstruction()
            {
                Angle = -180F,
                Speed = 120F
            });
            mBehaviourAlgorithm.Instructions.Add(new MoveHeadBehaviourInstruction()
            {
                NoAngle = 30F,
                NoSpeed = 90F
            });
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
                    Buddy.Vocal.SayKey("introstatesecondstep");
                    mIsFirstSentenceDone = true;
                }
                Buddy.Behaviour.SetMood((Mood)mValue.GetValue(mRandom.Next(mValue.Length)));
                if (mNumberOfRandom > NUMBER_OF_RANDOM)
                    mStep = Step.LAST_STEP;
            }
            else if(mStep == Step.LAST_STEP && !Buddy.Vocal.IsBusy)
            {
                if(!mIsSecondSentenceDone)
                {
                    Buddy.Vocal.SayKey("introstatethirdstep");
                    mIsSecondSentenceDone = true;
                }
                Buddy.Behaviour.Interpreter.Run(mBehaviourAlgorithm, OnEndBehaviourAlgorithm);
            }
        }
        
        private void OnEndBehaviourAlgorithm()
        {
            Trigger("MenuTrigger");
        }
    }
}

