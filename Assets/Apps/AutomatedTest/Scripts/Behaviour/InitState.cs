﻿using System.Collections;
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
 *  [Module]State   - Display all available test for this module - OnToggle add/remove the test to the SelectedTest list of the module.
 *  RunTestState    - Browse all Module in dictionary, and run all selected test for each module.
 *  TestLogState    - Display a basic log of each test perform, OK - KO. BONUS: Add a button that ask an email, and send to it a logfile with more feedback ?
 */

namespace BuddyApp.AutomatedTest
{
    public sealed class InitState : AStateMachineBehaviour
    {
        //  Create & Fill the Dictionary of available module here

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Create the dictionary
            AutomatedTestData.Instance.Modules = new Dictionary<AutomatedTestData.MODULES, AModuleTest>();

            // Fill the dictionary of module - Only the camera for now
            AutomatedTestData.Instance.Modules.Add(AutomatedTestData.MODULES.E_CAMERA, new CameraTest());
            AutomatedTestData.Instance.Modules.Add(AutomatedTestData.MODULES.E_MOTION, new MotionTest());

            Trigger("MenuTrigger");
        }
    }
}