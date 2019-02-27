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
            foreach (KeyValuePair<AutomatedTestData.MODULES, AModuleTest> lModule in AutomatedTestData.Instance.Modules)
            {
                Debug.LogWarning("-- RUN SELECTED TEST OF:" + lModule.Key.ToString() + " --");
                yield return lModule.Value.RunSelectedTest();
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