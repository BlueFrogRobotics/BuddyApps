using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class POneAwakening : AStateMachineBehaviour
    {

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OutOfBoxUtils.DebugColor("FIRST PHASE : ", "blue");
            Buddy.Actuators.Head.No.ResetPosition();
            Buddy.Actuators.Head.Yes.SetPosition(-9F, 45F , (iPos) =>
            {
                // Asleep
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.FALL_ASLEEP, false);

                StartCoroutine(OutOfBoxUtils.WaitTimeAsync(2F, () =>
                {
                    // Lifting head
                    Buddy.Actuators.Head.Yes.SetPosition(5F, 45F);

                    Buddy.Behaviour.Face.PlayEvent(FacialEvent.AWAKE, null, (iFacialEvent) =>
                    {
                        Buddy.Vocal.Say(Buddy.Resources.GetString("phaseonenewfriend"), (iSpeechOutput) =>
                        {
                            // Play BI here
                            StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                            {
                                // Run discovering after speech
                                Buddy.Vocal.Say(Buddy.Resources.GetString("phaseonearound"), (iOut) => {
                                    OutOfBoxData.Instance.Phase = OutOfBoxData.PhaseId.PhaseTwo;
                                    Trigger("Base");});
                            }));
                        });
                    });
                }));
            });
        }
    }
}

