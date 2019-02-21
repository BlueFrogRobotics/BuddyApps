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

        private bool mCoroutineInProcess;

        //  --- Method ---

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

        // Common interface for all MotionTest - TODO: Rework interface, use Footer for OK/KO button (see camera test)
        #region COMMON_UI
        private void DisplayTestUi(string TestLabel, System.Action iOnRepeat, System.Action iOnStopTest, System.Action iOnFailTest, System.Action iOnSuccessTest)
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With(iBuilder =>
                {
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetCenteredLabel(true);
                    lText.SetLabel(TestLabel + " test ...");

                    TButton lRepeatButton = iBuilder.CreateWidget<TButton>();
                    lRepeatButton.SetCenteredLabel(true);
                    lRepeatButton.SetLabel("Run Test");
                    lRepeatButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_repeat"));
                    lRepeatButton.OnClick.Add(iOnRepeat);

                    TButton lStopButton = iBuilder.CreateWidget<TButton>();
                    lStopButton.SetCenteredLabel(true);
                    lStopButton.SetLabel("Stop Test");
                    lStopButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_stop"));
                    lStopButton.OnClick.Add(iOnStopTest);
                 },
                // Action OnClickLeft
                iOnFailTest,
                // Text Left
                "KO",
                // Action OnClickRight
                iOnSuccessTest,
                // Text Right
                "OK");
        }
        #endregion

        // All TestRoutine of this module:

        #region ROTATION
        public IEnumerator RotationTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            // Show UI - And Implement all Ui callback
            DisplayTestUi("Rotation",
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
                mResultPool.Add("Rotation", false);
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("Rotation", true);
            });

            //  --- CODE ---
            DebugColor("RotationTest work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Toaster.Hide();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }
        #endregion

        #region ROTATION_YES
        public IEnumerator RotationYesTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            // Show UI - And Implement all Ui callback
            DisplayTestUi("RotationYes",
            () =>   // --- OnClickRepeat ---
            {
                StartCoroutine(RotationYesSequence());
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                StopCoroutine(RotationYesSequence());
                Buddy.Actuators.Head.Yes.Stop();
            },
            () =>   // --- OnClickTestFail ---
            {
                mTestInProcess = false;
                StopCoroutine(RotationYesSequence());
                Buddy.Actuators.Head.Yes.Stop();
                mResultPool.Add("RotationYes", false);
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                StopCoroutine(RotationYesSequence());
                Buddy.Actuators.Head.Yes.Stop();
                mResultPool.Add("RotationYes", true);
            });

            //  --- CODE ---
            DebugColor("RotationYes work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Toaster.Hide();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }

        private IEnumerator RotationYesSequence()
        {
            float lEpsilon = 0.1F;
            float lStartAngle = Buddy.Actuators.Head.Yes.Angle;

            DebugColor("-- HeadYes target: 20 --", "red");
            Buddy.Actuators.Head.Yes.SetPosition(20);
            yield return new WaitWhile(()=> 
            {
                if (Mathf.Abs(Buddy.Actuators.Head.Yes.Angle - 20) < lEpsilon)
                    return true;
                return false;
            });

            DebugColor("-- HeadYes target: 70 --", "red");
            Buddy.Actuators.Head.Yes.SetPosition(70);
            yield return new WaitWhile(() =>
            {
                if (Mathf.Abs(Buddy.Actuators.Head.Yes.Angle - 70) < lEpsilon)
                    return true;
                return false;
            });

            DebugColor("-- HeadYes target: StartAngle --", "red");
            Buddy.Actuators.Head.Yes.SetPosition(lStartAngle);
            yield return new WaitWhile(() =>
            {
                if (Mathf.Abs(Buddy.Actuators.Head.Yes.Angle - lStartAngle) < lEpsilon)
                    return true;
                return false;
            });
        }
        #endregion

        #region ROTATION_NO
        public IEnumerator RotationNoTests()
        {
            //  --- INIT ---
            mTestInProcess = true;
            mCoroutineInProcess = false;
            // Show UI - And Implement all Ui callback
            DisplayTestUi("RotationNo",
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
                mResultPool.Add("RotationNo", false);
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                mResultPool.Add("RotationNo", true);
            });

            //  --- CODE ---
            DebugColor("RotationNo work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
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
            DisplayTestUi("MoveForward",
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
                mResultPool.Add("MoveForward", false);
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                mResultPool.Add("MoveForward", true);
                Buddy.Navigation.Stop();
            });

            //  --- CODE ---
            DebugColor("MoveForward work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
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
            DisplayTestUi("MoveBackward",
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
                mResultPool.Add("MoveBackward", false);
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("MoveBackward", true);
            });

            //  --- CODE ---
            DebugColor("MoveBackward work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
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
            DisplayTestUi("ObstacleStop",
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
                mResultPool.Add("ObstacleStop", false);
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("ObstacleStop", true);
            });

            //  --- CODE ---
            DebugColor("ObstacleStop work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
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
            DisplayTestUi("ObstacleAvoidance",
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
                mResultPool.Add("ObstacleAvoidance", false);
            },
            () =>   // --- OnClickTestSuccess ---
            {
                mTestInProcess = false;
                Buddy.Navigation.Stop();
                mResultPool.Add("ObstacleAvoidance", true);
            });

            //  --- CODE ---
            DebugColor("ObstacleAvoidance work in progress", "blue");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.GUI.Toaster.Hide();
            yield return new WaitWhile(() => Buddy.GUI.Toaster.IsBusy);
        }
        #endregion
    }
}