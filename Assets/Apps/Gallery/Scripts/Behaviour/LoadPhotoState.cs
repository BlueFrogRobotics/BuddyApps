using BlueQuark;

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    public class LoadPhotoState : AStateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On State Enter...");

            // TODO: Check possible errors
            Trigger("TRIGGER_PHOTO_UPLOADED");
        }
		
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "On State Exit...");
        }
    }
}
