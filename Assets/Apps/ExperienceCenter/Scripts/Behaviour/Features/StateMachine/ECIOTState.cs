using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.ExperienceCenter
{
	public class ECIOTState : StateMachineBehaviour
	{
		
		private IOTBehaviour mBehaviour;

		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IOTBehaviour> ();
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
			BYOS.Instance.Interaction.VocalManager.StopAllCoroutines ();
			mBehaviour.InitBehaviour ();
		}

		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mBehaviour.StopBehaviour ();
		}

	}
}
