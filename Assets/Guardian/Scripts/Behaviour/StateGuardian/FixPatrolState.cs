using UnityEngine;
using BuddyOS;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class FixPatrolState : AStateGuardian
    {

        private NoHinge mNoHinge;
        private float mTimer=0.0f;
        private bool mTurnHead = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mNoHinge = BYOS.Instance.Motors.NoHinge;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;

            if (GuardianData.Instance.TurnHeadIsActive)
            {
                if (animator.GetBool("HasAlerted"))
                {
                    animator.SetBool("TurnHead", false);
                    mNoHinge.SetPosition(0);
                    mTimer = 0.0f;
                    mTurnHead = false;
                    animator.GetBehaviour<DetectionPatrolState>().IsDetectingMovement = false;
                    animator.GetBehaviour<DetectionPatrolState>().IsDetectingKidnapping = false;
                }
                else
                {
                    if (!mTurnHead && mTimer > 7.0f)
                    {
                        mTurnHead = true;
                        animator.SetBool("TurnHead", mTurnHead);
                        mTimer = 0.0f;
                        animator.GetBehaviour<DetectionPatrolState>().IsDetectingMovement = false;
                        animator.GetBehaviour<DetectionPatrolState>().IsDetectingKidnapping = false;
                    }
                    else if (mTurnHead && mTimer > 3.5f)
                    {
                        mTurnHead = false;
                        animator.SetBool("TurnHead", mTurnHead);
                        mNoHinge.SetPosition(0);
                        mTimer = 0.0f;
                        animator.GetBehaviour<DetectionPatrolState>().IsDetectingMovement = false;
                        animator.GetBehaviour<DetectionPatrolState>().IsDetectingKidnapping = false;
                    }
                    else if(!mTurnHead && mTimer>3.0f && mTimer < 7.0f)//Mathf.Abs( mNoHinge.CurrentAnglePosition)<2.1f && Mathf.Abs(mNoHinge.DestinationAnglePosition) < 2.1f)
                    {
                        animator.GetBehaviour<DetectionPatrolState>().IsDetectingMovement = true;
                        animator.GetBehaviour<DetectionPatrolState>().IsDetectingKidnapping = true;
                    }
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("ChangeState", false);
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
}