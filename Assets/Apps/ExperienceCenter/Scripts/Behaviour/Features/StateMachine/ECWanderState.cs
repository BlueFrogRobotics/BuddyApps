using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.ExperienceCenter{
public class ECWanderState : StateMachineBehaviour {

		private AnimatorManager mAnimatorManager;
		private WanderBehaviour mWalkBehaviour;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mWalkBehaviour = GameObject.Find("AIBehaviour").GetComponent<WanderBehaviour>();
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
			mWalkBehaviour.InitBehaviour ();
	}
			
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mWalkBehaviour.StopBehaviour ();
	}
			
}
}
