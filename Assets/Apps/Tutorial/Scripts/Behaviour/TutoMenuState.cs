using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Tutorial
{
    /// <summary>
    /// State for the menu : we display a menu with a vertical list toast. The vertical list toast allows the user to display a list of box vertically.
    /// And you have access to many parameters for these box like icons / actions / color of the writing...
    /// </summary>
    public class TutoMenuState : StateMachineBehaviour
    {

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {

                TVerticalListBox lBox = iBuilder.CreateBox();

                lBox.OnClick.Add(() => { Debug.Log("Click Box"); });

                lBox.SetLabel("Left", "Right label");

                //lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));

                lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));

                lBox.LeftButton.OnClick.Add(() => { Debug.Log("Click Left"); });



               // TRightSideButton lButton = lBox.CreateRightButton();

               //// lButton.SetIcon(Buddy.Resources.Get<Sprite>("icon"));

               // lButton.OnClick.Add(() => {

               //     Debug.Log("Click right");

               //     Buddy.GUI.Toaster.Hide();

               // });

            });
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
    }
}

