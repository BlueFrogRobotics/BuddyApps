using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class VocalTest : AModuleTest
    {
        public override string Name
        {
            get
            {
                return (Buddy.Resources.GetString("vocal"));
            }
        }

        private bool mTestInProcess;

        public override void InitTestList()
        {
            mAvailableTest = new List<string>();
            mAvailableTest.Add("say");
            return;
        }

        public override void InitPool()
        {
            mTestPool = new Dictionary<string, TestRoutine>();
            mTestPool.Add("say", SayTest);
            return;
        }

        public VocalTest()
        {
            mTestInProcess = false;
        }

        // All TestRoutine of this module:

        #region SAY_TESTS
        public IEnumerator SayTest()
        {
            //  --- INIT ---
            mTestInProcess = true;

            //  --- CODE ---
            DebugColor("SayTest work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }
        #endregion
    }
}
