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

        // All TestRoutine of this module:


        public IEnumerator RotationTests()
        {
            Debug.LogWarning("Rotation not implemented yet");
            //while (false)
            //    yield return null;
            yield break;
        }

        public IEnumerator RotationYesTests()
        {
            Debug.LogWarning("RotationYes not implemented yet");
            //while (false)
            //    yield return null;
            yield break;
        }

        public IEnumerator RotationNoTests()
        {
            Debug.LogWarning("RotationNo not implemented yet");
            //while (false)
            //    yield return null;
            yield break;
        }

        public IEnumerator MoveForwardTests()
        {
            Debug.LogWarning("MoveForward not implemented yet");
            //while (false)
            //    yield return null;
            yield break;
        }

        public IEnumerator MoveBackwardTests()
        {
            Debug.LogWarning("MoveBackward not implemented yet");
            //while (false)
            //    yield return null;
            yield break;
        }

        public IEnumerator ObstacleStopTests()
        {
            Debug.LogWarning("ObstacleStop not implemented yet");
            //while (false)
            //    yield return null;
            yield break;
        }

        public IEnumerator ObstacleAvoidanceTests()
        {
            Debug.LogWarning("ObstacleAvoidance not implemented yet");
            //while (false)
            //    yield return null;
            yield break;
        }
    }
}