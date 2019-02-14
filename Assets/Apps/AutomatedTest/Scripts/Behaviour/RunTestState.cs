using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.AutomatedTest
{
    public sealed class RunTestState : AStateMachineBehaviour
    {
        // Launch all selected test here, maybe an entire state is overkill for that

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            for (AutomatedTestData.MODULES i = 0; i < AutomatedTestData.MODULES.E_NB_MODULE; i++)
            {
                if (AutomatedTestData.Instance.Modules.ContainsKey(i))
                        AutomatedTestData.Instance.Modules[i].RunSelectedTest();
            }
            Trigger("TestLogTrigger");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}

