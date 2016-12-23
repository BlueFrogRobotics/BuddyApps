using System;
using UnityEngine;


namespace BuddyApp.Memory
{
	public class LinkStateMachineBehavior : StateMachineBehaviour
	{
		public LinkHandler link;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}
		
	}
}