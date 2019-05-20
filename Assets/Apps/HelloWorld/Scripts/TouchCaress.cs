using BlueQuark;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.HelloWorld
{
    public sealed class TouchCaress : AStateMachineBehaviour
    {
        public enum TouchSensor
        {
            BACK_HEAD,
            HEART,
            LEFT_HEAD,
            LEFT_SHOULDER,
            RIGHT_HEAD,
            RIGHT_SHOULDER,
        }

        private const float TIMEOUT = 15F;

        private bool mUpdateEnabled;

        private float mTimeStamp;

        private bool mTouched;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mTouched = false;
            mUpdateEnabled = false;
            mTimeStamp = -1000F;

            Buddy.Vocal.Say(Buddy.Resources.GetString("caressme"), (iSpeechOutput) =>
            {
                Buddy.Vocal.Say(Buddy.Resources.GetString("fingerineye"), (iSpeechOut) =>
                {
                    Buddy.Behaviour.SetMood(Mood.ANGRY, false);
                    StartCoroutine(HelloWorldUtils.WaitTimeAsync(1F, () =>
                    {
                        Buddy.Behaviour.ResetMood();

                        Buddy.Vocal.Say(Buddy.Resources.GetString("whotry"), (iSpeech) =>
                        {
                            // init face touch
                            Buddy.Behaviour.Face.OnTouch.Add(OnFaceTouch);
                            mUpdateEnabled = true;
                        });
                    }));
                });
            });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (mUpdateEnabled) {
                if (mTimeStamp < 0F)
                    mTimeStamp = Time.time;

                if (!Buddy.Behaviour.Interpreter.IsBusy) {
                    if (Buddy.Sensors.TouchSensors.BackHead.Value) {
                        mTouched = true;
                        mTimeStamp = Time.time;
                        HelloWorldUtils.DebugColor("BACKHEAD", "blue");
                        Buddy.Behaviour.Interpreter.Run("CenterHead01");
                    }
                    else if (Buddy.Sensors.TouchSensors.Heart.Value) {
                        mTouched = true;
                        mTimeStamp = Time.time;
                        HelloWorldUtils.DebugColor("Heart", "blue");
                        Buddy.Behaviour.Interpreter.Run("CenterHeart01");
                    }
                    else if (Buddy.Sensors.TouchSensors.LeftHead.Value) {
                        mTouched = true;
                        mTimeStamp = Time.time;
                        HelloWorldUtils.DebugColor("LeftHead", "blue");
                        Buddy.Behaviour.Interpreter.Run("LeftHead01");
                    }
                    else if (Buddy.Sensors.TouchSensors.LeftShoulder.Value) {
                        mTouched = true;
                        mTimeStamp = Time.time;
                        HelloWorldUtils.DebugColor("LeftShoulder", "blue");
                        Buddy.Behaviour.Interpreter.Run("LeftShoulder01");
                    }
                    else if (Buddy.Sensors.TouchSensors.RightHead.Value) {
                        mTouched = true;
                        mTimeStamp = Time.time;
                        HelloWorldUtils.DebugColor("RightHead", "blue");
                        Buddy.Behaviour.Interpreter.Run("RightHead01");
                    }
                    else if (Buddy.Sensors.TouchSensors.RightShoulder.Value) {
                        mTouched = true;
                        mTimeStamp = Time.time;
                        HelloWorldUtils.DebugColor("RightShoulder", "blue");
                        Buddy.Behaviour.Interpreter.Run("RightShoulder01");
                    }
                }

                if ((Time.time - mTimeStamp) > TIMEOUT) {
                    mUpdateEnabled = false;
                    if (!mTouched) {
                        HelloWorldUtils.DebugColor("TIMEOUT - TOUCHED", "red");
                        Buddy.Vocal.Say(Buddy.Resources.GetString("shy"), TransitionToTrigger);
                    }
                    else {
                        HelloWorldUtils.DebugColor("TIMEOUT - NOT TOUCHED", "red");
                        TransitionToTrigger(null);
                    }
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Behaviour.Face.OnTouch.Clear();
        }

        private void TransitionToTrigger(SpeechOutput iSpeech)
        {
            mUpdateEnabled = false;
            Buddy.Vocal.Say(Buddy.Resources.GetString("sadbuttrue"), (iSpeechOutput) =>
            {
                Buddy.Behaviour.Interpreter.StopAndClear();
                Buddy.Behaviour.Interpreter.Run("BML/BlinkRight01", () => { Trigger("LearnToTrigger"); });
            });
        }

        private void OnFaceTouch(FacialPart iFacePart)
        {
            HelloWorldUtils.DebugColor("ON FACE TOUCH", "blue");
            mTouched = true;
            mTimeStamp = Time.time;
            if (iFacePart == FacialPart.LEFT_EYE || iFacePart == FacialPart.RIGHT_EYE) {
                Buddy.Behaviour.Interpreter.StopAndClear();
                Buddy.Behaviour.Interpreter.Run("BML/Angry01", () => { TransitionToTrigger(null); });
            }
            else if (iFacePart == FacialPart.MOUTH) {
                // mouth
                if (!Buddy.Vocal.IsBusy)
                    Buddy.Vocal.Listen(SpeechRecognitionMode.GRAMMAR_ONLY);
            }
            else {
                Buddy.Behaviour.Interpreter.Run("BML/Happy01");
            }
        }
    }
}
