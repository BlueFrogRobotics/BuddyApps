using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.TakePhoto
{
	public class Landing : AStateMachineBehaviour
	{

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Interaction.TextToSpeech.SayKey("movehands");
			Trigger("LookForUser");
		}

	}
}