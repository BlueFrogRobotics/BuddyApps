using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class CameraTest : AModuleTest
    {
        public override string Name
        {
            get
            {
                return ("Camera");
            }
        }

        // Getter for AvailableTestList
        public override List<string> GetAvailableTest() { return mAvailableTest; }

        public override void InitTestList()
        {
            mAvailableTest = new List<string>();
            mAvailableTest.Add("MotionDetect");
            mAvailableTest.Add("FaceDetect");
            mAvailableTest.Add("HumanDetect");
            mAvailableTest.Add("SkeletonDetect");
            mAvailableTest.Add("TakePhoto");
            return;
        }

        public override void InitPool()
        {
            mTestPool = new Dictionary<string, TestRoutine>();
            mTestPool.Add("MotionDetect", MotionDetectTests);
            mTestPool.Add("FaceDetect", FaceDetectTests);
            mTestPool.Add("HumanDetect", HumanDetectTests);
            mTestPool.Add("SkeletonDetect", SkeletonDetectTests);
            mTestPool.Add("TakePhoto", TakePhotoTests);
            return;
        }

        // All TestRoutine of this module:

        public bool MotionDetectTests()
        {
            Debug.LogWarning("MotionDetect not implemented yet");
            return true;
        }

        public bool FaceDetectTests()
        {
            Debug.LogWarning("FaceDetect not implemented yet");
            return true;
        }

        public bool HumanDetectTests()
        {
            Debug.LogWarning("HumanDetect not implemented yet");
            return true;
        }

        public bool SkeletonDetectTests()
        {
            Debug.LogWarning("SkeletonDetect not implemented yet");
            return true;
        }

        public bool TakePhotoTests()
        {
            Debug.LogWarning("TakePhoto not implemented yet");
            return true;
        }
    }
}
