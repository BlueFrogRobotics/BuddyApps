using Buddy;
using Buddy.UI;
using UnityEngine;

namespace BuddyApp.Companion
{
	public class CompanionLayout : AWindowLayout
	{
		private Gauge mMovingDesire;
		private Gauge mInteractDesire;
		private Gauge mTeachDesire;
		private Gauge mLearnDesire;
		private Gauge mHelpDesire;
		private Gauge mHeadAngle;
		private OnOff mWheelsMotion;
		private OnOff mHeadMotion;
		private OnOff mTrigger;
		private OnOff mTriggerWander;
		private OnOff mDebug;
		//private Dropdown mState;

		public UnityEngine.UI.Text mState { get; set; }


		public override void Update()
		{
			mMovingDesire.Slider.value = CompanionData.Instance.mMovingDesire;
			mInteractDesire.Slider.value = CompanionData.Instance.mInteractDesire;
			mTeachDesire.Slider.value = CompanionData.Instance.mTeachDesire;
			mLearnDesire.Slider.value = CompanionData.Instance.mLearnDesire;
			mHelpDesire.Slider.value = CompanionData.Instance.mHelpDesire;
			mHeadAngle.Slider.value = CompanionData.Instance.HeadPosition;
		}

		public override void Build()
		{

			/*
             * Create needed widgets
             * ==> Which widget do I need for my app settings ?
             */

			//if (mState == null) {
			//	mState = (UnityEngine.UI.Text) CompanionActivity.Objects[0];
			//         }


			mMovingDesire = CreateWidget<Gauge>();
			mInteractDesire = CreateWidget<Gauge>();
			mTeachDesire = CreateWidget<Gauge>();
			mLearnDesire = CreateWidget<Gauge>();
			mHelpDesire = CreateWidget<Gauge>();
			mHeadAngle = CreateWidget<Gauge>();
			mWheelsMotion = CreateWidget<OnOff>();
			mHeadMotion = CreateWidget<OnOff>();
			mTrigger = CreateWidget<OnOff>();
			mTriggerWander = CreateWidget<OnOff>();
			mDebug = CreateWidget<OnOff>();
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

			mTeachDesire.Slider.minValue = 0;
			mTeachDesire.Slider.maxValue = 100;
			mTeachDesire.Slider.wholeNumbers = true;
			mTeachDesire.DisplayPercentage = true; /* Only the display will be in percentage, the value will still be within 0 and 10 */

			mLearnDesire.Slider.minValue = 0;
			mLearnDesire.Slider.maxValue = 100;
			mLearnDesire.Slider.wholeNumbers = true;
			mLearnDesire.DisplayPercentage = true; /* Only the display will be in percentage, the value will still be within 0 and 10 */

			mHelpDesire.Slider.minValue = 0;
			mHelpDesire.Slider.maxValue = 100;
			mHelpDesire.Slider.wholeNumbers = true;
			mHelpDesire.DisplayPercentage = true; /* Only the display will be in percentage, the value will still be within 0 and 10 */

			mHeadAngle.Slider.minValue = -30;
			mHeadAngle.Slider.maxValue = 60;
			mHeadAngle.Slider.wholeNumbers = true;
			mHeadAngle.DisplayPercentage = false;

			/*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */
			mWheelsMotion.IsActive = CompanionData.Instance.CanMoveBody;
			mHeadMotion.IsActive = CompanionData.Instance.CanMoveHead;
			mTrigger.IsActive = CompanionData.Instance.CanTrigger;
			mTriggerWander.IsActive = CompanionData.Instance.CanTriggerWander;
			mDebug.IsActive = CompanionData.Instance.Debug;
			mMovingDesire.Slider.value = CompanionData.Instance.mMovingDesire;
			mInteractDesire.Slider.value = CompanionData.Instance.mInteractDesire;
			mLearnDesire.Slider.value = CompanionData.Instance.mLearnDesire;
			mHelpDesire.Slider.value = CompanionData.Instance.mHelpDesire;
			mTeachDesire.Slider.value = CompanionData.Instance.mTeachDesire;
			mHeadAngle.Slider.value = CompanionData.Instance.HeadPosition;

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
				CompanionData.Instance.mMovingDesire = iVal;
			});

			mInteractDesire.OnUpdateEvent((iVal) => {
				CompanionData.Instance.mInteractDesire = iVal;
			});

			mLearnDesire.OnUpdateEvent((iVal) => {
				CompanionData.Instance.mLearnDesire = iVal;
			});

			mHelpDesire.OnUpdateEvent((iVal) => {
				CompanionData.Instance.mHelpDesire = iVal;
			});

			mTeachDesire.OnUpdateEvent((iVal) => {
				CompanionData.Instance.mTeachDesire = iVal;
			});

			mHeadAngle.OnUpdateEvent((iVal) => {
				CompanionData.Instance.HeadPosition = iVal;
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

			mDebug.OnSwitchEvent((iVal) => {
				CompanionData.Instance.Debug = iVal;
				mState.enabled = iVal;
			});

		}

		public override void LabelizeWidgets()
		{
			mMovingDesire.Label = BYOS.Instance.Dictionary.GetString("wanderdesire");
			mInteractDesire.Label = BYOS.Instance.Dictionary.GetString("interactdesire");
			mTeachDesire.Label = BYOS.Instance.Dictionary.GetString("teachdesire");
			mHelpDesire.Label = BYOS.Instance.Dictionary.GetString("helpdesire");
			mLearnDesire.Label = BYOS.Instance.Dictionary.GetString("learndesire");
			mHeadAngle.Label = "default head angle";
			mTrigger.Label = BYOS.Instance.Dictionary.GetString("cantrigger");
			mTriggerWander.Label = BYOS.Instance.Dictionary.GetString("cantrigger") + " wander";
			mDebug.Label = "Debug";

			mWheelsMotion.Label = BYOS.Instance.Dictionary.GetString("wheelsmotion");
			mHeadMotion.Label = BYOS.Instance.Dictionary.GetString("headmotion");
		}
	}
}