using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.BuddyLab
{
    public sealed class BLMenu : AStateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
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
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }
    }
}

