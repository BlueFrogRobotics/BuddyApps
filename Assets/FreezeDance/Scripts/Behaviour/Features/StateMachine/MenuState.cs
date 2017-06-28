using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.FreezeDance
{
    public class MenuState : AStateMachineBehaviour
    {

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.TextToSpeech.SayKey("nextaction");
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Toaster.Display<ChoiceToast>().With(
                   "menu",
                   new ButtonInfo()
                   {
                       Label = "start",
                       OnClick = () => Trigger("Start")
                   },
                   new ButtonInfo()
                   {
                       Label = "help",
                       OnClick = () => Trigger("Start")
                   },
                   new ButtonInfo()
                   {
                       Label = "quit",
                       OnClick = () => QuitApp()
                   });
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}