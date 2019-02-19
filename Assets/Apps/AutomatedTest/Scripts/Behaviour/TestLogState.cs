using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class TestLogState : AStateMachineBehaviour
    {
         /*
          *  -- TestLogFile Ideas --
          *  logFile_[date]_[time].txt:
          *  SemiAutomated Test [date] - [time]
          *  [RobotName] - [RobotUID] - [BRE]
          *  
          *  --- [ModuleName] : ---
          *  Test [TestName] :
          *  feedback
          *  feedback
          *  
          *  Test [TestName] :
          *  feedback
          *  feedback
          *  
          *  --- [ModuleName] : ---
          *  Test [TestName] :
          *  feedback
          *  feedback 
          */

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Trigger("MenuTrigger");
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Clear all selected test before going back to the menu.
            for (AutomatedTestData.MODULES lModule = 0; lModule < AutomatedTestData.MODULES.E_NB_MODULE; lModule++)
            {
                if (AutomatedTestData.Instance.Modules.ContainsKey(lModule))
                    AutomatedTestData.Instance.Modules[lModule].GetSelectedKey().Clear();
            }
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}
