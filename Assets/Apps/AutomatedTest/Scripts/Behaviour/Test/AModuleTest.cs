using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.AutomatedTest
{
    public abstract class AModuleTest
    {
        public abstract string Name { get; }

        // Define a TestRoutine function
        public delegate bool TestRoutine();

        // Test pool - Useful to call directly the function throught a key
        protected Dictionary<string, TestRoutine> mTestPool = null;

        // All available test for this module
        protected List<string> mAvailableTest = null;

        // Storage of each test to perform, selected by the user.
        public List<string> mSelectedTests { get; set; }

        // Getter for AvailableTest
        public abstract List<string> GetAvailableTest();

        // This function have to create and fill the TestPool
        public abstract void InitPool();

        // This function have to create and fill the AvailableTest List
        public abstract void InitTestList();


        public AModuleTest()
        {
            mSelectedTests = new List<string>();
            InitTestList();
            InitPool();
        }

        public void RunSelectedTest()
        {
            // Run each test
            foreach (string lTest in mSelectedTests)
            {
                if (mTestPool.ContainsKey(lTest))
                    mTestPool[lTest]();
            }
            // feedback here ?

            // Clear the list
            mSelectedTests.Clear();
        }

        public void SelectAllTest()
        {
            mSelectedTests.Clear();
            foreach (string lTest in mAvailableTest)
            {
                if (mTestPool.ContainsKey(lTest))
                    mSelectedTests.Add(lTest);
            }
        }
    }
}
