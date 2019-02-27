using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.AutomatedTest
{
    public abstract class AModuleTest : MonoBehaviour
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

        // Result Pool - It contain all last test perform with their result.
        // If a test failed mErrorPool is fill with error feedback
        protected Dictionary<string, bool> mResultPool;

        // Error Pool - It contain all error feedback of failed test. (user's feedback and/or system's feedback)
        protected Dictionary<string, string> mErrorPool;

        protected float mTimer;

        //  --- Method ---

        // Getter for AvailableTest
        public List<string> GetAvailableTest() { return mAvailableTest; }

        // This function have to create and fill the TestPool - This keys must have the same name of keys dictionnary language for the test
        public abstract void InitPool();

        // This function have to create and fill the AvailableTest List - The list contains all mTestPool keys.
        public abstract void InitTestList();

        public AModuleTest()
        {
            mSelectedKey = new List<string>();
            mResultPool = new Dictionary<string, bool>();
            mErrorPool = new Dictionary<string, string>();
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

        public IEnumerator TimeOut(float iTimer, System.Action iOnEndTimer)
        {
            mTimer = 0;
            while (mTimer < iTimer)
            {
                yield return null;
                mTimer += Time.deltaTime;
            }
            iOnEndTimer();
        }

        public IEnumerator RunSelectedTest()
        {
            // Clear all precedent result
            mResultPool.Clear();
            mErrorPool.Clear();

            // Run each TestRoutine
            foreach (string lTest in mSelectedKey)
            {
                if (mTestPool.ContainsKey(lTest))
                    yield return mTestPool[lTest]();
            }
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

        // --- mResultPool Method ---

        public Dictionary<string, bool> GetResult()
        {
            if (mResultPool == null || mResultPool.Count == 0)
                return null;
            return new Dictionary<string, bool>(mResultPool);
        }

        // --- mErrorPool Method ---

        public string GetErrorByTest(string iTest)
        {
            if (mErrorPool == null || mErrorPool.Count == 0)
                return "NoFeedback";
            if (ContainFeedbackError(iTest))
                return mErrorPool[iTest];
            return "NoFeedback";
        }

        public bool ContainFeedbackError(string iKey)
        {
            if (mErrorPool == null || mErrorPool.Count == 0)
                return false;
            return mErrorPool.ContainsKey(iKey);
        }

        public void ClearError()
        {
            mErrorPool.Clear();
        }

        // --- mSelectedKey Method ---

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
    }
}
