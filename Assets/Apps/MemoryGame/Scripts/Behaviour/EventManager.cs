using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System;

using BlueQuark;

namespace BuddyApp.MemoryGame
{
    public class EventManager : MonoBehaviour
    {
        private static float YES_ANGLE = 35.0f;
        private static float YES_ORIGIN_ANGLE = 15.0f;
        private static float NO_ANGLE = 40.0F;
        private static float NO_ORIGIN_ANGLE = 0.0F;

        private static float WHEEL_ANGLE = 45.0F;
        private static float MOTION_TIMEOUT = 2.0F;
        private bool mMotionRunning;

        private bool mIsMoving;
        public bool IsMoving() { return mIsMoving; }

        public enum BuddyMotion : int
        {
            NONE,
            HEAD_LEFT,
            HEAD_RIGHT,
            HEAD_DOWN,
            HEAD_UP,
            WHEEL_LEFT,
            WHEEL_RIGHT,
            WHEEL_BACK,
            WHEEL_FORWARD
        }

        /// <summary>
        /// Get the sound associated to the event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        SoundSample GetEventSound(MemoryEvent evt)
        {
            switch (evt)
            {
                case MemoryEvent.MOUTH:
                    return SoundSample.SURPRISED_3;
                case MemoryEvent.LEFT_EYE:
                    return SoundSample.SURPRISED_1;
                case MemoryEvent.RIGHT_EYE:
                    return SoundSample.SURPRISED_2;
                case MemoryEvent.RIGHT_HEAD:
                    return SoundSample.LAUGH_2;
                case MemoryEvent.LEFT_HEAD:
                    return SoundSample.LAUGH_1;
                case MemoryEvent.BACK_HEAD:
                    return SoundSample.LAUGH_3;
                case MemoryEvent.TURN_LEFT:
                    return SoundSample.CURIOUS_1;
                case MemoryEvent.TURN_RIGHT:
                    return SoundSample.CURIOUS_2;
                default:
                    return SoundSample.BEEP_1;
            }
        }

        /// <summary>
        /// Get the motion associated to an event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        private BuddyMotion GetEventMotion(MemoryEvent evt)
        {
            switch (evt)
            {
                case MemoryEvent.RIGHT_HEAD:
                    return BuddyMotion.HEAD_RIGHT;
                case MemoryEvent.LEFT_HEAD:
                    return BuddyMotion.HEAD_LEFT;
                case MemoryEvent.BACK_HEAD:
                    return BuddyMotion.HEAD_UP;
                case MemoryEvent.TURN_LEFT:
                    return BuddyMotion.WHEEL_LEFT;
                case MemoryEvent.TURN_RIGHT:
                    return BuddyMotion.WHEEL_RIGHT;
                default:
                    return BuddyMotion.NONE;
            }
        }        

        private void Start()
        {
            mIsMoving = false;
            mMotionRunning = false;
        }

        public void DoEvent(MemoryEvent evt)
        {
            Buddy.Actuators.Speakers.Media.Play(GetEventSound(evt));
            BuddyMotion motion = GetEventMotion(evt);
            if (motion == BuddyMotion.NONE)
            {
                // Facial event
                Buddy.Behaviour.Face.PlayEvent((FacialEvent)evt);
            }
            else
            {
                StartCoroutine(ControlBuddy(motion));
            }
        }

        public bool IsMotionEvent(MemoryEvent evt)
        {
            return (GetEventMotion(evt) != BuddyMotion.NONE);
        }

        private void MoveToPosition(BuddyMotion iMotion, bool origin = false)
        {
            if (iMotion == BuddyMotion.HEAD_LEFT || iMotion == BuddyMotion.HEAD_RIGHT)
            {
                mMotionRunning = true;
                float speed = 40F;
                float lTargetAngle = origin ? NO_ORIGIN_ANGLE : NO_ANGLE;
                if (iMotion == BuddyMotion.HEAD_RIGHT) lTargetAngle = -lTargetAngle;
                Buddy.Actuators.Head.No.SetPosition(lTargetAngle, speed, (angle) =>
                {
                    mMotionRunning = false;
                });
            }
            else if (iMotion == BuddyMotion.HEAD_UP)
            {
                mMotionRunning = true;
                float speed = 15F;
                float lTargetAngle = origin ? YES_ORIGIN_ANGLE : YES_ANGLE;
                Buddy.Actuators.Head.Yes.SetPosition(lTargetAngle, speed, (angle) =>
                {
                    mMotionRunning = false;
                });
            }
            else if (iMotion == BuddyMotion.WHEEL_LEFT || iMotion == BuddyMotion.WHEEL_RIGHT)
            {
                mMotionRunning = true;
                // As wheel movement is relative going back to origin with reverse rotation
                float lTargetAngle = origin ? -WHEEL_ANGLE : WHEEL_ANGLE;
                if (iMotion == BuddyMotion.WHEEL_RIGHT) lTargetAngle = -lTargetAngle;
                float speed = 70F;
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(lTargetAngle, speed, () =>
                {
                    mMotionRunning = false;
                });
            }
        }

        private IEnumerator ControlBuddy(BuddyMotion iMotion)
        {
            yield return new WaitUntil(() => !mIsMoving);

            mIsMoving = true;
            float lTimer = 0.0F;

            // Moving to position
            MoveToPosition(iMotion);
            yield return new WaitForSeconds(0.5f);
            // Wait until movement is done or timeout
            while ((mMotionRunning
                || Buddy.Actuators.Head.No.IsBusy
                || Buddy.Actuators.Head.Yes.IsBusy
                || Buddy.Actuators.Wheels.IsBusy)
                && lTimer < MOTION_TIMEOUT)
            {
                lTimer += Time.deltaTime;
                yield return null;
            }

            // Moving back to origin position
            lTimer = 0.0f;
            MoveToPosition(iMotion, true);
            yield return new WaitForSeconds(0.5f);
            // Wait until movement is done or timeout
            while ((mMotionRunning
                || Buddy.Actuators.Head.No.IsBusy
                || Buddy.Actuators.Head.Yes.IsBusy
                || Buddy.Actuators.Wheels.IsBusy)
                && lTimer < MOTION_TIMEOUT)
            {
                lTimer += Time.deltaTime;
                yield return null;
            }

            mIsMoving = false;
        }

        /*
        private IEnumerator ControlBuddy(BuddyMotion iMotion)
        {
            yield return new WaitUntil(() => !mIsMoving);

            mIsMoving = true;
            float lTimer = 0.0F;

            // Moving noHinge
            if (iMotion == BuddyMotion.HEAD_LEFT || iMotion == BuddyMotion.HEAD_RIGHT)
            {
                float lTargetAngle = (iMotion == BuddyMotion.HEAD_LEFT) ? NO_MOVE_ANGLE : -NO_MOVE_ANGLE;

                lTimer = 0.0f;
                // Put the head to the given direction
                Buddy.Actuators.Head.No.SetPosition(lTargetAngle);

                while (Buddy.Actuators.Head.No.IsBusy && lTimer < MOTION_TIMEOUT)
                {
                    lTimer += Time.deltaTime;
                    yield return null;
                }

                lTimer = 0.0f;
                // Put the head back
                Buddy.Actuators.Head.No.SetPosition(0.0F);

                yield return new WaitForSeconds(0.5f);
                while (Buddy.Actuators.Head.No.IsBusy && lTimer < 2.0F)
                {
                    lTimer += Time.deltaTime;
                    yield return null;
                }
            }
            else if (iMotion == BuddyMotion.HEAD_DOWN)
            {
                Debug.Log("Motion not available" + iMotion);
            }
            else if (iMotion == BuddyMotion.HEAD_UP)
            {
                float lOriginAngle = Buddy.Actuators.Head.Yes.Angle;
                float lTargetAngle = UP_ANGLE;
                lTimer = 0.0f;
                // Put the head to the given direction
                Buddy.Actuators.Head.Yes.SetPosition(lTargetAngle);

                yield return new WaitForSeconds(0.5f);
                while (Buddy.Actuators.Head.Yes.IsBusy && lTimer < 2.0F)
                {
                    lTimer += Time.deltaTime;
                    yield return null;
                }

                //// Wait for end of motion
                lTimer = 0.0f;
                // Put the head back
                Buddy.Actuators.Head.Yes.SetPosition(lOriginAngle);

                yield return new WaitForSeconds(0.5f);

                //// Wait for end of motion

                while (Buddy.Actuators.Head.Yes.IsBusy && lTimer < 2.0F)
                {
                    lTimer += Time.deltaTime;
                    yield return null;
                }

                // Turning wheels
            }
            else if (iMotion == BuddyMotion.WHEEL_LEFT || iMotion == BuddyMotion.WHEEL_RIGHT)
            {
                float angle = 45f;
                float speed = 70F;
                lTimer = 0.0f;
                if (iMotion == BuddyMotion.WHEEL_RIGHT)
                {
                    angle = -angle;
                }
                mMotionRunning = true;
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(angle, speed, () =>
                {
                    mMotionRunning = false;
                });

                while (mMotionRunning && lTimer < 2.0F)
                {
                    lTimer += Time.deltaTime;
                    yield return null;
                }

                mMotionRunning = true;
                lTimer = 0.0f;
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(-angle, speed, () =>
                {
                    mMotionRunning = false;
                });
                while (mMotionRunning && lTimer < 2.0F)
                {
                    lTimer += Time.deltaTime;
                    yield return null;
                }
            }

            yield return new WaitForSeconds(0.5F);

            mIsMoving = false;
        }*/
    }
}