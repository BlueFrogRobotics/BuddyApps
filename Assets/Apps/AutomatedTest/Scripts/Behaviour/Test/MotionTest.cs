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
                return (Buddy.Resources.GetString("motions"));
            }
        }

        private bool mTestInProcess;

        private bool mCoroutineInProcess;

        //  --- Method ---

        public override void InitTestList()
        {
            mAvailableTest = new List<string>();
            mAvailableTest.Add("rotation");
            mAvailableTest.Add("rotationyes");
            mAvailableTest.Add("rotationno");
            mAvailableTest.Add("moveforward");
            mAvailableTest.Add("movebackward");
            mAvailableTest.Add("obstaclestop");
            mAvailableTest.Add("obstacleavoidance");
            return;
        }

        public override void InitPool()
        {
            mTestPool = new Dictionary<string, TestRoutine>();
            mTestPool.Add("rotation", RotationTests);
            mTestPool.Add("rotationyes", RotationYesTests);
            mTestPool.Add("rotationno", RotationNoTests);
            mTestPool.Add("moveforward", MoveForwardTests);
            mTestPool.Add("movebackward", MoveBackwardTests);
            mTestPool.Add("obstaclestop", ObstacleStopTests);
            mTestPool.Add("obstacleavoidance", ObstacleAvoidanceTests);
            return;
        }

        public MotionTest()
        {
            mTestInProcess = false;
        }

        // Common interface for all MotionTest - TODO: Rework interface, use Footer for OK/KO button (see camera test)
        #region COMMON_UI
        private void DisplayTestUi(string iTestLabel, System.Action iOnRepeat, System.Action iOnStopTest, System.Action iOnFailTest, System.Action iOnSuccessTest)
        {
            // timer = 0
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString(iTestLabel));
            Buddy.GUI.Toaster.Display<ParameterToast>().With(iBuilder =>
                {
                    // timer = 0
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetCenteredLabel(true);
                    lText.SetLabel(Buddy.Resources.GetString("testinprogress"));

                    TButton lRepeatButton = iBuilder.CreateWidget<TButton>();
                    lRepeatButton.SetCenteredLabel(true);
                    lRepeatButton.SetLabel(Buddy.Resources.GetString("runtest"));
                    lRepeatButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_repeat"));
                    lRepeatButton.OnClick.Add(iOnRepeat);

                    TButton lStopButton = iBuilder.CreateWidget<TButton>();
                    lStopButton.SetCenteredLabel(true);
                    lStopButton.SetLabel(Buddy.Resources.GetString("stop"));
                    lStopButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_stop"));
                    lStopButton.OnClick.Add(iOnStopTest);
                },
                // Action OnClickLeft
                iOnFailTest,
                // Text Left
                Buddy.Resources.GetString("fail"),
                // Action OnClickRight
                iOnSuccessTest,
                // Text Right
                Buddy.Resources.GetString("success"));
        }
        #endregion

        // All TestRoutine of this module:

        #region ROTATION
        public IEnumerator RotationTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            // Show UI - And Implement all Ui callback
            DisplayTestUi("rotation",
            () =>   // --- OnClickRepeat ---
            {
                if (!Buddy.Navigation.IsBusy)
                    Buddy.Navigation.Run<DisplacementStrategy>().Rotate(180F, 60F);
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestFail ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("rotation", false);
                mErrorPool.Add("rotation", "From tester");
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("rotation", true);
            });

            //  --- CODE ---
            DebugColor("RotationTest work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }
        #endregion

        #region ROTATION_YES
        public IEnumerator RotationYesTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            mCoroutineInProcess = false;
            // Show UI - And Implement all Ui callback
            DisplayTestUi("rotationyes",
            () =>   // --- OnClickRepeat ---
            {
                if (!mCoroutineInProcess)
                {
                    mCoroutineInProcess = true;
                    StartCoroutine("RotationYesSequence");
                }
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                mCoroutineInProcess = false;
                StopCoroutine("RotationYesSequence");
                Buddy.Actuators.Head.Yes.ResetPosition();
            },
            () =>   // --- OnClickTestFail ---
            {
                mTestInProcess = false;
                mResultPool.Add("rotationyes", false);
                mErrorPool.Add("rotationyes", "From tester");
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                mResultPool.Add("rotationyes", true);
            });

            //  --- CODE ---
            DebugColor("RotationYes work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            mCoroutineInProcess = false;
            StopCoroutine("RotationYesSequence");
            Buddy.Actuators.Head.Yes.ResetPosition();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }

        private IEnumerator RotationYesSequence()
        {
            DebugColor("-- HeadYes target: 30 --", "red");
            Buddy.Actuators.Head.Yes.SetPosition(30F);

            yield return new WaitWhile(() =>
            {
                if (Buddy.Actuators.Head.Yes.Angle >= 27F)
                    return false;
                return true;
            });

            DebugColor("-- HeadYes target: -30 --", "red");
            Buddy.Actuators.Head.Yes.SetPosition(-30F);
            yield return new WaitWhile(() =>
            {
                if (Buddy.Actuators.Head.Yes.Angle <= -27F)
                    return false;
                return true;
            });

            DebugColor("-- HeadYes target: 0 --", "red");
            Buddy.Actuators.Head.Yes.SetPosition(0F);
            yield return new WaitWhile(() =>
            {
                if (Buddy.Actuators.Head.Yes.Angle <= 3F && Buddy.Actuators.Head.Yes.Angle >= -3F)
                    return false;
                return true;
            });
            mCoroutineInProcess = false;
        }
        #endregion

        #region ROTATION_NO
        public IEnumerator RotationNoTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            mCoroutineInProcess = false;
            // Show UI - And Implement all Ui callback
            DisplayTestUi("rotationno",
            () =>   // --- OnClickRepeat ---
            {
                if (!mCoroutineInProcess)
                {
                    mCoroutineInProcess = true;
                    StartCoroutine("RotationNoSequence");
                }
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                mCoroutineInProcess = false;
                StopCoroutine("RotationNoSequence");
                Buddy.Actuators.Head.No.ResetPosition();
            },
            () =>   // --- OnClickTestFail ---
            {
                mTestInProcess = false;
                mResultPool.Add("rotationno", false);
                mErrorPool.Add("rotationno", "From tester");
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                mResultPool.Add("rotationno", true);
            });

            //  --- CODE ---
            DebugColor("RotationNo work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            mCoroutineInProcess = false;
            StopCoroutine("RotationNoSequence");
            Buddy.Actuators.Head.No.ResetPosition();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }

        private IEnumerator RotationNoSequence()
        {
            DebugColor("-- HeadNo target: 30 --", "red");
            Buddy.Actuators.Head.No.SetPosition(30F);

            yield return new WaitWhile(() =>
            {
                if (Buddy.Actuators.Head.No.Angle >= 27F)
                    return false;
                return true;
            });

            DebugColor("-- HeadNo target: -30 --", "red");
            Buddy.Actuators.Head.No.SetPosition(-30F);
            yield return new WaitWhile(() =>
            {
                if (Buddy.Actuators.Head.No.Angle <= -27F)
                    return false;
                return true;
            });

            DebugColor("-- HeadNo target: 0 --", "red");
            Buddy.Actuators.Head.No.SetPosition(0F);
            yield return new WaitWhile(() =>
            {
                if (Buddy.Actuators.Head.No.Angle <= 3F && Buddy.Actuators.Head.No.Angle >= -3F)
                    return false;
                return true;
            });
            mCoroutineInProcess = false;
        }
        #endregion

        #region MOVE_FORWARD
        public IEnumerator MoveForwardTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            // Show UI - And Implement all Ui callback
            DisplayTestUi("moveforward",
            () =>   // --- OnClickRepeat ---
            {
                if (!Buddy.Navigation.IsBusy)
                    Buddy.Navigation.Run<DisplacementStrategy>().Move(1F);
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestFail ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("moveforward", false);
                mErrorPool.Add("moveforward", "From tester");
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                mResultPool.Add("moveforward", true);
                Buddy.Navigation.Stop();
            });

            //  --- CODE ---
            DebugColor("MoveForward work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }
        #endregion

        #region MOVE_BACKWARD
        public IEnumerator MoveBackwardTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            // Show UI - And Implement all Ui callback
            DisplayTestUi("movebackward",
            () =>   // --- OnClickRepeat ---
            {
                if (!Buddy.Navigation.IsBusy)
                    Buddy.Navigation.Run<DisplacementStrategy>().Move(-1F);
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestFail ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("movebackward", false);
                mErrorPool.Add("movebackward", "From tester");
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("movebackward", true);
            });

            //  --- CODE ---
            DebugColor("MoveBackward work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }
        #endregion

        #region OBSTACLE_STOP
        public IEnumerator ObstacleStopTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            // Show UI - And Implement all Ui callback
            DisplayTestUi("obstaclestop",
            () =>   // --- OnClickRepeat ---
            {
                if (!Buddy.Navigation.IsBusy)
                    Buddy.Navigation.Run<DisplacementStrategy>().Move(2F, ObstacleAvoidanceType.STOP);
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestFail ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("obstaclestop", false);
                mErrorPool.Add("obstaclestop", "From tester");
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("obstaclestop", true);
            });

            //  --- CODE ---
            DebugColor("ObstacleStop work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }
        #endregion

        #region OBSTACLE_AVOIDANCE
        public IEnumerator ObstacleAvoidanceTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            // Show UI - And Implement all Ui callback
            DisplayTestUi("obstacleavoidance",
            () =>   // --- OnClickRepeat ---
            {
                if (!Buddy.Navigation.IsBusy)
                    Buddy.Navigation.Run<DisplacementStrategy>().Move(2F, ObstacleAvoidanceType.TURN);
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestFail ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("obstacleavoidance", false);
                mErrorPool.Add("obstacleavoidance", "From tester");
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("obstacleavoidance", true);
            });

            //  --- CODE ---
            DebugColor("ObstacleAvoidance work in progress", "blue");

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