using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.MemoryGame
{
	public class GameIntro : AStateMachineBehaviour
	{

		public override void Start()
		{
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Actuators.Head.Yes.SetPosition(15);
            Buddy.Actuators.Head.No.SetPosition(0);

            Buddy.GUI.Header.DisplayParametersButton(true);
            Buddy.Vocal.Say(Buddy.Resources.GetRandomString("intro"), iOutput =>
            {
                Trigger("StartGame");
            });
        }


		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}
