using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.TestCoroutineDisplay
{
    public class replay : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("is busy : " + Buddy.GUI.Toaster.IsBusy);
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {

                iBuilder.CreateWidget<TText>().SetLabel("rejouer ?");
            },

                () => { PressedNo(); Buddy.GUI.Toaster.Hide(); },"no",

                () => { PressedYes(); Buddy.GUI.Toaster.Hide(); }, "yes"

            );
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private void PressedNo()
        {
            Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
            RedoTakePose();
        }

        private void PressedYes()
        {
            Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
            RedoTakePose();
        }

        private void RedoTakePose()
        {
            Trigger("start");
        }
    }

}

