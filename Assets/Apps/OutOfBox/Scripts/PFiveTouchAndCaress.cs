using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PFiveTouchAndCaress : AStateMachineBehaviour
    {
        private float mTimer;
        private bool mActiveTimer;
        private bool mEnded;

        public override void Start()
        {
            base.Start();

            mBehaviour = GetComponent<OutOfBoxBehaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("Base"));
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mEnded = false;
            mTimer = 0F;
            mActiveTimer = false;
            Buddy.Vocal.SayKey("pfivelovecaress", (iOut) => {
                if (!iOut.IsInterrupted) {
                    Buddy.Behaviour.SetMood(Mood.ANGRY, false);
                    StartCoroutine(OutOfBoxUtils.WaitTimeAsync(1F, () => {
                        Buddy.Vocal.SayKey("pfivedontlovecaress", (iOutTwo) => {
                            if (!iOutTwo.IsInterrupted) {
                                Buddy.Behaviour.ResetMood();
                                Buddy.Vocal.SayKey("pfivewhowantstotry", (iOutSpeech) => {
                                    if (!iOutSpeech.IsInterrupted) {
                                        Buddy.Behaviour.Face.OnTouch.Add(OnFaceTouched);
                                        mActiveTimer = true;
                                    }
                                });
                            }
                        });
                    }));
                }
            });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (mActiveTimer && mTimer < 11F) {
                mTimer += Time.deltaTime;
                OutOfBoxUtils.DebugColor("TIMER P FIVE : " + mTimer, "blue");
                if (!Buddy.Behaviour.Interpreter.IsBusy) {
                    if (Buddy.Sensors.TouchSensors.BackHead.Value) {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("BACKHEAD", "blue");
                        Buddy.Behaviour.Interpreter.Run("CenterHead01", () => { Buddy.Behaviour.SetMood(Mood.NEUTRAL); });
                    } else if (Buddy.Sensors.TouchSensors.Heart.Value) {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("Heart", "blue");
                        Buddy.Behaviour.Interpreter.Run("CenterHeart01", () => { Buddy.Behaviour.SetMood(Mood.NEUTRAL); });
                    } else if (Buddy.Sensors.TouchSensors.LeftHead.Value) {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("LeftHead", "blue");
                        Buddy.Behaviour.Interpreter.Run("LeftHead01", () => { Buddy.Behaviour.SetMood(Mood.NEUTRAL); });
                    } else if (Buddy.Sensors.TouchSensors.LeftShoulder.Value) {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("LeftShoulder", "blue");
                        Buddy.Behaviour.Interpreter.Run("LeftShoulder01", () => { Buddy.Behaviour.SetMood(Mood.NEUTRAL); });
                    } else if (Buddy.Sensors.TouchSensors.RightHead.Value) {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("RightHead", "blue");
                        Buddy.Behaviour.Interpreter.Run("RightHead01", () => { Buddy.Behaviour.SetMood(Mood.NEUTRAL); });
                    } else if (Buddy.Sensors.TouchSensors.RightShoulder.Value) {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("RightShoulder", "blue");
                        Buddy.Behaviour.Interpreter.Run("RightShoulder01", () => { Buddy.Behaviour.SetMood(Mood.NEUTRAL); });
                    }
                }

                if (mTimer > 10F && !mEnded) {
                    mEnded = true;
                    OutOfBoxUtils.DebugColor("TIMER FINI ", "blue");
                    Debug.LogWarning("Timer fini 2 ");
                    StartCoroutine(OutOfBoxUtils.PlayBIAsync(() => {
                        Debug.LogWarning("BI callback ");
                        Buddy.Vocal.SayKey("pfivetooshy", (iOutSpeech) => {
                            Debug.LogWarning("end of speech ");
                            if (!iOutSpeech.IsInterrupted) {
                                mBehaviour.PhaseDropDown.value = 5;
                                Trigger("Base");
                            }
                        });
                    })
                    );
                }
            }
        }

        private void OnFaceTouched(FacialPart iFacial)
        {
            OutOfBoxUtils.DebugColor("Face touched : ", "blue");
            mTimer = 0F;
            if (iFacial == FacialPart.LEFT_EYE || iFacial == FacialPart.RIGHT_EYE) {
                Buddy.Behaviour.Interpreter.StopAndClear();
                Buddy.Behaviour.Interpreter.Run("BML/Angry01", () => { Buddy.Behaviour.SetMood(Mood.NEUTRAL); });
            } else if (iFacial == FacialPart.MOUTH) {
                if (!Buddy.Vocal.IsBusy)
                    Buddy.Vocal.Listen(SpeechRecognitionMode.GRAMMAR_ONLY);
            } else
                Buddy.Behaviour.Interpreter.Run("BML/Happy01", () => { Buddy.Behaviour.SetMood(Mood.NEUTRAL); });
        }
    }
}

