using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public abstract class AModuleTest : MonoBehaviour
    {
        // Manual mode need user to press OK or KO after a test.
        // Auto mode, the user can still press OK or KO, but when it's possible the test result is automatically save.
        public enum TestMode
        {
            M_MANUAL,
            M_AUTO,
        };

        public abstract string Name { get; }

        // TestMode for this module
        private TestMode mTestMode;

        // Getter & setter
        public TestMode Mode
        {
            get { return mTestMode; }
            set { mTestMode = value; }
        }

        // Define a TestRoutine function
        public delegate IEnumerator TestRoutine();

        // Test pool - Useful to call directly the function throught a key
        // Be careful each element have to correspond with a key in the language dictionary
        protected Dictionary<string, TestRoutine> mTestPool = null;

        // All available test for this module
        // Be careful each element have to correspond with a key in the language dictionary
        protected List<string> mAvailableTest = null;

        // Result Pool - It contain all last test perform with their result.
        // If a test failed mErrorPool is fill with error feedback
        protected Dictionary<string, bool> mResultPool;

        // Error Pool - It contain all error feedback of failed test. (user's feedback and/or system's feedback)
        protected Dictionary<string, string> mErrorPool;

        protected bool mTestInProcess;
        
        // Storage of each test to perform, selected by the user.
        private List<string> mSelectedKey;

        private float mTimeElapsed;

        //  --- Method ---

        // Getter for AvailableTest
        public List<string> GetAvailableTest() { return mAvailableTest; }

        // This function have to create and fill the TestPool - This keys must have the same name of keys dictionnary language for the test
        public abstract void InitPool();

        // This function have to create and fill the AvailableTest List - The list contains all mTestPool keys.
        public abstract void InitTestList();

        public AModuleTest()
        {
            mTestMode = TestMode.M_MANUAL;
            mSelectedKey = new List<string>();
            mResultPool = new Dictionary<string, bool>();
            mErrorPool = new Dictionary<string, string>();
            mTestInProcess = false;
            InitTestList();
            InitPool();
        }

        //  -------------------- INTERNAL USE (LOCAL + SONS) --------------------

        protected void DebugColor(string msg, string color = null)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }

        protected void ResetTimeOut_deprecated()
        {
            mTimeElapsed = 0F;
        }

        protected IEnumerator TimeOut_deprecated(float iTimer, System.Action iOnEndTimer)
        {
            mTimeElapsed = 0;
            // This implementation allow us to reset the timeout, this way: mTimeElapsed = 0F;
            while (mTimeElapsed < iTimer)
            {
                yield return null;
                mTimeElapsed += Time.deltaTime;
            }

            // Hide header to avoid overlapping with Header & dialoger title
            Buddy.GUI.Header.HideTitle();
            // Warn user the timeout is reached
            Buddy.GUI.Dialoger.Display<IconToast>("Test: Timeout is reached.").With(Buddy.Resources.Get<Sprite>("os_icon_clock"));
            // Wait a little before hide the popup
            yield return new WaitForSeconds(2F);
            Buddy.GUI.Dialoger.Hide();
            yield return new WaitWhile(() => Buddy.GUI.Dialoger.IsBusy);
            iOnEndTimer();
        }

        //  -------------------- INTERFACE (USE 'TestInterface' CLASS IN FUTURE ?) --------------------

        /*
        **  This function display the common interface and a VideoStreamToast if a texture is provide.
        */
        protected void DisplayTestUi(string iTestLabel, System.Action iOnFailTest, System.Action iOnSuccessTest, Texture2D iTextureCam = null)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString(iTestLabel));
            if (iTextureCam)
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(iTextureCam);

            // Display of the common interface
            CommonInterface(iTestLabel, iOnFailTest, iOnSuccessTest);
        }

        /*
        **  Overload with no callback
        */
        protected void DisplayTestUi(string iTestLabel, Texture2D iTextureCam = null)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString(iTestLabel));
            if (iTextureCam)
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(iTextureCam);

            // Display of the common interface
            CommonInterface(iTestLabel, null, null);
        }

        /*
        **  This function display the common interface and a toaster with text & stop/repeat test button.
        */
        protected void DisplayTestUi(string iTestLabel, System.Action iOnRepeat, System.Action iOnStopTest, System.Action iOnFailTest, System.Action iOnSuccessTest, string iKeyText = null)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString(iTestLabel));
            Buddy.GUI.Toaster.Display<ParameterToast>().With(iBuilder =>
            {
                TText lText = iBuilder.CreateWidget<TText>();
                lText.SetCenteredLabel(true);
                if (string.IsNullOrEmpty(iKeyText))
                    lText.SetLabel(Buddy.Resources.GetString("testinprogress"));
                else
                    lText.SetLabel(Buddy.Resources.GetString(iKeyText));
            },
            // Action OnClickLeft
            iOnStopTest,
            // Text Left
            Buddy.Resources.Get<Sprite>("os_icon_stop"),
            // Action OnClickRight
            iOnRepeat,
            // Text Right
            Buddy.Resources.Get<Sprite>("os_icon_repeat"));

            // Display of the common interface
            CommonInterface(iTestLabel, iOnFailTest, iOnSuccessTest);
        }

        /*
        **  This function display the common interface: Footer with Success/Fail button
        */
        private void CommonInterface(string iTestLabel, System.Action iOnFailTest, System.Action iOnSuccessTest)
        {
            // Fail button
            FButton lFailButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lFailButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_close"));
            lFailButton.SetBackgroundColor(Color.red);
            lFailButton.SetIconColor(Color.white);
            // Add the test callback
            lFailButton.OnClick.Add(iOnFailTest);
            // Common test callback
            lFailButton.OnClick.Add(() =>
            {
                // Store test result
                mResultPool.Add(iTestLabel, false);
                // Store test feedback
                mErrorPool.Add(iTestLabel, "From tester");
                // Stop the test
                mTestInProcess = false;
            });

            // Success button
            FButton lSuccessButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lSuccessButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));
            lSuccessButton.SetBackgroundColor(Color.green);
            lSuccessButton.SetIconColor(Color.white);
            // Add the test callback
            lSuccessButton.OnClick.Add(iOnSuccessTest);
            // Common test callback
            lSuccessButton.OnClick.Add(() =>
            {
                // Store test result
                mResultPool.Add(iTestLabel, true);
                // Stop the test
                mTestInProcess = false;
            });
        }

        //  -------------------- EXTERNAL USE --------------------

        public IEnumerator RunSelectedTest()
        {
            // Clear all precedent result
            mResultPool.Clear();
            mErrorPool.Clear();

            // Run each TestRoutine
            foreach (string lTest in mSelectedKey)
            {
                // Run test
                if (mTestPool.ContainsKey(lTest))
                {
                    mTestInProcess = true;
                    DebugColor("RUN TEST", "blue");
                    yield return mTestPool[lTest]();
                }

                // Hide Interface
                Buddy.GUI.Header.HideTitle();
                Buddy.GUI.Footer.Hide();
                Buddy.GUI.Toaster.Hide();
                yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
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

        //  -------------------- RESULT POOL METHOD --------------------

        public Dictionary<string, bool> GetResult()
        {
            if (mResultPool == null || mResultPool.Count == 0)
                return null;
            return new Dictionary<string, bool>(mResultPool);
        }

        //  -------------------- ERROR POOL METHOD --------------------

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

        //  -------------------- SELECTED KEY METHOD --------------------

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
