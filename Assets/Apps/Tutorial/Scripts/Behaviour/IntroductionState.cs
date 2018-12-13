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
        private const int NUMBER_OF_RANDOM = 10;
        private Step mStep;
        private bool mIsFirstSentenceDone;
        private bool mIsSecondSentenceDone;
        private Random mRandom;
        private Array mValue;
        private float mTimer;
        private int mNumberOfRandom;
        private BehaviourAlgorithm mBehaviourAlgorithm;

        private bool mSequenceLaunched;

        private bool mStartRandom;

		public override void Start()
		{
			mStep = Step.FIRST_STEP;
		}
        
		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartRandom = false;
            mNumberOfRandom = 0;
            mTimer = 0F;
            mIsFirstSentenceDone = false;
            mIsSecondSentenceDone = false;
            mValue =  Enum.GetValues(typeof(Mood));
            mRandom = new Random();

            //Sequence of instructions which can be run after when we want.
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

            if(!mSequenceLaunched && !Buddy.Vocal.IsBusy)
            {
                Buddy.Vocal.SayKey("introstatehello", iInput => OnSecondAction());
            }
            if(mStartRandom)
            {
                mNumberOfRandom++;
                Buddy.Behaviour.SetMood((Mood)mValue.GetValue(mRandom.Next(mValue.Length)));
                if (mNumberOfRandom > NUMBER_OF_RANDOM)
                    mStartRandom = false;
            }
        }
        
        private void OnEndBehaviourAlgorithm()
        {
            Trigger("MenuTrigger");
        }

        private void OnSecondAction()
        {
            mSequenceLaunched = true;
            Buddy.Vocal.SayKey("introstatesecondstep", iInput => OnThirdAction());
        }

        private void OnThirdAction()
        {
            if(mNumberOfRandom > NUMBER_OF_RANDOM)
            {
                Buddy.Vocal.SayKey("introstatethirdstep", iInput => OnLastAction());
            }
        }

        private void OnLastAction()
        {
            Buddy.Behaviour.Interpreter.Run(mBehaviourAlgorithm, OnEndBehaviourAlgorithm);
        }
    }
}

