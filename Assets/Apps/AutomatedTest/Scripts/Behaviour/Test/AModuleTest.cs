using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.AutomatedTest
{
    public abstract class AModuleTest : MonoBehaviour //ask CORE If OK
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

        // Result Pool - It contain all last test perform with their result. (Just boolean for now)
        protected Dictionary<string, bool> mResultPool;

        //  --- Method ---

        // Getter for AvailableTest
        public List<string> GetAvailableTest() { return mAvailableTest; }

        // This function have to create and fill the TestPool
        public abstract void InitPool();

        // This function have to create and fill the AvailableTest List
        public abstract void InitTestList() ;


        public AModuleTest()
        {
            mSelectedKey = new List<string>();
            mResultPool = new Dictionary<string, bool>();
            InitTestList();
            InitPool();
        }

        protected void DebugColor(string msg, string color = null)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }

        public IEnumerator RunSelectedTest()
        {
            // Clear all precedent result
            mResultPool.Clear();

            // Run each TestRoutine
            foreach (string lTest in mSelectedKey)
            {
                if (mTestPool.ContainsKey(lTest))
                    yield return mTestPool[lTest]();
            }
        }

        public Dictionary<string, bool> GetResult()
        {
            if (mResultPool.Count == 0)
                return null;
            return new Dictionary<string, bool>(mResultPool);
        }

        public bool ContainSelectedTest(string iTest)
        {
            return mSelectedKey.Contains(iTest);
        }

        public void AddSelectedTest(string iTest)
        {
            if (!mSelectedKey.Contains(iTest))
                mSelectedKey.Add(iTest);
        }

        public void ClearSelectedTest()
        {
            mSelectedKey.Clear();
        }

        public void RemoveSelectedTest(string iTest)
        {
            mSelectedKey.Remove(iTest);
        }

        public int SelectedTestLength()
        {
            return mSelectedKey.Count;
        }

        public void SelectAllTest()
        {
            mSelectedKey.Clear();
            foreach (string lTest in mAvailableTest)
            {
                if (mTestPool.ContainsKey(lTest) && !mSelectedKey.Contains(lTest))
                    mSelectedKey.Add(lTest);
            }
        }
    }
}
