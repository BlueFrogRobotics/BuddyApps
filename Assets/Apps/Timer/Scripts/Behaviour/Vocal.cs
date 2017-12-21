using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using Buddy.UI;
using System;

namespace BuddyApp.Timer
{

	public class Vocal : AStateMachineBehaviour
	{
		VerticalCarouselInfo mCarouselHour;
		VerticalCarouselInfo mCarouselMinute;
		VerticalCarouselInfo mCarouselSecond;
		private int mHour;
		private int mMinute;
		private int mSecond;

		// Use this for initialization
		public override void Start()
		{
			BYOS.Instance.Primitive.Speaker.FX.Load(
				   BYOS.Instance.Resources.Load<AudioClip>("Wheel"), 0
			   );
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mSecond = 0;
			mHour = 0;
			mMinute = 0;
			Debug.Log("ENTER LISTEN test");
			Interaction.VocalManager.OnEndReco = GetAnswer;
			Interaction.VocalManager.OnError = NoAnswer;
			Interaction.VocalManager.StartInstantReco();

			mCarouselHour = new VerticalCarouselInfo();
			mCarouselHour.Text = "Hr";
			//mCarouselHour.Text = Dictionary.GetString("hours");
			mCarouselHour.LowerValue = 0;
			mCarouselHour.UpperValue = 5;
			mCarouselHour.OnScrollChange = iVal => {
				mHour = iVal;
				if (BYOS.Instance.Primitive.Speaker.FX.Status != SoundChannelStatus.PLAYING)
					BYOS.Instance.Primitive.Speaker.FX.Play(0);
			};


			mCarouselMinute = new VerticalCarouselInfo();
			mCarouselMinute.Text = "Min";
			//mCarouselMinute.Text = Dictionary.GetString("min") + "s";
			mCarouselMinute.LowerValue = 0;
			mCarouselMinute.UpperValue = 60;
			mCarouselMinute.OnScrollChange = iVal => {
				mMinute = iVal;
				if (BYOS.Instance.Primitive.Speaker.FX.Status != SoundChannelStatus.PLAYING)
					BYOS.Instance.Primitive.Speaker.FX.Play(0);
			};


			mCarouselSecond = new VerticalCarouselInfo();
			mCarouselSecond.Text = "Sec";
			//mCarouselSecond.Text = Dictionary.GetString("secs");
			mCarouselSecond.LowerValue = 0;
			mCarouselSecond.UpperValue = 60;
			mCarouselSecond.OnScrollChange = iVal => {
				mSecond = iVal;
				if (BYOS.Instance.Primitive.Speaker.FX.Status != SoundChannelStatus.PLAYING)
					BYOS.Instance.Primitive.Speaker.FX.Play(0);
			};

			Toaster.Display<VerticalCarouselToast>().With(mCarouselHour, mCarouselMinute, mCarouselSecond, OnValidate, OnCancel);

		}

		private void OnCancel()
		{
			QuitApp();
		}

		private void OnValidate()
		{
			CommonIntegers["finalcountdown"] = mHour * 3600 + mMinute * 60 + mSecond;
			Toaster.Hide();
			Trigger("CountDown");

		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Debug.Log("EXIT LISTEN");
		}

		private void GetAnswer(string iAnswer)
		{
			Utils.LogI(LogContext.APP, "GOT AN ANSWER: " + iAnswer);
			TimerData.Instance.VocalRequest = iAnswer.ToLower();
			Toaster.Hide();
			Trigger("ParseTime");
		}

		private void NoAnswer(STTError iError)
		{
			Utils.LogI(LogContext.APP, "VM error");
			Debug.Log("GOT NO ANSWER");
		}

	}
}
