using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using System;

namespace BuddyApp.Companion
{

	/// <summary>
	/// Manager class that controls the desire values
	/// </summary>
	public class DesireManager : MonoBehaviour
	{


		private ActionManager mActionManager;
		private float mPreviousTime;

		[SerializeField]
		private UnityEngine.UI.Text mState;

		void Start()
		{
			mActionManager = GetComponent<ActionManager>();
			mPreviousTime = 0F;
		}

		// Update desires
		void Update()
		{

			//	NONE
			//	WANDER,
			//DANCE,
			//FOLLOW,
			//GAME,
			//EDUTAINMENT,
			//SERVICE,
			//JOKE,
			//CHAT,
			//TOUCH_INTERACT,
			//LOOK_FOR_USER,
			//ASK_USER_PROFILE,
			//INFORM_WEATHER,
			//INFORM_MOOD

			if (Time.time - mPreviousTime > 1F) {
				switch (mActionManager.CurrentAction) {


					case BUDDY_ACTION.NONE:
						int lRand = UnityEngine.Random.Range(0, 5);

						if (lRand < 1)
							CompanionData.Instance.Bored += 5;
						break;

					case BUDDY_ACTION.WANDER:
						//TODO
						break;

					case BUDDY_ACTION.DANCE:
						//TODO
						break;

					case BUDDY_ACTION.FOLLOW:
						//TODO
						break;

					case BUDDY_ACTION.LOOK_FOR_USER:
						//TODO
						break;

					case BUDDY_ACTION.CHAT:
						//TODO
						break;

					case BUDDY_ACTION.TOUCH_INTERACT:
						// TODO
						break;

					case BUDDY_ACTION.ASK_USER_PROFILE:
						// TODO
						break;

					case BUDDY_ACTION.INFORM_MOOD:
						// TODO
						break;

					default:
						break;

				}






				if (mActionManager.CurrentAction == BUDDY_ACTION.NONE) {
					if (Time.time - mPreviousTime > 5F) {
						int lRand = UnityEngine.Random.Range(0, 2);

						if (lRand < 1) {
							CompanionData.Instance.Bored += 5;
						}
						mPreviousTime = Time.time;

					}
				}

			}
		}
	}
}
