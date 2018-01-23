using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.ExperienceCenter{
public class ECInitState : StateMachineBehaviour {

		private TcpServer mTcpServer;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			ExperienceCenterData.Instance.StatusTcp = "Offline";
			ExperienceCenterData.Instance.IPAddress = "-";
			ExperienceCenterData.Instance.StopDistance = 0.7f;
			ExperienceCenterData.Instance.NoiseTime = 0.5f;
			ExperienceCenterData.Instance.TableDistance = 1.5f;
			ExperienceCenterData.Instance.IOTDistance = 1.5f;
			ExperienceCenterData.Instance.HeadPoseTolerance = 0.2f;

			mTcpServer =  GameObject.Find ("AIBehaviour").GetComponent<TcpServer> ();
			mTcpServer.Init ();
			ExperienceCenterData.Instance.EnableHeadMovement = true;
			ExperienceCenterData.Instance.EnableBaseMovement = true;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
}
