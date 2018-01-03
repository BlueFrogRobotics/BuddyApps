using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class Charging : AStateMachineBehaviour
	{
		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Charging";
			Debug.Log("state: Charging");
			Interaction.TextToSpeech.Say("Je suis en train de me recharger, trop bien, trop décalé!", true);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			// TODO: robot can interact while charging but not move wheels.
			// Once battery is at 100, robot goes back to IDLE
			iAnimator.SetTrigger("IDLE");
		}

	}
}