﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.TakePhoto
{
	public sealed class Landing : AStateMachineBehaviour
	{

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Buddy.Vocal.SayKey("movehands");
			Trigger("LookForUser");
		}

	}
}