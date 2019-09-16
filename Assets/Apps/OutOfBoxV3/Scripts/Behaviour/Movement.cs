using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBoxV3
{
    public class Movement : AStateMachineBehaviour
    {


        public override void Start()
        {
            base.Start();

            mBehaviour = GetComponent<OutOfBoxV3Behaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("TRANSITION"));
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Actuators.Head.No.ResetPosition();
            Buddy.Actuators.Head.Yes.ResetPosition();
            Buddy.Vocal.SayKey("pfourfirststep", (EndSpeaking) => {
                if (!EndSpeaking.IsInterrupted)
                    Buddy.Navigation.Run<DisplacementStrategy>().Move(1.5F, 70F, () => {
                        StartCoroutine(OutOfBoxUtilsVThree.PlayBIAsync(() => {
                            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(60F, 70F, () => {
                                StartCoroutine(OutOfBoxUtilsVThree.PlayBIAsync(() => {
                                    Buddy.Navigation.Run<DisplacementStrategy>().Move(-1.5F, 70F, () => {
                                        StartCoroutine(OutOfBoxUtilsVThree.PlayBIAsync(() => {
                                            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(300F, 70F, () => {
                                                StartCoroutine(OutOfBoxUtilsVThree.PlayBIAsync(() => {
                                                    Buddy.Navigation.Run<DisplacementStrategy>().Rotate(-60F, 70F, () => {
                                                        StartCoroutine(OutOfBoxUtilsVThree.PlayBIAsync(() => {
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
    }
}
