using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.AutomatedTest
{
    public abstract class AModuleTest
    {
        public abstract string Name { get; }

        // Define a TestRoutine function
        public delegate IEnumerator TestRoutine();

        // Test pool - Useful to call directly the function throught a key
        protected Dictionary<string, TestRoutine> mTestPool = null;

        // All available test for this module
        protected List<string> mAvailableTest = null;

        // Storage of each test to perform, selected by the user.
        private  List<string> mSelectedKey;

        // Getter for mSelectedKey
        public List<string> GetSelectedKey() { return mSelectedKey; }

        // Getter for AvailableTest
        public List<string> GetAvailableTest() { return mAvailableTest; }

        // This function have to create and fill the TestPool
        public abstract void InitPool();

        // This function have to create and fill the AvailableTest List
        public abstract void InitTestList() ;


        public AModuleTest()
        {
            mSelectedKey = new List<string>();
            InitTestList();
            InitPool();
            Debug.LogWarning("CONSTRUCTOR 2");
        }


        public IEnumerator RunSelectedTest()
        {
            foreach (string lTest in mSelectedKey)
            {
                Debug.LogWarning("KEY:" + lTest);
                if (mTestPool.ContainsKey(lTest))
                    yield return mTestPool[lTest]();
            }
        }

        public void SelectAllTest()
        {
            mSelectedKey.Clear();
            foreach (string lTest in mAvailableTest)
            {
                if (mTestPool.ContainsKey(lTest))
                    mSelectedKey.Add(lTest);
            }
        }
    }
}
