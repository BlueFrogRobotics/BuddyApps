using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.TestCoroutineDisplay
{
    public class start : AStateMachineBehaviour
    {
        private const int COUNTDOWN_START = 4;
        private const float HOLD_POSE_TIME = 5F;

        private IEnumerator mStartCountDown;
        private bool mIsCoroutineLaunch;


        public override void Start()
        {
            mStartCountDown = null;
            mIsCoroutineLaunch = false;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Onstateenter start");
            mStartCountDown = null;
            Buddy.Vocal.EnableTrigger = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!mIsCoroutineLaunch)
            {
                Debug.Log("Launch coroutine");
                mIsCoroutineLaunch = true;
                mStartCountDown = CountDownImpl();
                StartCoroutine(mStartCountDown);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mIsCoroutineLaunch = false;
            mStartCountDown = null;
            if (mStartCountDown != null)
                StopCoroutine(mStartCountDown);
        }

        private IEnumerator CountDownImpl()
        {
            Debug.Log("Countdown");
            Buddy.GUI.Toaster.Display<CountdownToast>().With(COUNTDOWN_START, 0, 0, null, iCountDown =>
            {
                Debug.Log("IN CountDownImpl");
                Buddy.Vocal.Say(iCountDown.Second.ToString());
                if (iCountDown.IsDone)
                {
                    Debug.Log("3 CountDownImpl");
                    OnEndCoroutine();
                    Buddy.GUI.Toaster.Hide();
                }
            });
            yield return null;
        }

        private void OnEndCoroutine()
        {
            Trigger("replay");
        }

    }

}
