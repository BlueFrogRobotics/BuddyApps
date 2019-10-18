using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PFourMovement : AStateMachineBehaviour
    {


        public override void Start()
        {
            base.Start();

            mBehaviour = GetComponent<OutOfBoxBehaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("Base"));
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Actuators.Head.No.ResetPosition();
            Buddy.Actuators.Head.Yes.ResetPosition();
            Buddy.Vocal.SayKey("pfourfirststep", (EndSpeaking) => {
                if (!EndSpeaking.IsInterrupted)
                    Buddy.Navigation.Run<DisplacementStrategy>().Move(0.75F, 70F, () => {
                        StartCoroutine(OutOfBoxUtils.PlayBIAsync(() => {
                            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(60F, 70F, () => {
                                StartCoroutine(OutOfBoxUtils.PlayBIAsync(() => {
                                    Buddy.Navigation.Run<DisplacementStrategy>().Move(-0.75F, 70F, () => {
                                        StartCoroutine(OutOfBoxUtils.PlayBIAsync(() => {
                                            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(300F, 70F, () => {
                                                StartCoroutine(OutOfBoxUtils.PlayBIAsync(() => {
                                                    Buddy.Navigation.Run<DisplacementStrategy>().Rotate(-60F, 70F, () => {
                                                        StartCoroutine(OutOfBoxUtils.PlayBIAsync(() => {
                                                            mBehaviour.PhaseDropDown.value = 4;
                                                        }));
                                                    });
                                                }));
                                            });
                                        }));
                                    });
                                }));
                            });
                        }));
                    });
            });
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StopAllCoroutines();
            Buddy.Navigation.Stop();
            Buddy.Actuators.Wheels.Stop();
            Buddy.Actuators.Head.Stop();
            Buddy.Behaviour.Stop();
            Buddy.Vocal.StopAndClear();
            Buddy.Behaviour.ResetMood();
        }

    }
}
