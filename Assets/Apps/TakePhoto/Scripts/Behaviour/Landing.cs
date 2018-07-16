using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.TakePhoto
{
	public class Landing : AStateMachineBehaviour
	{

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Buddy.Vocal.SayKey("movehands");
			Trigger("LookForUser");
		}

	}
}