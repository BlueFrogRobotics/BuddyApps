using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class MotionTest: AModuleTest
    {
        public override string Name
        {
            get
            {
                return ("Motion Test");
            }
        }

        private bool mTestInProcess;

        public override void InitTestList()
        {
            mAvailableTest = new List<string>();
            mAvailableTest.Add("Rotation");
            mAvailableTest.Add("RotationYes");
            mAvailableTest.Add("RotationNo");
            mAvailableTest.Add("MoveForward");
            mAvailableTest.Add("MoveBackward");
            mAvailableTest.Add("ObstacleStop");
            mAvailableTest.Add("ObstacleAvoidance");
            return;
        }

        public override void InitPool()
        {
            mTestPool = new Dictionary<string, TestRoutine>();
            mTestPool.Add("Rotation", RotationTests);
            mTestPool.Add("RotationYes", RotationYesTests);
            mTestPool.Add("RotationNo", RotationNoTests);
            mTestPool.Add("MoveForward", MoveForwardTests);
            mTestPool.Add("MoveBackward", MoveBackwardTests);
            mTestPool.Add("ObstacleStop", ObstacleStopTests);
            mTestPool.Add("ObstacleAvoidance", ObstacleAvoidanceTests);
            return;
        }

        public MotionTest()
        {
            mTestInProcess = false;
        }

        // All TestRoutine of this module:

        #region ROTATION
        public IEnumerator RotationTests()
        {
            //  --- INIT ---
            mTestInProcess = true;

            // Show UI
            Buddy.GUI.Toaster.Display<ParameterToast>().With(iBuilder =>
            {
                TText lText = iBuilder.CreateWidget<TText>();
                lText.SetCenteredLabel(true);
                lText.SetLabel("Rotation test ...");

                TButton lRepeatButton = iBuilder.CreateWidget<TButton>();
                lRepeatButton.SetCenteredLabel(true);
                lRepeatButton.SetLabel("Run Test");
                lRepeatButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_repeat"));
                lRepeatButton.OnClick.Add(() =>
                {
                    if (!Buddy.Navigation.IsBusy)
                        Buddy.Navigation.Run<DisplacementStrategy>().Rotate(180F, 60F);
                });

                TButton lStopButton = iBuilder.CreateWidget<TButton>();
                lStopButton.SetCenteredLabel(true);
                lStopButton.SetLabel("Stop Test");
                lStopButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_stop"));
                lStopButton.OnClick.Add(() =>
                {
                    DebugColor("-- stop --", "blue");
                    Buddy.Navigation.Stop();
                });
            },
            // Action OnClickLeft
            () =>
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                // FileLog Rotation Test fail
            },
            // Text Left
            "KO",
            // Action OnClickRight
            () =>
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                // FileLog Rotation Test success
            },
            // Text Right
            "OK");

            //  --- CODE ---
            DebugColor("RotationTest work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Toaster.Hide();
            yield return new WaitUntil(() => Buddy.GUI.Toaster.IsBusy);
        }
        #endregion

        public IEnumerator RotationYesTests()
        {
            DebugColor("RotationYes not implemented yet", "blue");
            yield break;
        }

        public IEnumerator RotationNoTests()
        {
            DebugColor("RotationNo not implemented yet", "blue");
            yield break;
        }

        public IEnumerator MoveForwardTests()
        {
            DebugColor("MoveForward not implemented yet", "blue");
            yield break;
        }

        public IEnumerator MoveBackwardTests()
        {
            DebugColor("MoveBackward not implemented yet", "blue");
            yield break;
        }

        public IEnumerator ObstacleStopTests()
        {
            DebugColor("ObstacleStop not implemented yet", "blue");
            yield break;
        }

        public IEnumerator ObstacleAvoidanceTests()
        {
            DebugColor("ObstacleAvoidance not implemented yet", "blue");
            yield break;
        }
    }
}