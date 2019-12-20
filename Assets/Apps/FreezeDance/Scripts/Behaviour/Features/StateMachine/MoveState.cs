using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;


namespace BuddyApp.FreezeDance
{

    public class MoveState : AStateMachineBehaviour
    {
        private static float YES_ANGLE_MIN = 0.0f;
        private static float YES_ANGLE_MAX = 35.0f;
        private static float WHEEL_ANGLE_MIN = 80.0F;
        private static float WHEEL_ANGLE_MAX = 180.0F;
        private static float YES_SPEED = 40.0f;
        private static float WHEEL_SPEED = 170.0F;
        private static float MOTION_TIMEOUT = 2.0F;
        private int mRotationDirection;
        private FreezeDanceBehaviour mFreezeBehaviour;

        private bool mIsMoving;
        public bool IsMoving() { return mIsMoving; }

        public override void Start()
        {
            mIsMoving = false;
            mFreezeBehaviour = GetComponent<FreezeDanceBehaviour>();
            mRotationDirection = 1;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            yield return new WaitUntil(() => !mIsMoving);
            float lTimer = 0.0F;

            if (FreezeDanceData.Instance.NbPauseBeforeMvt < mFreezeBehaviour.NbPause)
            {
                mIsMoving = true;
                float lTargetAngle = Random.Range(WHEEL_ANGLE_MIN, WHEEL_ANGLE_MAX) * mRotationDirection;
                mRotationDirection = -mRotationDirection;
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(lTargetAngle, WHEEL_SPEED);
                Buddy.Behaviour.SetMood(Mood.HAPPY);
            }
            if (FreezeDanceData.Instance.NbPauseBeforeHeadMvt < mFreezeBehaviour.NbPause)
            {
                mIsMoving = true;
                float lTargetAngle = (Random.Range(0, 2) == 0) ? YES_ANGLE_MIN : YES_ANGLE_MAX;
                Buddy.Actuators.Head.Yes.SetPosition(lTargetAngle, YES_SPEED);
                Buddy.Behaviour.Face.LookAt(GazePosition.TOP_LEFT);
            }

            if (mIsMoving)
                yield return new WaitForSeconds(0.5f);
            // Wait until movement is done or timeout
            while (lTimer < MOTION_TIMEOUT
                && (Buddy.Actuators.Head.Yes.IsBusy
                || Buddy.Actuators.Wheels.IsBusy)
                )
            {
                lTimer += Time.deltaTime;
                yield return null;
            }

            Buddy.Behaviour.Face.LookAt(GazePosition.CENTER);
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            mIsMoving = false;
            Trigger("Detection");
        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

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
