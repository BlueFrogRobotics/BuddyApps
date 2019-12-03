using UnityEngine;
using System.Collections;
using System;

namespace BuddyApp.MemoryGame
{
	public class StartGameState : AStateMachineBehaviour
	{
		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            GetComponent<MemoryGameRandomLevel>().Init(MemoryGameData.Instance.Difficulty, MemoryGameData.Instance.MoveHead, MemoryGameData.Instance.MoveBody);
            Trigger("FirstLevel");
        }

	}
}