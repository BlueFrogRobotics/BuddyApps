using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class CompanionLanding : AStateMachineBehaviour
	{
		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDesireManager = GetComponent<DesireManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Landing";
			Debug.Log("state: Landing");
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			float lTimeInApp = Time.time - CompanionData.Instance.LastAppTime;


			//TODO: check when the robot was turn on for 1st wake up!

			if (lTimeInApp < 0.5F)
				Debug.Log("Warning, the app " + CompanionData.Instance.LastApp + " ran for less than 0.5 seconds. It may be an error??");

			// When we arrive in companion, depending on the previous application, we propose another request or...
			//switch (BYOS.AppUtils.GetAppCategory(CompanionData.Instance.LastApp)) {
			//	case "edutainment":
			//		if (lTimeInApp < 10F) {
			//			CompanionData.Instance.mTeachDesire += 15;
			//			// user changed his mind? go to propose edutainment?

			//		} else if (lTimeInApp < 100F) {
			//			CompanionData.Instance.mTeachDesire -= 50;
			//			// user changed his mind? go to propose edutainment?
			//		} else {
			//			CompanionData.Instance.mTeachDesire = 0;
			//			// user changed his mind? go to propose edutainment?
			//		}


			//		break;

			//	default:
			//		break;

			//}

			Trigger("Interact");
		}

	}
}