using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.BuddyLab
{
    public class BLMenu : AStateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Toaster.Display<VerticalListToast>().With( (iBuilder) =>
            {
                TVerticalListBox lBox1 = iBuilder.CreateBox();
                lBox1.SetLabel(Buddy.Resources.GetString("menusimple"));
                lBox1.OnClick.Add(() => {
                    Trigger("MakeProject");
                    Buddy.GUI.Toaster.Hide();
                });
                TVerticalListBox lBox2 = iBuilder.CreateBox();
                lBox2.SetLabel(Buddy.Resources.GetString("menuopen"));
                lBox2.OnClick.Add(() => {
                    Trigger("StartOpen");
                    Buddy.GUI.Toaster.Hide();
                });
                TVerticalListBox lBox3 = iBuilder.CreateBox();
                lBox3.SetLabel(Buddy.Resources.GetString("menututo"));
                lBox3.OnClick.Add(() => {
                    Trigger("StartTuto");
                    Buddy.GUI.Toaster.Hide();
                });
            }
                
            );
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

