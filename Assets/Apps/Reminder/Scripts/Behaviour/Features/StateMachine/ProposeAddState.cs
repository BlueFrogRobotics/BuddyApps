using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy.UI;

namespace BuddyApp.Reminder
{
    public class ProposeAddState : AStateMachineBehaviour
    {

        override public void Start()
        {

        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.TextToSpeech.Say("Voulez vous ajouter un mémo");
            ReminderManager.Command.Intent = Intent.NONE;
            Toaster.Display<BinaryQuestionToast>().With(
                "Voulez vous ajouter un mémo",
                () => Trigger("Add"),
                () => QuitApp()
            );
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }


    }
}