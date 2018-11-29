using BlueQuark;

using UnityEngine;

using System.Collections;

using System.Collections.Generic;

namespace BuddyApp.RemoteControl
{

    public class MoveManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject mMoveOn;

        [SerializeField]
        private GameObject mMoveOff;

        [SerializeField]
        private RobotController mRobotController = null;

        private bool mActive = true;

        // Use this for initialization
        void Start()
        {
            mMoveOn.SetActive(mActive);
            mMoveOff.SetActive(!mActive);
        }

        public void onToggleMove()
        {
            if (mRobotController) {
                mActive = !mActive;
                mMoveOn.SetActive(mActive);
                mMoveOff.SetActive(!mActive);
                mRobotController.DisableMovement();
            }
        }
    }
}