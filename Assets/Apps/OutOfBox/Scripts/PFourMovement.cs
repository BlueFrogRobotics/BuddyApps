using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PFourMovement : AStateMachineBehaviour
    {        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            OutOfBoxUtils.WaitTimeAsync(0.5F, () => { Buddy.Vocal.Say("pfourfirststep"); });

            Buddy.Navigation.Run<DisplacementStrategy>().Move(1.5F, 70F, () =>
            {
                StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                {
                    Buddy.Navigation.Run<DisplacementStrategy>().Rotate(60F, 70F, () =>
                    {
                        StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                        {
                            Buddy.Navigation.Run<DisplacementStrategy>().Move(-1.5F, 70F, () =>
                            {
                                StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                                {
                                    Buddy.Navigation.Run<DisplacementStrategy>().Rotate(300F, 70F, () =>
                                    {
                                        StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                                        {
                                            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(-60F, 70F, () =>
                                            {
                                                StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                                                {
                                                    OutOfBoxData.Instance.Phase = OutOfBoxData.PhaseId.PhaseFive;
                                                    Trigger("Base");
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
        }
    }
}
