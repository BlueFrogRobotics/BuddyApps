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
            StartCoroutine(RunTest());
        }


        private IEnumerator RunTest()
        {
            for (AutomatedTestData.MODULES lEntry = 0; lEntry < AutomatedTestData.MODULES.E_NB_MODULE; lEntry++)
            {
                Debug.LogWarning("-- RUN SELECTED TEST OF:" + lEntry.ToString() + " --");
                if (AutomatedTestData.Instance.Modules.ContainsKey(lEntry))
                    yield return AutomatedTestData.Instance.Modules[lEntry].RunSelectedTest();
            }
            Trigger("TestLogTrigger");
            Debug.LogWarning("-- END RUN TEST --");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}

