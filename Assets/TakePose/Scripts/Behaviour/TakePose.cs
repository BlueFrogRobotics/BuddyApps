using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Buddy;
using Buddy.UI;

namespace BuddyApp.TakePose
{
    public class TakePose : AStateMachineBehaviour
    {
        private const int COUNTDOWN_START = 3;
        private const float HOLD_POSE_TIME = 5F;

        private IEnumerator mStartCountDown;
        private IEnumerator mWaitForPicture;

        public override void Start()
        {
            mStartCountDown = null;
            mWaitForPicture = null;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mStartCountDown = null;
            mVocalManager.EnableTrigger = false;

            mTTS.SayKey("takepose");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mTTS.HasFinishedTalking && mStartCountDown == null)
                StartCountDown();
        }

        private void StartCountDown()
        {
            mStartCountDown = CountDownImpl();
            StartCoroutine(mStartCountDown);
        }

        private IEnumerator CountDownImpl()
        {
            mToaster.Display<CountdownToast>().With(COUNTDOWN_START, OnFinishCountdown);

            for (int i = COUNTDOWN_START; i > 0; --i)
            {
                mTTS.Say(i.ToString());
                yield return new WaitForSeconds(1F);
            }
        }

        private void OnFinishCountdown()
        {
            mWaitForPicture = WaitForPicture();
            StartCoroutine(mWaitForPicture);
        }

        private IEnumerator WaitForPicture()
        {
            SetFace();

            yield return new WaitForSeconds(HOLD_POSE_TIME);

            Trigger("Redo");
        }

        private void SetFace()
        {
            int lRandom = UnityEngine.Random.Range(0, 9);

            switch (lRandom)
            {

                case 0:
                    mSpeaker.Voice.Play(VoiceSound.SIGH);
                    mMood.Set(MoodType.ANGRY);

                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                        case 1:
                            mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
                            break;
                        default:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                    }
                    break;

                case 1:
                    mSpeaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
                    mMood.Set(MoodType.SURPRISED);
                    break;

                case 2:
                    mSpeaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
                    mMood.Set(MoodType.SCARED);
                    break;

                case 3:
                    mSpeaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
                    mMood.Set(MoodType.HAPPY);

                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            mFace.SetEvent(FaceEvent.SMILE);
                            break;
                        case 1:
                            mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
                            break;
                        default:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                    }
                    break;

                case 4:
                    mSpeaker.Voice.Play(VoiceSound.SIGH);
                    mMood.Set(MoodType.SICK);
                    break;

                case 5:
                    mSpeaker.Voice.Play(VoiceSound.SIGH);
                    mMood.Set(MoodType.TIRED);
                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                        case 1:
                            mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
                            break;
                        default:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                    }
                    break;

                case 6:
                    mSpeaker.Voice.Play(VoiceSound.SIGH);
                    mMood.Set(MoodType.THINKING);
                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                        case 1:
                            mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
                            break;
                        default:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                    }
                    break;

                case 7:
                    mSpeaker.Voice.Play(VoiceSound.SIGH);
                    mMood.Set(MoodType.GRUMPY);
                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                        case 1:
                            mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
                            break;
                        default:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                    }
                    break;


                case 8:
                    mSpeaker.FX.Play(FXSound.BEEP_2);
                    mMood.Set(MoodType.LOVE);
                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            mFace.SetEvent(FaceEvent.SMILE);
                            break;
                        case 1:
                            mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
                            break;
                        default:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                    }
                    break;

                default:
                    mMood.Set(MoodType.GRUMPY);
                    switch (UnityEngine.Random.Range(0, 1))
                    {
                        case 0:
                            mFace.SetEvent(FaceEvent.SMILE);
                            break;
                        case 1:
                            mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
                            break;
                        default:
                            mFace.SetEvent(FaceEvent.SCREAM);
                            break;
                    }
                    break;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mStartCountDown = null;
            mWaitForPicture = null;

            if (mStartCountDown != null)
                StopCoroutine(mStartCountDown);

            if (mWaitForPicture != null)
                StopCoroutine(mWaitForPicture);

            mMood.Set(MoodType.NEUTRAL);
        }
    }
}