using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class MotionTest : AModuleTest
    {
        public override string Name
        {
            get
            {
                return (Buddy.Resources.GetString("motions"));
            }
        }

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

        // All TestRoutine of this module:

        #region ROTATION
        public IEnumerator RotationTests()
        {
            //  --- INIT ---

            //  --- CODE ---
            DebugColor("RotationTest work in progress", "blue");
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
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestSuccess ---
            {
                Buddy.Navigation.Stop();
            });

            // --- MODE ---
            if (Mode == TestMode.M_AUTO && !Buddy.Navigation.IsBusy)
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(180F, 60F, () => { mResultPool.Add("rotation", true); mTestInProcess = false; });
            else if (Mode == TestMode.M_MANUAL && !Buddy.Navigation.IsBusy)
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(180F, 60F);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.Navigation.Stop();
        }
        #endregion

        #region ROTATION_YES
        public IEnumerator RotationYesTests()
        {
            //  --- INIT ---
            mCoroutineInProcess = false;

            //  --- CODE ---
            DebugColor("RotationYes work in progress", "blue");
            // Show UI - And Implement all Ui callback
            DisplayTestUi("rotationyes",
            () =>   // --- OnClickRepeat ---
            {
                if (!mCoroutineInProcess)
                    StartCoroutine(RotationYesSequence(null));
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                mCoroutineInProcess = false;
                StopCoroutine("RotationYesSequence");
                Buddy.Actuators.Head.Yes.ResetPosition();
            },
            null, null);

            // --- MODE ---
            if (Mode == TestMode.M_AUTO && !mCoroutineInProcess)
                StartCoroutine(RotationYesSequence(() => { mResultPool.Add("rotationyes", true); mTestInProcess = false; }));
            else if (Mode == TestMode.M_MANUAL && !mCoroutineInProcess)
                StartCoroutine(RotationYesSequence(null));

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            mCoroutineInProcess = false;
            StopCoroutine("RotationYesSequence");
            Buddy.Actuators.Head.Yes.ResetPosition();
        }

        private IEnumerator RotationYesSequence(System.Action iOnEndMove)
        {
            mCoroutineInProcess = true;
            DebugColor("-- HeadYes target: 30 --", "red");
            Buddy.Actuators.Head.Yes.SetPosition(30F);

            yield return new WaitWhile(() =>
            {
                if (Buddy.Actuators.Head.Yes.Angle >= 27F)
                    return false;
                return true;
            });

            DebugColor("-- HeadYes target: -8 --", "red");
            Buddy.Actuators.Head.Yes.SetPosition(-8F);
            yield return new WaitWhile(() =>
            {
                if (Buddy.Actuators.Head.Yes.Angle <= -5F)
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
            if (iOnEndMove != null)
                iOnEndMove();
            mCoroutineInProcess = false;
        }
        #endregion

        #region ROTATION_NO
        public IEnumerator RotationNoTests()
        {
            //  --- INIT ---
            mCoroutineInProcess = false;

            //  --- CODE ---
            DebugColor("RotationNo work in progress", "blue");
            // Show UI - And Implement all Ui callback
            DisplayTestUi("rotationno",
            () =>   // --- OnClickRepeat ---
            {
                if (!mCoroutineInProcess)
                {
                    mCoroutineInProcess = true;
                    StartCoroutine(RotationNoSequence(null));
                }
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                mCoroutineInProcess = false;
                StopCoroutine("RotationNoSequence");
                Buddy.Actuators.Head.No.ResetPosition();
            }, null, null);

            // --- MODE ---
            if (Mode == TestMode.M_AUTO && !mCoroutineInProcess)
                StartCoroutine(RotationNoSequence(() => { mResultPool.Add("rotationno", true); mTestInProcess = false; }));
            else if (Mode == TestMode.M_MANUAL && !mCoroutineInProcess)
                StartCoroutine(RotationNoSequence(null));

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            mCoroutineInProcess = false;
            StopCoroutine("RotationNoSequence");
            Buddy.Actuators.Head.No.ResetPosition();
        }

        private IEnumerator RotationNoSequence(System.Action iOnEndMove)
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
            if (iOnEndMove != null)
                iOnEndMove();
            mCoroutineInProcess = false;
        }
        #endregion

        #region MOVE_FORWARD
        public IEnumerator MoveForwardTests()
        {
            //  --- INIT ---

            //  --- CODE ---
            DebugColor("MoveForward work in progress", "blue");
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
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestSuccess ---
            {
                Buddy.Navigation.Stop();
            });

            // --- MODE ---
            if (Mode == TestMode.M_AUTO && !Buddy.Navigation.IsBusy)
                Buddy.Navigation.Run<DisplacementStrategy>().Move(1F, 0.6F, () => { mResultPool.Add("moveforward", true); mTestInProcess = false; });
            else if (Mode == TestMode.M_MANUAL && !Buddy.Navigation.IsBusy)
                Buddy.Navigation.Run<DisplacementStrategy>().Move(1F, 0.6F);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.Navigation.Stop();
        }
        #endregion

        #region MOVE_BACKWARD
        public IEnumerator MoveBackwardTests()
        {
            //  --- INIT ---

            //  --- CODE ---
            DebugColor("MoveBackward work in progress", "blue");
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
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestSuccess ---
            {
                Buddy.Navigation.Stop();
            });

            // --- MODE ---
            if (Mode == TestMode.M_AUTO && !Buddy.Navigation.IsBusy)
                Buddy.Navigation.Run<DisplacementStrategy>().Move(-1F, 0.6F, () => { mResultPool.Add("movebackward", true); mTestInProcess = false; });
            else if (Mode == TestMode.M_MANUAL && !Buddy.Navigation.IsBusy)
                Buddy.Navigation.Run<DisplacementStrategy>().Move(-1F, 0.6F);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.Navigation.Stop();
        }
        #endregion

        #region OBSTACLE_STOP
        public IEnumerator ObstacleStopTests()
        {
            //  --- INIT ---

            //  --- CODE ---
            DebugColor("ObstacleStop work in progress", "blue");
            // Show UI - And Implement all Ui callback
            DisplayTestUi("obstaclestop",
            () =>   // --- OnClickRepeat ---
            {
                if (!Buddy.Navigation.IsBusy)
                    Buddy.Navigation.Run<DisplacementStrategy>().Move(1F, ObstacleAvoidanceType.STOP);
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestFail ---
            {
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestSuccess ---
            {
                Buddy.Navigation.Stop();
            });

            // --- MODE ---
            if (Mode == TestMode.M_AUTO && !Buddy.Navigation.IsBusy)
                Buddy.Navigation.Run<DisplacementStrategy>().Move(1F, 0.6F, () => { mResultPool.Add("obstaclestop", true); mTestInProcess = false; }, ObstacleAvoidanceType.STOP);
            else if (Mode == TestMode.M_MANUAL && !Buddy.Navigation.IsBusy)
                Buddy.Navigation.Run<DisplacementStrategy>().Move(1F, 0.6F, ObstacleAvoidanceType.STOP);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.Navigation.Stop();
        }
        #endregion

        #region OBSTACLE_AVOIDANCE
        public IEnumerator ObstacleAvoidanceTests()
        {
            //  --- INIT ---

            //  --- CODE ---
            DebugColor("ObstacleAvoidance work in progress", "blue");
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
                Buddy.Navigation.Stop();
            },
            () =>   // --- OnClickTestSuccess ---
            {
                Buddy.Navigation.Stop();
            });

            // --- MODE ---
            if (Mode == TestMode.M_AUTO && !Buddy.Navigation.IsBusy)
                Buddy.Navigation.Run<DisplacementStrategy>().Move(1F, 0.6F, () => { mResultPool.Add("obstacleavoidance", true); mTestInProcess = false; }, ObstacleAvoidanceType.TURN);
            else if (Mode == TestMode.M_MANUAL && !Buddy.Navigation.IsBusy)
                Buddy.Navigation.Run<DisplacementStrategy>().Move(1F, 0.6F, ObstacleAvoidanceType.TURN);

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.Navigation.Stop();
        }
        #endregion
    }
}