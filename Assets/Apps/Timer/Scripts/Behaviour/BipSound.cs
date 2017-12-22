using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using Buddy.UI;

namespace BuddyApp.Timer
{

	public class BipSound : AStateMachineBehaviour
	{

		// Use this for initialization
		public override void Start()
		{
			BYOS.Instance.Primitive.Speaker.FX.Load(
				   BYOS.Instance.Resources.Load<AudioClip>("alarm"), 1
			   );
		}

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			BYOS.Instance.Primitive.Speaker.FX.Loop = true;
			BYOS.Instance.Primitive.Speaker.FX.Play(1);
			Interaction.Mood.Set(MoodType.SCARED);
			BYOS.Instance.Toaster.Display<TextToast>().With(Dictionary.GetString("touchhere"));
		}

		private void StopAlarm()
		{
			BYOS.Instance.Primitive.Speaker.FX.Loop = false;
			BYOS.Instance.Primitive.Speaker.FX.Stop();
			Interaction.Mood.Set(MoodType.NEUTRAL);
			QuitApp();
		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
				StopAlarm();
			}
		}

	}
}