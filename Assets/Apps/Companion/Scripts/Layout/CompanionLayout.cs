using Buddy;
using Buddy.UI;
using UnityEngine;

namespace BuddyApp.Companion
{
    public class CompanionLayout : AWindowLayout
    {
		private Gauge mMovingDesire;
		private Gauge mInteractDesire;
		private OnOff mWheelsMotion;
		private OnOff mHeadMotion;
		private OnOff mTrigger;
		private OnOff mTriggerWander;
		//private Dropdown mState;
		private Animator mAnimator;

		public override void Update()
		{
			mMovingDesire.Slider.value = CompanionData.Instance.MovingDesire;
			mInteractDesire.Slider.value = CompanionData.Instance.InteractDesire;
		}

		public override void Build()
        {
			/*
             * Create needed widgets
             * ==> Which widget do I need for my app settings ?
             */
			mMovingDesire = CreateWidget<Gauge>();
			mInteractDesire = CreateWidget<Gauge>();
			mWheelsMotion = CreateWidget<OnOff>();
			mHeadMotion = CreateWidget<OnOff>();
			mTrigger = CreateWidget<OnOff>();
			mTriggerWander = CreateWidget<OnOff>();
			//mState = CreateWidget<Dropdown>();

			/*
             * Set widgets parameters
             */
			mMovingDesire.Slider.minValue = 0;
			mMovingDesire.Slider.maxValue = 100;
			mMovingDesire.Slider.wholeNumbers = true;
			mMovingDesire.DisplayPercentage = true; /* Only the display will be in percentage, the value will still be within 0 and 10 */

			mInteractDesire.Slider.minValue = 0;
			mInteractDesire.Slider.maxValue = 100;
			mInteractDesire.Slider.wholeNumbers = true;
			mInteractDesire.DisplayPercentage = true; /* Only the display will be in percentage, the value will still be within 0 and 10 */
			

			/*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */
			mWheelsMotion.IsActive = CompanionData.Instance.CanMoveBody;
			mHeadMotion.IsActive = CompanionData.Instance.CanMoveHead;
			mTrigger.IsActive = CompanionData.Instance.CanTrigger;
			mTriggerWander.IsActive = CompanionData.Instance.CanTriggerWander;
			mMovingDesire.Slider.value = CompanionData.Instance.MovingDesire;
			mInteractDesire.Slider.value = CompanionData.Instance.InteractDesire;

			//mState.AddOption("IDLE");
			//mState.AddOption("WANDER");
			//mState.AddOption("USER DETECTED");
			//mState.AddOption("CRAZY BUDDY");
			//mState.AddOption("IN ARMS");
			//mState.AddOption("USER DISENGAGED");
			//mState.AddOption("TOUCHED ROBOT");
			//mState.AddOption("VOCAL TRIGGERED");
			//mState.AddOption("FOLLOW");
			//mState.AddOption("PROPOSE GAME");
			//mState.AddOption("ASK CHARGE");
			//mState.AddOption("CHARGING");
			//mState.AddOption("LOOKING FOR SOMEONE");
			//mState.AddOption("SAD BUDDY");

			//mState.SetDefault("STATE");


			/*
            * Set command to widgets
            * At each interaction with a widget, a callback will be called
            * ==> What must happen when I interacted with a widget ?
            */

			//mState.OnSelectEvent((string iLabel, object iAttachedObj, int iIndex) => {
				
   //         });

			mMovingDesire.OnUpdateEvent((iVal) => {
				CompanionData.Instance.MovingDesire = iVal;
			});


			mInteractDesire.OnUpdateEvent((iVal) => {
				CompanionData.Instance.InteractDesire = iVal;
			});

			mWheelsMotion.OnSwitchEvent((iVal) => {
				CompanionData.Instance.CanMoveBody = iVal;
				BYOS.Instance.Primitive.Motors.Wheels.Locked = !iVal;
			});

			mHeadMotion.OnSwitchEvent((iVal) => {
				CompanionData.Instance.CanMoveHead = iVal;
				BYOS.Instance.Primitive.Motors.YesHinge.Locked = !iVal;
				BYOS.Instance.Primitive.Motors.NoHinge.Locked = !iVal;
			});

			mTrigger.OnSwitchEvent((iVal) => {
				CompanionData.Instance.CanTrigger = iVal;
			});

			mTriggerWander.OnSwitchEvent((iVal) => {
				CompanionData.Instance.CanTriggerWander = iVal;
			});

		}

        public override void LabelizeWidgets()
        {
			mMovingDesire.Label = BYOS.Instance.Dictionary.GetString("wanderdesire");
			mInteractDesire.Label = BYOS.Instance.Dictionary.GetString("interactdesire");
			mTrigger.Label = BYOS.Instance.Dictionary.GetString("cantrigger");
			mTriggerWander.Label = BYOS.Instance.Dictionary.GetString("cantrigger") + " wander";

			mWheelsMotion.Label = BYOS.Instance.Dictionary.GetString("wheelsmotion");
			mHeadMotion.Label = BYOS.Instance.Dictionary.GetString("headmotion");
		}
    }
}