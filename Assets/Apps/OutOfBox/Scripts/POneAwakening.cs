using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class POneAwakening : AStateMachineBehaviour
    {
        private bool mExit;

        public override void Start()
        {
            base.Start();
            Buddy.Actuators.Speakers.Volume = 1F;
            mBehaviour = GetComponent<OutOfBoxBehaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("Base"));
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mExit = false;
            OutOfBoxUtils.DebugColor("FIRST PHASE : ", "blue");
            Buddy.Actuators.Head.No.ResetPosition();
            Buddy.Actuators.Head.Yes.SetPosition(-9F, 45F, (iPos) => {
                // Asleep
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.FALL_ASLEEP, false);

                StartCoroutine(WaitTime());
            });

        }


        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mExit = true;
            StopAllCoroutines();
            Buddy.Navigation.Stop();
            Buddy.Actuators.Wheels.Stop();
            Buddy.Actuators.Head.Stop();
            Buddy.Behaviour.Stop();
            Buddy.Vocal.StopAndClear();
            Buddy.Behaviour.ResetMood();
        }

        private IEnumerator WaitTime()
        {
            yield return new WaitForSeconds(2F);
            if (!mExit) {
                Buddy.Actuators.Head.Yes.SetPosition(5F, 45F);
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.AWAKE, null, (iFacialEvent) => {
                    Buddy.Vocal.SayKey("phaseonenewfriend", (iSpeechOutput) => {
                        if (!mExit) {
                            // Play BI here
                            StartCoroutine(OutOfBoxUtils.PlayBIAsync(() => {
                                if (!mExit) {
                                    // Run discovering after speech
                                    Buddy.Vocal.SayKey("phaseonearound", (iOut) => {
                                        mBehaviour.PhaseDropDown.value = 1;
                                    });
                                }
                            }));
                        }
                    });
                });
            }
        }

    }
}

