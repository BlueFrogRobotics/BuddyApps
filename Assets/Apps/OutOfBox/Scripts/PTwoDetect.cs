using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PTwoDetect : AStateMachineBehaviour
    {


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            HeadPositionDetect();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private void HeadPositionDetect()
        {
            if (Buddy.Actuators.Head.No.Angle != 0F)
                Buddy.Actuators.Head.No.ResetPosition();
            if(Buddy.Actuators.Head.Yes.Angle != 5F)
                Buddy.Actuators.Head.Yes.SetPosition(5F, 45F);
        }



    }

}
