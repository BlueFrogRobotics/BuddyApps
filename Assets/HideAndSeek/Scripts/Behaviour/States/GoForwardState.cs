using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class GoForwardState : AStateMachineBehaviour
    {
        private const float MIN_DISTANCE = 0.40F;

        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            // mWheels.MoveDistance(100, 100, 10, 0.1f);
            mWheels.TurnAngle(180.0f, 100.0f, 0.02f);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mWheels.TurnAngle(30.0f, 200.0f, 0.1f);
            if (mWheels.Status == MovingState.MOTIONLESS)
            {
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            iAnimator.ResetTrigger("ChangeState");
        }

        /// <summary>
        /// Checks if infrared sensors detect obstructions
        /// </summary>
        /// <returns>True for any obstruction</returns>
        private bool AnyObstructionsInfrared()
        {

            float lLeftDistance = mIRSensors.Left.Distance;
            float lMiddleDistanceMiddle = mIRSensors.Middle.Distance;
            float lRightDistance = mIRSensors.Right.Distance;
            return IsCollisionEminent(lLeftDistance)
                || IsCollisionEminent(lMiddleDistanceMiddle)
                || IsCollisionEminent(lRightDistance);
        }

        /// <summary>
        /// Checks if ultrasound sensors detect obstructions
        /// </summary>
        /// <returns>True for any obstruction</returns>
        private bool AnyObstructionsUltrasound()
        {
            float lLeftDistance = mUSSensors.Left.Distance;
            float lRightDistance = mUSSensors.Right.Distance;
            return IsCollisionEminent(lLeftDistance)
                || IsCollisionEminent(lRightDistance);
        }

        /// <summary>
        /// Detect if the collision is eminent
        /// </summary>
        /// <param name="iCollisionDistance">Distance detected by the sensor</param>
        /// <returns></returns>
        private bool IsCollisionEminent(float iCollisionDistance)
        {
            return iCollisionDistance != 0.0F && iCollisionDistance < MIN_DISTANCE;
        }

        /// <summary>
        /// Checks for any obstruction
        /// </summary>
        /// <returns>True for any obstruction</returns>
        private bool AnyObstructions()
        {
            return AnyObstructionsInfrared();
        }
    }
}