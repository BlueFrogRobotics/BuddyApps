using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

/*
 *      -- Ideas (Outdated) --
 *  
 *  Fill a dictionary of string as key and a function TestRoutine as value.
 *  During selection state, fill a list of key, the order has to be deterministic.
 *  Exemple: On a list of available test (A, B, C, D, E, F) if i choose randomly F, A, C, E, D, all this test must run in alphabetic order.
 *  Then at RunTestState, just browse the list of test and run it, retrieving the associate function with the dico.
 *  
 *      -- Actually --
 *      
 *  InitState       - Fill the dictionary of ModuleTest object.
 *  MenuState       - Display each available module of test
 *  ModuleState     - Display all available test for the selected module - OnToggle add/remove the test to the SelectedTest list of the module.
 *  RunTestState    - Browse all Module in dictionary, and run all selected test for each module.
 *  TestLogState    - Display a basic log of each test perform, OK - KO. BONUS: Add a button that ask an email, and send to it a logfile with more feedback ?
 *  
 */

namespace BuddyApp.AutomatedTest
{
    public sealed class InitState : AStateMachineBehaviour
    {
        //  Create & Fill the Dictionary of available module here

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GameObject lTestManager = GetGameObject(0);
            AModuleTest lModule;

            // Check if a TestManager exist
            if (lTestManager == null)
            {
                Debug.LogError("Please attach a TestManager object to AppBehaviour.");
                return;
            }

            ModuleManager.GetInstance().Initialize();

            // --- Get the MotionTest Script --
            if ((lModule = lTestManager.GetComponent<MotionTest>()) == null)
            {
                Debug.LogError("Please attach a MotionTest Script to the TestManager.");
                return;
            }
            ModuleManager.GetInstance().Modules.Add(ModuleManager.MODULES.E_MOTION, lModule);

            // --- Get the CameraTest Script --
            if ((lModule = lTestManager.GetComponent<CameraTest>()) == null)
            {
                Debug.LogError("Please attach a CameraTest Script to the TestManager.");
                return;
            }
            ModuleManager.GetInstance().Modules.Add(ModuleManager.MODULES.E_CAMERA, lModule);

            // --- Get the VocalTest Script --
            if ((lModule = lTestManager.GetComponent<VocalTest>()) == null)
            {
                Debug.LogError("Please attach a VocalTest Script to the TestManager.");
                return;
            }
            ModuleManager.GetInstance().Modules.Add(ModuleManager.MODULES.E_VOCAL, lModule);

            // --- Get the GuiTest Script --
            // not implemented yet

            Trigger("MenuTrigger");
        }
    }
}