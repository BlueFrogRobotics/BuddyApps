using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.TakePose
{
    public class TakePose : AStateMachineBehaviour
    {
        private const int COUNTDOWN_START = 4;
        private const float HOLD_POSE_TIME = 5F;

        private IEnumerator mStartCountDown;
        private IEnumerator mWaitForPicture;

        private bool mCoroutineLaunch;

        public override void Start()
        {
            mStartCountDown = null;
            mWaitForPicture = null;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ON STATE ENTER TAKE POSE");
            mCoroutineLaunch = false;
            mStartCountDown = null;
            Buddy.Vocal.EnableTrigger = false;

            Buddy.Vocal.SayKey("takepose");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Buddy.Vocal.IsSpeaking && mStartCountDown == null && !mCoroutineLaunch)
            {
                StartCountDown();
            }
        }

        private void StartCountDown()
        {
            mCoroutineLaunch = true;
            mStartCountDown = CountDownImpl();
            StartCoroutine(mStartCountDown);
        }

        private IEnumerator CountDownImpl()
        {
            Buddy.GUI.Toaster.Display<CountdownToast>().With(COUNTDOWN_START, 0, 0, null, iCountDown =>
             {
                 Buddy.Vocal.Say(iCountDown.Second.ToString());
                 if (iCountDown.IsDone)
                 {
                     OnFinishCountdown();
                     Buddy.GUI.Toaster.Hide();
                 }
             });
            yield return null;
        }

        private void OnFinishCountdown()
        {
            Debug.Log("OnfinishCountdown");
            mWaitForPicture = WaitForPicture();
            StartCoroutine(mWaitForPicture);
        }

        private IEnumerator WaitForPicture()
        {
            SetFace(); 

            yield return new WaitForSeconds(HOLD_POSE_TIME);
            Debug.Log("Trigger Redo");
            Trigger("Redo");
        }

        private void SetFace()
        {
            Debug.Log("SetFace");
            int lRandom = UnityEngine.Random.Range(0, 9);
            switch (lRandom)
            {

                case 0:
                    Debug.Log("case 0 ");
                    Buddy.Actuators.Speakers.Vocal.Play(SoundSample.SIGH);
                    Buddy.Behaviour.Face.SetFacialExpression(FacialExpression.ANGRY);

                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                        case 1:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.BLINK_EYES);
                            break;
                        default:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                    }
                    break;

                case 1:
                    Debug.Log("case 1 ");

                    Buddy.Actuators.Speakers.Vocal.Play(SoundSample.RANDOM_CURIOUS);
                    Buddy.Behaviour.Face.SetFacialExpression(FacialExpression.SURPRISED);
                    break;

                case 2:
                    Debug.Log("case 2 ");

                    Buddy.Actuators.Speakers.Vocal.Play(SoundSample.RANDOM_SURPRISED);
                    Buddy.Behaviour.Face.SetFacialExpression(FacialExpression.SCARED);
                    break;

                case 3:
                    Debug.Log("case 3 ");

                    Buddy.Actuators.Speakers.Vocal.Play(SoundSample.RANDOM_LAUGH);
                    Buddy.Behaviour.Face.SetFacialExpression(FacialExpression.HAPPY);

                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.SMILE);
                            break;
                        case 1:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.BLINK_EYES);
                            break;
                        default:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                    }
                    break;

                case 4:
                    Debug.Log("case 4 ");

                    Buddy.Actuators.Speakers.Vocal.Play(SoundSample.SIGH);
                    Buddy.Behaviour.SetMood(Mood.SICK);
                    break;

                case 5:
                    Debug.Log("case 5 ");

                    Buddy.Actuators.Speakers.Vocal.Play(SoundSample.SIGH);
                    Buddy.Behaviour.SetMood(Mood.TIRED);
                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                        case 1:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.BLINK_EYES);
                            break;
                        default:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                    }
                    break;

                case 6:
                    Debug.Log("case 6 ");

                    Buddy.Actuators.Speakers.Vocal.Play(SoundSample.SIGH);
                    Buddy.Behaviour.SetMood(Mood.THINKING);
                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                        case 1:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.BLINK_EYES);
                            break;
                        default:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                    }
                    break;

                case 7:
                    Debug.Log("case 7 ");

                    Buddy.Actuators.Speakers.Vocal.Play(SoundSample.SIGH);
                    Buddy.Behaviour.SetMood(Mood.GRUMPY);
                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                        case 1:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.BLINK_EYES);
                            break;
                        default:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                    }
                    break;


                case 8:
                    Debug.Log("case 8 ");

                    Buddy.Actuators.Speakers.Vocal.Play(SoundSample.BEEP_2);
                    Buddy.Behaviour.SetMood(Mood.LOVE);
                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.SMILE);
                            break;
                        case 1:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.BLINK_EYES);
                            break;
                        default:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                    }
                    break;

                default:
                    Debug.Log("case defaut ");

                    Buddy.Behaviour.SetMood(Mood.GRUMPY);
                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.SMILE);
                            break;
                        case 1:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.BLINK_EYES);
                            break;
                        default:
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.RELATIVE_EVENT);
                            break;
                    }
                    break;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mCoroutineLaunch = false;
            mStartCountDown = null;
            mWaitForPicture = null;

            if (mStartCountDown != null)
                StopCoroutine(mStartCountDown);

            if (mWaitForPicture != null)
                StopCoroutine(mWaitForPicture);

            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        }
    }
}