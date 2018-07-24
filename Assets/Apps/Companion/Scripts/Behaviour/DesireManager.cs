﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using System;

namespace BuddyApp.Companion
{

	public enum DESIRE
	{
		MOVE,
		INTERACT,
		TEACH,
		LEARN,
		HELP,
		EXPRESSMOOD
	}

	/// <summary>
	/// Manager class that controls the desire values
	/// </summary>
	public class DesireManager : MonoBehaviour
	{


		private ActionManager mActionManager;
		private float mPreviousTime;
		private CompanionData mCompaData;

		void Start()
		{
			mActionManager = GetComponent<ActionManager>();
			mPreviousTime = 0F;
			mCompaData = CompanionData.Instance;
		}

		// Update desires
		void Update()
		{
			if (GetMaxDesireValue() < 100) {

				if (CompanionData.Instance.mProactivity > 0)
					// if 100%, update every seconds, else every 100/mProactivity seconds
					if ((Time.time - mPreviousTime) > (100F / CompanionData.Instance.mProactivity)) {
						mPreviousTime = Time.time;

						switch (mActionManager.CurrentAction) {


							case BUDDY_ACTION.NONE:
								UpdateDesire(2, 1, 0, 1, 1);
								break;

							case BUDDY_ACTION.WANDER:
								UpdateDesire(-2, 1, 1, 1, 1);
								break;

							case BUDDY_ACTION.DANCE:
								UpdateDesire(-5, 1, 0, 1, 1);
								break;

							case BUDDY_ACTION.FOLLOW:

								UpdateDesire(-1, 1, 2, 2, 2);
								break;

							case BUDDY_ACTION.LOOK_FOR_USER:
								UpdateDesire(-2, 1, 1, 0, 2);
								break;

							case BUDDY_ACTION.CHAT:
								UpdateDesire(1, 0, -1, -1, -1);
								break;

							case BUDDY_ACTION.TOUCH_INTERACT:
								UpdateDesire(-1, 2, 2, 2, 0);
								break;

							case BUDDY_ACTION.ASK_USER_PROFILE:
								UpdateDesire(1, 1, 0, -5, 1);
								break;

							case BUDDY_ACTION.EXPRESS_MOOD:
								UpdateDesire(-2, 1, 0, 1, 1);
								break;

							default:
								break;
						}
					}

				NormalizeDesires();
			}
		}

		private void NormalizeDesires()
		{
			NormalizeDesire(ref mCompaData.mMovingDesire);

			NormalizeDesire(ref mCompaData.mInteractDesire);

			NormalizeDesire(ref mCompaData.mLearnDesire);

			NormalizeDesire(ref mCompaData.mTeachDesire);

			NormalizeDesire(ref mCompaData.mHelpDesire);
		}

		public void MultiplyDesires(float iCoef)
		{
			MultiplyDesire(ref mCompaData.mMovingDesire, iCoef);

			MultiplyDesire(ref mCompaData.mInteractDesire, iCoef);

			MultiplyDesire(ref mCompaData.mLearnDesire, iCoef);

			MultiplyDesire(ref mCompaData.mTeachDesire, iCoef);

			MultiplyDesire(ref mCompaData.mHelpDesire, iCoef);
		}

		private void MultiplyDesire(ref int ioDesire, float iCoef)
		{
			ioDesire = (int)(iCoef * ioDesire);
			NormalizeDesire(ref ioDesire);
		}

		private void NormalizeDesire(ref int ioDesire)
		{
			if (ioDesire > 100)
				ioDesire = 100;

			if (ioDesire < 0)
				ioDesire = 0;
		}


		public DESIRE GetMainDesire()
		{
			if (Math.Abs(BYOS.Instance.Interaction.InternalState.Positivity) + Math.Abs(BYOS.Instance.Interaction.InternalState.Energy) > 5 && mActionManager.LastMoodExpression > 200F)
				return DESIRE.EXPRESSMOOD;

			else if (IsMaxDesire(mCompaData.mInteractDesire))
				return DESIRE.INTERACT;

			else if (IsMaxDesire(mCompaData.mHelpDesire))
				return DESIRE.HELP;

			else if (IsMaxDesire(mCompaData.mLearnDesire))
				return DESIRE.LEARN;

			else if (IsMaxDesire(mCompaData.mTeachDesire))
				return DESIRE.TEACH;

			else
				return DESIRE.MOVE;
		}

		private bool IsMaxDesire(int iDesire)
		{
			return (Math.Max(mCompaData.mInteractDesire, Math.Max(mCompaData.mHelpDesire, Math.Max(mCompaData.mLearnDesire, Math.Max(mCompaData.mMovingDesire, mCompaData.mTeachDesire)))) == iDesire);
		}

		public int GetMaxDesireValue()
		{
			int lMaxInternalValue = Math.Abs(BYOS.Instance.Interaction.InternalState.Positivity) + Math.Abs(BYOS.Instance.Interaction.InternalState.Energy);
			if (lMaxInternalValue > 5)
				return Math.Max(Math.Min(lMaxInternalValue, 100), Math.Max(mCompaData.mInteractDesire, Math.Max(mCompaData.mHelpDesire, Math.Max(mCompaData.mLearnDesire, Math.Max(mCompaData.mMovingDesire, mCompaData.mTeachDesire)))));

			return Math.Max(mCompaData.mInteractDesire, Math.Max(mCompaData.mHelpDesire, Math.Max(mCompaData.mLearnDesire, Math.Max(mCompaData.mMovingDesire, mCompaData.mTeachDesire))));
		}

		public void UpdateDesire(int iMoving, int iHelp, int iInteract, int iLearn, int iTeach)
		{

			// Update only one desire to avoid having same value
			int lRand = UnityEngine.Random.Range(0, 8);

			if (lRand == 0)
				mCompaData.mMovingDesire += iMoving + BYOS.Instance.Interaction.InternalState.Energy;
			else if (lRand == 1)
				mCompaData.mHelpDesire += iHelp + BYOS.Instance.Interaction.InternalState.Positivity;
			else if (lRand == 2)
				mCompaData.mInteractDesire += iInteract;
			else if (lRand == 3)
				mCompaData.mLearnDesire += iLearn;
			else if (lRand == 4)
				mCompaData.mTeachDesire += iTeach;
		}

	}
}