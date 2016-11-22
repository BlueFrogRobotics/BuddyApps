using UnityEngine;
using System.Collections;
using BuddyOS;

public class TurnState : AStateGuardian {

    [SerializeField]
    private string mParameterName = "";

    [SerializeField]
    private int mParameterValue = 1;

    private Motors mMotors;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        mMotors = BYOS.Instance.Motors;
        int angle = animator.GetInteger("Angle");
        animator.SetInteger("Angle", angle + 30);
        mMotors.Wheels.TurnAngle(30.0f, 70.0F, 0.02F);

    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(mMotors.Wheels.Status != MobileBaseStatus.MOTIONLESS)
        {
            animator.SetInteger(mParameterName, mParameterValue);
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
